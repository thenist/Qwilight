using MoonSharp.Interpreter;
using Qwilight.Compute;
using Qwilight.Note;
using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.IO;

namespace Qwilight.Compiler
{
    public abstract class BaseCompiler
    {
        public static BaseCompiler GetCompiler(BaseNoteFile noteFile, CancellationTokenSource setCancelCompiler) => noteFile switch
        {
            PMSFile pmsFile => new PMSCompiler(pmsFile, setCancelCompiler),
            BMSONFile bmsonFile => new BMSONCompiler(bmsonFile, setCancelCompiler),
            BMSFile bmsFile => new BMSCompiler(bmsFile, setCancelCompiler),
            _ => default
        };

        public Component.InputMode InputMode { get; set; }

        public BaseNoteFile NoteFile { get; }

        public CancellationTokenSource SetCancelCompiler { get; }

        public int NoteFormatID { get; }

        public List<BaseNote> Notes { get; } = new();

        public Component ComponentValue { get; set; }

        public BaseCompiler(BaseNoteFile noteFile, CancellationTokenSource setCancelCompiler)
        {
            NoteFile = noteFile;
            SetCancelCompiler = setCancelCompiler;
            NoteFormatID = DB.Instance.GetFormat(noteFile);
        }

        public abstract void CompileImpl(Computing targetComputing, byte[] noteFileContents, int salt);

        public abstract void CompileImpl(DefaultCompute defaultComputer, byte[] noteFileContents, bool loadParallelItems);

        public Dictionary<double, double> WaitBPMMap { get; } = new();

        public Dictionary<double, double> WaitStopMap { get; } = new();

        public SortedDictionary<double, double> PositionBPMMap { get; } = new();

        public SortedDictionary<double, int> PositionStandNoteCountMap { get; } = new();

        public SortedDictionary<double, double> WaitMultiplierMap { get; } = new(Comparer<double>.Create((x, y) => y.CompareTo(x)));

        public double HighestPosition { get; set; }

        public abstract double GetWaitValue(double waitPosition);

        void OnCompiled(Computing targetComputing)
        {
            targetComputing.InputMode = InputMode;
        }

        public void Compile(Computing targetComputing, byte[] noteFileContents, int salt)
        {
            try
            {
                HandleCompile(targetComputing, noteFileContents, salt);
                OnCompiled(targetComputing);
                targetComputing.OnCompiled();
            }
            catch (Exception e)
            {
                targetComputing.OnFault(e);
            }
        }

        void HandleCompile(Computing targetComputing, byte[] noteFileContents, int salt)
        {
            CompileImpl(targetComputing, noteFileContents, salt);
            if (!string.IsNullOrEmpty(targetComputing.NoteDrawingName))
            {
                targetComputing.NoteDrawingPath = Path.Combine(NoteFile.EntryItem.EntryPath, targetComputing.NoteDrawingName);
            }
            if (!string.IsNullOrEmpty(targetComputing.BannerDrawingName))
            {
                targetComputing.BannerDrawingPath = Path.Combine(NoteFile.EntryItem.EntryPath, targetComputing.BannerDrawingName);
            }
            var trailerAudioName = targetComputing.TrailerAudioName;
            targetComputing.TrailerAudioPath = Path.Combine(NoteFile.EntryItem.EntryPath, string.IsNullOrEmpty(trailerAudioName) ? "PREVIEW.WAV" : trailerAudioName);
            var rawHighestBPM = targetComputing.LevyingBPM;
            if (PositionBPMMap.Count > 0)
            {
                targetComputing.LowestBPM = Math.Min(PositionBPMMap.Values.Min(), targetComputing.LevyingBPM);
                targetComputing.HighestBPM = Math.Max(PositionBPMMap.Values.Max(), targetComputing.LevyingBPM);
                rawHighestBPM = targetComputing.HighestBPM;
                var bpmWaitMap = new Dictionary<double, double>();
                var lastBPM = targetComputing.LevyingBPM;
                var lastBPMPosition = 0.0;
                foreach (var (bpmPosition, bpm) in PositionBPMMap)
                {
                    if (bpmPosition <= HighestPosition)
                    {
                        bpmWaitMap[lastBPM] = bpmWaitMap.GetValueOrDefault(lastBPM) + GetWaitValue(bpmPosition) - GetWaitValue(lastBPMPosition);
                        lastBPMPosition = bpmPosition;
                        lastBPM = bpm;
                    }
                }
                bpmWaitMap[lastBPM] = bpmWaitMap.GetValueOrDefault(lastBPM) + GetWaitValue(HighestPosition) - GetWaitValue(lastBPMPosition);
                var longestWait = bpmWaitMap.Values.Max();
                var longestBPMs = bpmWaitMap.Where(pair => pair.Value == longestWait).Select(pair => pair.Key).ToArray();
                if (longestBPMs.Length > 0)
                {
                    var validBPMs = longestBPMs.Where(bpm => bpm > 0.5 && bpm < 65536).ToArray();
                    if (validBPMs.Length > 0)
                    {
                        targetComputing.BPM = validBPMs.Min();
                    }
                    else
                    {
                        var validBPMWaitValues = bpmWaitMap.Where(pair =>
                        {
                            var bpm = pair.Key;
                            return bpm > 0.5 && bpm < 65536;
                        }).ToArray();
                        if (validBPMWaitValues.Length > 0)
                        {
                            var validLongestWait = validBPMWaitValues.Max(pair => pair.Value);
                            targetComputing.BPM = validBPMWaitValues.Where(pair => pair.Value == validLongestWait).Min().Key;
                        }
                        else
                        {
                            var bpm = longestBPMs.Max();
                            if (bpm % 100001 == 0)
                            {
                                targetComputing.BPM = bpm / 100001;
                            }
                            else
                            {
                                targetComputing.BPM = bpm;
                            }
                        }
                    }
                }
            }
            else
            {
                if (targetComputing.LevyingBPM % 100001 == 0)
                {
                    targetComputing.BPM = targetComputing.LevyingBPM / 100001;
                }
                else
                {
                    targetComputing.BPM = targetComputing.LevyingBPM;
                }
                targetComputing.LowestBPM = targetComputing.BPM;
                targetComputing.HighestBPM = targetComputing.BPM;
            }
            targetComputing.Length = GetWaitValue(HighestPosition);
            targetComputing.IsHellBPM &= rawHighestBPM >= 65536;
            if (PositionStandNoteCountMap.Count > 0)
            {
                var positionStandNoteCounts = PositionStandNoteCountMap.ToArray();
                var lowestPosition = 0;
                var lowestWait = GetWaitValue(positionStandNoteCounts[lowestPosition].Key);
                var lowestCount = positionStandNoteCounts[lowestPosition].Value;
                var highestInputCount = lowestCount;
                var i = 1;
                while (i < positionStandNoteCounts.Length)
                {
                    var positionStandNoteCount = positionStandNoteCounts[i];
                    if (GetWaitValue(positionStandNoteCount.Key) - lowestWait < 1000.0)
                    {
                        highestInputCount += positionStandNoteCount.Value;
                        ++i;
                    }
                    else
                    {
                        targetComputing.HighestInputCount = Math.Max(highestInputCount, targetComputing.HighestInputCount);
                        highestInputCount -= lowestCount;
                        ++lowestPosition;
                        lowestWait = GetWaitValue(positionStandNoteCounts[lowestPosition].Key);
                        lowestCount = positionStandNoteCounts[lowestPosition].Value;
                    }
                }
                targetComputing.HighestInputCount = Math.Max(highestInputCount, targetComputing.HighestInputCount);
            }
            targetComputing.PlatformText = Utility.GetPlatformText(targetComputing.Title, targetComputing.Artist, targetComputing.GenreText, targetComputing.LevelText);
            targetComputing.AssistFileName = Path.GetFileName(Utility.GetFiles(NoteFile.EntryItem.EntryPath).FirstOrDefault(filePath => filePath.IsTailCaselsss(".txt") && filePath.ContainsCaselsss("README")));
        }

