using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IniParser;
using IniParser.Model;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Windows.System;

namespace Qwilight.ViewModel
{
    public sealed partial class NoteFileViewModel : BaseViewModel
    {
        BaseNoteFile[] _rawNoteFileCollection;
        BaseNoteFile[] _detailedNoteFileCollection;
        BaseNoteFile _noteFile;
        EntryItem _entryItemValue;

        public override double TargetLength => 0.8;

        public override double TargetHeight => 0.6;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public EntryItem EntryItemValue
        {
            get => _entryItemValue;

            set
            {
                if (value != null && SetProperty(ref _entryItemValue, value, nameof(EntryItemValue)))
                {
                    OnPropertyChanged(nameof(Drawing));
                    OnPropertyChanged(nameof(Title));
                    _rawNoteFileCollection = value.NoteFiles;
                    NoteFileCollection = value.WellNoteFiles.ToArray();
                    NoteFile = value.NoteFile;
                }
            }
        }

        public bool IsNoteFileNotLogical => NoteFile?.IsLogical == false;

        public ImageSource Drawing => EntryItemValue?.DrawingInNoteFileWindow;

        public string Title => EntryItemValue?.Title;

        public BaseNoteFile NoteFile
        {
            get => _noteFile;

            set
            {
                if (value != null && SetProperty(ref _noteFile, value, nameof(NoteFile)))
                {
                    OnPropertyChanged(nameof(IsNoteFileNotLogical));
                    var mainViewModel = ViewModels.Instance.MainValue;
                    mainViewModel.EntryItemValue.ModifyNotePosition(Array.IndexOf(_rawNoteFileCollection, value));
                    mainViewModel.NotifyNoteFile();
                }
            }
        }

        public BaseNoteFile[] NoteFileCollection
        {
            get => _detailedNoteFileCollection;

            set => SetProperty(ref _detailedNoteFileCollection, value, nameof(NoteFileCollection));
        }

        public void OnPointLower() => ViewModels.Instance.MainValue.HandleLevyNoteFile();

