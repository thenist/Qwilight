﻿using HtmlAgilityPack;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Buffers;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Qwilight
{
    public sealed class LevelSystem
    {
        public static readonly string EntryPath = Path.Combine(QwilightComponent.QwilightEntryPath, "Level");

        public static readonly LevelSystem Instance = QwilightComponent.GetBuiltInData<LevelSystem>(nameof(LevelSystem));

        public Comparer<string> WantLevelIDEquality { get; }

        public ObservableCollection<string> LevelFileNames { get; } = new();

        public ObservableCollection<string> LevelIDCollection { get; } = new();

        public Dictionary<string, string> LevelID128s { get; } = new();

        public Dictionary<string, string> LevelID256s { get; } = new();

        public Dictionary<string, LevelNoteFile> LevelID128NoteFiles { get; } = new();

        public Dictionary<string, LevelNoteFile> LevelID256NoteFiles { get; } = new();

        public LevelSystem()
        {
            WantLevelIDEquality = Comparer<string>.Create((x, y) => LevelIDCollection.IndexOf(x).CompareTo(LevelIDCollection.IndexOf(y)));
        }

        public void LoadLevelFiles() => Utility.SetUICollection(LevelFileNames, Utility.GetFiles(EntryPath).Where(levelFile => !Path.GetFileName(levelFile).StartsWith('#') && levelFile.IsTailCaselsss(".json")).Select(levelFile => Path.GetFileNameWithoutExtension(levelFile)).ToArray());

        public void LoadJSON(bool isNotify)
        {
            LevelID128s.Clear();
            LevelID256s.Clear();
            LevelID128NoteFiles.Clear();
            LevelID256NoteFiles.Clear();
            LevelIDCollection.Clear();
            var wantLevelName = Configure.Instance.LastWantLevelName;
            if (!string.IsNullOrEmpty(wantLevelName))
            {
                try
                {
                    var levelFilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Level", $"{wantLevelName}.json");
                    if (File.Exists(levelFilePath))
                    {
                        var levelTableFilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Level", $"#{wantLevelName}.json");
                        if (File.Exists(levelTableFilePath))
                        {
                            var levelTable = Utility.GetJSON<JSON.BMSTable?>(File.ReadAllText(levelTableFilePath));
                            if (levelTable.HasValue)
                            {
                                var levelTableValue = levelTable.Value;
                                var levelTexts = new List<object>();
                                var levelTitle = levelTableValue.symbol;
                                foreach (var levelTableData in Utility.GetJSON<JSON.BMSTableData[]>(File.ReadAllText(levelFilePath)))
                                {
                                    var level = levelTableData.level;
                                    var noteID128 = levelTableData.md5.ToLowerInvariant();
                                    var www = levelTableData.url;
                                    if (!string.IsNullOrEmpty(noteID128))
                                    {
                                        LevelID128s[noteID128] = levelTitle + level;
                                        LevelID128NoteFiles[noteID128] = new LevelNoteFile(levelTableData, levelTitle + level);
                                    }
                                    var noteID256 = levelTableData.sha256.ToLowerInvariant();
                                    if (!string.IsNullOrEmpty(noteID256))
                                    {
                                        LevelID256s[noteID256] = levelTitle + level;
                                        LevelID256NoteFiles[noteID256] = new LevelNoteFile(levelTableData, levelTitle + level);
                                    }
                                    var levelText = level.ToString();
                                    if (!levelTexts.Contains(levelText))
                                    {
                                        levelTexts.Add(levelText);
                                    }
                                }
                                var levels = levelTableValue.level_order.Select(level => level.ToString()).ToArray();
                                levelTexts.Sort((x, y) => Array.IndexOf(levels, x).CompareTo(Array.IndexOf(levels, y)));
                                foreach (var levelText in levelTexts)
                                {
                                    LevelIDCollection.Add(levelTitle + levelText);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (isNotify)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.LoadLevelFault, e.Message));
                    }
                }
            }
        }

        public async ValueTask LoadWww(string www)
        {
            var data = ArrayPool<byte>.Shared.Rent(QwilightComponent.SendUnit);
            try
            {
                var levelCompiler = new HtmlDocument();
                using (var s = await TwilightSystem.Instance.GetWwwParallel(www).ConfigureAwait(false))
                {
                    levelCompiler.Load(s);
                }
                using (var s = await TwilightSystem.Instance.GetWwwParallel(WebUtility.HtmlDecode(ModifyDataValue(levelCompiler.CreateNavigator().SelectSingleNode("/html/head/meta[@name='bmstable']/@content")?.ToString()
                    ?? levelCompiler.CreateNavigator().SelectSingleNode("/html/body/meta[@name='bmstable']/@content")?.ToString()
                    ?? levelCompiler.CreateNavigator().SelectSingleNode("/html/head/body/meta[@name='bmstable']/@content")?.ToString()))).ConfigureAwait(false))
                {
                    var levelTable = await Utility.GetJSON<JSON.BMSTable?>(s).ConfigureAwait(false);
                    if (levelTable.HasValue)
                    {
                        var levelTableValue = levelTable.Value;
                        var savingLevelItem = new NotifyItem
                        {
                            Text = LanguageSystem.Instance.SavingLevelContents,
                            Variety = NotifySystem.NotifyVariety.Levying,
                            OnStop = wipeTotal => false
                        };
                        UIHandler.Instance.HandleParallel(() => ViewModels.Instance.NotifyValue.NotifyItemCollection.Insert(0, savingLevelItem));
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingLevelItem.Text, true, null, null, NotifySystem.SaveLevelID);
                        var target = ModifyDataValue(levelTableValue.data_url);
                        var levelTableFileName = levelTableValue.name;
                        foreach (var targetFileName in Path.GetInvalidFileNameChars())
                        {
                            levelTableFileName = levelTableFileName.Replace(targetFileName.ToString(), string.Empty);
                        }
                        using (var wwwClient = new HttpClient())
                        {
                            wwwClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                            using (var hrm = await wwwClient.GetAsync(target, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                            {
                                savingLevelItem.MaxStatus = hrm.Content.Headers.ContentLength ?? 0L;
                            }
                            using (var fs = File.Open(Path.Combine(EntryPath, $"{levelTableFileName}.json"), FileMode.Create))
                            using (var ws = await wwwClient.GetStreamAsync(target).ConfigureAwait(false))
                            {
                                var length = 0;
                                while ((length = await ws.ReadAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false)) > 0)
                                {
                                    await fs.WriteAsync(data.AsMemory(0, length)).ConfigureAwait(false);
                                    savingLevelItem.Status += length;
                                }
                            }
                        }
                        using (var fs = File.Open(Path.Combine(EntryPath, $"#{levelTableFileName}.json"), FileMode.Create))
                        {
                            s.Position = 0;
                            await s.CopyToAsync(fs).ConfigureAwait(false);
                        }
                        savingLevelItem.Variety = NotifySystem.NotifyVariety.Quit;
                        savingLevelItem.Text = LanguageSystem.Instance.SavedLevelContents;
                        savingLevelItem.OnStop = wipeTotal => true;
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, LanguageSystem.Instance.SavedLevelContents, true, null, null, NotifySystem.SaveLevelID);
                        Configure.Instance.LevelTargetMap[levelTableFileName] = www;
                        LoadLevelFiles();
                        Configure.Instance.LastWantLevelName = levelTableFileName;
                    }
                }

                string ModifyDataValue(string dataValue)
                {
                    if (!Uri.IsWellFormedUriString(dataValue, UriKind.Absolute))
                    {
                        if (www.Substring(www.LastIndexOf('/')).Contains('.') || www.EndsWith('/'))
                        {
                            return $"{www}/../{dataValue}";
                        }
                        else
                        {
                            return $"{www}/{dataValue}";
                        }
                    }
                    else
                    {
                        return dataValue;
                    }
                }
            }
            catch (Exception e)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.NotValidNetLevelContents, e.Message), true, null, null, NotifySystem.SaveLevelID);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }
}