        public void Compile(DefaultCompute defaultComputer, bool loadParallelItems)
        {
            try
            {
                if (NoteFile.IsValid())
                {
                    defaultComputer.NoteFileContents = NoteFile.GetContents();
                    HandleCompile(defaultComputer, defaultComputer.NoteFileContents, defaultComputer.ModeComponentValue.Salt);
                    HandleCompile(defaultComputer, defaultComputer.NoteFileContents, loadParallelItems);
                    OnCompiled(defaultComputer);
                    defaultComputer.OnCompiled();
                }
                else
                {
                    throw new InvalidOperationException(LanguageSystem.Instance.EditedNoteFileFault);
                }
            }
            catch (OperationCanceledException)
            {
                defaultComputer.OnStopped();
            }
            catch (ScriptRuntimeException e)
            {
                defaultComputer.OnFault(e);
            }
            catch (Exception e)
            {
                defaultComputer.OnFault(e);
            }
            finally
            {
                defaultComputer.SetCompilingStatus(0.0);
            }
        }

        void HandleCompile(DefaultCompute defaultComputer, byte[] noteFileContents, bool loadParallelItems)
        {
            CompileImpl(defaultComputer, noteFileContents, loadParallelItems);
            if (defaultComputer.IsLongNoteStand1)
            {
                defaultComputer.TotalNotes = Notes.Sum(note => note.HasStand ? 1 : 0);
            }
            else
            {
                defaultComputer.TotalNotes = Notes.Sum(note => note.HasStand ? note.LongWait > 0.0 ? 2 : 1 : 0);
            }

            if (defaultComputer.LoopingBanalMedia != null)
            {
                defaultComputer.WaitMediaNoteMap.NewValue(0.0, new MediaNote
                {
                    MediaMode = MediaNote.Mode.Default,
                    MediaItem = defaultComputer.LoopingBanalMedia,
                    HasContents = true
                });
            }
            if (defaultComputer.LoopingBanalFailedMedia != null)
            {
                defaultComputer.WaitMediaNoteMap.NewValue(0.0, new MediaNote
                {
                    MediaMode = MediaNote.Mode.Failed,
                    MediaItem = defaultComputer.LoopingBanalFailedMedia,
                    HasContents = true
                });
            }

            // 특정 도전 과제용 라이프 게이지를 사용합니다.
            if (defaultComputer.ModeComponentValue.HitPointsModeValue == ModeComponent.HitPointsMode.Test)
            {
                defaultComputer.HitPointsValue = 0.001;
            }

            // 1P를 2P로 복사합니다.
            if (defaultComputer.ModeComponentValue.PutCopyNotesAvailable)
            {
                switch (InputMode)
                {
                    case Component.InputMode._5_1:
                        switch (defaultComputer.ModeComponentValue.PutCopyNotesValueV2)
                        {
                            case ModeComponent.PutCopyNotes.Copy:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var targetNote = CopyNote(Notes[i]);
                                    if (targetNote != null)
                                    {
                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 12;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput += 5;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                            case ModeComponent.PutCopyNotes.P1Symmetric:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var note = Notes[i];
                                    var targetNote = CopyNote(note);
                                    if (targetNote != null)
                                    {
                                        if (note.LevyingInput != 1)
                                        {
                                            note.LevyingInput = 8 - note.LevyingInput;
                                        }

                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 12;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput += 5;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                            case ModeComponent.PutCopyNotes.P2Symmetric:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var targetNote = CopyNote(Notes[i]);
                                    if (targetNote != null)
                                    {
                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 12;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput = 13 - targetNote.LevyingInput;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                        }
                        InputMode = Component.InputMode._10_2;
                        break;
                    case Component.InputMode._7_1:
                        switch (defaultComputer.ModeComponentValue.PutCopyNotesValueV2)
                        {
                            case ModeComponent.PutCopyNotes.Copy:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var targetNote = CopyNote(Notes[i]);
                                    if (targetNote != null)
                                    {
                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 16;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput += 7;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                            case ModeComponent.PutCopyNotes.P1Symmetric:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var note = Notes[i];
                                    var targetNote = CopyNote(note);
                                    if (targetNote != null)
                                    {
                                        if (note.LevyingInput != 1)
                                        {
                                            note.LevyingInput = 10 - note.LevyingInput;
                                        }

                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 16;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput += 7;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                            case ModeComponent.PutCopyNotes.P2Symmetric:
                                for (var i = Notes.Count - 1; i >= 0; --i)
                                {
                                    var targetNote = CopyNote(Notes[i]);
                                    if (targetNote != null)
                                    {
                                        if (targetNote.LevyingInput == 1)
                                        {
                                            targetNote.LevyingInput = 16;
                                        }
                                        else
                                        {
                                            targetNote.LevyingInput = 17 - targetNote.LevyingInput;
                                        }
                                        Notes.Add(targetNote);
                                    }
                                }
                                break;
                        }
                        InputMode = Component.InputMode._14_2;
                        break;
                }

                BaseNote CopyNote(BaseNote note) => note switch
                {
                    TrapNote trapNote => new TrapNote(trapNote.LogicalY, trapNote.Wait, trapNote.AudioNotes, trapNote.LevyingInput),
                    LongNote longNote => new LongNote(longNote.LogicalY, longNote.Wait, longNote.AudioNotes, longNote.LevyingInput, longNote.LongWait, longNote.LongHeight),
                    VoidNote voidNote => new VoidNote(voidNote.LogicalY, voidNote.Wait, voidNote.AudioNotes, voidNote.LevyingInput),
                    InputNote inputNote => new InputNote(inputNote.LogicalY, inputNote.Wait, inputNote.AudioNotes, inputNote.LevyingInput),
                    _ => null
                };
            }

            var levyingInputMode = InputMode;
            var inputFavorModeValue = defaultComputer.ModeComponentValue.InputFavorModeValue;
            if (inputFavorModeValue != ModeComponent.InputFavorMode.Default)
            {
                var defaultFavorInputs = Component.FavorInputs[(int)InputMode, (int)inputFavorModeValue];
                for (var i = Notes.Count - 1; i >= 0; --i)
                {
                    var note = Notes[i];
                    if (note.HasInput)
                    {
                        var input = defaultFavorInputs[note.LevyingInput];
                        if (input > 0)
                        {
                            note.LevyingInput = input;
                        }
                        else
                        {
                            Notes.RemoveAt(i);
                            foreach (var audioNote in note.AudioNotes)
                            {
                                defaultComputer.WaitAudioNoteMap.NewValue(note.Wait, audioNote);
                            }
                        }
                    }
                }
                switch (inputFavorModeValue)
                {
                    case ModeComponent.InputFavorMode.Mode4:
                        InputMode = Component.InputMode._4;
                        break;
                    case ModeComponent.InputFavorMode.Mode5:
                        InputMode = Component.InputMode._5;
                        break;
                    case ModeComponent.InputFavorMode.Mode6:
                        InputMode = Component.InputMode._6;
                        break;
                    case ModeComponent.InputFavorMode.Mode7:
                        InputMode = Component.InputMode._7;
                        break;
                    case ModeComponent.InputFavorMode.Mode8:
                        InputMode = Component.InputMode._8;
                        break;
                    case ModeComponent.InputFavorMode.Mode9:
                        InputMode = Component.InputMode._9;
                        break;
                    case ModeComponent.InputFavorMode.Mode51:
                        InputMode = Component.InputMode._5_1;
                        break;
                    case ModeComponent.InputFavorMode.Mode71:
                        InputMode = Component.InputMode._7_1;
                        break;
                    case ModeComponent.InputFavorMode.Mode102:
                        InputMode = Component.InputMode._10_2;
                        break;
                    case ModeComponent.InputFavorMode.Mode142:
                        InputMode = Component.InputMode._14_2;
                        break;
                    case ModeComponent.InputFavorMode.Mode10:
                        InputMode = Component.InputMode._10;
                        break;
                    case ModeComponent.InputFavorMode.Mode242:
                        InputMode = Component.InputMode._24_2;
                        break;
                    case ModeComponent.InputFavorMode.Mode484:
                        InputMode = Component.InputMode._48_4;
                        break;
                }
            }

            var inputCount = Component.InputCounts[(int)InputMode];
            var autoableInputCount = Component.AutoableInputCounts[(int)InputMode];
            var autoableInputs = Component.AutoableInputs[(int)InputMode];
            var isCounterWave = defaultComputer.ModeComponentValue.WaveModeValue == ModeComponent.WaveMode.Counter;

            foreach (var (wait, bpm) in WaitBPMMap)
            {
                defaultComputer.WaitBPMMap[wait] = bpm;
            }
            foreach (var (wait, stop) in WaitStopMap)
            {
                var lastBPMs = defaultComputer.WaitBPMMap.Where(pair => pair.Key <= wait).ToArray();
                var lastBPM = lastBPMs.Length > 0 ? lastBPMs.Last().Value : defaultComputer.LevyingBPM;
                ComponentValue.SetBPM(lastBPM);
                defaultComputer.WaitBPMMap[wait] = 0.0;
                defaultComputer.WaitBPMMap[wait + ComponentValue.MillisMeter * stop] = lastBPM;
            }
            var lastWait = double.MaxValue;
            foreach (var (wait, multiplier) in WaitMultiplierMap)
            {
                var lastBPMs = defaultComputer.WaitBPMMap.Where(pair => pair.Key <= wait).ToArray();
                defaultComputer.WaitBPMMap[wait] = lastBPMs.Length > 0 ? lastBPMs.Last().Value : defaultComputer.LevyingBPM;
                foreach (var targetWait in defaultComputer.WaitBPMMap.Keys.Where(targetWait => wait <= targetWait && targetWait < lastWait).ToArray())
                {
                    defaultComputer.WaitBPMMap[targetWait] *= multiplier;
                }
                lastWait = wait;
            }

            switch (defaultComputer.ModeComponentValue.BPMModeValue)
            {
                case ModeComponent.BPMMode.Not:
                    defaultComputer.WaitBPMMap.Clear();
                    defaultComputer.LevyingBPM = defaultComputer.BPM;
                    ComponentValue = new(defaultComputer.LevyingBPM);
                    foreach (var note in Notes)
                    {
                        note.LogicalY = ComponentValue.LevyingHeight - note.Wait * ComponentValue.LogicalYMillis;
                        note.LongHeight = note.LongWait * ComponentValue.LogicalYMillis;
                    }
                    break;
            }

            var audioNoteSaltComputer = new Random(defaultComputer.ModeComponentValue.Salt);
            foreach (var (wait, audioNotes) in defaultComputer.WaitAudioNoteMap.ToArray())
            {
                defaultComputer.WaitAudioNoteMap[wait] = audioNotes.Select(audioNote => new AudioNote()
                {
                    AudioItem = audioNote.AudioItem,
                    Length = audioNote.Length,
                    AudioLevyingPosition = audioNote.AudioLevyingPosition,
                    Salt = audioNoteSaltComputer.Next()
                }).ToList();
            }

            Notes.Sort();
            var inputNotes = Notes.Where(note => note.HasInput).ToArray();
            if (inputNotes.Length > 0)
            {
                var saltComputer = new Random(defaultComputer.ModeComponentValue.Salt);
                foreach (var inputNote in inputNotes)
                {
                    inputNote.Salt = saltComputer.Next();
                }
                var inputSaltComputer = new Random(defaultComputer.ModeComponentValue.Salt);
                var inputSalts = new int[inputCount + 1];
                for (var i = inputCount; i >= 0; --i)
                {
                    inputSalts[i] = inputSaltComputer.Next();
                }
                foreach (var toSaltNote in inputNotes)
                {
                    toSaltNote.InputSalt = inputSalts[toSaltNote.LevyingInput];
                }
                if (defaultComputer.ModeComponentValue.NoteSaltModeValue != ModeComponent.NoteSaltMode.Default)
                {
                    var noteSaltComputer = new Random(defaultComputer.ModeComponentValue.Salt);
                    var isHalfInputSalt = defaultComputer.ModeComponentValue.NoteSaltModeValue == ModeComponent.NoteSaltMode.HalfInputSalt;
                    switch (defaultComputer.ModeComponentValue.NoteSaltModeValue)
                    {
                        case ModeComponent.NoteSaltMode.InputSalt:
                        case ModeComponent.NoteSaltMode.HalfInputSalt:
                            SaltInput(inputNotes);
                            break;
                        case ModeComponent.NoteSaltMode.MeterSalt:
                            var meterWaitCount = defaultComputer.MeterWaitMap.Count;
                            var levyingMeterWait = defaultComputer.MeterWaitMap[0];
                            switch (defaultComputer.NoteSaltModeDate)
                            {
                                case Component.NoteSaltModeDate._1_14_27:
                                    for (var i = 1; i < meterWaitCount; ++i)
                                    {
                                        var endMeterWait = defaultComputer.MeterWaitMap[i];
                                        var inputNotesInMeter = inputNotes.Where(note =>
                                        {
                                            var wait = note.Wait;
                                            return levyingMeterWait <= wait && wait < endMeterWait;
                                        }).ToArray();
                                        if (inputNotesInMeter.Length > 0 && endMeterWait <= inputNotesInMeter.Max(note => note.Wait + note.LongWait))
                                        {
                                            SaltInput(Array.Empty<BaseNote>());
                                            continue;
                                        }
                                        SaltInput(inputNotes.Where(note =>
                                        {
                                            var wait = note.Wait;
                                            return levyingMeterWait <= wait && wait < endMeterWait;
                                        }).ToArray());
                                        levyingMeterWait = endMeterWait;
                                        defaultComputer.SetCompilingStatus((double)i / (meterWaitCount - 1));
                                    }
                                    break;
                                case Component.NoteSaltModeDate._1_6_11:
                                    for (var i = 1; i <= meterWaitCount; ++i)
                                    {
                                        var endMeterWait = i < meterWaitCount ? defaultComputer.MeterWaitMap[i] : double.PositiveInfinity;
                                        var inputNotesInMeter = inputNotes.Where(note =>
                                        {
                                            var wait = note.Wait;
                                            return levyingMeterWait <= wait && wait < endMeterWait;
                                        }).ToArray();
                                        if (inputNotesInMeter.All(note => note.Wait + note.LongWait < endMeterWait))
                                        {
                                            SaltInput(inputNotesInMeter);
                                            levyingMeterWait = endMeterWait;
                                        }
                                        defaultComputer.SetCompilingStatus((double)i / meterWaitCount);
                                    }
                                    break;
                            }
                            break;
                        case ModeComponent.NoteSaltMode.Symmetric:
                            foreach (var inputNote in inputNotes)
                            {
                                var input = inputNote.LevyingInput;
                                if (!autoableInputs.Contains(input))
                                {
                                    inputNote.LevyingInput = autoableInputCount switch
                                    {
                                        0 => inputCount + 1 - input,
                                        1 => inputCount + 2 - input,
                                        2 => inputCount + 1 - input,
                                        4 => inputCount + 1 - input,
                                        _ => default
                                    };
                                }
                            }
                            break;
                        case ModeComponent.NoteSaltMode.Salt:
                            var endStatus = inputNotes.Length;
                            var status = 0;
                            var saltedNotes = new List<BaseNote>();
                            switch (defaultComputer.NoteSaltModeDate)
                            {
                                case Component.NoteSaltModeDate._1_14_27:
                                    foreach (var inputNote in inputNotes)
                                    {
                                        var input = inputNote.LevyingInput;
                                        if (!autoableInputs.Contains(input))
                                        {
                                            var levyingInput = autoableInputCount switch
                                            {
                                                0 => 1,
                                                1 => 2,
                                                2 => 2,
                                                4 => 3,
                                                _ => default
                                            };
                                            var lastInput = autoableInputCount switch
                                            {
                                                0 => inputCount + 1,
                                                1 => inputCount + 1,
                                                2 => inputCount,
                                                4 => inputCount - 1,
                                                _ => default
                                            };
                                            var wait = inputNote.Wait;
                                            var longWait = inputNote.LongWait;
                                            var levyingSaltInput = inputNote.Salt % (lastInput - levyingInput) + levyingInput;
                                            var saltInput = levyingSaltInput;
                                            var loopCount = lastInput - levyingInput;
                                            do
                                            {
                                                if (saltedNotes.Any(note =>
                                                {
                                                    return note != inputNote && note.LevyingInput == saltInput && note.IsCollided(inputNote);
                                                }))
                                                {
                                                    if (++saltInput >= lastInput)
                                                    {
                                                        saltInput = levyingInput;
                                                    }
                                                    if (--loopCount > 0)
                                                    {
                                                        continue;
                                                    }
                                                }
                                                break;
                                            } while (true);
                                            saltedNotes.Add(inputNote);
                                            inputNote.LevyingInput = saltInput;
                                        }
                                    }
                                    break;
                                case Component.NoteSaltModeDate._1_6_11:
                                    foreach (var inputNote in inputNotes)
                                    {
                                        var input = inputNote.LevyingInput;
                                        if (!autoableInputs.Contains(input))
                                        {
                                            var inputCountLength = inputCount - autoableInputCount;
                                            var saltedInputs = new int[inputCountLength];
                                            for (var i = inputCountLength - 1; i >= 0; --i)
                                            {
                                                switch (autoableInputCount)
                                                {
                                                    case 0:
                                                        saltedInputs[i] = i + 1;
                                                        break;
                                                    case 1:
                                                    case 2:
                                                        saltedInputs[i] = i + 2;
                                                        break;
                                                    case 4:
                                                        saltedInputs[i] = i + 3;
                                                        break;
                                                }
                                            }
                                            saltedInputs = saltedInputs.Except(saltedNotes.Where(note => note.IsCollided(inputNote)).Select(note => note.LevyingInput)).ToArray();
                                            noteSaltComputer.Shuffle(saltedInputs);
                                            inputNote.LevyingInput = saltedInputs.First();
                                            saltedNotes.Add(inputNote);
                                        }
                                        defaultComputer.SetCompilingStatus((double)++status / endStatus);
                                    }
                                    break;
                            }
                            break;
                    }
                    // 라인 랜덤
                    void SaltInput(BaseNote[] notes)
                    {
                        var inputCountLength = inputCount - autoableInputCount;
                        var saltedInputs = new int[inputCountLength];
                        for (var i = inputCountLength - 1; i >= 0; --i)
                        {
                            switch (autoableInputCount)
                            {
                                case 0:
                                    saltedInputs[i] = i + 1;
                                    break;
                                case 1:
                                case 2:
                                    saltedInputs[i] = i + 2;
                                    break;
                                case 4:
                                    saltedInputs[i] = i + 3;
                                    break;
                            }
                        }
                        switch (defaultComputer.NoteSaltModeDate)
                        {
                            case Component.NoteSaltModeDate._1_14_27:
                                if (isHalfInputSalt)
                                {
                                    var endInputCountHalfLength = (int)Math.Ceiling(inputCountLength / 2.0);
                                    for (var i = inputCountLength - 1; i > endInputCountHalfLength; --i)
                                    {
                                        var j = endInputCountHalfLength + noteSaltComputer.Next(inputCountLength - endInputCountHalfLength);
                                        (saltedInputs[i], saltedInputs[j]) = (saltedInputs[j], saltedInputs[i]);
                                    }
                                    var levyingInputCountHalfLength = inputCountLength / 2;
                                    for (var i = levyingInputCountHalfLength - 1; i > 0; --i)
                                    {
                                        var j = noteSaltComputer.Next(levyingInputCountHalfLength);
                                        (saltedInputs[i], saltedInputs[j]) = (saltedInputs[j], saltedInputs[i]);
                                    }
                                }
                                else
                                {
                                    for (var i = inputCountLength - 1; i > 0; --i)
                                    {
                                        var j = noteSaltComputer.Next(inputCountLength);
                                        (saltedInputs[i], saltedInputs[j]) = (saltedInputs[j], saltedInputs[i]);
                                    }
                                }
                                break;
                            case Component.NoteSaltModeDate._1_6_11:
                                if (isHalfInputSalt)
                                {
                                    var inputCountHalfLength = inputCountLength / 2;
                                    var frontSaltedInputs = saltedInputs.Take(inputCountHalfLength).ToArray();
                                    noteSaltComputer.Shuffle(frontSaltedInputs);
                                    var tailSaltedInputs = saltedInputs.TakeLast(inputCountHalfLength).ToArray();
                                    noteSaltComputer.Shuffle(tailSaltedInputs);
                                    saltedInputs = frontSaltedInputs.Concat(saltedInputs.Skip(inputCountHalfLength).SkipLast(inputCountHalfLength)).Concat(tailSaltedInputs).ToArray();
                                }
                                else
                                {
                                    noteSaltComputer.Shuffle(saltedInputs);
                                }
                                break;
                        }
                        foreach (var note in notes)
                        {
                            var input = note.LevyingInput;
                            if (!autoableInputs.Contains(input))
                            {
                                note.LevyingInput = autoableInputCount switch
                                {
                                    0 => saltedInputs[input - 1],
                                    1 => saltedInputs[input - 2],
                                    2 => saltedInputs[input - 2],
                                    4 => saltedInputs[input - 3],
                                    _ => default
                                };
                            }
                        }
                    }
                }
            }

            var waitAudioNotes = defaultComputer.WaitAudioNoteMap.Where(pair => pair.Value.Count > 0).ToArray();
            var notes = Notes.Where(note => note.HasContents).ToArray();
            defaultComputer.Length = Utility.Max(notes.Select(note => note.Wait + note.LongWait).DefaultIfEmpty(0.0).Max(),
                    waitAudioNotes.Length > 0 ? waitAudioNotes.Max(pair => pair.Key) : 0.0,
                    defaultComputer.WaitMediaNoteMap.Count > 0 ? defaultComputer.WaitMediaNoteMap.Keys.Max() : 0.0
            );

            var lastLevyingPosition = defaultComputer.Length;
            var lastLevyingPositions = new double[5];
            for (var i = lastLevyingPositions.Length - 1; i >= 0; --i)
            {
                lastLevyingPositions[i] = lastLevyingPosition;
            }
            foreach (var (wait, mediaNotes) in defaultComputer.WaitMediaNoteMap)
            {
                foreach (var mediaNote in defaultComputer.WaitMediaNoteMap[wait])
                {
                    var mediaItem = mediaNote.MediaItem;
                    mediaNote.Length = mediaItem == null || double.IsInfinity(mediaItem.Length) ? lastLevyingPositions[(int)mediaNote.MediaMode] - wait : mediaItem.Length;
                    lastLevyingPositions[(int)mediaNote.MediaMode] = wait;
                }
            }

            defaultComputer.AudioLength = Utility.Max(notes.Select(note =>
            {
                var audioNotes = note.AudioNotes;
                if (audioNotes.Count > 0)
                {
                    return note.Wait + note.LongWait + audioNotes.Max(audioNote => audioNote.Length ?? audioNote.AudioItem?.Length ?? 0.0);
                }
                else
                {
                    return note.Wait + note.LongWait;
                }
            }).DefaultIfEmpty(0.0).Max(),
                waitAudioNotes.Length > 0 ? waitAudioNotes.Max(pair => pair.Key + pair.Value.Max(audioNote => audioNote.Length ?? audioNote.AudioItem?.Length ?? 0.0)) : 0.0,
                defaultComputer.WaitMediaNoteMap.Count > 0 ? defaultComputer.WaitMediaNoteMap.Max(pair => pair.Key + pair.Value.Max(mediaNote => mediaNote.Length)) : 0.0
            );

            // 거꾸로 플레이합니다.
            if (isCounterWave)
            {
                foreach (var note in Notes)
                {
                    note.Wait = defaultComputer.Length + Component.QuitWait - note.Wait - note.LongWait + Component.QuitWait;
                }

                var waitAudioNoteMap = new SortedDictionary<double, List<AudioNote>>(defaultComputer.WaitAudioNoteMap);
                defaultComputer.WaitAudioNoteMap.Clear();
                foreach (var wait in waitAudioNoteMap.Keys)
                {
                    var audioNotes = waitAudioNoteMap[wait];
                    foreach (var audioNote in audioNotes)
                    {
                        Utility.NewValue(defaultComputer.WaitAudioNoteMap, defaultComputer.Length + Component.QuitWait - wait + Component.QuitWait - (audioNote.AudioItem?.Length ?? 0.0), audioNote);
                    }
                }

                var waitMediaNoteMap = new SortedDictionary<double, List<MediaNote>>(defaultComputer.WaitMediaNoteMap);
                defaultComputer.WaitMediaNoteMap.Clear();
                foreach (var wait in waitMediaNoteMap.Keys)
                {
                    var mediaNotes = waitMediaNoteMap[wait];
                    foreach (var mediaNote in mediaNotes)
                    {
                        Utility.NewValue(defaultComputer.WaitMediaNoteMap, defaultComputer.Length + Component.QuitWait - wait + Component.QuitWait - mediaNote.Length, mediaNote);
                    }
                }

                var lastBPM = defaultComputer.LevyingBPM;
                var waitBPMMap = new Queue<KeyValuePair<double, double>>(defaultComputer.WaitBPMMap);
                defaultComputer.WaitBPMMap.Clear();
                foreach (var (wait, bpm) in waitBPMMap)
                {
                    defaultComputer.WaitBPMMap[defaultComputer.Length - wait] = lastBPM;
                    lastBPM = bpm;
                }
                defaultComputer.LevyingBPM = lastBPM;

                SetWaitLogicalYMap();

                foreach (var note in Notes)
                {
                    note.LogicalY = defaultComputer.WaitLogicalYMap[note.Wait];
                    note.LongHeight = note.LongWait > 0.0 ? defaultComputer.WaitLogicalYMap[note.Wait] - defaultComputer.WaitLogicalYMap[note.Wait + note.LongWait] : 0.0;
                }
            }
            else
            {
                SetWaitLogicalYMap();
            }
            void SetWaitLogicalYMap()
            {
                var loopingCounterSet = new SortedSet<double>();
                var waitBPMMap = new Queue<KeyValuePair<double, double>>(defaultComputer.WaitBPMMap);
                foreach (var note in Notes)
                {
                    var wait = note.Wait;
                    loopingCounterSet.Add(wait);
                    var longWait = note.LongWait;
                    if (longWait > 0.0)
                    {
                        loopingCounterSet.Add(wait + longWait);
                    }
                }
                foreach (var wait in defaultComputer.WaitAudioNoteMap.Keys)
                {
                    loopingCounterSet.Add(wait);
                }
                foreach (var wait in defaultComputer.WaitMediaNoteMap.Keys)
                {
                    loopingCounterSet.Add(wait);
                }
                var quitLength = defaultComputer.Length + Component.QuitWait;
                for (var loopingCounter = -Component.LevyingWait; loopingCounter <= quitLength; loopingCounter += 1.0)
                {
                    SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                    loopingCounterSet.Add(loopingCounter);
                }

                defaultComputer.WaitLogicalYMap.Clear();
                var lastLoopingCounter = loopingCounterSet.Min();
                ComponentValue.SetBPM(defaultComputer.LevyingBPM);
                defaultComputer.WaitLogicalYMap[lastLoopingCounter] = Component.StandardHeight - ComponentValue.LogicalYMillis * (lastLoopingCounter + Component.LevyingWait);
                foreach (var loopingCounter in loopingCounterSet)
                {
                    SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                    defaultComputer.WaitLogicalYMap[loopingCounter] = defaultComputer.WaitLogicalYMap[lastLoopingCounter] - Utility.GetDistance(ComponentValue, waitBPMMap, lastLoopingCounter, loopingCounter, out _);
                    lastLoopingCounter = loopingCounter;
                }
            }

            // 배경음을 노트로 만듭니다.
            Notes.Sort();
            var setNoteModeValue = defaultComputer.ModeComponentValue.SetNoteModeValue;
            if (setNoteModeValue == ModeComponent.SetNoteMode.Put || setNoteModeValue == ModeComponent.SetNoteMode.VoidPut)
            {
                var voidPutInputs = Component.VoidPutInputs[(int)levyingInputMode, (int)InputMode];
                var putNoteSet = defaultComputer.ModeComponentValue.PutNoteSet;
                var putNoteSetMillis = defaultComputer.ModeComponentValue.PutNoteSetMillis;
                var endStatus = defaultComputer.WaitAudioNoteMap.Count;
                var status = 0;
                foreach (var (wait, audioNotes) in defaultComputer.WaitAudioNoteMap)
                {
                    foreach (var audioNote in audioNotes.ToArray())
                    {
                        if (audioNote.Salt % 100 < putNoteSet)
                        {
                            switch (setNoteModeValue)
                            {
                                case ModeComponent.SetNoteMode.Put:
                                    switch (autoableInputCount)
                                    {
                                        case 0:
                                            NewNote(audioNote, GetInputs(1, inputCount));
                                            break;
                                        case 1:
                                            NewNote(audioNote, GetInputs(2, inputCount));
                                            break;
                                        case 2:
                                            NewNote(audioNote, GetInputs(2, inputCount - 1));
                                            break;
                                    }
                                    break;
                                case ModeComponent.SetNoteMode.VoidPut:
                                    NewNote(audioNote, voidPutInputs);
                                    break;
                            }
                            int[] GetInputs(int levyingInput, int lastInput)
                            {
                                var inputs = new int[lastInput - levyingInput + 1];
                                for (var i = lastInput; i >= levyingInput; --i)
                                {
                                    inputs[i - levyingInput] = i;
                                }
                                return inputs;
                            }
                            void NewNote(AudioNote audioNote, int[] inputs)
                            {
                                if (inputs.Length > 0)
                                {
                                    var levyingPutInput = audioNote.Salt % inputs.Length;
                                    var putInput = levyingPutInput;
                                    var targetInput = inputs[putInput];
                                    do
                                    {
                                        if (Notes.Any(note =>
                                        {
                                            var noteWait = note.Wait;
                                            return note.HasInput && note.LevyingInput == targetInput && noteWait - putNoteSetMillis <= wait && wait <= noteWait + putNoteSetMillis + note.LongWait;
                                        }))
                                        {
                                            if (++putInput >= inputs.Length)
                                            {
                                                putInput = 0;
                                            }
                                            if (putInput == levyingPutInput)
                                            {
                                                break;
                                            }
                                            targetInput = inputs[putInput];
                                            continue;
                                        }
                                        Notes.Add(new InputNote(defaultComputer.WaitLogicalYMap[wait], wait, [audioNote], targetInput));
                                        audioNotes.Remove(audioNote);
                                        break;
                                    } while (true);
                                }
                            }
                        }
                    }
                    defaultComputer.SetCompilingStatus((double)++status / endStatus);
                }
            }

            Notes.Sort();
            switch (defaultComputer.ModeComponentValue.NoteModifyModeValue)
            {
                // 롱 노트를 단일 노트로 만듭니다.
                case ModeComponent.NoteModifyMode.InputNote:
                    for (var i = Notes.Count - 1; i >= 0; --i)
                    {
                        var note = Notes[i];
                        if (note.LongWait > 0.0)
                        {
                            Notes[i] = new InputNote(note.LogicalY, note.Wait, note.AudioNotes, note.LevyingInput);
                        }
                    }
                    break;
                // 단일 노트를 롱 노트로 만듭니다.
                case ModeComponent.NoteModifyMode.LongNote:
                    var waitBPMMap = new Queue<KeyValuePair<double, double>>(defaultComputer.WaitBPMMap);
                    var lowestLongNoteModify = defaultComputer.ModeComponentValue.ConfigureLowestLongNoteModify;
                    var highestLongNoteModify = defaultComputer.ModeComponentValue.ConfigureHighestLongNoteModify;
                    var distanceLongNoteModify = highestLongNoteModify - lowestLongNoteModify;
                    for (var i = inputCount; i > 0; --i)
                    {
                        var targetInputNotes = Notes.Where(note => note.LevyingInput == i).ToArray();
                        var targetInputNoteCount = targetInputNotes.Length;
                        for (var j = 0; j < targetInputNoteCount; ++j)
                        {
                            var targetInputNote = targetInputNotes[j];
                            if (j < targetInputNoteCount - 1 && targetInputNote.LongWait == 0.0 && targetInputNote.HasStand)
                            {
                                var longNoteModify = distanceLongNoteModify > 0.0 ? lowestLongNoteModify + targetInputNote.Salt % distanceLongNoteModify : lowestLongNoteModify;
                                var targetNote = targetInputNotes[j + 1];
                                var noteWait = targetInputNote.Wait;
                                var targetLoopingCounter = targetNote.Wait;
                                var loopingCounter = targetLoopingCounter - longNoteModify;
                                if (noteWait < loopingCounter)
                                {
                                    var lastBPMs = defaultComputer.WaitBPMMap.Where(pair => pair.Key <= loopingCounter).ToArray();
                                    ComponentValue.SetBPM(lastBPMs.Length > 0 ? lastBPMs.Last().Value : defaultComputer.LevyingBPM);
                                    var distance = Utility.GetDistance(ComponentValue, waitBPMMap, loopingCounter, targetLoopingCounter, out _);
                                    Notes.Remove(targetInputNote);
                                    Notes.Add(new LongNote(targetInputNote.LogicalY, noteWait, targetInputNote.AudioNotes, targetInputNote.LevyingInput, targetLoopingCounter - noteWait - longNoteModify, targetInputNote.LogicalY - targetNote.LogicalY - distance));
                                }
                            }
                        }
                    }
                    break;
            }

            // 실시간 NPS 그래프 계산
            var isAutoLongNote = defaultComputer.IsAutoLongNote;
            var waitInputNoteCountMap = new SortedDictionary<double, int>();
            var waitAutoableInputNoteCountMap = new SortedDictionary<double, int>();
            foreach (var note in Notes.Where(note => note.HasStand))
            {
                var wait = note.Wait;
                waitInputNoteCountMap[wait] = waitInputNoteCountMap.GetValueOrDefault(wait) + 1;
                var isAutoableInput = autoableInputs.Contains(note.LevyingInput);
                if (isAutoableInput)
                {
                    waitAutoableInputNoteCountMap[wait] = waitAutoableInputNoteCountMap.GetValueOrDefault(wait) + 1;
                }
                if (!isAutoLongNote)
                {
                    var longWait = note.LongWait;
                    if (longWait > 0.0)
                    {
                        waitInputNoteCountMap[wait + longWait] = waitInputNoteCountMap.GetValueOrDefault(wait + longWait) + 1;
                        if (isAutoableInput)
                        {
                            waitAutoableInputNoteCountMap[wait + longWait] = waitAutoableInputNoteCountMap.GetValueOrDefault(wait + longWait) + 1;
                        }
                    }
                }
            }
            SetInputNoteCounts(waitInputNoteCountMap, defaultComputer.InputNoteCounts);
            SetInputNoteCounts(waitAutoableInputNoteCountMap, defaultComputer.AutoableInputNoteCounts);
            void SetInputNoteCounts(SortedDictionary<double, int> waitInputNoteCountMap, List<double> inputNoteCounts)
            {
                if (waitInputNoteCountMap.Count > 0)
                {
                    var length100 = defaultComputer.Length / 100;
                    if (length100 > 0.0)
                    {
                        var length1000 = length100 / 1000;
                        var lowestWait = 0.0;
                        var inputNoteCount = 0;
                        var i = 0;
                        while (i < waitInputNoteCountMap.Count)
                        {
                            var waitInputNoteCount = waitInputNoteCountMap.ElementAt(i);
                            if (waitInputNoteCount.Key - lowestWait < length100)
                            {
                                inputNoteCount += waitInputNoteCount.Value;
                                ++i;
                            }
                            else
                            {
                                inputNoteCounts.Add(inputNoteCount / length1000);
                                inputNoteCount = 0;
                                lowestWait += length100;
                            }
                        }
                        while (lowestWait < defaultComputer.Length)
                        {
                            inputNoteCounts.Add(inputNoteCount / length1000);
                            inputNoteCount = 0;
                            lowestWait += length100;
                        }
                    }
                    else
                    {
                        inputNoteCounts.Add(waitInputNoteCountMap.Single().Value);
                    }
                }
            }

            // 건너뛰기와 하이라이트 계산
            Notes.Sort();
            inputNotes = Notes.Where(note => note.HasInput).Where(note => note.HasStand).ToArray();
            if (inputNotes.Length > 0)
            {
                defaultComputer.PassableWait = Math.Round(inputNotes.First().Wait - Component.PassableWait);
                var lastNoteWait = inputNotes.Max(note => note.Wait + note.LongWait);
                if (double.IsNaN(defaultComputer.AudioLevyingPosition))
                {
                    var targetValue = double.NegativeInfinity;
                    var audioLevyingPosition = 0.0;
                    for (var i = defaultComputer.InputNoteCounts.Count - 1; i > 0; --i)
                    {
                        var value = (defaultComputer.InputNoteCounts[i] - defaultComputer.InputNoteCounts[i - 1]) * Math.Sqrt(50 * 50 - Math.Pow(50 - i, 2));
                        if (value >= targetValue)
                        {
                            audioLevyingPosition = defaultComputer.Length * (i - 1) / 100;
                            targetValue = value;
                        }
                    }
                    defaultComputer.AudioLevyingPosition = audioLevyingPosition;
                }
            }
            else
            {
                if (double.IsNaN(defaultComputer.AudioLevyingPosition))
                {
                    defaultComputer.AudioLevyingPosition = 0.0;
                }
            }

            // 리플레이 노트
            foreach (var inputEvent in defaultComputer.Comment.Inputs.Where(inputEvent => inputEvent.Input > 0))
            {
                var wait = Utility.SetCommentWait(defaultComputer.CommentWaitDate, defaultComputer.AudioMultiplier, inputEvent.Wait);
                wait = Math.Floor(wait);
                var input = inputEvent.Input;
                input = input & 255;
                Notes.Add(new CommentNote(defaultComputer.WaitLogicalYMap[wait], wait, input));
            }

            foreach (var note in Notes)
            {
                if (note.HasStand)
                {
                    foreach (var audioNote in note.AudioNotes)
                    {
                        defaultComputer.WaitInputAudioMap.NewValue(note.Wait, audioNote);
                    }
                }
            }

            if (defaultComputer.IsPostableItemMode)
            {
                var avatarsCount = defaultComputer.AvatarsCount;
                var rateItem = defaultComputer.ValidNetMode switch
                {
                    1 => Math.Sqrt(5.0) / 100.0,
                    2 => 5.0 / 100.0,
                    3 => Math.Pow(5.0, 2) / 100.0,
                    4 => avatarsCount,
                    _ => default
                } / avatarsCount;
                foreach (var note in Notes)
                {
                    if (note.HasStand && Random.Shared.NextDouble() < rateItem)
                    {
                        note.SetItem(Random.Shared.Next(), defaultComputer.AllowedPostableItems);
                    }
                }
            }

            defaultComputer.ValidatedTotalNotes = Notes.Sum(note => note.HasStand ? note.LongWait > 0.0 ? 2 : 1 : 0);

            Notes.Sort();
            Notes.ForEach(note =>
            {
                defaultComputer.Notes.Add(note);
                note.ID = defaultComputer.Notes.Count - 1;
            });
        }
    }
}