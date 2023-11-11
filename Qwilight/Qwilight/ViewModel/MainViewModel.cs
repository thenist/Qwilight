using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ionic.Zip;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using UtfUnknown;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;
using Xml2CSharp;
using Clipboard = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace Qwilight.ViewModel
{
    public sealed partial class MainViewModel : Model
    {
        public enum Mode
        {
            NoteFile, Computing = 2, Quit
        }

        public sealed class Fading
        {
            public double Status { get; set; }

            public int Layer { get; set; }

            public DefaultCompute Computer { get; set; }

            public bool IsComputerStable { get; set; }
        }

        readonly PausableAudioHandler _pausableAudioHandler = new();
        readonly ConcurrentDictionary<int, EntryItem> _entryItems = new();
        readonly HashSet<DefaultEntryItem> _alreadyLoadedDefaultEntryItems = new();
        readonly DispatcherTimer _loadDefaultCommentHandler = new(DispatcherPriority.Input, HandlingUISystem.Instance.UIHandler)
        {
            Interval = TimeSpan.FromMilliseconds(QwilightComponent.StandardUILoopMillis)
        };
        readonly DispatcherTimer _loadTwilightCommentHandler = new(DispatcherPriority.Input, HandlingUISystem.Instance.UIHandler)
        {
            Interval = TimeSpan.FromMilliseconds(QwilightComponent.StandardUILoopMillis)
        };
        readonly DispatcherTimer _autoComputerHandler = new(DispatcherPriority.Input, HandlingUISystem.Instance.UIHandler)
        {
            Interval = TimeSpan.FromMilliseconds(QwilightComponent.StandardWaitMillis)
        };
        readonly DispatcherTimer _wantHandler = new(DispatcherPriority.Input, HandlingUISystem.Instance.UIHandler)
        {
            Interval = TimeSpan.FromMilliseconds(QwilightComponent.StandardWaitMillis)
        };
        readonly ModeComponent _defaultModeComponentValue = new();
        readonly FileSystemWatcher _fsw = new(QwilightComponent.EdgeEntryPath)
        {
            EnableRaisingEvents = true
        };
        DispatcherTimer _fadeInHandler;
        bool _isAvailable = true;
        string _twilightCommentText0 = string.Empty;
        string _twilightCommentText1 = string.Empty;
        string _qwilightFileName;
        Mode _mode;
        int _lastEntryItemID;
        bool _isCommentMode;
        CommentItem _defaultCommentItem;
        CommentItem _twilightCommentItem;
        int _entryItemPosition;
        EntryItem _entryItemValue;
        CancellationTokenSource _setCancelDefaultEntryItem;
        int? _lastLowerMillis;
        bool _wasLowerMillis = true;
        bool _isUILoading;
        bool _isDefaultEntryLoading;
        bool _isWantInputPointed;
        bool _isCommentaryInputPointed;
        bool _wasSiteContainerOpened;
        bool _wasCommentOpened;
        bool _isDefaultCommentLoading;
        bool _isTwilightCommentLoading;
        bool? _twilightCommentFavor;
        string _twilightCommentTotalFavor;
        bool _isWowItemLoading;
        string _twilightCommentary = string.Empty;
        bool _isWPFViewVisible = true;
        bool _isLoaded;
        double _windowDPI;

        static bool IsNotModal(BaseViewModel targetViewModel = null)
        {
            foreach (var windowViewModel in ViewModels.Instance.WindowViewModels)
            {
                if (windowViewModel == targetViewModel)
                {
                    continue;
                }
                if (windowViewModel.IsOpened && windowViewModel.IsModal)
                {
                    return false;
                }
            }
            return true;
        }

        public List<DefaultEntryItem> DefaultEntryItems { get; } = new()
        {
            DefaultEntryItem.Total
        };

        public bool IsBPMVisible => EntryItemValue?.NoteFile?.HasBPMMap != true;

        public bool IsBPM1Visible => EntryItemValue?.NoteFile?.HasBPMMap == true;

        bool HasNotInput(BaseViewModel targetViewModel = null) => IsNotModal(targetViewModel) && IsAvailable && (!IsWPFViewVisible || !_isWantInputPointed && !_isCommentaryInputPointed && !ViewModels.Instance.SiteContainerValue.HasPointedInput);

        public double WindowDPI
        {
            get => _windowDPI;

            set => SetProperty(ref _windowDPI, value, nameof(WindowDPI));
        }

        public ConcurrentDictionary<string, EntryItem> EventNoteEntryItems { get; } = new();

        public ConcurrentDictionary<string, BaseNoteFile> NoteID512s { get; } = new();

        public ConcurrentDictionary<string, BaseNoteFile> NoteID128s { get; } = new();

        public int NoteDrawingCount => _entryItems.Values.Sum(entryItem => entryItem.NoteFiles.Count(noteFile => noteFile.HasNoteDrawing));

        public ObservableCollection<CommentItem> DefaultCommentCollection { get; } = new();

        public ObservableCollection<CommentItem> TwilightCommentCollection { get; } = new();

        public double Status { get; set; }

        public ObservableCollection<EntryItem> EntryItems { get; set; } = new();

        public ObservableCollection<WowItem> TotalWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> HighestWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> StandWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> BandWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> TotalAtWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> HighestAtWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> StandAtWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> BandAtWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> Ability5KWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> Ability7KWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> Ability9KWowItemCollection { get; } = new();

        public ObservableCollection<WowItem> LevelWowItemCollection { get; } = new();

        public Dictionary<string, EntryItem> LastEntryItems { get; } = new();

        public DefaultCompute Computer { get; set; }

        public AutoCompute AutoComputer { get; set; }

        public DefaultCompute FadingViewComputer
        {
            get
            {
                var fadingComputer = FadingValue.Computer;
                if (FadingValue.IsComputerStable)
                {
                    return fadingComputer;
                }
                else
                {
                    if (fadingComputer.NoteFile == AutoComputer?.NoteFile)
                    {
                        return AutoComputer;
                    }
                    else
                    {
                        return fadingComputer;
                    }
                }
            }
        }

        public ModeComponent ModeComponentValue { get; } = Configure.Instance.ModeComponentValue;

        public bool IsWPFViewVisible
        {
            get => _isWPFViewVisible;

            set
            {
                if (SetProperty(ref _isWPFViewVisible, value, nameof(IsWPFViewVisible)))
                {
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.SetD2DViewVisibility,
                        Contents = !value
                    });
                    IsVital = IsComputingMode && !value;
                }
            }
        }

        public bool IsVital { get; set; }

        public bool HasPoint { get; set; }

        public string TwilightCommentary
        {
            get => _twilightCommentary;

            set => SetProperty(ref _twilightCommentary, value, nameof(TwilightCommentary));
        }

        public Computing GetHandlingComputing() => IsNoteFileMode ? EntryItemValue?.NoteFile : GetHandlingComputer();

        public DefaultCompute GetHandlingComputer() => ModeValue switch
        {
            Mode.NoteFile => AutoComputer,
            Mode.Computing => Computer,
            Mode.Quit => AutoComputer?.IsHandling == true ? AutoComputer : Computer,
            _ => default
        };

        public bool IsPausing => AutoComputer?.IsPausing ?? _pausableAudioHandler.IsPausing;

        public string TwilightCommentText0
        {
            get => _twilightCommentText0;

            set => SetProperty(ref _twilightCommentText0, value, nameof(TwilightCommentText0));
        }

        public string TwilightCommentText1
        {
            get => _twilightCommentText1;

            set => SetProperty(ref _twilightCommentText1, value, nameof(TwilightCommentText1));
        }

        public bool IsDefaultCommentLoading
        {
            get => _isDefaultCommentLoading;

            set => SetProperty(ref _isDefaultCommentLoading, value, nameof(IsDefaultCommentLoading));
        }

        public bool IsTwilightCommentLoading
        {
            get => _isTwilightCommentLoading;

            set
            {
                if (SetProperty(ref _isTwilightCommentLoading, value, nameof(IsTwilightCommentLoading)))
                {
                    OnPropertyChanged(nameof(IsEntryItemBanned));
                }
            }
        }

        public bool? TwilightCommentFavor
        {
            get => _twilightCommentFavor;

            set
            {
                if (SetProperty(ref _twilightCommentFavor, value))
                {
                    OnPropertyChanged(nameof(LowerTwilightCommentFavorPaint));
                    OnPropertyChanged(nameof(HigherTwilightCommentFavorPaint));
                    NotifyCanTwilightFavor();
                }
            }
        }

        public bool CanLowerTwilightCommentFavor => IsNoteFileNotLogical && !IsEntryItemBanned && TwilightSystem.Instance.IsSignedIn && TwilightCommentFavor != true;

        public Brush LowerTwilightCommentFavorPaint => TwilightCommentFavor == false ? Paints.Paint1 : Paints.Paint4;

        public bool CanHigherTwilightCommentFavor => IsNoteFileNotLogical && !IsEntryItemBanned && TwilightSystem.Instance.IsSignedIn && TwilightCommentFavor != false;

        public Brush HigherTwilightCommentFavorPaint => TwilightCommentFavor == true ? Paints.Paint3 : Paints.Paint4;

        public string TwilightCommentTotalFavor
        {
            get => _twilightCommentTotalFavor;

            set => SetProperty(ref _twilightCommentTotalFavor, value, nameof(TwilightCommentTotalFavor));
        }

        public bool IsWowLoading
        {
            get => _isWowItemLoading;

            set => SetProperty(ref _isWowItemLoading, value, nameof(IsWowLoading));
        }

        public void NotifyIsPausing() => OnPropertyChanged(nameof(IsPausing));

        public Fading FadingValue { get; } = new();

        public bool IsAvailable
        {
            get => _isAvailable;

            set
            {
                if (SetProperty(ref _isAvailable, value, nameof(IsAvailable)) && value)
                {
                    PointEntryView();
                }
            }
        }

        public bool IsDefaultEntryLoading
        {
            get => _isDefaultEntryLoading;

            set
            {
                if (SetProperty(ref _isDefaultEntryLoading, value, nameof(IsDefaultEntryLoading)) && !value)
                {
                    PointEntryView();
                }
            }
        }

        public bool IsUILoading
        {
            get => _isUILoading;

            set
            {
                if (SetProperty(ref _isUILoading, value, nameof(IsUILoading)) && !value)
                {
                    PointEntryView();
                }
            }
        }

        public string HighestInputCountText => Utility.GetHighestInputCountText(EntryItemValue?.NoteFile?.AverageInputCount ?? default, EntryItemValue?.NoteFile?.HighestInputCount ?? default, ModeComponentValue.AudioMultiplier);

        public string BPMText => Utility.GetBPMText(EntryItemValue?.NoteFile?.BPM ?? Component.StandardBPM, ModeComponentValue.Multiplier, ModeComponentValue.AudioMultiplier);

        public string LengthText => Utility.GetLengthText(EntryItemValue?.NoteFile?.Length ?? default);

        public bool IsEntryItemEventNote => !string.IsNullOrEmpty(EntryItemValue?.EventNoteID);

        public bool IsNoteFileNotLogical => EntryItemValue?.NoteFile?.IsLogical == false;

        public bool IsNoteFileAvailable => !string.IsNullOrEmpty(EntryItemValue?.NoteFile?.NoteFilePath);

        public bool CanSaveAsBundle => (IsEntryItemEventNote || IsNoteFileNotLogical) && TwilightSystem.Instance.IsSignedIn;

        public bool HasAssistFile => !string.IsNullOrEmpty(EntryItemValue?.NoteFile?.AssistFileName);

        public bool CanModifyModeComponent => IsNoteFileMode && !IsCommentMode;

        public void NotifyCanSaveAsBundle() => OnPropertyChanged(nameof(CanSaveAsBundle));

        public bool IsEntryItemBanned => EntryItemValue?.IsBanned != false;

        public bool CanTwilightCommentary => IsNoteFileNotLogical && !IsEntryItemBanned && TwilightSystem.Instance.IsSignedIn;

        public void NotifyCanTwilightCommentary() => OnPropertyChanged(nameof(CanTwilightCommentary));

        public void NotifyCanTwilightFavor()
        {
            OnPropertyChanged(nameof(CanLowerTwilightCommentFavor));
            OnPropertyChanged(nameof(CanHigherTwilightCommentFavor));
        }

        public double DefaultLength => IsComputingMode ? UI.Instance.DefaultLength : BaseUI.Instance.DefaultLength;

        public double DefaultHeight => IsComputingMode ? UI.Instance.DefaultHeight : BaseUI.Instance.DefaultHeight;

        public bool IsNoteFileMode => ModeValue == Mode.NoteFile;

        public bool IsComputingMode => ModeValue == Mode.Computing;

        public bool IsQuitMode => ModeValue == Mode.Quit;

        public Mode ModeValue
        {
            get => _mode;

            set
            {
                if (SetProperty(ref _mode, value))
                {
                    OnPropertyChanged(nameof(IsNoteFileMode));
                    OnPropertyChanged(nameof(IsComputingMode));
                    OnPropertyChanged(nameof(IsQuitMode));
                    OnPropertyChanged(nameof(CanModifyModeComponent));
                    OnPropertyChanged(nameof(DefaultLength));
                    OnPropertyChanged(nameof(DefaultHeight));
                    OnWPFViewVisibilityModified();
                    ViewModels.Instance.NotifyWindowViewModels();
                    BaseUI.Instance.InitEvents();
                    Configure.Instance.UIConfigureValue.NotifyInputMode();
                    switch (value)
                    {
                        case Mode.NoteFile:
                            if (_wasSiteContainerOpened)
                            {
                                ViewModels.Instance.SiteContainerValue.Open(false);
                                _wasSiteContainerOpened = false;
                            }
                            if (_wasCommentOpened)
                            {
                                ViewModels.Instance.CommentValue.Open(false);
                                _wasCommentOpened = false;
                            }
                            NotifySystem.Instance.NotifyPending();
                            break;
                        case Mode.Computing:
                            _wasSiteContainerOpened = ViewModels.Instance.SiteContainerValue.IsOpened;
                            ViewModels.Instance.SiteContainerValue.Close(false);
                            ViewModels.Instance.NoteFileValue.Close(false);
                            break;
                        case Mode.Quit:
                            _wasCommentOpened = ViewModels.Instance.CommentValue.IsOpened;
                            ViewModels.Instance.CommentValue.Close(false);
                            break;
                    }
                }
            }
        }

        public CommentItem DefaultCommentItem
        {
            get => _defaultCommentItem;

            set => SetProperty(ref _defaultCommentItem, value, nameof(DefaultCommentItem));
        }

        public CommentItem TwilightCommentItem
        {
            get => _twilightCommentItem;

            set => SetProperty(ref _twilightCommentItem, value, nameof(TwilightCommentItem));
        }

        public int EntryItemPosition
        {
            get => _entryItemPosition;

            set
            {
                if (SetProperty(ref _entryItemPosition, value, nameof(EntryItemPosition)) && _isLoaded && value != -1)
                {
                    Configure.Instance.LastEntryItemPositions[Configure.Instance.LastDefaultEntryItem?.DefaultEntryPath ?? string.Empty] = value;
                }
            }
        }

        public EntryItem EntryItemValue
        {
            get => _entryItemValue;

            set
            {
                if (SetProperty(ref _entryItemValue, value, nameof(EntryItemValue)))
                {
                    OnPropertyChanged(nameof(IsEntryItemEventNote));
                    if (value != null)
                    {
                        if (ViewModels.Instance.NoteFileValue.IsOpened)
                        {
                            ViewModels.Instance.NoteFileValue.EntryItemValue = value;
                        }
                        MoveEntryView();
                        if (_isLoaded)
                        {
                            LastEntryItems[Configure.Instance.LastDefaultEntryItem?.DefaultEntryPath ?? string.Empty] = value;
                        }
                    }
                    NotifyNoteFile();
                }
            }
        }

        public bool IsCommentMode
        {
            get => _isCommentMode;

            set
            {
                if (SetProperty(ref _isCommentMode, value, nameof(IsCommentMode)))
                {
                    OnPropertyChanged(nameof(CanModifyModeComponent));
                    if (value)
                    {
                        _defaultModeComponentValue.CopyAs(ModeComponentValue, null, false);
                    }
                    else
                    {
                        ModeComponentValue.CopyAs(_defaultModeComponentValue, null, false);
                    }
                }
            }
        }

        public async void OnLoaded(nint handle)
        {
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.SetWindowedMode
            });

            StillSystem.Instance.Init(handle);
            MIDISystem.Instance.HandleSystem();

            AudioSystem.Instance.LoadDefaultAudio();
            AudioSystem.Instance.LoadBanalAudio();
            DrawingSystem.Instance.LoadDefaultDrawing();
            DrawingSystem.Instance.LoadVeilDrawing();

            LevelSystem.Instance.LoadJSON(false);

            await ValveSystem.Instance.Init();
            await Task.Run(() =>
            {
                BaseUI.Instance.LoadUI(null, Configure.Instance.BaseUIItemValue, false);
                UI.Instance.LoadUI(null, Configure.Instance.UIItemValue, false);

                Utility.HandleLongParallel(TVSystem.Instance.HandleSystem, false);
                Utility.HandleLongParallel(IlluminationSystem.Instance.HandleSystem, false);
                DefaultControllerSystem.Instance.HandleSystem();
                Utility.HandleLongParallel(DrawingSystem.Instance.HandleSystem);
                Utility.HandleLongParallel(TwilightSystem.Instance.HandleSystem, false);
                Utility.HandleLongParallel(PlatformSystem.Instance.HandleSystem, false);
                Utility.HandleLongParallel(FlintSystem.Instance.HandleSystem, false);
                Utility.HandleLongParallel(() => ControllerSystem.Instance.HandleSystem(handle));

                if (Configure.Instance.AudioMultiplierAtone)
                {
                    AudioSystem.Instance.SetAudioMultiplierAtone(true, ModeComponentValue.AudioMultiplier);
                }

                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.FadingLoadingView
                });

                if (!string.IsNullOrEmpty(Configure.Instance.ConfigureFault))
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, Configure.Instance.ConfigureFault);
                }
                if (!string.IsNullOrEmpty(GPUConfigure.Instance.GPUConfigureFault))
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, GPUConfigure.Instance.GPUConfigureFault);
                }
                if (!string.IsNullOrEmpty(DB.Instance.DBFault))
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, DB.Instance.DBFault);
                }

                if (!Configure.Instance.IsLoaded)
                {
                    ViewModels.Instance.AssistValue.Open();
                    Configure.Instance.IsLoaded = true;
                }

                SetDefaultEntryItems();
                if (Configure.Instance.AutoGetQwilight)
                {
                    GetQwilight(true);
                }

                if (QwilightComponent.IsTest)
                {
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.Quit,
                        Contents = false
                    });
                }
            });

            _isLoaded = true;
        }

        public void OnFileAs(DragEventArgs e)
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (filePaths != null)
            {
                Task.Run(async () =>
                {
                    var wasEventNote = false;
                    var lastYamlFileName = string.Empty;
                    DefaultEntryItem lastDefaultEntryItem = null;
                    foreach (var filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            if (QwilightComponent.NoteFileFormats.Any(format => filePath.IsTailCaselsss(format)))
                            {
                                FlintNoteFile(filePath, 0);
                            }
                            else if (filePath.IsTailCaselsss(".zip"))
                            {
                                using var zipFile = ZipFile.Read(filePath);
                                var yamlFileName = Path.GetFileNameWithoutExtension(zipFile.Where(zipEntry => zipEntry.FileName.IsTailCaselsss(".yaml")).OrderBy(zipEntry => zipEntry.FileName).FirstOrDefault()?.FileName);
                                if (string.IsNullOrEmpty(yamlFileName))
                                {
                                    HandleNoteBundle(filePath);
                                }
                                else
                                {
                                    var savingBundleItem = new NotifyItem
                                    {
                                        Text = LanguageSystem.Instance.SavingUIContents,
                                        Variety = NotifySystem.NotifyVariety.Levying,
                                        OnStop = isTotal => false,
                                    };
                                    try
                                    {
                                        HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.NotifyValue.NotifyItemCollection.Insert(0, savingBundleItem));
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                        zipFile.ExtractProgress += (sender, e) =>
                                        {
                                            savingBundleItem.LevyingStatus = e.EntriesExtracted;
                                            savingBundleItem.QuitStatus = e.EntriesTotal;
                                            savingBundleItem.NotifyBundleStatus();
                                        };
                                        zipFile.ExtractAll(string.IsNullOrEmpty(Path.GetDirectoryName(yamlFileName)) ? Path.Combine(QwilightComponent.UIEntryPath, yamlFileName) : QwilightComponent.UIEntryPath, ExtractExistingFileAction.OverwriteSilently);
                                        savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                        savingBundleItem.Text = LanguageSystem.Instance.SavedUIContents;
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, savingBundleItem.Text);
                                        lastYamlFileName = yamlFileName;
                                    }
                                    catch (Exception e)
                                    {
                                        savingBundleItem.Variety = NotifySystem.NotifyVariety.Stopped;
                                        savingBundleItem.Text = string.Format(LanguageSystem.Instance.SaveUIFault, e.Message);
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, savingBundleItem.Text);
                                    }
                                    finally
                                    {
                                        savingBundleItem.OnStop = isTotal => true;
                                    }
                                }
                            }
                            else if (filePath.IsTailCaselsss(".rar") || filePath.IsTailCaselsss(".7z"))
                            {
                                HandleNoteBundle(filePath);
                            }
                            else if (filePath.IsTailCaselsss(".lr2crs"))
                            {
                                var date = DateTime.Now;
                                using var fs = File.OpenRead(filePath);
                                foreach (var eventNote in (new XmlSerializer(typeof(Courselist)).Deserialize(fs) as Courselist).Course)
                                {
                                    try
                                    {
                                        var eventNoteID = eventNote.Hash[32..];
                                        for (var m = eventNoteID.Length - 32; m > 0; m -= 32)
                                        {
                                            eventNoteID = eventNoteID.Insert(m, ":0/");
                                        }
                                        eventNoteID += ":0";
                                        var eventNoteName = eventNote.Title;
                                        var eventNoteVariety = DB.EventNoteVariety.MD5;
                                        await DB.Instance.SetEventNote(eventNoteID, eventNoteName, date, eventNoteVariety);
                                        wasEventNote = true;
                                    }
                                    catch (SQLiteException)
                                    {
                                    }
                                }
                            }
                        }
                        else if (Directory.Exists(filePath))
                        {
                            var defaultEntryItem = new DefaultEntryItem
                            {
                                DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Default,
                                DefaultEntryPath = filePath,
                                Layer = Configure.Instance.DefaultEntryItems.Count
                            };
                            HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.ModifyDefaultEntryValue.DefaultEntryItemCollection.Add(defaultEntryItem));
                            Configure.Instance.DefaultEntryItems.Add(defaultEntryItem);
                            lastDefaultEntryItem = defaultEntryItem;
                        }
                    }
                    if (wasEventNote)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpenedEventNotes);
                        LoadEventNoteEntryItems();
                        Want();
                    }
                    if (!string.IsNullOrEmpty(lastYamlFileName))
                    {
                        ViewModels.Instance.ConfigureValue.TabPosition = 0;
                        ViewModels.Instance.ConfigureValue.TabPositionComputing = 1;
                        ViewModels.Instance.ConfigureValue.TabPositionUI = lastYamlFileName.StartsWith('@') ? 0 : 1;
                        ViewModels.Instance.ConfigureValue.Open();
                    }
                    if (lastDefaultEntryItem != null)
                    {
                        HandlingUISystem.Instance.HandleParallel(() =>
                        {
                            if (!IsDefaultEntryLoading)
                            {
                                Configure.Instance.LastDefaultEntryItem = lastDefaultEntryItem;
                                SetDefaultEntryItems();
                            }
                        });
                    }
                });
            }
        }

        public void OnInputWantPointed(bool isWantInputPointed) => _isWantInputPointed = isWantInputPointed;

        public void OnInputTwilightCommentaryPointed(bool isCommentaryInputPointed) => _isCommentaryInputPointed = isCommentaryInputPointed;

        public void OnSetPoint(bool hasPoint)
        {
            if (!hasPoint)
            {
                Computer?.AutoPause();
                DefaultControllerSystem.Instance.Init();
                ControllerSystem.Instance.Init();
                MIDISystem.Instance.Init();
            }
            HasPoint = hasPoint;
        }

        public void OnFitMode() => PointEntryView();

        public void OnWindowDPIModified(double windowDPI)
        {
            WindowDPI = windowDPI;
            OnModified();
            OnMove();
        }

        public void OnModified()
        {
            if (Configure.Instance.WindowedMode)
            {
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.GetWindowArea,
                    Contents = new Action<int, int, int, int>((windowAreaPosition0, windowAreaPosition1, windowAreaLength, windowAreaHeight) =>
                    {
                        Configure.Instance.WindowLengthV2 = (int)(windowAreaLength / WindowDPI);
                        Configure.Instance.WindowHeightV2 = (int)(windowAreaHeight / WindowDPI);
                    })
                });
            }
            DrawingSystem.Instance.OnModified();
        }

        public void OnMove() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.GetWindowArea,
            Contents = new Action<int, int, int, int>((windowAreaPosition0, windowAreaPosition1, windowAreaLength, windowAreaHeight) =>
            {
                Configure.Instance.WindowPosition0V2 = windowAreaPosition0;
                Configure.Instance.WindowPosition1V2 = windowAreaPosition1;
            })
        });

        public void OnTwilightCommentary(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.Commentary, new
                {
                    noteID = EntryItemValue.NoteFile.GetNoteID512(),
                    commentary = TwilightCommentary
                });
                var avatarID = TwilightSystem.Instance.AvatarID;
                var twilightCommentItem = TwilightCommentCollection.SingleOrDefault(twilightCommentItem => twilightCommentItem.AvatarWwwValue.AvatarID == avatarID);
                if (twilightCommentItem != null)
                {
                    twilightCommentItem.TwilightCommentary = string.IsNullOrEmpty(TwilightCommentary) ? string.Empty : $"💬 {TwilightCommentary}";
                }
                TwilightCommentary = string.Empty;
            }
        }

        public void OnDefaultCommentViewInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Delete && DefaultCommentItem != null)
            {
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.ViewAllowWindow,
                    Contents = new object[]
                    {
                        LanguageSystem.Instance.WipeCommentNotify,
                        MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                        new Action<MESSAGEBOX_RESULT>(r =>
                        {
                            var defaultCommentItem = DefaultCommentItem;
                             if (r == MESSAGEBOX_RESULT.IDYES && defaultCommentItem != null)
                             {
                                var defaultCommentFilePath = defaultCommentItem.CommentID;
                                var eventNoteID = EntryItemValue.EventNoteID;
                                DB.Instance.WipeComment(defaultCommentFilePath);
                                if (string.IsNullOrEmpty(eventNoteID))
                                {
                                    Utility.WipeFile(defaultCommentFilePath);
                                }
                                else
                                {
                                    Utility.WipeFile(Path.Combine(QwilightComponent.CommentEntryPath, Path.ChangeExtension(defaultCommentFilePath, ".zip")));
                                }
                                DefaultCommentCollection.Remove(defaultCommentItem);
                            }
                        })
                    }
                });
            }
        }

        public void OnWant()
        {
            _wantHandler.Stop();
            _wantHandler.Start();
        }

        public void OnPointLower(MouseButtonEventArgs e)
        {
            if (IsAvailable && IsNotModal())
            {
                var pointInput = e.ChangedButton;
                var isAlt = pointInput == MouseButton.Right;
                if (pointInput == MouseButton.Left || isAlt)
                {
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.GetWPFView,
                        Contents = new Action<IInputElement>(view =>
                        {
                            var pointPosition = e.GetPosition(view);
                            var pointPositionX = pointPosition.X;
                            var pointPositionY = pointPosition.Y;
                            var statusPoint = BaseUI.Instance.StatusPoint;
                            var inputNoteCountViewPoint = BaseUI.Instance.InputNoteCountViewPoint;
                            if (statusPoint?.Length > 4 && Utility.IsPoint(statusPoint, pointPositionX, pointPositionY))
                            {
                                var statusPosition0 = statusPoint[0];
                                var statusPosition1 = statusPoint[1];
                                var statusLength = statusPoint[2];
                                var statusHeight = statusPoint[3];
                                MoveStatus(isAlt ? -Component.LevyingWait : statusPoint[4] switch
                                {
                                    0 => 1 - (pointPositionY - statusPosition1) / statusHeight,
                                    1 => (pointPositionY - statusPosition1) / statusHeight,
                                    2 => 1 - (pointPositionX - statusPosition0) / statusLength,
                                    3 => (pointPositionX - statusPosition0) / statusLength,
                                    _ => default,
                                }, e.ClickCount == 2);
                            }
                            else if (inputNoteCountViewPoint?.Length > 2 && Utility.IsPoint(inputNoteCountViewPoint, pointPositionX, pointPositionY))
                            {
                                MoveStatus(isAlt ? -Component.LevyingWait : (pointPositionX - inputNoteCountViewPoint[0]) / inputNoteCountViewPoint[2], e.ClickCount == 2);
                            }
                        })
                    });
                }
                if (pointInput == MouseButton.Middle)
                {
                    MoveEntryView();
                    PointEntryView();
                }
            }
        }

        public void SetLastDefaultEntryItem(DefaultEntryItem defaultEntryItem)
        {
            if (!IsDefaultEntryLoading && (Configure.Instance.LastDefaultEntryItem == null ^ defaultEntryItem == null))
            {
                Configure.Instance.WantInput = string.Empty;
                Configure.Instance.LastDefaultEntryItem = defaultEntryItem;
                LoadDefaultEntryItem(false);
            }
        }

        public void OnEssentialInputLower(KeyEventArgs e)
        {
            if (ViewModels.Instance.InputValue.IsOpened || ViewModels.Instance.InputStandardValue.IsOpened || ViewModels.Instance.InputStandardControllerValue.IsOpened)
            {
                e.Handled = true;
            }
        }

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System when e.SystemKey != Key.F4:
                case Key.Escape:
                case Key.Space:
                    e.Handled = true;
                    break;
            }
        }

        public async void OnEntryViewInputLower(KeyEventArgs e)
        {
            if (IsNoteFileMode)
            {
                var rawInput = e.Key;
                if (rawInput == Key.C && Utility.HasInput(VirtualKey.LeftControl))
                {
                    var dataBundle = new DataPackage();
                    if (IsEntryItemEventNote)
                    {
                        switch (EntryItemValue.EventNoteVariety)
                        {
                            case DB.EventNoteVariety.MD5:
                                if (EntryItemValue.NoteFiles.All(noteFile => !noteFile.IsLogical))
                                {
                                    dataBundle.SetText(string.Join('/', EntryItemValue.NoteFiles.Select(noteFile => noteFile.GetNoteID512())));
                                    Clipboard.SetContent(dataBundle);
                                }
                                else
                                {
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Join(", ", EntryItemValue.NoteFiles.Where(noteFile => !noteFile.IsLogical).Select(noteFile => noteFile.Title)));
                                }
                                foreach (var noteFile in EntryItemValue.NoteFiles.Where(noteFile => !noteFile.IsLogical))
                                {
                                    await TwilightSystem.Instance.PostWwwParallel($"{QwilightComponent.QwilightAPI}/note", noteFile.GetContents());
                                }
                                break;
                            case DB.EventNoteVariety.Qwilight:
                                dataBundle.SetText(EntryItemValue.EventNoteID);
                                Clipboard.SetContent(dataBundle);
                                foreach (var noteFile in EntryItemValue.NoteFiles.Where(noteFile => !noteFile.IsLogical))
                                {
                                    await TwilightSystem.Instance.PostWwwParallel($"{QwilightComponent.QwilightAPI}/note", noteFile.GetContents());
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (IsNoteFileNotLogical)
                        {
                            dataBundle.SetText(EntryItemValue?.NoteFile?.GetNoteID512(null));
                            Clipboard.SetContent(dataBundle);
                        }
                    }
                }
                else
                {
                    var titleLetter = Utility.GetLetter(rawInput);
                    if (string.IsNullOrEmpty(titleLetter))
                    {
                        switch (rawInput)
                        {
                            case Key.Enter:
                                HandleLevyNoteFile();
                                break;
                            case Key.Left:
                                LowerNoteFile();
                                e.Handled = true;
                                break;
                            case Key.Right:
                                HigherNoteFile();
                                e.Handled = true;
                                break;
                            case Key.Back:
                                SetLastDefaultEntryItem(null);
                                break;
                            case Key.Up:
                                LowerEntryItem();
                                e.Handled = true;
                                break;
                            case Key.Down:
                                HigherEntryItem();
                                e.Handled = true;
                                break;
                            case Key.Delete:
                                if (Configure.Instance.LastDefaultEntryItem != null)
                                {
                                    var eventNoteID = EntryItemValue?.EventNoteID;
                                    if (string.IsNullOrEmpty(eventNoteID))
                                    {
                                        if (IsNoteFileNotLogical)
                                        {
                                            if (EntryItemValue.CanWipeNoteFile && !Utility.HasInput(VirtualKey.LeftShift))
                                            {
                                                WeakReferenceMessenger.Default.Send<ICC>(new()
                                                {
                                                    IDValue = ICC.ID.ViewAllowWindow,
                                                    Contents = new object[]
                                                    {
                                                        LanguageSystem.Instance.WipeNoteFileNotify,
                                                        MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                                        new Action<MESSAGEBOX_RESULT>(r =>
                                                        {
                                                            if (r == MESSAGEBOX_RESULT.IDYES)
                                                            {
                                                                var targetNoteFile = EntryItemValue.NoteFile;
                                                                Utility.WipeFile(targetNoteFile.NoteFilePath);
                                                                LoadEntryItem(targetNoteFile.DefaultEntryItem, targetNoteFile.EntryItem.EntryPath);
                                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.WipeNoteFileOK);
                                                            }
                                                        })
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                CloseAutoComputer("Default");
                                                WeakReferenceMessenger.Default.Send<ICC>(new()
                                                {
                                                    IDValue = ICC.ID.ViewAllowWindow,
                                                    Contents = new object[]
                                                    {
                                                        LanguageSystem.Instance.WipeEntryItemNotify,
                                                        MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                                        new Action<MESSAGEBOX_RESULT>(r =>
                                                        {
                                                            if (r == MESSAGEBOX_RESULT.IDYES)
                                                            {
                                                                Utility.WipeEntry(EntryItemValue.EntryPath);
                                                                LoadEntryItem(EntryItemValue.DefaultEntryItem, EntryItemValue.EntryPath);
                                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.WipeEntryOK);
                                                            }
                                                        })
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        WeakReferenceMessenger.Default.Send<ICC>(new()
                                        {
                                            IDValue = ICC.ID.ViewAllowWindow,
                                            Contents = new object[]
                                            {
                                                LanguageSystem.Instance.WipeEventNoteContents,
                                                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                                new Action<MESSAGEBOX_RESULT>(async r =>
                                                {
                                                    if (r == MESSAGEBOX_RESULT.IDYES)
                                                    {
                                                        await DB.Instance.WipeEventNote(eventNoteID);
                                                        LoadEventNoteEntryItems();
                                                        Want();
                                                    }
                                                })
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    var defaultEntryItem = EntryItemValue.DefaultEntryItem;
                                    var defaultEntryVarietyValue = defaultEntryItem?.DefaultEntryVarietyValue;
                                    if (defaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Default || defaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite)
                                    {
                                        WeakReferenceMessenger.Default.Send<ICC>(new()
                                        {
                                            IDValue = ICC.ID.ViewAllowWindow,
                                            Contents = new object[]
                                            {
                                                defaultEntryItem.WipeNotify,
                                                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                                new Action<MESSAGEBOX_RESULT>(r =>
                                                {
                                                    if (r == MESSAGEBOX_RESULT.IDYES)
                                                    {
                                                        Configure.Instance.DefaultEntryItems.Remove(defaultEntryItem);
                                                        SetDefaultEntryItems();
                                                    }
                                                })
                                            }
                                        });
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        var foundEntryItem = EntryItems.Skip(EntryItemPosition + 1).FirstOrDefault(IsSatisfy) ?? EntryItems.FirstOrDefault(IsSatisfy);
                        if (foundEntryItem != null)
                        {
                            EntryItemValue = foundEntryItem;
                        }

                        bool IsSatisfy(EntryItem entryItem) => entryItem.Title?.IsFrontCaselsss(titleLetter) == true;
                    }
                }
            }
        }

        public void OnEntryViewPointingLower(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                ViewModels.Instance.NoteFileValue.EntryItemValue = EntryItemValue;
                ViewModels.Instance.NoteFileValue.Open();
            }
        }

        public void OnDefaultCommentViewModified() => OnCommentItemModified(DefaultCommentItem);

        public void OnTwilightCommentViewModified() => OnCommentItemModified(TwilightCommentItem);

        void OnCommentItemModified(CommentItem commentItem)
        {
            IsCommentMode = commentItem != null;
            if (IsCommentMode)
            {
                ModeComponentValue.CopyAs(commentItem.ModeComponentValue, null, false);
            }
            VerifyNoteFile(ModeComponentValue.Salt);
        }

        public void OnDefaultComment()
        {
            var defaultCommentItem = DefaultCommentItem;
            if (defaultCommentItem != null)
            {
                if (EntryItemValue != null)
                {
                    try
                    {
                        IsCommentMode = false;
                        var defaultModeComponentValue = ModeComponentValue.Clone();
                        ModeComponentValue.CopyAs(defaultCommentItem.ModeComponentValue, null, false);
                        if (string.IsNullOrEmpty(EntryItemValue.EventNoteID))
                        {
                            using var fs = File.OpenRead(Path.Combine(QwilightComponent.CommentEntryPath, defaultCommentItem.CommentID));
                            SetQuitMode(new QuitCompute(new[] { EntryItemValue.NoteFile }, new[] { Comment.Parser.ParseFrom(fs) }, defaultModeComponentValue, defaultCommentItem, null));
                        }
                        else
                        {
                            using var zipFile = ZipFile.Read(Path.Combine(QwilightComponent.CommentEntryPath, Path.ChangeExtension(defaultCommentItem.CommentID, ".zip")));
                            SetQuitMode(new QuitCompute(EntryItemValue.NoteFiles, Enumerable.Range(0, EntryItemValue.NoteFiles.Length).Select(i =>
                            {
                                var zipEntry = zipFile[i.ToString()];
                                using var rms = PoolSystem.Instance.GetDataFlow((int)zipEntry.UncompressedSize);
                                zipEntry.Extract(rms);
                                rms.Position = 0;
                                return Comment.Parser.ParseFrom(rms);
                            }).ToArray(), defaultModeComponentValue, defaultCommentItem, EntryItemValue));
                        }
                    }
                    catch (Exception e)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.FaultCommentContents, e.Message));
                    }
                }
            }
        }

        public async void OnTwilightComment()
        {
            var noteID = EntryItemValue?.NoteFile?.GetNoteID512();
            var commentID = TwilightCommentItem?.CommentID;
            if (!string.IsNullOrEmpty(noteID) && !string.IsNullOrEmpty(commentID))
            {
                using var s = await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/comment?noteID={noteID}&commentID={commentID}");
                var noteFile = EntryItemValue?.NoteFile;
                var twilightCommentItem = TwilightCommentItem;
                if (IsNoteFileMode && noteFile?.GetNoteID512() == noteID && twilightCommentItem?.CommentID == commentID)
                {
                    if (s.Length > 0)
                    {
                        IsCommentMode = false;
                        var defaultModeComponentValue = ModeComponentValue.Clone();
                        ModeComponentValue.CopyAs(twilightCommentItem.ModeComponentValue, null, false);
                        SetQuitMode(new QuitCompute(new[] { noteFile }, new[] { Comment.Parser.ParseFrom(s) }, defaultModeComponentValue, twilightCommentItem, null));
                    }
                }
            }
        }

        public void OnLevyNoteFile(MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2 && e.ChangedButton == MouseButton.Left)
            {
                HandleLevyNoteFile();
            }
        }

        [RelayCommand]
        static void OnSignIn() => ViewModels.Instance.SignInValue.Open();

        [RelayCommand]
        static void OnLevelWindow() => ViewModels.Instance.LevelValue.Open();

        [RelayCommand]
        static void OnWantWindow() => ViewModels.Instance.WantValue.Open();

        [RelayCommand]
        static void OnViewMyAvatar()
        {
            var avatarViewModel = ViewModels.Instance.AvatarValue;
            avatarViewModel.AvatarID = TwilightSystem.Instance.AvatarID;
            avatarViewModel.Open();
        }

        [RelayCommand]
        static void OnViewMyBundle() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallBundle, TwilightSystem.Instance.AvatarID);

        [RelayCommand]
        static void OnNotSignIn() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewAllowWindow,
            Contents = new object[]
            {
                LanguageSystem.Instance.NotSignInNotify,
                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                new Action<MESSAGEBOX_RESULT>(r =>
                {
                    if (r == MESSAGEBOX_RESULT.IDYES)
                    {
                        Configure.Instance.SetCipher(string.Empty);
                        Configure.Instance.AutoSignIn = false;
                        TwilightSystem.Instance.SendParallel<object>(Event.Types.EventID.NotSignIn, null);
                    }
                })
            }
        });

        [RelayCommand]
        void OnViewFile()
        {
            var noteFilePath = EntryItemValue?.NoteFile?.NoteFilePath;
            if (string.IsNullOrEmpty(noteFilePath))
            {
                var lastDefaultEntryItem = Configure.Instance.LastDefaultEntryItem;
                if (lastDefaultEntryItem != null)
                {
                    if (lastDefaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Default || lastDefaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Essential)
                    {
                        Utility.OpenAs(lastDefaultEntryItem.DefaultEntryPath);
                    }
                }
                else
                {
                    Utility.OpenAs(EntryItemValue?.DefaultEntryItem?.DefaultEntryPath);
                }
            }
            else
            {
                Utility.OpenAs(Path.GetDirectoryName(noteFilePath));
            }
        }

        [RelayCommand]
        void OnViewAssistFile()
        {
            var noteFile = EntryItemValue.NoteFile;
            var assistFilePath = Path.Combine(noteFile.EntryItem.EntryPath, noteFile.AssistFileName);
            if (File.Exists(assistFilePath))
            {
                var format = DB.Instance.GetFormat(noteFile);
                if (format == -1)
                {
                    var formatComputer = CharsetDetector.DetectFromFile(assistFilePath).Detected;
                    format = formatComputer != null && formatComputer.Confidence >= 0.875 && formatComputer.Encoding != null ? formatComputer.Encoding.CodePage : 932;
                }
                var assistFileViewModel = ViewModels.Instance.AssistFileValue;
                assistFileViewModel.Title = noteFile.EntryItem.Title;
                assistFileViewModel.Assist = File.ReadAllText(assistFilePath, Encoding.GetEncoding(format));
                assistFileViewModel.Open();
            }
        }

        [RelayCommand]
        static void OnModeComponent(int? e)
        {
            if (e.HasValue)
            {
                ViewModels.Instance.ModifyModeComponentValue.ModeComponentVariety = e.Value;
                ViewModels.Instance.ModifyModeComponentValue.Open();
            }
        }

        [RelayCommand]
        void OnPause() => Pause();

        public void Pause()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (AutoComputer != null)
                        {
                            AutoComputer.SetPause = !AutoComputer.SetPause;
                        }
                        else
                        {
                            _pausableAudioHandler.IsPausing = !_pausableAudioHandler.IsPausing;
                        }
                        break;
                    case Mode.Computing:
                        if (Computer.CanSetPosition)
                        {
                            Computer.SetPause = !Computer.SetPause;
                        }
                        break;
                }
            }
        }

        [RelayCommand]
        static void OnAutoSalt() => Configure.Instance.SaltAuto = !Configure.Instance.SaltAuto;

        [RelayCommand]
        static void OnConfigure() => ViewModels.Instance.ConfigureValue.Open();

        [RelayCommand]
        static void OnComment() => ViewModels.Instance.CommentValue.Open();

        [RelayCommand]
        void OnLowerTwilightCommentFavor()
        {
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetFavor, new
            {
                noteID = EntryItemValue.NoteFile.GetNoteID512(),
                favor = TwilightCommentFavor == false ? null : false as bool?
            });
            LoadTwilightCommentCollection();
        }

        [RelayCommand]
        void OnHigherTwilightCommentFavor()
        {
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetFavor, new
            {
                noteID = EntryItemValue.NoteFile.GetNoteID512(),
                favor = TwilightCommentFavor == true ? null : true as bool?
            });
            LoadTwilightCommentCollection();
        }

        public MainViewModel()
        {
            _loadDefaultCommentHandler.Tick += async (sender, e) =>
            {
                (sender as DispatcherTimer).Stop();
                if (EntryItemValue != null)
                {
                    var targetNoteFiles = EntryItemValue.NoteFiles;
                    var commentItems = await (IsEntryItemEventNote ? DB.Instance.GetCommentItems(targetNoteFiles[0], EntryItemValue.EventNoteID, targetNoteFiles.Length) : DB.Instance.GetCommentItems(EntryItemValue.NoteFile, EntryItemValue.EventNoteID, 1));

                    DefaultCommentCollection.Clear();
                    foreach (var commentItem in commentItems)
                    {
                        DefaultCommentCollection.Add(commentItem);
                    }
                    IsDefaultCommentLoading = false;
                }
            };
            _loadTwilightCommentHandler.Tick += async (sender, e) =>
            {
                (sender as DispatcherTimer).Stop();
                var noteFile = EntryItemValue?.NoteFile;
                if (noteFile != null)
                {
                    var noteID = noteFile.GetNoteID512();
                    var twilightWwwComment = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwComment?>($"{QwilightComponent.QwilightAPI}/comment?noteID={noteID}&avatarID={TwilightSystem.Instance.AvatarID}&language={Configure.Instance.Language}&target={Configure.Instance.UbuntuNetItemTarget}");
                    if (twilightWwwComment.HasValue)
                    {
                        var twilightWwwCommentValue = twilightWwwComment.Value;
                        var comments = twilightWwwCommentValue.comments;
                        if (comments == null)
                        {
                            noteFile.IsBanned = true;
                            OnPropertyChanged(nameof(IsEntryItemBanned));
                        }
                        else
                        {
                            noteFile = EntryItemValue?.NoteFile;
                            if (noteFile?.GetNoteID512() == noteID)
                            {
                                var commentItems = Utility.GetCommentItems(comments, noteFile);
                                TwilightCommentTotalFavor = twilightWwwCommentValue.totalFavor.ToString("👍 #,##0");
                                TwilightCommentFavor = twilightWwwCommentValue.favor;
                                HandlingUISystem.Instance.HandleParallel(() =>
                                {
                                    TwilightCommentCollection.Clear();
                                    foreach (var commentItem in commentItems)
                                    {
                                        TwilightCommentCollection.Add(commentItem);
                                    }
                                });
                                var targetComment = commentItems.Where(comment => comment.AvatarWwwValue.AvatarID == TwilightSystem.Instance.AvatarID).SingleOrDefault();
                                if (targetComment != null)
                                {
                                    TwilightCommentText0 = (Array.IndexOf(commentItems, targetComment) + 1).ToString("＃#,##0");
                                    TwilightCommentText1 = commentItems.Length.ToString("／#,##0");
                                    if (noteFile.HandledValue != BaseNoteFile.Handled.Band1)
                                    {
                                        if (targetComment.IsP)
                                        {
                                            noteFile.HandledValue = BaseNoteFile.Handled.Band1;
                                        }
                                        else if (targetComment.ModeComponentValue.HitPointsModeValue == ModeComponent.HitPointsMode.Highest)
                                        {
                                            noteFile.HandledValue = BaseNoteFile.Handled.HighestClear;
                                        }
                                        else if (targetComment.ModeComponentValue.HitPointsModeValue == ModeComponent.HitPointsMode.Higher && noteFile.HandledValue != BaseNoteFile.Handled.HighestClear)
                                        {
                                            noteFile.HandledValue = BaseNoteFile.Handled.HigherClear;
                                        }
                                        else if (noteFile.HandledValue != BaseNoteFile.Handled.HigherClear && noteFile.HandledValue != BaseNoteFile.Handled.HighestClear)
                                        {
                                            noteFile.HandledValue = BaseNoteFile.Handled.Clear;
                                        }
                                    }
                                    DB.Instance.SetHandled(noteFile);
                                }
                            }
                        }
                    }
                    IsTwilightCommentLoading = false;
                }
            };
            _autoComputerHandler.Tick += (sender, e) =>
            {
                (sender as DispatcherTimer).Stop();
                if (IsNoteFileMode)
                {
                    HandleAutoComputerImmediately(true);
                }
            };
            _wantHandler.Tick += (sender, e) =>
            {
                (sender as DispatcherTimer).Stop();
                Want();
            };
            _fsw.Renamed += (sender, e) =>
            {
                if (e.OldFullPath.IsTailCaselsss(".crdownload") && QwilightComponent.BundleFileFormats.Any(format => e.FullPath.IsTailCaselsss(format)))
                {
                    var filePath = e.FullPath;
                    Utility.WaitUntilCanOpen(filePath);
                    var rar = filePath.IsTailCaselsss(".rar");
                    var lzma = filePath.IsTailCaselsss(".7z");
                    if (rar || lzma)
                    {
                        using IArchive rarLZMAFile = rar ? RarArchive.Open(filePath) : SevenZipArchive.Open(filePath);
                        if (rarLZMAFile.Entries.All(rarLZMAEntry => QwilightComponent.NoteFileFormats.Any(format => rarLZMAEntry.Key.IsTailCaselsss(format))))
                        {
                            Utility.OpenAs(filePath);
                        }
                        else
                        {
                            HandleNoteBundle(filePath);
                        }
                    }
                    else
                    {
                        using var zipFile = ZipFile.Read(filePath);
                        if (zipFile.All(zipEntry => QwilightComponent.NoteFileFormats.Any(format => zipEntry.FileName.IsTailCaselsss(format))))
                        {
                            Utility.OpenAs(filePath);
                        }
                        else
                        {
                            HandleNoteBundle(filePath);
                        }
                    }
                }
            };
        }

        public void HandleLevyNoteFile(EntryItem entryItemValue = null, WwwLevelData wwwLevelDataValue = null, ModeComponent defaultModeComponentValue = null)
        {
            if (entryItemValue != null || HasNotInput())
            {
                entryItemValue ??= EntryItemValue;

                var onLevyNoteFile = entryItemValue?.NoteFile?.OnLevyNoteFile;
                if (onLevyNoteFile != null)
                {
                    onLevyNoteFile();
                }
                else
                {
                    if (ViewModels.Instance.HasSiteViewModel(siteViewModel => siteViewModel.IsNetSite))
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.DefaultComputeInNetSiteFault);
                    }
                    else
                    {
                        var isSaltNoteFile = entryItemValue?.NoteFile is SaltNoteFile;
                        if (isSaltNoteFile)
                        {
                            SaltEntryView();
                            entryItemValue = EntryItemValue;
                        }

                        if (entryItemValue != null)
                        {
                            var eventNoteID = entryItemValue.EventNoteID;
                            var noteFiles = entryItemValue.NoteFiles;
                            var targetNoteFile = string.IsNullOrEmpty(eventNoteID) ? entryItemValue.NoteFile : noteFiles.First();
                            if (string.IsNullOrEmpty(eventNoteID))
                            {
                                Utility.HandleUIAudio("Levy Note File");
                                IsCommentMode = false;
                                ModeComponentValue.ComputingValue = targetNoteFile;
                                ModeComponentValue.CanModifyMultiplier = true;
                                ModeComponentValue.CanModifyAudioMultiplier = true;
                                SetComputingMode(new(new[] { targetNoteFile }, null, defaultModeComponentValue, TwilightSystem.Instance.AvatarID, TwilightSystem.Instance.GetAvatarName(), wwwLevelDataValue, null, null, null));
                            }
                            else
                            {
                                if (noteFiles.Any(noteFile => string.IsNullOrEmpty(noteFile.NoteFilePath)))
                                {
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvailableEventNoteFileFault);
                                }
                                else
                                {
                                    Utility.HandleUIAudio("Levy Note File");
                                    IsCommentMode = false;
                                    ModeComponentValue.ComputingValue = targetNoteFile;
                                    ModeComponentValue.CanModifyMultiplier = true;
                                    ModeComponentValue.CanModifyAudioMultiplier = true;
                                    SetComputingMode(new(noteFiles, null, defaultModeComponentValue, TwilightSystem.Instance.AvatarID, TwilightSystem.Instance.GetAvatarName(), wwwLevelDataValue, null, entryItemValue, null));
                                }
                            }
                        }

                        if (isSaltNoteFile)
                        {
                            EntryItemPosition = 1;
                        }
                    }
                }
            }
        }

        public void Close()
        {
            _fsw.Dispose();

            IsCommentMode = false;
            var defaultModeComponentValue = Computer?.DefaultModeComponentValue;
            if (defaultModeComponentValue != null)
            {
                ModeComponentValue.CopyAs(defaultModeComponentValue);
            }

            Configure.Instance.Save();
            GPUConfigure.Instance.Save();
            FastDB.Instance.Save();

            AudioSystem.Instance.Dispose();
            AudioInputSystem.Instance.Dispose();
            TwilightSystem.Instance.Dispose();
            IlluminationSystem.Instance.Dispose();
            ValveSystem.Instance.Dispose();

            Utility.ModifyHwMode(QwilightComponent.DefaultHwMode);

            PIDClass.Instance.Dispose();

            if (File.Exists(_qwilightFileName))
            {
                if (_qwilightFileName.IsTailCaselsss(".zip"))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(Path.Combine(QwilightComponent.UtilityEntryPath, "Igniter.exe"), $"\"{_qwilightFileName}\"")
                        {
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        Utility.OpenAs(_qwilightFileName);
                    }
                }
                else
                {
                    Utility.OpenAs(_qwilightFileName);
                }
            }
        }

        void WipeEntryItems(Func<EntryItem, bool> wipeEntryItemCondition, Func<BaseNoteFile, bool> wipeNoteIDCondition, bool wipeEventNoteEntryItems)
        {
            foreach (var (entryItemID, entryItem) in _entryItems)
            {
                if (wipeEntryItemCondition(entryItem))
                {
                    _entryItems.Remove(entryItemID, out _);
                }
            }
            if (wipeNoteIDCondition != null)
            {
                foreach (var (noteID, noteFile) in NoteID512s)
                {
                    if (wipeNoteIDCondition(noteFile))
                    {
                        NoteID512s.Remove(noteID, out _);
                    }
                }
                foreach (var (noteID, noteFile) in NoteID128s)
                {
                    if (wipeNoteIDCondition(noteFile))
                    {
                        NoteID128s.Remove(noteID, out _);
                    }
                }
            }
            if (wipeEventNoteEntryItems)
            {
                EventNoteEntryItems.Clear();
            }
        }

        public void LoadEventNoteEntryItems()
        {
            WipeEntryItems(entryItem => !string.IsNullOrEmpty(entryItem.EventNoteID), null, true);
            foreach (var (eventNoteID, eventNoteName, eventNoteDate, eventNoteVariety) in DB.Instance.GetEventNote())
            {
                try
                {
                    var noteIDs = eventNoteVariety switch
                    {
                        DB.EventNoteVariety.Qwilight => NoteID512s,
                        DB.EventNoteVariety.MD5 => NoteID128s,
                        _ => default,
                    };
                    var entryItem = new EntryItem
                    {
                        Title = eventNoteName,
                        EventNoteID = eventNoteID,
                        EventNoteName = eventNoteName,
                        EventNoteVariety = eventNoteVariety,
                        ModifiedDate = eventNoteDate,
                        EntryItemID = Interlocked.Increment(ref _lastEntryItemID)
                    };
                    var noteFiles = eventNoteID.Split('/').Select(noteID => noteIDs.TryGetValue(noteID, out var noteFile) ? noteFile : new NotAvailableNoteFile(noteID, entryItem.DefaultEntryItem, entryItem));
                    var (latestDate, handledCount) = DB.Instance.GetDate(default, eventNoteID);
                    entryItem.LatestDate = latestDate;
                    entryItem.HandledCount = handledCount;
                    entryItem.NoteFiles = noteFiles.ToArray();
                    entryItem.WellNoteFiles = noteFiles.ToList();
                    _entryItems[entryItem.EntryItemID] = entryItem;
                    EventNoteEntryItems[eventNoteID] = entryItem;
                }
                catch
                {
                }
            }
        }

        public void HandleNoteBundle(string filePath)
        {
            var savingBundleItem = new NotifyItem
            {
                Text = LanguageSystem.Instance.SavingFileContents,
                Variety = NotifySystem.NotifyVariety.Levying,
                OnStop = isTotal => false,
            };
            try
            {
                HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.NotifyValue.NotifyItemCollection.Insert(0, savingBundleItem));
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                var bundleEntryItem = Configure.Instance.LastDefaultEntryItem ?? DefaultEntryItem.EssentialBundle;
                if (bundleEntryItem.DefaultEntryVarietyValue != DefaultEntryItem.DefaultEntryVariety.Default && bundleEntryItem.DefaultEntryVarietyValue != DefaultEntryItem.DefaultEntryVariety.Essential)
                {
                    bundleEntryItem = DefaultEntryItem.EssentialBundle;
                }
                var bundleEntryPath = Path.Combine(bundleEntryItem.DefaultEntryPath, Path.GetFileNameWithoutExtension(filePath));
                Directory.CreateDirectory(bundleEntryPath);
                var rar = filePath.IsTailCaselsss(".rar");
                var lzma = filePath.IsTailCaselsss(".7z");
                if (rar || lzma)
                {
                    using IArchive rarLZMAFile = rar ? RarArchive.Open(filePath) : SevenZipArchive.Open(filePath);
                    foreach (var rarLZMAEntry in rarLZMAFile.Entries)
                    {
                        try
                        {
                            rarLZMAEntry.WriteToDirectory(bundleEntryPath, new()
                            {
                                ExtractFullPath = true
                            });
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    using var zipFile = ZipFile.Read(filePath, new()
                    {
                        Encoding = Encoding.GetEncoding(932)
                    });
                    zipFile.ExtractProgress += (sender, e) =>
                    {
                        savingBundleItem.LevyingStatus = e.EntriesExtracted;
                        savingBundleItem.QuitStatus = e.EntriesTotal;
                        savingBundleItem.NotifyBundleStatus();
                    };
                    zipFile.ExtractAll(bundleEntryPath, ExtractExistingFileAction.OverwriteSilently);
                }
                LoadEntryItem(bundleEntryItem, bundleEntryPath);
                savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                savingBundleItem.Text = LanguageSystem.Instance.SavedFileContents;
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, savingBundleItem.Text);
            }
            catch (Exception e)
            {
                savingBundleItem.Variety = NotifySystem.NotifyVariety.Stopped;
                savingBundleItem.Text = string.Format(LanguageSystem.Instance.SaveFileFault, e.Message);
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, savingBundleItem.Text);
            }
            finally
            {
                savingBundleItem.OnStop = isTotal => true;
            }
        }

        void MoveStatus(double status, bool isEnter)
        {
            if (!_autoComputerHandler.IsEnabled && AutoComputer?.IsHandling == true)
            {
                AutoComputer.LevyingWait = AutoComputer.Length * status;
                AutoComputer.SetUndo = true;
                if (isEnter)
                {
                    EnterAutoComputingMode();
                }
            }
        }

        public void EnterAutoComputingMode()
        {
            if (!IsComputingMode)
            {
                Fade(() =>
                {
                    Computer = AutoComputer;
                    ModeValue = Mode.Computing;
                    Configure.Instance.NotifyTutorial(Configure.TutorialID.ModifyAutoMode);
                }, AutoComputer, true, 2);
            }
        }

        public void LoadDefaultEntryItem(bool isF5)
        {
            var lastDefaultEntryItem = Configure.Instance.LastDefaultEntryItem;
            if (lastDefaultEntryItem == null)
            {
                Want();
            }
            else
            {
                _setCancelDefaultEntryItem = new();
                IsDefaultEntryLoading = true;
                Task.Run(() =>
                {
                    using (_setCancelDefaultEntryItem)
                    {
                        try
                        {
                            var lastDefaultEntryPath = lastDefaultEntryItem.DefaultEntryPath;
                            switch (lastDefaultEntryItem.DefaultEntryVarietyValue)
                            {
                                case DefaultEntryItem.DefaultEntryVariety.Total:
                                    foreach (var defaultEntryPath in DefaultEntryItems.Where(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Essential || defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Default))
                                    {
                                        LoadDefaultEntryItemImpl(defaultEntryPath);
                                    }
                                    break;
                                case DefaultEntryItem.DefaultEntryVariety.Essential:
                                case DefaultEntryItem.DefaultEntryVariety.Default:
                                    LoadDefaultEntryItemImpl(lastDefaultEntryItem);
                                    break;
                                case DefaultEntryItem.DefaultEntryVariety.Favorite:
                                    foreach (var frontEntryPath in lastDefaultEntryItem.FrontEntryPaths)
                                    {
                                        LoadDefaultEntryItemImpl(frontEntryPath == QwilightComponent.BundleEntryPath ? DefaultEntryItem.EssentialBundle : new()
                                        {
                                            DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Default,
                                            DefaultEntryPath = frontEntryPath
                                        });
                                    }
                                    break;
                            }

                            void LoadDefaultEntryItemImpl(DefaultEntryItem defaultEntryItem)
                            {
                                var defaultEntryPath = defaultEntryItem.DefaultEntryPath;
                                if (defaultEntryPath != null)
                                {
                                    var defaultEntryDate = Directory.GetLastWriteTime(defaultEntryPath);
                                    isF5 = isF5 || defaultEntryDate > FastDB.Instance.GetDefaultEntryItemDate(defaultEntryPath);
                                    if (isF5)
                                    {
                                        FastDB.Instance.SetDefaultEntryItemDate(defaultEntryPath, defaultEntryDate);
                                        FastDB.Instance.WipeDefaultEntryItem(defaultEntryPath);
                                    }
                                }
                                if ((isF5 && _alreadyLoadedDefaultEntryItems.Remove(defaultEntryItem)) || !_alreadyLoadedDefaultEntryItems.Contains(defaultEntryItem))
                                {
                                    WipeEntryItems(entryItem => defaultEntryItem == entryItem.DefaultEntryItem, noteFile => noteFile.DefaultEntryItem == defaultEntryItem, false);
                                    var defaultEntryData = FastDB.Instance.GetDefaultEntryItems(defaultEntryPath);
                                    var entryPaths = new ConcurrentBag<string>(defaultEntryData);
                                    var status = 0;
                                    var endStatus = 0;
                                    if (entryPaths.IsEmpty)
                                    {
                                        var defaultEntryPaths = Utility.GetEntry(defaultEntryPath);
                                        status = 0;
                                        endStatus = defaultEntryPaths.Length;
                                        if (Directory.Exists(defaultEntryPath))
                                        {
                                            LoadEntry(defaultEntryPath);
                                        }
                                        void LoadEntry(string targetEntryPath)
                                        {
                                            _setCancelDefaultEntryItem.Token.ThrowIfCancellationRequested();
                                            foreach (var entryPath in defaultEntryPath == targetEntryPath ? defaultEntryPaths : Utility.GetEntry(targetEntryPath))
                                            {
                                                LoadEntry(entryPath);
                                                if (defaultEntryPath == targetEntryPath)
                                                {
                                                    Status = (double)Interlocked.Increment(ref status) / endStatus;
                                                }
                                            }
                                            entryPaths.Add(targetEntryPath);
                                            FastDB.Instance.SetDefaultEntryItem(defaultEntryPath, targetEntryPath);
                                        }
                                    }
                                    status = 0;
                                    endStatus = entryPaths.Count;
                                    try
                                    {
                                        if (isF5)
                                        {
                                            foreach (var entryPath in entryPaths)
                                            {
                                                FastDB.Instance.WipeEntryItem(entryPath);
                                            }
                                        }
                                        Utility.HandleHMP(entryPaths, Configure.Instance.LoadingBin, entryPath =>
                                        {
                                            LoadEntryItem(defaultEntryItem, entryPath, _setCancelDefaultEntryItem);
                                            Status = (double)Interlocked.Increment(ref status) / endStatus;
                                        }, _setCancelDefaultEntryItem.Token);
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        throw;
                                    }
                                    catch
                                    {
                                        _alreadyLoadedDefaultEntryItems.Remove(defaultEntryItem);
                                        FastDB.Instance.WipeDefaultEntryItem(defaultEntryPath);
                                    }
                                }
                                _alreadyLoadedDefaultEntryItems.Add(defaultEntryItem);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        finally
                        {
                            IsDefaultEntryLoading = false;
                            Status = 0.0;
                            LoadEventNoteEntryItems();
                            Want();
                            NotifySystem.Instance.NotifyPending();
                            ViewModels.Instance.SiteContainerValue.SetComputingValues();
                        }
                    }
                });
            }
        }

        public void LoadEntryItem(DefaultEntryItem defaultEntryItem, string entryPath, bool wantEntryItem = true)
        {
            var targetEntryPaths = new List<string>();
            targetEntryPaths.Add(entryPath);
            if (Directory.Exists(entryPath))
            {
                LoadEntry(entryPath);
                void LoadEntry(string targetEntryPath)
                {
                    foreach (var entryPath in Utility.GetEntry(targetEntryPath))
                    {
                        LoadEntry(entryPath);
                    }
                    targetEntryPaths.Add(targetEntryPath);
                }
            }

            var wantEntryItems = wantEntryItem ? new List<EntryItem>() : null;
            foreach (var targetEntryPath in targetEntryPaths)
            {
                foreach (var entryItem in _entryItems.Values.Where(entryItem => entryItem.EntryPath == targetEntryPath))
                {
                    _entryItems.Remove(entryItem.EntryItemID, out _);
                }
                FastDB.Instance.WipeEntryItem(targetEntryPath);
                var entryItems = LoadEntryItem(defaultEntryItem, targetEntryPath, null);
                wantEntryItems?.AddRange(entryItems);
            }
            LoadEventNoteEntryItems();
            Want(wantEntryItems?.LastOrDefault());
            NotifySystem.Instance.NotifyPending();
            ViewModels.Instance.SiteContainerValue.SetComputingValues();
        }

        IEnumerable<EntryItem> LoadEntryItem(DefaultEntryItem defaultEntryItem, string entryPath, CancellationTokenSource setCancelDefaultEntryItem)
        {
            foreach (var (noteID, _) in NoteID512s.Where(pair => pair.Value.EntryItem.EntryPath == entryPath))
            {
                NoteID512s.Remove(noteID, out _);
            }
            foreach (var (noteID, _) in NoteID128s.Where(pair => pair.Value.EntryItem.EntryPath == entryPath))
            {
                NoteID128s.Remove(noteID, out _);
            }
            var tmpEntryItem = Configure.Instance.GroupEntry ? NewEntryItem() : null;
            var targetNoteFiles = new List<BaseNoteFile>();
            var fastEntryItems = FastDB.Instance.GetEntryItems(entryPath).ToArray();
            if (fastEntryItems.Length > 0)
            {
                foreach (var fastEntryItem in fastEntryItems)
                {
                    var noteFilePath = fastEntryItem.noteFilePath;
                    var noteID128 = fastEntryItem.noteID128;
                    var noteID256 = fastEntryItem.noteID256;
                    var noteID512 = fastEntryItem.noteID512;
                    for (var dataID = fastEntryItem.dataIDCount - 1; dataID >= 0; --dataID)
                    {
                        var entryItem = tmpEntryItem ?? NewEntryItem();
                        var noteFile = BaseNoteFile.GetNoteFiles(noteFilePath, defaultEntryItem, entryItem, dataID)?.Single();
                        if (noteFile != null)
                        {
                            try
                            {
                                noteFile.SetNoteIDs(noteID128, noteID256, noteID512);
                                if (!FastDB.Instance.GetNoteFile(noteFile))
                                {
                                    noteFile.SetData(Environment.TickCount, setCancelDefaultEntryItem);
                                }
                                targetNoteFiles.Add(noteFile);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception e)
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Save, $"{Path.GetFileName(noteFilePath)} ({e.Message})");
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var noteFilePath in Utility.GetFiles(entryPath).Where(noteFilePath => QwilightComponent.NoteFileFormats.Any(format => noteFilePath.IsTailCaselsss(format))))
                {
                    var entryItem = tmpEntryItem ?? NewEntryItem();
                    var noteFiles = BaseNoteFile.GetNoteFiles(noteFilePath, defaultEntryItem, entryItem, -1)?.ToArray();
                    if (noteFiles != null)
                    {
                        foreach (var noteFile in noteFiles)
                        {
                            try
                            {
                                if (!FastDB.Instance.GetNoteFile(noteFile))
                                {
                                    noteFile.SetData(Environment.TickCount, setCancelDefaultEntryItem);
                                }
                                targetNoteFiles.Add(noteFile);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception e)
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Save, $"{Path.GetFileName(noteFile.NoteFilePath)}:{noteFile.DataID} ({e.Message})");
                            }
                            finally
                            {
                                FastDB.Instance.SetEntryItem(entryPath, noteFile, noteFiles.Length);
                            }
                        }
                    }
                }
                if (targetNoteFiles.Count > 0)
                {
                    FastDB.Instance.SetEntryItem(entryPath, null, 0);
                }
            }
            if (targetNoteFiles.Count > 0)
            {
                foreach (var targetNoteFile in targetNoteFiles)
                {
                    NoteID512s[targetNoteFile.GetNoteID512()] = targetNoteFile;
                    NoteID128s[targetNoteFile.GetNoteID128()] = targetNoteFile;
                }
                if (tmpEntryItem != null)
                {
                    targetNoteFiles.Sort((x, y) =>
                    {
                        var level = x.LevelValue.CompareTo(y.LevelValue);
                        return level != 0 ? level : x.LevelTextValue.CompareTo(y.LevelTextValue);
                    });
                    tmpEntryItem.NoteFiles = targetNoteFiles.ToArray();
                    tmpEntryItem.CompatibleNoteFiles = tmpEntryItem.NoteFiles;
                    tmpEntryItem.WellNoteFiles = targetNoteFiles;
                    tmpEntryItem.NotePosition = Math.Min(DB.Instance.GetNotePosition(entryPath), targetNoteFiles.Count - 1);
                    _entryItems[tmpEntryItem.EntryItemID] = tmpEntryItem;
                    return new[] { tmpEntryItem };
                }
                else
                {
                    foreach (var targetNoteFile in targetNoteFiles)
                    {
                        targetNoteFile.EntryItem.NoteFiles = new[] { targetNoteFile };
                        targetNoteFile.EntryItem.CompatibleNoteFiles = targetNoteFiles.ToArray();
                        targetNoteFile.EntryItem.WellNoteFiles = targetNoteFile.EntryItem.NoteFiles.ToList();
                        targetNoteFile.EntryItem.NotePosition = 0;
                        _entryItems[targetNoteFile.EntryItem.EntryItemID] = targetNoteFile.EntryItem;
                    }
                    return targetNoteFiles.Select(targetNoteFile => targetNoteFile.EntryItem);
                }
            }
            else
            {
                return Array.Empty<EntryItem>();
            }

            EntryItem NewEntryItem() => new()
            {
                DefaultEntryItem = defaultEntryItem,
                ModifiedDate = Directory.GetLastWriteTime(entryPath),
                EntryPath = entryPath,
                EntryItemID = Interlocked.Increment(ref _lastEntryItemID)
            };
        }

        public void InitModeComponent()
        {
            ModeComponentValue.InitModeComponent();
            NotifyBPMText();
            NotifyHighestInputCountText();
            OnJudgmentMeterMillisModified();
            ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
            ViewModels.Instance.WwwLevelValue.WwwLevelGroupValue?.NotifyIsCompatible();
            HandleAutoComputer();
        }

        public void SaltEntryView()
        {
            var entryItemPosition = 1 + (EntryItems[1].IsLogical ? 1 : 0) + RandomNumberGenerator.GetInt32(EntryItems.Count - EntryItems.Count(entryItem => entryItem.IsLogical));
            if (entryItemPosition == EntryItemPosition)
            {
                ++entryItemPosition;
            }
            EntryItemValue = EntryItems.ElementAtOrDefault(entryItemPosition);
        }

        public void Want(EntryItem wantEntryItem = null)
        {
            if (IsNoteFileMode && !IsDefaultEntryLoading)
            {
                HandlingUISystem.Instance.HandleParallel(() =>
                {
                    var wantInput = Configure.Instance.WantInput;
                    var isNotWantInput = string.IsNullOrEmpty(wantInput);
                    var lastDefaultEntryItem = Configure.Instance.LastDefaultEntryItem;
                    var validEntryItems = new List<EntryItem>();
                    var lastDefaultEntryPath = lastDefaultEntryItem?.DefaultEntryPath ?? string.Empty;
                    if (lastDefaultEntryItem == null)
                    {
                        validEntryItems.Add(EntryItem.DefaultEntryConfigureEntryItem);
                        validEntryItems.AddRange(DefaultEntryItems.Select(defaultEntryItem => defaultEntryItem.GetEntryItem(true, Interlocked.Increment(ref _lastEntryItemID))).Where(entryItem => isNotWantInput || entryItem.Title.Contains(wantInput) || entryItem.Artist.Contains(wantInput)));
                        EntryItems = new(validEntryItems);
                        OnPropertyChanged(nameof(EntryItems));
                    }
                    else
                    {
                        var fitMode = Configure.Instance.FitModeValue;
                        var inputWantNoteVariety = Configure.Instance.InputWantNoteVariety;
                        var defaultEntryVariety = lastDefaultEntryItem.DefaultEntryVarietyValue;
                        var isTotalWantNoteVariety = Configure.Instance.IsTotalWantNoteVariety;
                        var hasEventNoteVariety = inputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote];
                        var isDefaultEntryNotFavorite = defaultEntryVariety != DefaultEntryItem.DefaultEntryVariety.Favorite;
                        var isDefaultEntryLogical = defaultEntryVariety == DefaultEntryItem.DefaultEntryVariety.Total || !isDefaultEntryNotFavorite;
                        var entryItems = _entryItems.Values.Where(entryItem =>
                        {
                            var defaultEntryItem = entryItem.DefaultEntryItem;
                            return defaultEntryItem?.DefaultEntryVarietyValue != DefaultEntryItem.DefaultEntryVariety.Net && (isDefaultEntryLogical || !string.IsNullOrEmpty(entryItem.EventNoteID) || lastDefaultEntryItem == defaultEntryItem);
                        }).ToArray();
                        var isWantBannedTotal = Configure.Instance.WantBannedValue == Configure.WantBanned.Total;
                        var inputWantMode = Configure.Instance.InputWantMode;
                        var inputWantLevel = Configure.Instance.InputWantLevel;
                        var IsNotWantLevel = !Configure.Instance.WantLevelTextValue;
                        var inputWantHandled = Configure.Instance.InputWantHandled;
                        var lowestWantLevelTextValue = Configure.Instance.LowestWantLevelTextValue;
                        var highestWantLevelTextValue = Configure.Instance.HighestWantLevelTextValue;
                        var isNotWantBPM = !Configure.Instance.WantBPM;
                        var lowestWantBPM = Configure.Instance.LowestWantBPM;
                        var highestWantBPM = Configure.Instance.HighestWantBPM;
                        var isNotWantAverageInputCount = !Configure.Instance.WantAverageInputCount;
                        var lowestWantAverageInputCount = Configure.Instance.LowestWantAverageInputCount;
                        var highestWantAverageInputCount = Configure.Instance.HighestWantAverageInputCount;
                        var isNotWantHighestInputCount = !Configure.Instance.WantHighestInputCount;
                        var lowestWantHighestInputCount = Configure.Instance.LowestWantHighestInputCount;
                        var highestWantHighestInputCount = Configure.Instance.HighestWantHighestInputCount;
                        var wantLevelIDs = Configure.Instance.WantLevelIDs;
                        var isWantLevelSystem = Configure.Instance.WantLevelSystem;
                        var isNotWantLevelItem = !Configure.Instance.WantLevelIDs.Any();
                        var isNotWantHellBPM = !Configure.Instance.WantHellBPM;
                        var levelID128s = LevelSystem.Instance.LevelID128s;
                        var levelID256s = LevelSystem.Instance.LevelID256s;
                        var levelID128NoteFiles = new Dictionary<string, LevelNoteFile>(LevelSystem.Instance.LevelID128NoteFiles);
                        var levelID256NoteFiles = new Dictionary<string, LevelNoteFile>(LevelSystem.Instance.LevelID256NoteFiles);
                        var logicalNoteFiles = new HashSet<BaseNoteFile>();
                        var titles = new List<string>();
                        var bpms = new List<double>();
                        var lengths = new List<double>();
                        var artists = new List<string>();
                        foreach (var entryItem in entryItems)
                        {
                            var isEntryItemEventNote = !string.IsNullOrEmpty(entryItem.EventNoteID);
                            var wellNoteFiles = entryItem.WellNoteFiles;
                            wellNoteFiles.Clear();
                            var noteFiles = entryItem.NoteFiles;
                            if (isEntryItemEventNote)
                            {
                                if (noteFiles.Any(noteFile => IsSatisfy(noteFile, true)))
                                {
                                    wellNoteFiles.AddRange(noteFiles);
                                    validEntryItems.Add(entryItem);
                                }
                            }
                            else
                            {
                                foreach (var noteFile in noteFiles)
                                {
                                    if (!logicalNoteFiles.Contains(noteFile) && IsSatisfy(noteFile, false))
                                    {
                                        wellNoteFiles.Add(noteFile);
                                        logicalNoteFiles.Add(noteFile);
                                    }
                                }
                                if (wellNoteFiles.Count > 0)
                                {
                                    if (!wellNoteFiles.Contains(entryItem.NoteFile))
                                    {
                                        entryItem.NotePosition = Array.IndexOf(noteFiles, wellNoteFiles.First());
                                    }
                                    validEntryItems.Add(entryItem);
                                }
                            }
                            if (wellNoteFiles.Count > 0)
                            {
                                var highestLevelTextValue = double.MinValue;
                                var highestWantLevelID = string.Empty;
                                var highestInputCount = 0;
                                var averageInputCount = 0.0;
                                var totalNotes = 0;
                                DateTime? latestDate = null;
                                var handledCount = 0;
                                titles.Clear();
                                bpms.Clear();
                                lengths.Clear();
                                artists.Clear();
                                foreach (var wellNoteFile in wellNoteFiles)
                                {
                                    levelID128NoteFiles.Remove(wellNoteFile.GetNoteID128());
                                    levelID256NoteFiles.Remove(wellNoteFile.GetNoteID256());
                                    titles.Add(Utility.GetTitle(wellNoteFile.Title));
                                    bpms.Add(wellNoteFile.BPM);
                                    artists.Add(wellNoteFile.Artist);
                                    lengths.Add(wellNoteFile.Length);
                                    totalNotes = Math.Max(totalNotes, wellNoteFile.TotalNotes);
                                    highestInputCount = Math.Max(highestInputCount, wellNoteFile.HighestInputCount);
                                    averageInputCount = Math.Max(averageInputCount, wellNoteFile.AverageInputCount);
                                    if (!isEntryItemEventNote)
                                    {
                                        if ((latestDate ?? DateTime.MinValue) < (wellNoteFile.LatestDate ?? DateTime.MinValue))
                                        {
                                            latestDate = wellNoteFile.LatestDate;
                                        }
                                        var wantLevelID = wellNoteFile.WantLevelID;
                                        if (string.IsNullOrEmpty(highestWantLevelID) || LevelSystem.Instance.WantLevelIDEquality.Compare(wantLevelID, highestWantLevelID) > 0)
                                        {
                                            highestWantLevelID = wantLevelID;
                                        }
                                        handledCount += wellNoteFile.HandledCount;
                                    }
                                    var levelTextValue = wellNoteFile.LevelTextValue;
                                    if (!double.IsNaN(levelTextValue) && highestLevelTextValue < levelTextValue)
                                    {
                                        highestLevelTextValue = levelTextValue;
                                    }
                                }
                                entryItem.Artist = Utility.GetFavoriteItem(artists);
                                entryItem.BPM = Utility.GetFavoriteItem(bpms);
                                entryItem.Length = Utility.GetFavoriteItem(lengths);
                                entryItem.TotalNotes = totalNotes;
                                entryItem.LevelTextValue = highestLevelTextValue;
                                entryItem.HighestInputCount = highestInputCount;
                                if (!isEntryItemEventNote)
                                {
                                    entryItem.Title = Utility.GetFavoriteItem(titles);
                                    entryItem.LatestDate = latestDate;
                                    entryItem.HandledCount = handledCount;
                                    entryItem.WantLevelID = highestWantLevelID;
                                }
                                fitMode.SetFittedText(entryItem);
                            }
                        }
                        fitMode.Fit(validEntryItems);
                        if (validEntryItems.Count >= 2)
                        {
                            validEntryItems.Insert(0, EntryItem.SaltEntryItem);
                        }
                        validEntryItems.Insert(0, lastDefaultEntryItem.GetEntryItem(false, Interlocked.Increment(ref _lastEntryItemID)));
                        if (isWantLevelSystem)
                        {
                            validEntryItems.AddRange(levelID128NoteFiles.Values.Concat(levelID256NoteFiles.Values).Where(levelNoteFile => IsAtLeastSatisfy(levelNoteFile, false)).Distinct().GroupBy(arg => arg.WantLevelID).OrderBy(wantLevelID => wantLevelID.Key, LevelSystem.Instance.WantLevelIDEquality).Select(levelNoteFiles => new EntryItem
                            {
                                Title = levelNoteFiles.Key,
                                NoteFiles = levelNoteFiles.ToArray(),
                                WellNoteFiles = levelNoteFiles.Cast<BaseNoteFile>().ToList(),
                                IsLogical = true,
                                EntryPath = levelNoteFiles.Key,
                                LogicalVarietyValue = EntryItem.LogicalVariety.Level,
                                EntryItemID = Interlocked.Increment(ref _lastEntryItemID)
                            }));
                        }
                        EntryItems = new(validEntryItems);
                        OnPropertyChanged(nameof(EntryItems));

                        bool IsAtLeastSatisfy(BaseNoteFile noteFile, bool isEntryItemEventNote)
                        {
                            var noteID128 = noteFile.GetNoteID128();
                            var noteID256 = noteFile.GetNoteID256();
                            if (isWantLevelSystem && levelID128s.TryGetValue(noteID128, out var levelID128) && (isNotWantLevelItem || wantLevelIDs.Contains(levelID128)))
                            {
                                noteFile.WantLevelID = levelID128;
                            }
                            else if (isWantLevelSystem && levelID256s.TryGetValue(noteID256, out var levelID256) && (isNotWantLevelItem || wantLevelIDs.Contains(levelID256)))
                            {
                                noteFile.WantLevelID = levelID256;
                            }
                            else
                            {
                                noteFile.WantLevelID = string.Empty;
                            }
                            return (!isWantLevelSystem || isNotWantLevelItem || !string.IsNullOrEmpty(noteFile.WantLevelID)) &&
                            (isTotalWantNoteVariety || (hasEventNoteVariety ? isEntryItemEventNote || inputWantNoteVariety[(int)noteFile.NoteVarietyValue] : !isEntryItemEventNote && inputWantNoteVariety[(int)noteFile.NoteVarietyValue])) &&
                            (isNotWantInput ||
                                (noteFile.Title.ContainsCaselsss(wantInput)) ||
                                (noteFile.Artist.ContainsCaselsss(wantInput)) ||
                                (noteFile.LevelText.ContainsCaselsss(wantInput)) ||
                                (noteFile.Genre.ContainsCaselsss(wantInput)) ||
                                (noteFile.Tag.ContainsCaselsss(wantInput))
                            );
                        }

                        bool IsSatisfy(BaseNoteFile noteFile, bool isEntryItemEventNote)
                        {
                            var levelTextValue = noteFile.LevelTextValue;
                            var bpm = noteFile.BPM;
                            var averageInputCount = noteFile.AverageInputCount;
                            var highestInputCount = noteFile.HighestInputCount;
                            return IsAtLeastSatisfy(noteFile, isEntryItemEventNote) &&
                            (isDefaultEntryNotFavorite || noteFile.FavoriteEntryItems.Contains(lastDefaultEntryItem)) &&
                            (isDefaultEntryLogical || noteFile.DefaultEntryItem == lastDefaultEntryItem) &&
                            (isWantBannedTotal || !noteFile.IsBanned) &&
                            inputWantMode[(int)noteFile.InputMode] &&
                            inputWantLevel[(int)noteFile.LevelValue] &&
                            inputWantHandled[(int)noteFile.HandledValue] &&
                            (IsNotWantLevel || double.IsNaN(levelTextValue) || (lowestWantLevelTextValue <= levelTextValue && levelTextValue <= highestWantLevelTextValue)) &&
                            (isNotWantBPM || (lowestWantBPM <= bpm && bpm <= highestWantBPM)) &&
                            (isNotWantAverageInputCount || (lowestWantAverageInputCount <= averageInputCount && averageInputCount <= highestWantAverageInputCount)) &&
                            (isNotWantHighestInputCount || (lowestWantHighestInputCount <= highestInputCount && highestInputCount <= highestWantHighestInputCount)) &&
                            (isNotWantHellBPM || noteFile.IsHellBPM);
                        }
                    }

                    wantEntryItem ??= LastEntryItems.GetValueOrDefault(lastDefaultEntryPath);
                    EntryItemValue = EntryItems.Contains(wantEntryItem) ? wantEntryItem : EntryItems.ElementAtOrDefault(Configure.Instance.LastEntryItemPositions.GetValueOrDefault(lastDefaultEntryPath));
                });
            }
        }

        public void FlintNoteFile(string noteFilePath, int levyingMeter)
        {
            var targetNoteFile = BaseNoteFile.GetNoteFiles(noteFilePath, null, new()
            {
                EntryPath = Path.GetDirectoryName(noteFilePath),
            }, -1).Single();
            if (targetNoteFile != null)
            {
                targetNoteFile.SetData(Environment.TickCount);

                var defaultModeComponentValue = ModeComponentValue.Clone();
                ModeComponentValue.InitModeComponent();
                ModeComponentValue.ComputingValue = targetNoteFile;
                ModeComponentValue.CanModifyMultiplier = true;
                ModeComponentValue.CanModifyAudioMultiplier = true;
                SetComputingMode(new FlintCompute(targetNoteFile, defaultModeComponentValue, TwilightSystem.Instance.AvatarID, TwilightSystem.Instance.GetAvatarName(), levyingMeter));
            }
        }

        void Fade(Action onFade, DefaultCompute fadingComputer, bool isFadingComputerStable, int fadingViewLayer)
        {
            _fadeInHandler?.Stop();

            IsAvailable = false;

            FadingValue.Computer = fadingComputer;
            FadingValue.IsComputerStable = isFadingComputerStable;
            FadingValue.Layer = fadingViewLayer;

            var millis = BaseUI.Instance.FadingPropertyValues[(int)ModeValue]?[FadingValue.Layer]?.Millis;
            var fadingCounter = Stopwatch.StartNew();
            new DispatcherTimer(QwilightComponent.StandardFrametime, DispatcherPriority.Render, (sender, e) =>
            {
                if (millis > 0)
                {
                    var millisValue = millis.Value;
                    FadingValue.Status = Math.Min(millisValue, fadingCounter.GetMillis()) / millisValue;
                }
                else
                {
                    FadingValue.Status = 1.0;
                }

                if (FadingValue.Status == 1.0)
                {
                    (sender as DispatcherTimer).Stop();

                    FadingValue.Layer = 0;
                    onFade();

                    var millis = BaseUI.Instance.FadingPropertyValues[(int)ModeValue]?[FadingValue.Layer]?.Millis;
                    fadingCounter.Restart();
                    _fadeInHandler = new(QwilightComponent.StandardFrametime, DispatcherPriority.Render, (sender, e) =>
                    {
                        if (millis > 0)
                        {
                            var millisValue = millis.Value;
                            FadingValue.Status = 1.0 - Math.Min(millisValue, fadingCounter.GetMillis()) / millisValue;
                        }
                        else
                        {
                            FadingValue.Status = 0.0;
                        }

                        if (FadingValue.Status == 0.0)
                        {
                            (sender as DispatcherTimer).Stop();

                            IsAvailable = true;
                            WeakReferenceMessenger.Default.Send<ICC>(new()
                            {
                                IDValue = ICC.ID.PointZMaxView
                            });
                        }
                    }, HandlingUISystem.Instance.UIHandler);
                }
            }, HandlingUISystem.Instance.UIHandler);
        }

        public void UndoUnitMultiplier()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        ModeComponentValue.UndoUnitMultiplier();
                        break;
                    case Mode.Computing:
                        Computer.ModeComponentValue.UndoUnitMultiplier();
                        break;
                }
            }
        }

        public void LowerMultiplier()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (ModeComponentValue.LowerMultiplier())
                        {
                            Utility.HandleUIAudio("Multiplier");
                            NotifyBPMText();
                        }
                        break;
                    case Mode.Computing:
                        Computer.LowerMultiplier();
                        break;
                }
            }
        }

        public void HigherMultiplier()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (ModeComponentValue.HigherMultiplier())
                        {
                            Utility.HandleUIAudio("Multiplier");
                            NotifyBPMText();
                        }
                        break;
                    case Mode.Computing:
                        Computer.HigherMultiplier();
                        break;
                }
            }
        }

        public void LowerAudioMultiplier()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (ModeComponentValue.LowerAudioMultiplier())
                        {
                            Utility.HandleUIAudio("Audio Multiplier");
                            NotifyBPMText();
                            NotifyHighestInputCountText();
                            HandleAutoComputer();
                            ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
                        }
                        break;
                    case Mode.Computing:
                        Computer.LowerAudioMultiplier();
                        break;
                }
            }
        }

        public void HigherAudioMultiplier()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (ModeComponentValue.HigherAudioMultiplier())
                        {
                            Utility.HandleUIAudio("Audio Multiplier");
                            NotifyBPMText();
                            NotifyHighestInputCountText();
                            HandleAutoComputer();
                            ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
                        }
                        break;
                    case Mode.Computing:
                        Computer.HigherAudioMultiplier();
                        break;
                }
            }
        }

        public void HigherDefaultSpinningMode()
        {
            if (HasNotInput())
            {
                Configure.Instance.DefaultSpinningModeValue = (Configure.DefaultSpinningMode)(((int)Configure.Instance.DefaultSpinningModeValue + 3) % 4);
            }
        }

        public void LowerDefaultSpinningMode()
        {
            if (HasNotInput())
            {
                Configure.Instance.DefaultSpinningModeValue = (Configure.DefaultSpinningMode)(((int)Configure.Instance.DefaultSpinningModeValue + 1) % 4);
            }
        }

        public void HandleModifyAutoMode()
        {
            if (HasNotInput())
            {
                Computer.ModifyAutoMode();
            }
        }

        public void HandleMediaMode()
        {
            if (HasNotInput())
            {
                Configure.Instance.Media = !Configure.Instance.Media;
                var handlingComputer = GetHandlingComputer();
                if (handlingComputer != null)
                {
                    handlingComputer.SetWait();
                    MediaSystem.Instance.HandleDefaultIfAvailable(handlingComputer);
                    MediaSystem.Instance.HandleIfAvailable(handlingComputer);
                }
            }
        }

        public void LowerEntryItem()
        {
            if (HasNotInput() && EntryItemPosition >= 1)
            {
                --EntryItemPosition;
                Utility.HandleUIAudio("Lower Entry Item");
                BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyEntryItem);
            }
        }

        public void HigherEntryItem()
        {
            if (HasNotInput() && EntryItemPosition < EntryItems.Count - 1)
            {
                ++EntryItemPosition;
                Utility.HandleUIAudio("Higher Entry Item");
                BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyEntryItem);
            }
        }

        public void LowerNoteFile()
        {
            if (HasNotInput() && EntryItemValue?.LowerNoteFile() == true)
            {
                DB.Instance.SetNotePosition(EntryItemValue);
                NotifyNoteFile();
                Utility.HandleUIAudio("Lower Note File");
                BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyNoteFile);
            }
        }

        public void HigherNoteFile()
        {
            if (HasNotInput() && EntryItemValue?.HigherNoteFile() == true)
            {
                DB.Instance.SetNotePosition(EntryItemValue);
                NotifyNoteFile();
                Utility.HandleUIAudio("Higher Note File");
                BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyNoteFile);
            }
        }

        public void HandleF1()
        {
            if (HasNotInput(ViewModels.Instance.AssistValue))
            {
                ViewModels.Instance.AssistValue.Toggle();
            }
        }

        public void HandleF5()
        {
            if (HasNotInput() && !IsDefaultEntryLoading)
            {
                var lastDefaultEntryItem = Configure.Instance.LastDefaultEntryItem;
                if (lastDefaultEntryItem != null)
                {
                    if (EntryItemValue != null && !EntryItemValue.IsLogical && string.IsNullOrEmpty(EntryItemValue.EventNoteID))
                    {
                        WeakReferenceMessenger.Default.Send((ICC)new()
                        {
                            IDValue = ICC.ID.ViewAllowWindow,
                            Contents = new object[]
                            {
                                string.Format(LanguageSystem.Instance.F5Notify1, lastDefaultEntryItem, Path.GetFileName(EntryItemValue.EntryPath)),
                                MESSAGEBOX_STYLE.MB_YESNOCANCEL | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                new Action<MESSAGEBOX_RESULT>(r =>
                                {
                                    switch (r)
                                    {
                                        case MESSAGEBOX_RESULT.IDYES:
                                            LoadDefaultEntryItem(true);
                                            break;
                                        case MESSAGEBOX_RESULT.IDNO:
                                            LoadEntryItem(EntryItemValue.DefaultEntryItem, EntryItemValue.EntryPath);
                                            break;
                                    }
                                })
                            }
                        });
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send((ICC)new()
                        {
                            IDValue = ICC.ID.ViewAllowWindow,
                            Contents = new object[]
                            {
                                string.Format(LanguageSystem.Instance.F5Notify0, Configure.Instance.LastDefaultEntryItem),
                                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                new Action<MESSAGEBOX_RESULT>(r =>
                                {
                                    if (r == MESSAGEBOX_RESULT.IDYES)
                                    {
                                        LoadDefaultEntryItem(true);
                                    }
                                })
                            }
                        });
                    }
                }
            }
        }

        public void HandleF6()
        {
            if (IsComputingMode)
            {
                Utility.SetUIItem(Configure.Instance.UIItemValue, Configure.Instance.UIItemValue);
            }
            else
            {
                Utility.SetBaseUIItem(Configure.Instance.BaseUIItemValue, Configure.Instance.BaseUIItemValue);
            }
        }

        public void HandleF7()
        {
            if (HasNotInput(ViewModels.Instance.VoteValue))
            {
                ViewModels.Instance.VoteValue.Toggle();
            }
        }

        public void HandleF8()
        {
            if (TwilightSystem.Instance.IsEstablished)
            {
                ViewModels.Instance.SiteContainerValue.Toggle();
            }
        }

        public void HandleF9()
        {
            if (IsNoteFileMode)
            {
                var favoriteEntryItems = Configure.Instance.DefaultEntryItems.Where(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite).ToArray();
                switch (favoriteEntryItems.Length)
                {
                    case 0:
                        ViewModels.Instance.ModifyDefaultEntryValue.Open();
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotFavoritesF10);
                        break;
                    case 1:
                        DefaultEntryItem favoriteEntryItemModified = null;
                        var targetFavoriteEntryItem = favoriteEntryItems.Single();
                        var noteFiles = EntryItemValue.NoteFiles;
                        var setFavorites = noteFiles.Any(noteFile => !noteFile.FavoriteEntryItems.Contains(targetFavoriteEntryItem));
                        foreach (var noteFile in noteFiles)
                        {
                            if (!noteFile.IsLogical)
                            {
                                if (setFavorites)
                                {
                                    if (noteFile.FavoriteEntryItems.Add(targetFavoriteEntryItem))
                                    {
                                        favoriteEntryItemModified = targetFavoriteEntryItem;
                                    }
                                }
                                else
                                {
                                    if (noteFile.FavoriteEntryItems.Remove(targetFavoriteEntryItem))
                                    {
                                        favoriteEntryItemModified = targetFavoriteEntryItem;
                                    }
                                }
                                foreach (var favoriteEntryItem in noteFile.FavoriteEntryItems)
                                {
                                    favoriteEntryItem.FrontEntryPaths.Add(noteFile.DefaultEntryItem.DefaultEntryPath);
                                }
                                DB.Instance.SetFavoriteEntry(noteFile);
                            }
                        }
                        if (favoriteEntryItemModified != null)
                        {
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(setFavorites ? LanguageSystem.Instance.SetFavoritesF10 : LanguageSystem.Instance.WipeFavoritesF10, favoriteEntryItemModified.FavoriteEntryName));
                            if (favoriteEntryItemModified == Configure.Instance.LastDefaultEntryItem)
                            {
                                Want();
                            }
                        }
                        break;
                    default:
                        HandlingUISystem.Instance.HandleParallel(() =>
                        {
                            var favoriteEntryViewModel = ViewModels.Instance.FavoriteEntryValue;
                            if (!favoriteEntryViewModel.IsOpened)
                            {
                                favoriteEntryViewModel.EntryItem = EntryItemValue;
                                favoriteEntryViewModel.Mode = 1;
                            }
                            favoriteEntryViewModel.Toggle();
                        });
                        break;
                }
            }
        }

        public void HandleF10()
        {
            if (TwilightSystem.Instance.IsSignedIn && HasNotInput(ViewModels.Instance.WwwLevelValue))
            {
                ViewModels.Instance.WwwLevelValue.Toggle();
            }
        }

        public void HandleF11()
        {
            if (HasNotInput(ViewModels.Instance.NotifyValue))
            {
                ViewModels.Instance.NotifyValue.Toggle();
            }
        }

        public void HandleF12() => StillSystem.Instance.Save();

        public void HandleESC()
        {
            if (IsAvailable)
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                        if (CloseOpenedViewModel())
                        {
                            return;
                        }
                        if (IsCommentMode)
                        {
                            DefaultCommentItem = null;
                            TwilightCommentItem = null;
                            PointEntryView();
                            return;
                        }
                        if (IsDefaultEntryLoading)
                        {
                            _setCancelDefaultEntryItem.Cancel();
                            return;
                        }
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.Quit,
                            Contents = true
                        });
                        break;
                    case Mode.Computing:
                        if (!CloseOpenedViewModel())
                        {
                            if (string.IsNullOrEmpty(UI.Instance.FaultText))
                            {
                                if (Computer.IsPausingWindowOpened)
                                {
                                    Computer.Unpause();
                                }
                                else
                                {
                                    Computer.Pause();
                                }
                            }
                            else
                            {
                                Computer.SetNoteFileMode();
                            }
                        }
                        break;
                    case Mode.Quit:
                        if (!CloseOpenedViewModel())
                        {
                            Computer.SetNoteFileMode();
                        }
                        break;
                }

                bool CloseOpenedViewModel()
                {
                    var zMaxValue = int.MinValue;
                    BaseViewModel zMaxViewModel = null;
                    foreach (var windowViewModel in ViewModels.Instance.WindowViewModels)
                    {
                        var zValue = windowViewModel.Zvalue;
                        if (windowViewModel.IsOpened && zMaxValue < zValue)
                        {
                            zMaxValue = zValue;
                            zMaxViewModel = windowViewModel;
                        }
                    }
                    if (zMaxViewModel != null)
                    {
                        zMaxViewModel.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void HandleShift(bool isInput)
        {
            if (HasNotInput())
            {
                if (isInput)
                {
                    if (_wasLowerMillis && !IsCommentMode)
                    {
                        _wasLowerMillis = false;
                        var millis = Environment.TickCount;
                        if (_lastLowerMillis.HasValue)
                        {
                            if (millis - _lastLowerMillis.Value > 500)
                            {
                                _lastLowerMillis = millis;
                            }
                            else
                            {
                                InitModeComponent();
                                _lastLowerMillis = null;
                            }
                        }
                        else
                        {
                            _lastLowerMillis = millis;
                        }
                    }
                }
                else
                {
                    _wasLowerMillis = true;
                }
            }
        }

        public void HandleSpace()
        {
            if (HasNotInput(ViewModels.Instance.ConfigureValue))
            {
                switch (ModeValue)
                {
                    case Mode.NoteFile:
                    case Mode.Computing when Computer.IsPausingWindowOpened:
                    case Mode.Quit:
                        ViewModels.Instance.ConfigureValue.Toggle();
                        break;
                }
            }
        }

        public void HandleEnter()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.Computing:
                        if (Computer.IsPausingWindowOpened)
                        {
                            switch (Configure.Instance.DefaultSpinningModeValue)
                            {
                                case Configure.DefaultSpinningMode.Unpause:
                                    Computer.Unpause();
                                    break;
                                case Configure.DefaultSpinningMode.Configure:
                                    ViewModels.Instance.ConfigureValue.Open();
                                    break;
                                case Configure.DefaultSpinningMode.Undo when Computer.CanUndo:
                                    Computer.SetUndo = true;
                                    break;
                                case Configure.DefaultSpinningMode.Stop:
                                    Computer.Unpause();
                                    Computer.SetNoteFileMode();
                                    break;
                            }
                        }
                        else
                        {
                            if (Computer.IsPassable && !Computer.IsPausing)
                            {
                                Computer.SetPass = true;
                            }
                        }
                        break;
                    case Mode.Quit:
                        Computer.SetNoteFileMode();
                        break;
                }
            }
        }

        public void HandleInitComment()
        {
            if (HasNotInput())
            {
                if (ViewModels.Instance.HasSiteViewModel(siteViewModel => siteViewModel.IsNetSite))
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.DefaultComputeInNetSiteFault);
                }
                else
                {
                    IsCommentMode = false;
                    var defaultModeComponentValue = Computer.DefaultModeComponentValue ?? ModeComponentValue.Clone();
                    var defaultMultiplierValue = defaultModeComponentValue.MultiplierValue;
                    ModeComponentValue.CopyAs(Computer.ModeComponentValue, Computer.MyNoteFiles.First());
                    ModeComponentValue.CanModifyMultiplier = true;
                    ModeComponentValue.MultiplierValue = defaultMultiplierValue;
                    ModeComponentValue.CanModifyAudioMultiplier = true;
                    SetComputingMode(new(Computer.MyNoteFiles, null, defaultModeComponentValue, TwilightSystem.Instance.AvatarID, TwilightSystem.Instance.GetAvatarName(), Computer.WwwLevelDataValue, null, Computer.EventNoteEntryItem));
                }
            }
        }

        public void HandleViewComment()
        {
            if (HasNotInput())
            {
                if (Computer.IsPostableItemMode)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.PostableItemModeViewCommentFault);
                }
                else
                {
                    IsCommentMode = false;
                    var defaultModeComponentValue = Computer.DefaultModeComponentValue ?? ModeComponentValue.Clone();
                    ModeComponentValue.CopyAs(Computer.ModeComponentValue, Computer.MyNoteFiles.First(), false);
                    ModeComponentValue.SentMultiplier = Computer.TotallyLevyingMultiplier;
                    ModeComponentValue.CanModifyMultiplier = false;
                    ModeComponentValue.AudioMultiplier = Computer.TotallyLevyingAudioMultiplier;
                    ModeComponentValue.CanModifyAudioMultiplier = false;
                    SetComputingMode(new CommentCompute(Computer.MyNoteFiles, Computer.Comments, defaultModeComponentValue, Computer.AvatarID, Computer.AvatarName, null, Computer.EventNoteEntryItem, null, double.NaN));
                }
            }
        }

        public void HandleViewComment(NetItem netItem)
        {
            if (netItem.Comment != null && HasNotInput())
            {
                IsCommentMode = false;
                var defaultModeComponentValue = Computer.DefaultModeComponentValue ?? ModeComponentValue.Clone();
                ModeComponentValue.CopyAs(netItem.CommentItem.ModeComponentValue, Computer.NoteFile, false);
                SetComputingMode(new CommentCompute(new[] { Computer.NoteFile }, new[] { netItem.Comment }, defaultModeComponentValue, netItem.AvatarID, netItem.AvatarName, null, null, null, Computer.LoopingCounter));
            }
        }

        public void Input<T>(T[][][] inputConfigure, T rawInput, bool isInput, DefaultCompute.InputFlag inputFlag = DefaultCompute.InputFlag.Not, byte inputPower = byte.MaxValue)
        {
            if (HasNotInput())
            {
                var inputMap = inputConfigure[(int)Computer.InputMode];
                for (var i = inputMap.Length - 1; i > 0; --i)
                {
                    if (Array.IndexOf(inputMap[i], rawInput) != -1)
                    {
                        Computer.Input(isInput ? i : -i, inputFlag, inputPower);
                    }
                }
            }
        }

        public void PostItem(int postableItemPosition)
        {
            if (HasNotInput())
            {
                Computer.PostItem(postableItemPosition);
            }
        }

        public void HandleUndo()
        {
            if (HasNotInput())
            {
                switch (ModeValue)
                {
                    case Mode.Computing:
                        if (Computer.CanUndo && Computer.IsPausingWindowOpened)
                        {
                            Computer.SetUndo = true;
                        }
                        break;
                    case Mode.Quit:
                        HandleInitComment();
                        break;
                }
            }
        }

        void MoveEntryView() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.MoveEntryView,
            Contents = EntryItemValue
        });

        public void PointEntryView() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.PointEntryView
        });

        public void OnWPFViewVisibilityModified()
        {
            IsWPFViewVisible = IsNoteFileMode || ViewModels.Instance.WindowViewModels.Any(windowViewModel => windowViewModel.IsOpened);
            ViewModels.Instance.NotifyWindowViewModels();
            DrawingSystem.Instance.OnModified();
            var handlingComputer = GetHandlingComputer();
            if (handlingComputer != null)
            {
                MediaSystem.Instance.HandleDefaultIfAvailable(handlingComputer);
                MediaSystem.Instance.HandleIfAvailable(handlingComputer);
            }
            MediaSystem.Instance.HandleDefaultIfAvailable(BaseUI.Instance);
            MediaSystem.Instance.HandleIfAvailable(BaseUI.Instance);
            TVSystem.Instance.HandleSystemIfAvailable();
        }

        public async void GetQwilight(bool isSilent)
        {
            try
            {
                var taehuiQwilight = await TwilightSystem.Instance.GetWwwParallel<JSON.TaehuiQwilight?>($"{QwilightComponent.TaehuiNetFE}/qwilight/qwilight.json");
                if (taehuiQwilight.HasValue)
                {
                    var taehuiQwilightValue = taehuiQwilight.Value;
                    var date = Version.Parse(taehuiQwilightValue.date);
                    if (QwilightComponent.Date < date || QwilightComponent.HashText != taehuiQwilightValue.hash)
                    {
                        SaveQwilight();
                    }
                    else if (!isSilent)
                    {
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.ViewAllowWindow,
                            Contents = new object[]
                            {
                                LanguageSystem.Instance.AlreadyLatestDate,
                                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                                new Action<MESSAGEBOX_RESULT>(r =>
                                {
                                    if (r == MESSAGEBOX_RESULT.IDYES)
                                    {
                                        SaveQwilight();
                                    }
                                })
                            }
                        });
                    }

                    async void SaveQwilight()
                    {
                        var data = ArrayPool<byte>.Shared.Rent(QwilightComponent.SendUnit);
                        var savingBundleItem = new NotifyItem
                        {
                            Text = LanguageSystem.Instance.SavingQwilightContents,
                            Variety = NotifySystem.NotifyVariety.Levying,
                            OnStop = isTotal => false
                        };
                        try
                        {
                            HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.NotifyValue.NotifyItemCollection.Insert(0, savingBundleItem));
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                            var title = taehuiQwilightValue.title;
                            var tmpFileName = Path.GetTempFileName();
                            var target = $"{QwilightComponent.TaehuiNetFE}/qwilight/{title}";
                            using (var wwwClient = new HttpClient())
                            {
                                using (var hrm = await wwwClient.GetAsync(target, HttpCompletionOption.ResponseHeadersRead))
                                {
                                    savingBundleItem.QuitStatus = hrm.Content.Headers.ContentLength ?? 0L;
                                }
                                using (var s = await wwwClient.GetStreamAsync(target))
                                using (var fs = File.OpenWrite(tmpFileName))
                                {
                                    var length = 0;
                                    while ((length = await s.ReadAsync(data.AsMemory(0, data.Length))) > 0)
                                    {
                                        await fs.WriteAsync(data.AsMemory(0, length));
                                        savingBundleItem.LevyingStatus += length;
                                        savingBundleItem.NotifyBundleStatus();
                                    }
                                }
                            }
                            savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                            savingBundleItem.Text = LanguageSystem.Instance.SavedQwilightContents;
                            savingBundleItem.OnStop = isTotal =>
                            {
                                Utility.WipeFile(_qwilightFileName);
                                return true;
                            };
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                            var qwilightFileName = Path.ChangeExtension(tmpFileName, Path.GetExtension(title));
                            Utility.MoveFile(tmpFileName, qwilightFileName);
                            _qwilightFileName = qwilightFileName;
                        }
                        catch (Exception e)
                        {
                            if (!isSilent)
                            {
                                savingBundleItem.Text = string.Format(LanguageSystem.Instance.GetQwilightFault, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!isSilent)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.GetQwilightFault, e.Message));
                }
            }
        }

        public void WipeFavoriteEntry()
        {
            foreach (var entryItem in _entryItems.Values)
            {
                entryItem.WipeFavoriteEntry();
            }
        }

        public void LoadDefaultCommentCollection()
        {
            HandlingUISystem.Instance.HandleParallel(DefaultCommentCollection.Clear);
            IsDefaultCommentLoading = IsNoteFileNotLogical;
            if (IsDefaultCommentLoading)
            {
                _loadDefaultCommentHandler.Stop();
                _loadDefaultCommentHandler.Start();
            }
        }

        public void LoadTwilightCommentCollection()
        {
            HandlingUISystem.Instance.HandleParallel(TwilightCommentCollection.Clear);
            TwilightCommentTotalFavor = "👍";
            TwilightCommentFavor = null;
            TwilightCommentText0 = string.Empty;
            TwilightCommentText1 = string.Empty;
            IsTwilightCommentLoading = IsNoteFileNotLogical && !IsEntryItemBanned;
            if (IsTwilightCommentLoading)
            {
                _loadTwilightCommentHandler.Stop();
                _loadTwilightCommentHandler.Start();
            }
        }

        public async void LoadWowItemCollection()
        {
            IsWowLoading = true;

            var twilightWwwWow = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwWow?>($"{QwilightComponent.QwilightAPI}/wow");
            if (twilightWwwWow.HasValue)
            {
                var twilightWwwWowValue = twilightWwwWow.Value;

                TotalWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.totalAvatars)
                {
                    TotalWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.HandledContents)));
                }
                HighestWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.highestAvatars)
                {
                    HighestWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.CountContents)));
                }
                StandWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.standAvatars)
                {
                    StandWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.StandContents)));
                }
                BandWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.bandAvatars)
                {
                    BandWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.BandContents)));
                }
                TotalAtWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.totalAvatarsAt)
                {
                    TotalAtWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.HandledContents)));
                }
                HighestAtWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.highestAvatarsAt)
                {
                    HighestAtWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.CountContents)));
                }
                StandAtWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.standAvatarsAt)
                {
                    StandAtWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.StandContents)));
                }
                BandAtWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.bandAvatarsAt)
                {
                    BandAtWowItemCollection.Add(new(data, value => value.ToString(LanguageSystem.Instance.BandContents)));
                }
                Ability5KWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.ability5KAvatars)
                {
                    Ability5KWowItemCollection.Add(new(data, value => value.ToString("#,##0.## Point")));
                }
                Ability7KWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.ability7KAvatars)
                {
                    Ability7KWowItemCollection.Add(new(data, value => value.ToString("#,##0.## Point")));
                }
                Ability9KWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.ability9KAvatars)
                {
                    Ability9KWowItemCollection.Add(new(data, value => value.ToString("#,##0.## Point")));
                }
                LevelWowItemCollection.Clear();
                foreach (var data in twilightWwwWowValue.levelAvatars)
                {
                    LevelWowItemCollection.Add(new(data, value => $"LV. {value}"));
                }
            }

            IsWowLoading = false;
        }

        public void NotifyNoteFile()
        {
            IsCommentMode = false;
            VerifyNoteFile(Configure.Instance.SetSalt ? ModeComponentValue.Salt : Environment.TickCount);
            LoadCommentCollection();
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
            {
                situationValue = (int)UbuntuItem.UbuntuSituation.NoteFileMode,
                situationText = EntryItemValue?.NoteFile?.PlatformText ?? "Idle"
            });
        }

        public void LoadCommentCollection()
        {
            if (!BaseUI.Instance.HasCommentPoint || ViewModels.Instance.CommentValue.IsOpened)
            {
                switch (Configure.Instance.CommentViewTabPosition)
                {
                    case 0:
                        LoadDefaultCommentCollection();
                        break;
                    case 1:
                        LoadTwilightCommentCollection();
                        break;
                }
            }
        }

        public void VerifyNoteFile(int salt)
        {
            if (EntryItemValue != null)
            {
                if (IsNoteFileNotLogical)
                {
                    var targetNoteFile = EntryItemValue.NoteFile;
                    if (!FastDB.Instance.GetNoteFile(targetNoteFile) || targetNoteFile.IsSalt)
                    {
                        try
                        {
                            targetNoteFile.SetData(salt);
                        }
                        catch
                        {
                        }
                    }
                    ViewModels.Instance.SiteContainerValue.CallSetNoteFile();
                    ModeComponentValue.ComputingValue = targetNoteFile;
                    ModeComponentValue.Salt = salt;
                    ModeComponentValue.SetLowestAutoLongNoteModify();
                    ModeComponentValue.SetAutoPutNoteSetMillis();
                    OnJudgmentMeterMillisModified();
                    targetNoteFile.NotifyModel();
                }
                EntryItemValue.NotifyModel();
            }
            OnPropertyChanged(nameof(IsNoteFileNotLogical));
            OnPropertyChanged(nameof(IsNoteFileAvailable));
            OnPropertyChanged(nameof(HasAssistFile));
            OnPropertyChanged(nameof(LengthText));
            OnPropertyChanged(nameof(IsBPM1Visible));
            OnPropertyChanged(nameof(IsBPMVisible));
            OnPropertyChanged(nameof(IsEntryItemBanned));
            Configure.Instance.UIConfigureValue.NotifyInputMode();
            NotifyBPMText();
            NotifyHighestInputCountText();
            NotifyCanTwilightCommentary();
            NotifyCanTwilightFavor();
            NotifyCanSaveAsBundle();
            HandleAutoComputer();
        }

        public void OnJudgmentMeterMillisModified()
        {
            var noteFile = EntryItemValue?.NoteFile;
            if (noteFile != null)
            {
                if (Configure.Instance.AutoJudgmentMeterMillis)
                {
                    Configure.Instance.JudgmentMeterMillis = Math.Min
                    (
                        Math.Abs(Component.GetJudgmentMillis(Configure.Instance.AutoJudgmentMeterMillisItemValue.Judged, ModeComponentValue, noteFile.JudgmentStage, Component.LatestJudgmentMode, Component.LatestJudgmentMap, Component.LatestLongNoteAssist, 0)),
                        Math.Abs(Component.GetJudgmentMillis(Configure.Instance.AutoJudgmentMeterMillisItemValue.Judged, ModeComponentValue, noteFile.JudgmentStage, Component.LatestJudgmentMode, Component.LatestJudgmentMap, Component.LatestLongNoteAssist, 1))
                    );
                }
                noteFile.SetJudgmentMillisTexts(ModeComponentValue);
            }
        }

        public void NotifyBPMText() => OnPropertyChanged(nameof(BPMText));

        public void NotifyHighestInputCountText() => OnPropertyChanged(nameof(HighestInputCountText));

        public override void NotifyModel()
        {
            base.NotifyModel();
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.SetNoteFileModeWindowInputs
            });
            DrawingSystem.Instance.OnModified();
            BaseUI.Instance.NotifyModel();
            UI.Instance.NotifyModel();
            TwilightSystem.Instance.NotifyModel();
            Configure.Instance.NotifyModel();
            LanguageSystem.Instance.NotifyModel();
            ControllerSystem.Instance.NotifyModel();
            MIDISystem.Instance.NotifyModel();
            Configure.Instance.NotifyModel();
            AvatarTitleSystem.Instance.WipeAvatarTitles();
            ViewModels.Instance.ModifyModeComponentValue.SetModeComponentValues();
        }

        public void SetNoteFileMode(string faultText = null)
        {
            switch (ModeValue)
            {
                case Mode.Computing:
                    Fade(HandleImpl, Computer, true, 1);
                    break;
                case Mode.Quit:
                    Fade(HandleImpl, GetHandlingComputer(), true, 1);
                    break;
            }

            void HandleImpl()
            {
                if (Computer.DefaultModeComponentValue != null)
                {
                    ModeComponentValue.CopyAs(Computer.DefaultModeComponentValue, Computer);
                    ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
                }
                ModeComponentValue.CanModifyMultiplier = true;
                ModeComponentValue.CanModifyAudioMultiplier = true;
                ModeValue = Mode.NoteFile;
                Computer.AtNoteFileMode();
                EntryItemValue = null;
                Want();
                if (!string.IsNullOrEmpty(faultText))
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, faultText);
                }
            }
        }

        public void SetComputingMode(DefaultCompute targetComputer)
        {
            switch (ModeValue)
            {
                case Mode.NoteFile:
                    Fade(HandleImpl, targetComputer, false, 1);
                    break;
                case Mode.Computing:
                    HandleImpl();
                    break;
                case Mode.Quit:
                    Fade(HandleImpl, GetHandlingComputer(), true, 1);
                    break;
            }

            void HandleImpl()
            {
                (AutoComputer ?? Computer)?.Migrate(targetComputer);
                var defaultComputer = Computer;
                Computer = targetComputer;
                Computer.HandleCompiler();
                ModeValue = Mode.Computing;
                defaultComputer?.Close();
                CloseAutoComputer();
            }
        }

        public void SetQuitMode(DefaultCompute defaultComputer)
        {
            switch (ModeValue)
            {
                case Mode.NoteFile:
                    Fade(HandleImpl, defaultComputer, false, 0);
                    break;
                case Mode.Computing:
                    Fade(HandleImpl, Computer, true, Computer.FadingViewLayer);
                    break;
                case Mode.Quit:
                    HandleImpl();
                    break;
            }

            void HandleImpl()
            {
                defaultComputer.SetJudgmentMillis();
                defaultComputer.AtQuitMode();
                defaultComputer.NotifyCompute();
                Computer = defaultComputer;
                ModeValue = Mode.Quit;
            }
        }

        public void SetDefaultEntryItems()
        {
            var lastDefaultEntryItem = Configure.Instance.LastDefaultEntryItem;
            Utility.SetUICollection(DefaultEntryItems, new List<DefaultEntryItem>(Configure.Instance.DefaultEntryItems)
            {
                DefaultEntryItem.Total,
                DefaultEntryItem.EssentialBundle
            }, defaultEntryItem =>
            {
                if (defaultEntryItem == lastDefaultEntryItem)
                {
                    Configure.Instance.LastDefaultEntryItem = null;
                }
                WipeEntryItems(entryItem => defaultEntryItem == entryItem.DefaultEntryItem, noteFile => defaultEntryItem == noteFile.DefaultEntryItem, false);
                _alreadyLoadedDefaultEntryItems.Remove(defaultEntryItem);
            });
            DefaultEntryItems.Sort();
            LoadDefaultEntryItem(false);
        }

        public void WipeLoadedDefaultEntryItems()
        {
            _alreadyLoadedDefaultEntryItems.Clear();
            FastDB.Instance.WipeDefaultEntryItems();
        }

        public void HandleAutoComputer()
        {
            _autoComputerHandler.Stop();
            _autoComputerHandler.Start();
        }

        public void HandleAutoComputerImmediately(bool doTrailerAudio)
        {
            if (!ViewModels.Instance.VoteValue.IsOpened && !ViewModels.Instance.LevelVoteValue.IsOpened)
            {
                var targetNoteFile = EntryItemValue?.NoteFile;
                if (targetNoteFile != null)
                {
                    if (targetNoteFile.IsLogical)
                    {
                        CloseAutoComputer(targetNoteFile.LogicalAudioFileName);
                    }
                    else
                    {
                        if (Configure.Instance.AutoCompute)
                        {
                            if (AutoComputer == null || !targetNoteFile.IsContinuous(AutoComputer.NoteFile) || AutoComputer.SetStop)
                            {
                                NewAutoComputer(double.NaN, false);
                            }
                            else if (targetNoteFile != AutoComputer.NoteFile ||
                                !ModeComponentValue.IsCompatible(AutoComputer.CompatibleModeComponentValue) ||
                                Configure.Instance.LoopUnit != AutoComputer.Comment.LoopUnit ||
                                Configure.Instance.LoadedMedia != AutoComputer.LoadedMedia ||
                                Configure.Instance.BanalMedia != AutoComputer.BanalMedia || Configure.Instance.AlwaysBanalMedia != AutoComputer.AlwaysBanalMedia || Configure.Instance.BanalMediaFilePath != AutoComputer.BanalMediaFilePath ||
                                Configure.Instance.BanalFailedMedia != AutoComputer.BanalFailedMedia || Configure.Instance.AlwaysBanalFailedMedia != AutoComputer.AlwaysBanalFailedMedia || Configure.Instance.BanalFailedMediaFilePath != AutoComputer.BanalFailedMediaFilePath)
                            {
                                NewAutoComputer(AutoComputer.IsHandling ? AutoComputer.LoopingCounter : AutoComputer.LevyingWait, true);
                            }
                            else
                            {
                                if (Configure.Instance.AlwaysNotP2Position != AutoComputer.AlwaysNotP2Position ||
                                    Configure.Instance.InputMappingValue != AutoComputer.InputMappingValue)
                                {
                                    Task.Run(AutoComputer.SetUIMap);
                                }
                            }
                        }
                        else
                        {
                            CloseAutoComputer("Default");
                            targetNoteFile.SetConfigure();
                        }

                        void NewAutoComputer(double levyingWait, bool doMigrate)
                        {
                            ModeComponentValue.ComputingValue = targetNoteFile;
                            var autoComputer = new AutoCompute(new[] { targetNoteFile }, null, TwilightSystem.Instance.AvatarID, TwilightSystem.Instance.GetAvatarName(), -1, levyingWait);
                            var targetComputer = doMigrate ? autoComputer : null;
                            var isMigrate = AutoComputer != null && targetComputer != null;
                            if (isMigrate)
                            {
                                AutoComputer.Migrate(targetComputer);
                            }
                            var trailerAudioFilePath = Utility.GetAvailable(targetNoteFile.TrailerAudioPath, Utility.AvailableFlag.Audio);
                            if (doTrailerAudio && !string.IsNullOrEmpty(trailerAudioFilePath))
                            {
                                CloseAutoComputer(null);
                                if (!isMigrate)
                                {
                                    AudioSystem.Instance.HandleImmediately(trailerAudioFilePath, autoComputer, autoComputer.TrailerAudioHandler);
                                }
                                autoComputer.SetPause = true;
                            }
                            else
                            {
                                CloseAutoComputer(isMigrate ? null : "Default");
                            }
                            AutoComputer = autoComputer;
                            AutoComputer.HandleCompiler();
                            if (IsComputingMode)
                            {
                                Computer.Close();
                                Computer = AutoComputer;
                            }
                        }
                    }
                }
                else
                {
                    CloseAutoComputer("Default");
                }
            }
        }

        public void CloseAutoComputer(string audioFileName = null)
        {
            _autoComputerHandler.Stop();
            AutoComputer?.Close();
            AutoComputer = null;
            ClosePausableAudioHandler(audioFileName);
        }

        public void ClosePausableAudioHandler(string audioFileName = null)
        {
            var defaultAudioVarietyValue = Configure.Instance.DefaultAudioVarietyValue;
            if (audioFileName == "Default")
            {
                switch (defaultAudioVarietyValue)
                {
                    case Configure.DefaultAudioVariety.Not:
                        audioFileName = null;
                        break;
                    case Configure.DefaultAudioVariety.Favor:
                        audioFileName = nameof(AudioSystem.Instance.DefaultAudio);
                        break;
                }
            }
            if (_pausableAudioHandler.AudioFileName != audioFileName)
            {
                AudioSystem.Instance.Fade(_pausableAudioHandler, QwilightComponent.StandardWaitMillis);
                if (IsNoteFileMode || audioFileName == null)
                {
                    _pausableAudioHandler.AudioFileName = audioFileName;
                    _pausableAudioHandler.IsPausing = audioFileName == null;
                    if (audioFileName != null)
                    {
                        switch (audioFileName)
                        {
                            case nameof(AudioSystem.Instance.DefaultAudio):
                                AudioSystem.Instance.Handle(new()
                                {
                                    AudioLevyingPosition = _pausableAudioHandler.GetAudioPosition(),
                                    AudioItem = AudioSystem.Instance.DefaultAudio
                                }, AudioSystem.SEAudio, 1.0, false, _pausableAudioHandler, QwilightComponent.StandardWaitMillis);
                                break;
                            default:
                                Utility.HandleUIAudio(audioFileName, null, _pausableAudioHandler, QwilightComponent.StandardWaitMillis);
                                break;
                        }
                    }
                }
            }
        }
    }
}