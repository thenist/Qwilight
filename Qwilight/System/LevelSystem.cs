using HtmlAgilityPack;
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

        public ObservableCollection<string> LevelCollection { get; } = new();

        public Dictionary<string, string> LevelID128s { get; } = new();

        public Dictionary<string, string> LevelID256s { get; } = new();

        public Dictionary<string, LevelNoteFile> LevelID128NoteFiles { get; } = new();

        public Dictionary<string, LevelNoteFile> LevelID256NoteFiles { get; } = new();

        public LevelSystem()
        {
            WantLevelIDEquality = Comparer<string>.Create((x, y) => LevelCollection.IndexOf(x).CompareTo(LevelCollection.IndexOf(y)));
        }

        public void LoadLevelFiles() => Utility.SetUICollection(LevelFileNames, Utility.GetFiles(EntryPath).Where(levelFile => !Path.GetFileName(levelFile).StartsWith('#') && levelFile.IsTailCaselsss(".json")).Select(levelFile => Path.GetFileNameWithoutExtension(levelFile)).ToArray());

        public void LoadJSON(bool isNotify)
        {
            LevelID128s.Clear();
            LevelID256s.Clear();
            LevelID128NoteFiles.Clear();
            LevelID256NoteFiles.Clear();
            LevelCollection.Clear();
            var levelName = Configure.Instance.WantLevelName;
            if (!string.IsNullOrEmpty(levelName))
            {
                try
                {
                    var levelFilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Level", $"{levelName}.json");
                    if (File.Exists(levelFilePath))
                    {
                        var levelTableFilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Level", $"#{levelName}.json");
                        if (File.Exists(levelTableFilePath))
                        {
                            var levelTable = Utility.GetJSON<JSON.BMSTable?>(File.ReadAllText(levelTableFilePath));
                            if (levelTable.HasValue)
                            {
                                var levelTableValue = levelTable.Value;
                                var levelTexts = new List<object>();
                                var levelTitle = levelTableValue.symbol;
                                foreach (var levelData in Utility.GetJSON<JSON.BMSTableData[]>(File.ReadAllText(levelFilePath)))
                                {
                                    var level = levelData.level;
                                    var noteID128 = levelData.md5;
                                    var www = levelData.url;
                                    if (!string.IsNullOrEmpty(noteID128))
                                    {
                                        LevelID128s[$"{noteID128}:0"] = levelTitle + level;
                                        LevelID128NoteFiles[$"{noteID128}:0"] = new LevelNoteFile(levelData, levelTitle + level);
                                    }
                                    var noteID256 = levelData.sha256;
                                    if (!string.IsNullOrEmpty(noteID256))
                                    {
                                        LevelID256s[$"{noteID256}:0"] = levelTitle + level;
                                        LevelID256NoteFiles[$"{noteID256}:0"] = new LevelNoteFile(levelData, levelTitle + level);
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
                                    LevelCollection.Add(levelTitle + levelText);
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
                var o = new HtmlDocument();
                using var os = await TwilightSystem.Instance.GetWwwParallel(www).ConfigureAwait(false);
                o.Load(os);
                using var s = await TwilightSystem.Instance.GetWwwParallel(WebUtility.HtmlDecode(ModifyDataValue(o.CreateNavigator().SelectSingleNode("/html/head/meta[@name='bmstable']/@content")?.ToString()
                    ?? o.CreateNavigator().SelectSingleNode("/html/body/meta[@name='bmstable']/@content")?.ToString()
                    ?? o.CreateNavigator().SelectSingleNode("/html/head/body/meta[@name='bmstable']/@content")?.ToString()))).ConfigureAwait(false);
                var levelTable = Utility.GetJSON<JSON.BMSTable?>(s);
                s.Position = 0;
                if (levelTable.HasValue)
                {
                    var levelTableValue = levelTable.Value;
                    var savingBundleItem = new NotifyItem
                    {
                        Text = LanguageSystem.Instance.SavingLevelContents,
                        Variety = NotifySystem.NotifyVariety.Levying,
                        OnStop = isTotal => false
                    };
                    HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.NotifyValue.NotifyItemCollection.Insert(0, savingBundleItem));
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                    var target = ModifyDataValue(levelTableValue.data_url);
                    var levelTableFileName = levelTableValue.name;
                    foreach (var targetFileName in Path.GetInvalidFileNameChars())
                    {
                        levelTableFileName = levelTableFileName.Replace(targetFileName.ToString(), string.Empty);
                    }
                    using (var wwwClient = new HttpClient())
                    {
                        wwwClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                        using (var fs = File.Open(Path.Combine(EntryPath, $"{levelTableFileName}.json"), FileMode.Create))
                        using (var ts = await wwwClient.GetAsync(target).ConfigureAwait(false))
                        {
                            savingBundleItem.QuitStatus = ts.Content.Headers.ContentLength ?? 0L;
                            var length = 0;
                            while ((length = await (await ts.Content.ReadAsStreamAsync().ConfigureAwait(false)).ReadAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false)) > 0)
                            {
                                await fs.WriteAsync(data.AsMemory(0, length)).ConfigureAwait(false);
                                savingBundleItem.LevyingStatus += length;
                                savingBundleItem.NotifyBundleStatus();
                            }
                        }
                    }
                    using (var fs = File.Open(Path.Combine(EntryPath, $"#{levelTableFileName}.json"), FileMode.Create))
                    {
                        await s.CopyToAsync(fs).ConfigureAwait(false);
                    }
                    savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                    savingBundleItem.Text = LanguageSystem.Instance.SavedLevelContents;
                    savingBundleItem.OnStop = isTotal => true;
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, LanguageSystem.Instance.SavedLevelContents);
                    Configure.Instance.LevelTargetMap[levelTableFileName] = www;
                    LoadLevelFiles();
                    Configure.Instance.WantLevelName = levelTableFileName;
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
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.NotValidNetLevelContents, e.Message));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }
}