        [RelayCommand]
        async Task OnEditNote()
        {
            var noteFile = EntryItemValue?.NoteFile;
            if (noteFile?.IsLogical == false)
            {
                try
                {
                    var flintFilePath = Path.Combine(QwilightComponent.CPUAssetsEntryPath, "Flint.exe");
                    var noteFilePath = noteFile.NoteFilePath;
                    if (QwilightComponent.BMSNoteFileFormats.Any(format => noteFilePath.IsTailCaselsss(format)))
                    {
                        if (Utility.HasInput(VirtualKey.LeftMenu) || !File.Exists(Configure.Instance.BMSEditorFilePath))
                        {
                            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
                            {
                                Filters = [".exe"]
                            });
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                Configure.Instance.BMSEditorFilePath = filePath;
                                await Edit().ConfigureAwait(false);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.ModifyEditorContents);
                            }
                        }
                        else
                        {
                            await Edit().ConfigureAwait(false);
                        }

                        async ValueTask Edit()
                        {
                            var bmseViewerFilePath = Path.Combine(Path.GetDirectoryName(Configure.Instance.BMSEditorFilePath), "bmse_viewer.ini");
                            var bmseFilePath = Path.Combine(Path.GetDirectoryName(Configure.Instance.BMSEditorFilePath), "bmse.ini");
                            var bmscFilePath = Path.Combine(Path.GetDirectoryName(Configure.Instance.BMSEditorFilePath), "iBMSC.Settings.xml");
                            if (File.Exists(bmseViewerFilePath) && File.Exists(bmseFilePath))
                            {
                                var bmseViewerData = (await File.ReadAllTextAsync(bmseViewerFilePath, Encoding.ASCII).ConfigureAwait(false)).Trim(Environment.NewLine.ToCharArray()).Split(Environment.NewLine).ToList();
                                bmseViewerData.AddRange(Enumerable.Repeat(string.Empty, 6 - bmseViewerData.Count % 6));
                                var data = $"""
Qwilight
{flintFilePath}
-P -N0 <filename>
-P -N<measure> <filename>
-S

""";
                                if (!bmseViewerData.Contains(flintFilePath))
                                {
                                    bmseViewerData.AddRange(data.Split(Environment.NewLine));
                                    await File.WriteAllTextAsync(bmseViewerFilePath, string.Join(Environment.NewLine, bmseViewerData) + Environment.NewLine, Encoding.UTF8).ConfigureAwait(false);
                                }

                                var bmseCompiler = new FileIniDataParser();
                                IniData bmse;
                                using (var sr = File.OpenText(bmseFilePath))
                                {
                                    bmse = bmseCompiler.ReadData(sr);
                                    bmse["View"]["ViewerNum"] = (bmseViewerData.IndexOf(flintFilePath) / 6).ToString();
                                }
                                using (var fw = File.Open(bmseFilePath, FileMode.Create))
                                using (var sw = new StreamWriter(fw, new UTF8Encoding(false)))
                                {
                                    bmseCompiler.WriteData(sw, bmse);
                                }
                                Utility.OpenAs(Configure.Instance.BMSEditorFilePath, $"\"{noteFilePath}\"");
                            }
                            else if (File.Exists(bmscFilePath))
                            {
                                var bmscCompiler = new XmlDocument();
                                bmscCompiler.LoadXml(await File.ReadAllTextAsync(bmscFilePath).ConfigureAwait(false));

                                var nodesViewer = bmscCompiler.SelectNodes("/iBMSC/Player/Player").Cast<XmlNode>().ToList();
                                var flintID = nodesViewer.FindIndex(node => Utility.EqualsCaseless(node.Attributes["Path"].Value, flintFilePath));
                                if (flintID == -1)
                                {
                                    flintID = nodesViewer.Count;
                                    var flintViewer = bmscCompiler.CreateElement("Player");
                                    flintViewer.SetAttribute("Index", flintID.ToString());
                                    flintViewer.SetAttribute("Path", flintFilePath);
                                    flintViewer.SetAttribute("FromBeginning", "-P -N0 \"<filename>\"");
                                    flintViewer.SetAttribute("FromHere", "-P -N<measure> \"<filename>\"");
                                    flintViewer.SetAttribute("Stop", "-S");
                                    var nodeViewer = bmscCompiler.SelectSingleNode("/iBMSC/Player");
                                    nodeViewer.AppendChild(flintViewer);
                                    nodeViewer.Attributes["Count"].Value = nodeViewer.ChildNodes.Count.ToString();
                                    nodeViewer.Attributes["CurrentPlayer"].Value = flintID.ToString();
                                }
                                else
                                {
                                    var nodeViewer = bmscCompiler.SelectSingleNode("/iBMSC/Player");
                                    nodeViewer.Attributes["CurrentPlayer"].Value = flintID.ToString();
                                }
                                using (var fw = File.Open(bmscFilePath, FileMode.Create))
                                using (var sw = new StreamWriter(fw, Encoding.Unicode))
                                {
                                    bmscCompiler.Save(sw);
                                }
                                Utility.OpenAs(Configure.Instance.BMSEditorFilePath, $"\"{noteFilePath}\"");
                            }
                            else
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.EditorConfigureFault);
                            }
                        }
                    }
                    else if (QwilightComponent.BMSONNoteFileFormats.Any(format => noteFilePath.IsTailCaselsss(format)))
                    {
                        if (Utility.HasInput(VirtualKey.LeftMenu) || !File.Exists(Configure.Instance.BMSONEditorFilePath))
                        {
                            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
                            {
                                Filters = [".exe"]
                            });
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                Configure.Instance.BMSONEditorFilePath = filePath;
                                Edit();
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.ModifyEditorContents);
                            }
                        }
                        else
                        {
                            Edit();
                        }

                        void Edit()
                        {
                            var bms1FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BmsONE", "Settings.ini");
                            if (File.Exists(bms1FilePath))
                            {
                                var bms1Compiler = new FileIniDataParser();
                                IniData bms1;
                                using (var sr = File.OpenText(bms1FilePath))
                                {
                                    bms1 = bms1Compiler.ReadData(sr);
                                    var targetViewer = bms1["ExternalViewer"];
                                    var targetViewerID = Utility.ToInt32(targetViewer["ViewerCount"]);
                                    flintFilePath = flintFilePath.Replace(@"\", @"\\");
                                    if (!Enumerable.Range(0, targetViewerID).Any(i => targetViewer[$@"Viewer{i}\Path"] == flintFilePath))
                                    {
                                        targetViewer[$@"Viewer{targetViewerID}\Name"] = "Qwilight";
                                        targetViewer[$@"Viewer{targetViewerID}\Path"] = flintFilePath;
                                        targetViewer[$@"Viewer{targetViewerID}\Icon"] = "";
                                        targetViewer[$@"Viewer{targetViewerID}\ArgumentPlayLevy"] = "-P -N0 $(filename)";
                                        targetViewer[$@"Viewer{targetViewerID}\ArgumentPlayHere"] = "-P -N$(measure) $(filename)";
                                        targetViewer[$@"Viewer{targetViewerID}\ArgumentStop"] = "-S";
                                        targetViewer[$@"Viewer{targetViewerID}\WorkingDirectory"] = "$(exedir)";
                                        targetViewer["CurrentViewer"] = targetViewerID.ToString();
                                        targetViewer["ViewerCount"] = (targetViewerID + 1).ToString();
                                    }
                                }
                                using (var fw = File.Open(bms1FilePath, FileMode.Create))
                                using (var sw = new StreamWriter(fw, Encoding.UTF8))
                                {
                                    bms1Compiler.WriteData(sw, bms1);
                                }
                                Utility.OpenAs(Configure.Instance.BMSONEditorFilePath, $"\"{noteFilePath}\"");
                            }
                            else
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.EditorConfigureFault);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.EditorSoftwareFault, e.Message));
                }
            }
        }

        [RelayCommand]
        void OnModifyFavoriteEntry(int mode)
        {
            var favoriteEntryViewModel = ViewModels.Instance.FavoriteEntryValue;
            favoriteEntryViewModel.NoteFile = NoteFile;
            favoriteEntryViewModel.EntryItem = EntryItemValue;
            favoriteEntryViewModel.Mode = mode;
            favoriteEntryViewModel.Open();
        }

        [RelayCommand]
        void OnWipeHandled(int mode)
        {
            switch (mode)
            {
                case 0:
                    DB.Instance.WipeHandled(NoteFile);
                    NoteFile.HandledValue = new()
                    {
                        IDValue = BaseNoteFile.Handled.ID.Not
                    };
                    break;
                case 1:
                    foreach (var noteFile in EntryItemValue.NoteFiles)
                    {
                        DB.Instance.WipeHandled(noteFile);
                        noteFile.HandledValue = new()
                        {
                            IDValue = BaseNoteFile.Handled.ID.Not
                        };
                    }
                    break;
            }
        }

        [RelayCommand]
        void OnNewEventNote()
        {
            var eventNoteViewModel = ViewModels.Instance.EventNoteValue;
            eventNoteViewModel.NoteFileCollection.Add(EntryItemValue.NoteFile);
            eventNoteViewModel.Open();
        }

        [RelayCommand]
        void OnModifyEventNoteName()
        {
            var inputTextViewModel = ViewModels.Instance.InputTextValue;
            inputTextViewModel.Text = LanguageSystem.Instance.ModifyEventNoteNameContents;
            inputTextViewModel.Input = EntryItemValue.EventNoteName;
            inputTextViewModel.HandleOK = new(text =>
            {
                DB.Instance.ModifyEventNoteName(EntryItemValue.EventNoteID, text);
                EntryItemValue.EventNoteName = text;
            });
            inputTextViewModel.Open();
        }

        [RelayCommand]
        void OnSaveAsNoteFilesBundle()
        {
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
            {
                bundleVariety = BundleItem.BundleVariety.NoteFiles,
                bundleName = EntryItemValue.Title,
                bundleEntryPath = EntryItemValue.EntryPath
            });
        }

        [RelayCommand]
        void OnSaveAsNoteFileBundle()
        {
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
            {
                bundleVariety = BundleItem.BundleVariety.NoteFile,
                bundleName = EntryItemValue.NoteFile.Title,
                bundleEntryPath = EntryItemValue.NoteFile.NoteFilePath,
                etc = string.Join('/', EntryItemValue.NoteFile.EntryItem.CompatibleNoteFiles.Select(noteFile => noteFile.GetNoteID512()))
            });
        }

        [RelayCommand]
        void OnSaveAsEventNoteBundle()
        {
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
            {
                bundleVariety = BundleItem.BundleVariety.EventNote,
                bundleName = EntryItemValue.EventNoteName,
                bundleEntryPath = EntryItemValue.EventNoteID,
                etc = EntryItemValue.EventNoteVariety switch
                {
                    DB.EventNoteVariety.MD5 => "MD5",
                    DB.EventNoteVariety.Qwilight => "Qwilight",
                    _ => default
                }
            });
        }

        [RelayCommand]
        void OnNoteFileFormatID(int? e)
        {
            if (e.HasValue)
            {
                var noteFormatID = e.Value;
                Configure.Instance.NoteFormatID = noteFormatID;
                var noteFile = EntryItemValue?.NoteFile;
                if (noteFile != null)
                {
                    DB.Instance.SetNoteFormat(noteFile, noteFormatID);
                    FastDB.Instance.WipeNoteFile(noteFile);
                }
                var mainViewModel = ViewModels.Instance.MainValue;
                mainViewModel.VerifyNoteFile(mainViewModel.ModeComponentValue.Salt);
            }
        }

        [RelayCommand]
        void OnEntryItemFormatID(int? e)
        {
            if (e.HasValue)
            {
                Configure.Instance.NoteFormatID = e.Value;
                var noteFiles = EntryItemValue?.NoteFiles;
                if (noteFiles != null)
                {
                    foreach (var noteFile in noteFiles)
                    {
                        if (!noteFile.IsLogical)
                        {
                            DB.Instance.SetNoteFormat(noteFile, e.Value);
                            FastDB.Instance.WipeNoteFile(noteFile);
                        }
                    }
                }
                var mainViewModel = ViewModels.Instance.MainValue;
                mainViewModel.VerifyNoteFile(mainViewModel.ModeComponentValue.Salt);
            }
        }
    }
}