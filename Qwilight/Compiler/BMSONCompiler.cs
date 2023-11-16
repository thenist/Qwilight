using Qwilight.Compute;
using Qwilight.Note;
using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using UtfUnknown;

namespace Qwilight.Compiler
{
    public sealed class BMSONCompiler : BaseCompiler
    {
        static int GetBMSONInput(object x, Component.InputMode inputMode)
        {
            if (x != null)
            {
                switch (inputMode)
                {
                    case Component.InputMode.InputMode4:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode5:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode6:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode7:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode8:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode9:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode10:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode51:
                        switch ((int)(long)x)
                        {
                            case 8:
                                return 1;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                return (int)(long)x + 1;
                        }
                        break;
                    case Component.InputMode.InputMode71:
                        switch ((int)(long)x)
                        {
                            case 8:
                                return 1;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                return (int)(long)x + 1;
                        }
                        break;
                    case Component.InputMode.InputMode102:
                        switch ((int)(long)x)
                        {
                            case 8:
                                return 1;
                            case 16:
                                return 12;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                return (int)(long)x + 1;
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                                return (int)(long)x - 2;
                        }
                        break;
                    case Component.InputMode.InputMode142:
                        switch ((int)(long)x)
                        {
                            case 8:
                                return 1;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                return (int)(long)x + 1;
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                                return (int)(long)x;
                        }
                        break;
                    case Component.InputMode.InputMode242:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                                return (int)(long)x + 1;
                            case 25:
                                return 1;
                            case 26:
                                return 26;
                        }
                        break;
                    case Component.InputMode.InputMode484:
                        switch ((int)(long)x)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                                return (int)(long)x + 2;
                            case 27:
                            case 28:
                            case 29:
                            case 30:
                            case 31:
                            case 32:
                            case 33:
                            case 34:
                            case 35:
                            case 36:
                            case 37:
                            case 38:
                            case 39:
                            case 40:
                            case 41:
                            case 42:
                            case 43:
                            case 44:
                            case 45:
                            case 46:
                            case 47:
                            case 48:
                            case 49:
                            case 50:
                                return (int)(long)x;
                            case 25:
                                return 1;
                            case 26:
                                return 2;
                            case 51:
                                return 51;
                            case 52:
                                return 52;
                        }
                        break;
                }
            }
            return default;
        }

        readonly Dictionary<long, double> _bmsonPositionLogicalYMap = new();
        readonly SortedDictionary<double, double> _bmsonPositionWaitMap = new();
        JSON.BMSON _text;
        bool _hasMedia;
        long _res;

        public BMSONCompiler(BMSONFile bmsonFile, CancellationTokenSource setCancelCompiler) : base(bmsonFile, setCancelCompiler)
        {
        }

        public override double GetWaitValue(double waitPosition) => _bmsonPositionWaitMap[waitPosition];

        public override void CompileImpl(Computing targetComputing, byte[] noteFileContents, int salt)
        {
            using (var rms = PoolSystem.Instance.GetDataFlow(noteFileContents))
            {
                var format = NoteFormatID;
                if (format == -1)
                {
                    var formatComputer = CharsetDetector.DetectFromStream(rms).Detected;
                    rms.Position = 0;
                    format = formatComputer != null && formatComputer.Confidence >= 0.875 && formatComputer.Encoding != null ? formatComputer.Encoding.CodePage : 65001;
                }
                _text = Utility.GetJSON<JSON.BMSON>(Encoding.GetEncoding(format).GetString(rms.GetBuffer(), 0, (int)rms.Length));
            }
            var title = _text.info.title;
            var titleAssister0 = _text.info.subtitle;
            var titleAssister1 = _text.info.chart_name;
            targetComputing.Title = $"{title}{(string.IsNullOrEmpty(titleAssister0) || title.EndsWith(titleAssister0) ? string.Empty : $" {titleAssister0}")}{(string.IsNullOrEmpty(titleAssister1) || title.EndsWith(titleAssister1) ? string.Empty : $" {titleAssister1}")}";
            targetComputing.Artist = string.Join(" / ", _text.info.subartists.Prepend(_text.info.artist));
            targetComputing.Genre = _text.info.genre;
            targetComputing.BannerDrawingName = _text.info.banner_image;
            var levelTextValue = _text.info.level;
            targetComputing.LevelTextValue = levelTextValue;
            levelTextValue = Math.Abs(levelTextValue);
            if (levelTextValue < 100)
            {
                targetComputing.LevelText = $"LV. {levelTextValue}";
            }
            else
            {
                targetComputing.LevelText = $"LV. {levelTextValue % 100:00}";
            }
            var longNoteVariety = _text.info.ln_type;
            targetComputing.IsAutoLongNote = longNoteVariety != 2 && longNoteVariety != 3;
            switch (_text.info.mode_hint)
            {
                case "generic-4keys":
                    InputMode = Component.InputMode.InputMode4;
                    break;
                case "generic-5keys":
                case "popn-5k":
                    InputMode = Component.InputMode.InputMode5;
                    break;
                case "generic-6keys":
                    InputMode = Component.InputMode.InputMode6;
                    break;
                case "generic-7keys":
                    InputMode = Component.InputMode.InputMode7;
                    break;
                case "generic-8keys":
                    InputMode = Component.InputMode.InputMode8;
                    break;
                case "generic-9keys":
                case "popn-9k":
                    InputMode = Component.InputMode.InputMode9;
                    break;
                case "generic-10keys":
                    InputMode = Component.InputMode.InputMode10;
                    break;
                case "beat-5k":
                    InputMode = Component.InputMode.InputMode51;
                    break;
                case "beat-10k":
                    InputMode = Component.InputMode.InputMode102;
                    break;
                case "beat-14k":
                    InputMode = Component.InputMode.InputMode142;
                    break;
                case "keyboard-24k":
                    InputMode = Component.InputMode.InputMode242;
                    break;
                case "keyboard-24k-double":
                    InputMode = Component.InputMode.InputMode484;
                    break;
                default:
                    InputMode = Component.InputMode.InputMode71;
                    break;
            }
            targetComputing.JudgmentStage = 1500 / _text.info.judge_rank - 10;
            targetComputing.HitPointsValue = Math.Abs(0.07605 * _text.info.total / (_text.info.total + 650));
            targetComputing.NoteDrawingName = string.IsNullOrEmpty(_text.info.back_image) ? string.IsNullOrEmpty(_text.info.eyecatch_image) ? _text.info.title_image : _text.info.eyecatch_image : _text.info.back_image;

            var bmsonPositionSet = new SortedSet<long>
            {
                0L
            };
            var audioCount = 0;
            foreach (var audioChannel in _text.sound_channels)
            {
                if (!string.IsNullOrEmpty(audioChannel.name))
                {
                    ++audioCount;
                }
                foreach (var note in audioChannel.notes)
                {
                    var bmsonPosition = note.y;
                    var input = GetBMSONInput(note.x, InputMode);
                    var isInput = input > 0;
                    var isLongInput = note.l > 0L;
                    bmsonPositionSet.Add(bmsonPosition);
                    if (isInput)
                    {
                        PositionStandNoteCountMap[bmsonPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsonPosition) + 1;
                        var isAutoableInput = Component.AutoableInputs[(int)InputMode].Contains(input);
                        if (isLongInput)
                        {
                            ++targetComputing.TotalNotes;
                            if (isAutoableInput)
                            {
                                ++targetComputing.AutoableNotes;
                            }
                            ++targetComputing.LongNotes;
                        }
                        else
                        {
                            ++targetComputing.TotalNotes;
                            if (isAutoableInput)
                            {
                                ++targetComputing.AutoableNotes;
                            }
                        }
                    }
                    if (isLongInput)
                    {
                        bmsonPosition += note.l;
                        bmsonPositionSet.Add(bmsonPosition);
                    }
                    HighestPosition = Math.Max(HighestPosition, bmsonPosition);
                }
            }
            if (targetComputing.TotalNotes == 0)
            {
                InputMode = Component.InputMode.InputMode51;
            }
            foreach (var trapChannel in _text.mine_channels)
            {
                foreach (var note in trapChannel.notes)
                {
                    var bmsonPosition = note.y;
                    var input = GetBMSONInput(note.x, InputMode);
                    var isInput = input > 0;
                    bmsonPositionSet.Add(bmsonPosition);
                    if (isInput)
                    {
                        ++targetComputing.TrapNotes;
                    }
                    HighestPosition = Math.Max(HighestPosition, bmsonPosition);
                }
            }
            foreach (var paint in _text.bga.bga_events)
            {
                bmsonPositionSet.Add(paint.y);
                _hasMedia = true;
            }
            foreach (var paint in _text.bga.layer_events)
            {
                bmsonPositionSet.Add(paint.y);
            }
            foreach (var paint in _text.bga.poor_events)
            {
                bmsonPositionSet.Add(paint.y);
            }
            foreach (var line in _text.lines)
            {
                bmsonPositionSet.Add(line.y);
                HighestPosition = Math.Max(HighestPosition, line.y);
            }
            foreach (var bpmEvent in _text.bpm_events)
            {
                var bmsonPosition = bpmEvent.y;
                PositionBPMMap.Add(bmsonPosition, bpmEvent.bpm);
                bmsonPositionSet.Add(bmsonPosition);
            }
            var bmsonPositionStopMap = new SortedDictionary<long, long>();
            foreach (var stopEvent in _text.stop_events)
            {
                var bmsonPosition = stopEvent.y;
                bmsonPositionStopMap.Add(bmsonPosition, bmsonPositionStopMap.GetValueOrDefault(bmsonPosition) + stopEvent.duration);
                bmsonPositionSet.Add(bmsonPosition);
            }
            targetComputing.LevyingBPM = _text.info.init_bpm;
            ComponentValue = new Component(targetComputing.LevyingBPM);
            _res = _text.info.resolution * 4;
            var lastBMSONPosition = 0L;
            var lastWait = 0.0;
            var lastBPM = targetComputing.LevyingBPM;
            foreach (var bmsonPosition in bmsonPositionSet)
            {
                _bmsonPositionLogicalYMap[bmsonPosition] = bmsonPosition * -ComponentValue.LogicalYMeter / _res;
                lastWait += ComponentValue.MillisMeter * (bmsonPosition - lastBMSONPosition) / _res;
                if (PositionBPMMap.TryGetValue(bmsonPosition, out var bpm))
                {
                    lastBPM = bpm;
                    ComponentValue.SetBPM(bpm);
                }
                _bmsonPositionWaitMap[bmsonPosition] = lastWait;
                if (bmsonPositionStopMap.TryGetValue(bmsonPosition, out var stop))
                {
                    lastWait += ComponentValue.MillisMeter * stop / _res;
                }
                lastBMSONPosition = bmsonPosition;
            }
            targetComputing.IsBanned = audioCount < 2 || targetComputing.TotalNotes == 0;
            targetComputing.IsHellBPM = targetComputing.TrapNotes > 0;
        }

        public override void CompileImpl(DefaultCompute defaultComputer, byte[] noteFileContents)
        {
            var audioFileNameAudioItemMap = new ConcurrentDictionary<string, AudioItem?>();
            var mediaIDHandledItemMap = new ConcurrentDictionary<long, IHandledItem>();
            var parallelItems = new ConcurrentBag<Action>();
            if (defaultComputer.LoadContents)
            {
                try
                {
                    var noteDrawingPath = Utility.GetAvailable(defaultComputer.NoteDrawingPath, Utility.AvailableFlag.Drawing);
                    if (!string.IsNullOrEmpty(noteDrawingPath))
                    {
                        defaultComputer.NoteHandledDrawingItem = new HandledDrawingItem
                        {
                            Drawing = DrawingSystem.Instance.Load(noteDrawingPath, defaultComputer),
                            DefaultDrawing = DrawingSystem.Instance.LoadDefault(noteDrawingPath, defaultComputer)
                        };
                    }
                }
                catch
                {
                }
            }
            var isBanalMedia = (!_hasMedia || defaultComputer.AlwaysBanalMedia) && defaultComputer.BanalMedia;
            defaultComputer.LoadBanalMedia(isBanalMedia, defaultComputer.BanalFailedMedia, parallelItems);
            foreach (var audioFileName in _text.sound_channels.Select(audioChannel => audioChannel.name).Concat(_text.mine_channels.Select(traoChannel => traoChannel.name)))
            {
                parallelItems.Add(() =>
                {
                    try
                    {
                        var audioFilePath = Path.IsPathFullyQualified(audioFileName) ? audioFileName : Utility.GetAvailable(Path.Combine(NoteFile.EntryItem.EntryPath, audioFileName), Utility.AvailableFlag.Audio);
                        if (!string.IsNullOrEmpty(audioFilePath))
                        {
                            audioFileNameAudioItemMap[audioFileName] = AudioSystem.Instance.Load(audioFilePath, defaultComputer, 1F);
                        }
                    }
                    catch
                    {
                    }
                });
            }
            if (defaultComputer.LoadedMedia && !isBanalMedia)
            {
                foreach (var paint in _text.bga.bga_header)
                {
                    parallelItems.Add(() =>
                    {
                        try
                        {
                            var mediaFilePath = Path.IsPathFullyQualified(paint.name) ? paint.name : Utility.GetAvailable(Path.Combine(NoteFile.EntryItem.EntryPath, paint.name), Utility.AvailableFlag.Drawing | Utility.AvailableFlag.Media);
                            if (!string.IsNullOrEmpty(mediaFilePath))
                            {
                                mediaIDHandledItemMap[Utility.ToInt64(paint.id.ToString())] = (Utility.GetAvailable(mediaFilePath)) switch
                                {
                                    Utility.AvailableFlag.Drawing => new HandledDrawingItem
                                    {
                                        Drawing = DrawingSystem.Instance.Load(mediaFilePath, defaultComputer),
                                        DefaultDrawing = DrawingSystem.Instance.LoadDefault(mediaFilePath, defaultComputer)
                                    },
                                    Utility.AvailableFlag.Media => MediaSystem.Instance.Load(mediaFilePath, defaultComputer),
                                    _ => null,
                                };
                            }
                        }
                        catch
                        {
                            mediaIDHandledItemMap[Utility.ToInt64(paint.id.ToString())] = null;
                        }
                    });
                }
            }

            if (defaultComputer.LoadContents)
            {
                var endStatus = parallelItems.Count;
                var status = 0;
                Utility.HandleLowlyParallelly(parallelItems, Configure.Instance.CompilingBin, parallelItem =>
                {
                    parallelItem();
                    defaultComputer.SetCompilingStatus((double)Interlocked.Increment(ref status) / endStatus);
                }, SetCancelCompiler?.Token);
            }

            if (!isBanalMedia)
            {
                var hasContents = defaultComputer.LoadedMedia;
                foreach (var mediaEvent in _text.bga.bga_events)
                {
                    var bmsonPosition = mediaEvent.y;
                    defaultComputer.WaitMediaNoteMap.Into(_bmsonPositionWaitMap[bmsonPosition], new MediaNote
                    {
                        MediaMode = MediaNote.Mode.Default,
                        MediaItem = mediaIDHandledItemMap.GetValueOrDefault(Utility.ToInt64(mediaEvent.id.ToString())),
                        HasContents = hasContents
                    });
                }
                foreach (var mediaEvent in _text.bga.layer_events)
                {
                    var bmsonPosition = mediaEvent.y;
                    defaultComputer.WaitMediaNoteMap.Into(_bmsonPositionWaitMap[bmsonPosition], new MediaNote
                    {
                        MediaMode = MediaNote.Mode.Layer,
                        MediaItem = mediaIDHandledItemMap.GetValueOrDefault(Utility.ToInt64(mediaEvent.id.ToString())),
                        HasContents = hasContents
                    });
                }
                foreach (var mediaEvent in _text.bga.poor_events)
                {
                    var bmsonPosition = mediaEvent.y;
                    defaultComputer.WaitMediaNoteMap.Into(_bmsonPositionWaitMap[bmsonPosition], new MediaNote
                    {
                        MediaMode = MediaNote.Mode.Failed,
                        MediaItem = mediaIDHandledItemMap.GetValueOrDefault(Utility.ToInt64(mediaEvent.id.ToString())),
                        HasContents = hasContents
                    });
                }
            }
            Notes.AddRange(Enumerable.Range(0, _text.lines.Length).Select(i =>
            {
                var bmsonPosition = _text.lines[i].y;
                var logicalY = ComponentValue.LevyingHeight + _bmsonPositionLogicalYMap[bmsonPosition];
                var wait = _bmsonPositionWaitMap[bmsonPosition];
                defaultComputer.MeterWaitMap[i] = wait;
                return new MeterNote(logicalY, wait, i);
            }));
            foreach (var audioChannel in _text.sound_channels)
            {
                var lastAudioNotePosition = 0L;
                var notes = audioChannel.notes.OrderBy(note => note.y).ToArray();
                var notesLength = notes.Length;
                for (var i = 0; i < notesLength; ++i)
                {
                    var note = notes[i];
                    var isContinuous = note.c;
                    var bmsonPosition = note.y;
                    var logicalY = ComponentValue.LevyingHeight + _bmsonPositionLogicalYMap[bmsonPosition];
                    var wait = _bmsonPositionWaitMap[bmsonPosition];
                    audioFileNameAudioItemMap.TryGetValue(audioChannel.name, out var audioItem);
                    var audioNote = new AudioNote
                    {
                        AudioLevyingPosition = isContinuous ? (uint)(wait - _bmsonPositionWaitMap[lastAudioNotePosition]) : 0U,
                        AudioItem = audioItem,
                        Length = i + 1 < notesLength ? (uint?)(_bmsonPositionWaitMap[notes[i + 1].y] - wait) : null
                    };
                    var input = GetBMSONInput(note.x, InputMode);
                    if (input > 0)
                    {
                        var targetNote = Notes.Find(note => note.Wait == wait && note.LevyingInput == input);
                        if (targetNote != null)
                        {
                            targetNote.AudioNotes.Add(audioNote);
                        }
                        else
                        {
                            if (note.l > 0L)
                            {
                                Notes.Add(new LongNote(logicalY, wait, new[] { audioNote }, input, _bmsonPositionWaitMap[bmsonPosition + note.l] - wait, logicalY - (ComponentValue.LevyingHeight + _bmsonPositionLogicalYMap[bmsonPosition + note.l])));
                            }
                            else
                            {
                                Notes.Add(new InputNote(logicalY, wait, new[] { audioNote }, input));
                            }
                        }
                    }
                    else if (audioItem != null)
                    {
                        defaultComputer.WaitAudioNoteMap.Into(wait, audioNote);
                    }
                    if (!isContinuous)
                    {
                        lastAudioNotePosition = bmsonPosition;
                    }
                }
            }
            foreach (var trapChannel in _text.mine_channels)
            {
                foreach (var note in trapChannel.notes.OrderBy(note => note.y))
                {
                    var input = GetBMSONInput(note.x, InputMode);
                    if (input > 0)
                    {
                        var bmsonPosition = note.y;
                        var logicalY = ComponentValue.LevyingHeight + _bmsonPositionLogicalYMap[bmsonPosition];
                        var wait = _bmsonPositionWaitMap[bmsonPosition];
                        audioFileNameAudioItemMap.TryGetValue(trapChannel.name, out var audioItem);
                        var audioNote = new AudioNote
                        {
                            AudioItem = audioItem
                        };
                        Notes.Add(new TrapNote(logicalY, wait, new[] { audioNote }, input, note.damage > 0));
                    }
                }
            }
            foreach (var (bmsonPosition, bpm) in PositionBPMMap)
            {
                WaitBPMMap[_bmsonPositionWaitMap[bmsonPosition]] = bpm;
            }
            foreach (var stopEvent in _text.stop_events)
            {
                WaitStopMap[_bmsonPositionWaitMap[stopEvent.y]] = (double)stopEvent.duration / _res;
            }
        }
    }
}