using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qwilight
{
    public sealed class FastDB
    {
        public struct FastClass
        {
            public Version date;
            public Dictionary<string, FastNoteFile> noteFiles = new();
            public Dictionary<string, List<FastEntryItem>> entryItems = new();
            public Dictionary<string, List<string>> defaultEntryItems = new();
            public Dictionary<string, DateTime> defaultEntryItemDates = new();

            public FastClass()
            {
            }
        }

        static readonly JsonSerializerOptions _defaultJSONConfigure = Utility.GetJSONConfigure(defaultJSONConfigure =>
        {
            defaultJSONConfigure.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            defaultJSONConfigure.IgnoreReadOnlyProperties = true;
        });
        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "DB.json");

        public static readonly FastDB Instance = new();

        readonly ConcurrentDictionary<string, FastNoteFile> _fastNoteFiles = new();
        readonly ConcurrentDictionary<string, ConcurrentBag<FastEntryItem>> _fastEntryItems = new();
        readonly ConcurrentDictionary<string, ConcurrentBag<string>> _fastDefaultEntryItems = new();
        readonly ConcurrentDictionary<string, DateTime> _fastDefaultEntryItemDates = new();

        public void Load()
        {
            try
            {
                if (File.Exists(_fileName))
                {
                    var fastClass = Utility.GetJSON<FastClass>(File.ReadAllText(_fileName, Encoding.UTF8), _defaultJSONConfigure);
                    foreach (var (noteID, fastNoteFile) in fastClass.noteFiles)
                    {
                        _fastNoteFiles[noteID] = fastNoteFile;
                    }
                    foreach (var (entryPath, fastEntryItems) in fastClass.entryItems)
                    {
                        _fastEntryItems[entryPath] = new ConcurrentBag<FastEntryItem>(fastEntryItems);
                    }
                    foreach (var (defaultEntryPath, fastDefaultEntryItems) in fastClass.defaultEntryItems)
                    {
                        _fastDefaultEntryItems[defaultEntryPath] = new ConcurrentBag<string>(fastDefaultEntryItems);
                    }
                    foreach (var (defaultEntryPath, defaultEntryItemDate) in fastClass.defaultEntryItemDates)
                    {
                        _fastDefaultEntryItemDates[defaultEntryPath] = defaultEntryItemDate;
                    }
                }
            }
            catch
            {
            }
        }

        public bool GetNoteFile(BaseNoteFile noteFile)
        {
            if (_fastNoteFiles.TryGetValue(noteFile.GetNoteID512(), out var fastNoteFile))
            {
                noteFile.InputMode = fastNoteFile.inputMode;
                noteFile.Title = fastNoteFile.title;
                noteFile.Artist = fastNoteFile.artist;
                noteFile.Genre = fastNoteFile.genre;
                noteFile.LevelText = fastNoteFile.levelText;
                noteFile.LevelTextValue = fastNoteFile.levelTextValue;
                noteFile.BPM = fastNoteFile.bpm;
                noteFile.JudgmentStage = fastNoteFile.judgmentStage;
                noteFile.HitPointsValue = fastNoteFile.hitPointsValue;
                noteFile.LevelValue = fastNoteFile.levelValue;
                noteFile.NoteDrawingName = fastNoteFile.noteDrawingName;
                if (!string.IsNullOrEmpty(noteFile.NoteDrawingName))
                {
                    noteFile.NoteDrawingPath = Path.Combine(noteFile.EntryItem.EntryPath, noteFile.NoteDrawingName);
                }
                noteFile.BannerDrawingName = fastNoteFile.bannerDrawingName;
                if (!string.IsNullOrEmpty(noteFile.BannerDrawingName))
                {
                    noteFile.BannerDrawingPath = Path.Combine(noteFile.EntryItem.EntryPath, noteFile.BannerDrawingName);
                }
                var trailerAudioName = fastNoteFile.trailerAudioName;
                noteFile.TrailerAudioName = trailerAudioName;
                noteFile.TrailerAudioPath = Path.Combine(noteFile.EntryItem.EntryPath, string.IsNullOrEmpty(trailerAudioName) ? "PREVIEW.WAV" : trailerAudioName);
                noteFile.TotalNotes = fastNoteFile.totalNotes;
                noteFile.AutoableNotes = fastNoteFile.autoableNotes;
                noteFile.LongNotes = fastNoteFile.longNotes;
                noteFile.TrapNotes = fastNoteFile.trapNotes;
                noteFile.Length = fastNoteFile.length;
                noteFile.Tag = fastNoteFile.tag;
                noteFile.IsBanned = fastNoteFile.isBanned;
                noteFile.LowestBPM = fastNoteFile.lowestBPM;
                noteFile.HighestBPM = fastNoteFile.highestBPM;
                noteFile.HighestInputCount = fastNoteFile.highestInputCount;
                noteFile.IsHellBPM = fastNoteFile.isHellBPM;
                noteFile.IsAutoLongNote = fastNoteFile.isAutoLongNote;
                noteFile.IsSalt = fastNoteFile.isSalt;
                noteFile.AssistFileName = fastNoteFile.assistFileName;
                noteFile.PlatformText = Utility.GetPlatformText(noteFile.Title, noteFile.Artist, noteFile.GenreText, noteFile.LevelText);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetNoteFile(BaseNoteFile noteFile)
        {
            _fastNoteFiles[noteFile.GetNoteID512()] = new FastNoteFile
            {
                inputMode = noteFile.InputMode,
                artist = noteFile.Artist,
                title = noteFile.Title,
                genre = noteFile.Genre,
                levelText = noteFile.LevelText,
                levelTextValue = noteFile.LevelTextValue,
                bpm = noteFile.BPM,
                judgmentStage = noteFile.JudgmentStage,
                hitPointsValue = noteFile.HitPointsValue,
                levelValue = noteFile.LevelValue,
                noteDrawingName = noteFile.NoteDrawingName,
                bannerDrawingName = noteFile.BannerDrawingName,
                trailerAudioName = noteFile.TrailerAudioName,
                totalNotes = noteFile.TotalNotes,
                autoableNotes = noteFile.AutoableNotes,
                longNotes = noteFile.LongNotes,
                trapNotes = noteFile.TrapNotes,
                length = noteFile.Length,
                tag = noteFile.Tag,
                isBanned = noteFile.IsBanned,
                lowestBPM = noteFile.LowestBPM,
                highestBPM = noteFile.HighestBPM,
                highestInputCount = noteFile.HighestInputCount,
                isHellBPM = noteFile.IsHellBPM,
                isAutoLongNote = noteFile.IsAutoLongNote,
                isSalt = noteFile.IsSalt,
                assistFileName = noteFile.AssistFileName
            };
        }

        public void WipeNoteFile(BaseNoteFile noteFile)
        {
            _fastNoteFiles.Remove(noteFile.GetNoteID512(), out _);
        }

        public IEnumerable<FastEntryItem> GetEntryItems(string entryPath)
        {
            if (_fastEntryItems.TryGetValue(entryPath, out var fastEntryItems))
            {
                return fastEntryItems;
            }
            else
            {
                return Array.Empty<FastEntryItem>();
            }
        }

        public void WipeEntryItem(string entryPath)
        {
            _fastEntryItems.Remove(entryPath, out _);
        }

        public void SetEntryItem(string entryPath, BaseNoteFile noteFile, int[] dataIDs)
        {
            _fastEntryItems.GetOrAdd(entryPath, entryPath => new()).Add(new FastEntryItem
            {
                noteFilePath = noteFile?.NoteFilePath,
                noteID128 = noteFile?.GetNoteID128(),
                noteID256 = noteFile?.GetNoteID256(),
                noteID512 = noteFile?.GetNoteID512(),
                dataIDs = dataIDs,
            });
        }

        public IEnumerable<string> GetDefaultEntryItems(string defaultEntryPath)
        {
            if (_fastDefaultEntryItems.TryGetValue(defaultEntryPath, out var fastDefaultEntryItems))
            {
                return fastDefaultEntryItems;
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public void WipeDefaultEntryItem(string defaultEntryPath)
        {
            _fastDefaultEntryItems.Remove(defaultEntryPath, out _);
        }

        public void WipeDefaultEntryItems()
        {
            _fastDefaultEntryItems.Clear();
        }

        public void SetDefaultEntryItem(string defaultEntryPath, string entryPath)
        {
            _fastDefaultEntryItems.GetOrAdd(defaultEntryPath, defaultEntryPath => new()).Add(entryPath);
        }

        public DateTime GetDefaultEntryItemDate(string defaultEntryPath)
        {
            if (_fastDefaultEntryItemDates.TryGetValue(defaultEntryPath, out var fastDefaultEntryItemDate))
            {
                return fastDefaultEntryItemDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public void SetDefaultEntryItemDate(string defaultEntryPath, DateTime date)
        {
            _fastDefaultEntryItemDates[defaultEntryPath] = date;
        }

        public void Clear()
        {
            _fastNoteFiles.Clear();
            _fastEntryItems.Clear();
            _fastDefaultEntryItems.Clear();
        }

        public void Save()
        {
            File.WriteAllText(_fileName, Utility.SetJSON<FastClass>(new FastClass
            {
                date = QwilightComponent.Date,
                noteFiles = _fastNoteFiles.ToDictionary(pair => pair.Key, pair => pair.Value),
                entryItems = _fastEntryItems.ToDictionary(pair => pair.Key, pair => pair.Value.ToList()),
                defaultEntryItems = _fastDefaultEntryItems.ToDictionary(pair => pair.Key, pair => pair.Value.ToList()),
                defaultEntryItemDates = _fastDefaultEntryItemDates.ToDictionary(pair => pair.Key, pair => pair.Value),
            }, _defaultJSONConfigure));
        }
    }
}