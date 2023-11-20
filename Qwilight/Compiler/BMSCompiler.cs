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
    public class BMSCompiler : BaseCompiler
    {
        static readonly char[] _delimiters = new[] { ':', ' ', '.', ';' };
        static readonly Dictionary<string, int> _formatMap = new()
        {
            { "1e517d8a7f7e34e58db1c62aeaf590b7445eb56f12d976af196afb9cda98339108346cacc8939c3cb004eee75f141f1e8104fa7ee425fa95acdf705b3c8064dc:0", 949 },
            { "8247524b799f34b78ff725cd97a2775fd7aed0f8637b2ad9899dc3cb87d9559d0c1ba093671fbf451a6faed185624e1ce01cef43c07f837885489777cd9a6947:0", 949 },
            { "85b2df20ed4b96f7aa62e17fb0480e74d19720b218586f676d90031794c11dba4cba02a65c73e3ed12759a228d7ac9e1d1e1a2f30e20cc74a06c296afe8a1a4f:0", 949 },
            { "af7a5b1825e19909fcd7e03160eb0b62c1ae8368b7825c890c3c7cfc9849e7c6d37aaa45688b88f6d2659587e2a1ae73d59f0ff64427ae1a1b6283cd12fa17e8:0", 949 },
            { "86f8bc62e9b1aeee9cc86111d2411704d20f0746be9da5cbb18f201eb34887923385b55f30736886fa4bb6211b7ce4fa3b5069485eb9c6e8f71e7634d2590fee:0", 949 },
            { "09e972e16e0ab8ee1cb85e67ed6fc3d80a3e6250eeabfcd1c63f6060a14d27fa753fc7d1474daaf8abc0f220d4077533ec06214cc22d12192bfd275145485ee9:0", 949 },
            { "bfd28d1ae0e65cf0196f599936103b565f95f2ce5bc456ed358269e9d9b4c3b5a7c0fc326482336940fe40c3f73ab4e0429c41921c6c23c3c73932a751dc0938:0", 949 },
            { "7754d64f9443af0dd6aa82f79d940ac19e263086caa4d4b4be6c28b51ad80fdd07cbe0fa588fff044a5df3fd818ca7ef3fd7726131da599035544cefcacce20f:0", 949 },
            { "af492b5a23c46b31de7aa624d6348ec3dc1b64221962c1fcc2fbdc92df332ee0ad8af235bb4de842a35dd2b76882f1ac0f41b39e57c67a4f014cd851c2e33bea:0", 949 },
            { "46cdd63acf234ef9830dfcfa3c37e2cb56076672109a08c20f7009064589d2de6980ec694cac11c0ad470fb01c3dc52c280968fcdff05be239ff0eae7f1598a0:0", 949 },
            { "c7c3884ca7033c7b250658d9b18c64f6850a99c69b7b74e478c554cbefcdb3165382c8ef48f17d08765a4ba3c21a69a3e1d480f7fb25ccbde03f5ce0addd8310:0", 949 },
            { "3ad9a18d77732225061e38064b6643491c8e999f6081c308f24f4a459cce7e739ca118e371bbf7b032b42c389736e48b53a966b0b0761b67e27a52257bb142d5:0", 949 },
            { "d1fbc5e49c22c75adc263aca6ae1cf04f5f25efbdfa42256953ee7437c345524cbfa8764b9a0336cada2d0c59fa31e4f5a67af1b71a96596d7532c874c4d507c:0", 949 },
            { "7a94c200d203f690d3cc17393ec5413c5d0213a16dff4b3dad75c94e4c1e6a1295cab5c8dd875c76c97f402e541ff2ee15a95c601095622e5eb6789c67dcd071:0", 949 },
            { "5ac89954044141254dace3e8da705b3ded9b99a54e83d2e70bcd7094ea4446ab9661992669ba67350fd6384d091631bca3bdecd5683c7db2aa06fcb566fb2927:0", 949 },
            { "f404418123d17e4560e9b32d2375dc120c53b9ee850f9ebe6920949950fee3c0397d1cac13537200b0bfbcd754daa71b8db4358418c8e530a9c825fb2309b61e:0", 949 },
            { "beb151001ec1ba01bdcbc269dffb054a664cb5a5bb5979c9dfccaaab9c4fb19bda1e943919ee3cfd3fae022a81b41bffba3565acf5d6d0434dd3a7c5d49bf4c4:0", 949 },
            { "60f8b3d9a7fd626150311623d4f6a428b053eca9a9b062e38db688f65646de92ae44a10a001582e4f4a0d49bac124fd4ae1869799731e2bb1053816ad1d5e451:0", 949 },
            { "cad4905526fd2c9d76ebc0763199fda80be35eb162624290e6cc9c567446fa4eb00588a5a22d0ee2ee4e28c6c684463a172e1ba35329bafde3f201f6fc3c3bd2:0", 949 },
            { "b90e46e5bb5f52f55ac451cf7d13073019670c0dc19f27d5e31fd8d9ee8c2d088cbb1768cb42569a0007992ff512ed7b93a2b43aa5462e0284995e14c97e36a3:0", 949 },
            { "73050fa440f63c2a9f9844824a70df5226e6e0ed1a27b4e9e45b18a9e0b988697cfaf9360a4f568c121213fcd4e518acfbf60e7bb981d8302c0a90b91d990542:0", 949 },
            { "f04bb8bfbd9abd19ef6f166f9a12e750f12270e393881b18acaf902de5e3d50baa14e030a6d598789d1f491912f954ab9dfc82e565f9f7bc7fd4a2f4ea2c4269:0", 949 },
            { "2ee33d8150c20832627ca7c0ce0987ec464575414b989cb48583083707459c5dafd399f22d1f61b720290b8a77bfd26e4b3a95bcb190b3dbcafc29fa40dacf5d:0", 949 },
            { "e9762943bf3434eb1a15aab69e485ed9927c8c6ca9a1f6c7e51756f17fa191fe9a50d96fa2c2764fddb10a1bd716c7c6b0c36d1c64f9fffd94a554bb1de67b96:0", 949 }
        };
        static readonly Dictionary<string, string> _mediaMap = new()
        {
            { "f09141432f4d624126a10a502e0542d6724f1d316cf8987f556c0063be8b77e9a0fd2cd731528e09385c175110af4eabf78fc00e97d1733c4e056a887ccf31e1:0", "BGA.mpg" }
        };

        class BMSInputItem : IComparable<BMSInputItem>
        {
            public InputNote InputNote { get; init; }

            public int CompareTo(BMSInputItem other) => InputNote.CompareTo(other.InputNote);
        }

        sealed class BMSLongInputItem : BMSInputItem
        {
            public string BMSID { get; init; } = string.Empty;
        }

        class EarlyBMSInputItem : IComparable<EarlyBMSInputItem>
        {
            public double BMSPosition { get; init; }

            public int CompareTo(EarlyBMSInputItem other) => BMSPosition.CompareTo(other.BMSPosition);
        }

        sealed class EarlyBMSLongInputItem : EarlyBMSInputItem
        {
            public string BMSID { get; init; } = string.Empty;
        }

        readonly List<string> _lines = new();
        readonly SortedDictionary<double, double> _bmsPositionLogicalYMap = new();
        readonly SortedDictionary<double, double> _bmsPositionMultiplierMap = new();
        readonly Dictionary<string, double> _bmsIDStopMap = new();
        readonly Dictionary<string, double> _bmsIDMultiplierMap = new();
        readonly SortedDictionary<double, double> _bmsPositionWaitMap = new();
        bool _hasMedia;
        bool _hasFailedMedia;
        string _longNoteBMSID;
        int _highestMeter;
        bool _is4K;
        bool _is6K;

        public BMSCompiler(BMSFile bmsFile, CancellationTokenSource setCancelCompiler) : base(bmsFile, setCancelCompiler)
        {
        }

        public override double GetWaitValue(double waitPosition) => _bmsPositionWaitMap[waitPosition];

        public override void CompileImpl(Computing targetComputing, byte[] noteFileContents, int salt)
        {
            var bmsPositionStopMap = new SortedDictionary<double, double>();
            var meterMeterMultiplierMap = new SortedDictionary<int, double>
            {
                { -1, 1.0 }
            };
            var bmsIDBPMMap = new Dictionary<string, double>();
            using (var rms = PoolSystem.Instance.GetDataFlow(noteFileContents))
            {
                var format = NoteFormatID;
                if (format == -1)
                {
                    if (!_formatMap.TryGetValue(NoteFile.GetNoteID512(), out format))
                    {
                        var formatComputer = CharsetDetector.DetectFromStream(rms).Detected;
                        rms.Position = 0;
                        format = formatComputer != null && formatComputer.Confidence >= 0.875 && formatComputer.Encoding != null ? formatComputer.Encoding.CodePage : 932;
                    }
                }
                using var sr = new StreamReader(rms, Encoding.GetEncoding(format), false);
                var saltComputer = new Random(salt);
                var lastBin = 0;
                var isValidStatement = true;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith('#'))
                    {
                        var lineAt1 = line[1..];
                        var delimited = lineAt1.Split(_delimiters, 2);
                        var property = delimited[0];
                        if (property.EqualsCaseless("IF"))
                        {
                            isValidStatement = lastBin == Utility.ToInt32(delimited[1]);
                            continue;
                        }
                        if (isValidStatement)
                        {
                            if (delimited.Length > 1)
                            {
                                if (property.EqualsCaseless("RANDOM"))
                                {
                                    var rValue = Utility.ToInt32(delimited[1]);
                                    lastBin = 1 + saltComputer.Next(rValue);
                                    if (rValue > 1)
                                    {
                                        targetComputing.IsSalt = true;
                                    }
                                    continue;
                                }
                            }
                            _lines.Add(line);
                        }
                        else if (property.EqualsCaseless("ELSE") || property.EqualsCaseless("ENDIF") || property.EqualsCaseless("IFEND"))
                        {
                            isValidStatement = true;
                        }
                    }
                }
            }
            foreach (var line in _lines)
            {
                var lineAt1 = line[1..];
                var delimited = lineAt1.Split(_delimiters, 2);
                if (IsMainBMSData(lineAt1))
                {
                    var property = delimited.Length > 1 ? delimited[0] : delimited[0].Substring(0, 5);
                    property = property.PadLeft(5, '0');
                    var meter = Utility.ToInt32(property.Substring(0, 3));
                    _highestMeter = Math.Max(meter, _highestMeter);
                    if (property[3] == '0' && property[4] == '2')
                    {
                        meterMeterMultiplierMap[meter] = Utility.ToFloat64(delimited.Length > 1 ? delimited[1] : delimited[0][5..]);
                    }
                }
                else
                {
                    var property = delimited[0].Trim();
                    if (delimited.Length > 1)
                    {
                        var data = delimited[1].Trim();
                        if (property.IsFrontCaselsss("BPM"))
                        {
                            if (property.Length > 3)
                            {
                                bmsIDBPMMap[property.Substring(3, 2)] = Utility.ToFloat64(data);
                            }
                            else
                            {
                                if (Utility.ToFloat64(data, out var levyingBPM))
                                {
                                    targetComputing.LevyingBPM = levyingBPM;
                                }
                                else
                                {
                                    var delimitedData = data.Split(" ");
                                    if (delimitedData.Length > 1)
                                    {
                                        bmsIDBPMMap[delimitedData[0]] = Utility.ToFloat64(delimitedData[1]);
                                    }
                                    else
                                    {
                                        targetComputing.LevyingBPM = Utility.ToFloat64(data.Split('-')[0]);
                                    }
                                }
                            }
                        }
                        else if (property.IsFrontCaselsss("BMP"))
                        {
                            if (property == "00")
                            {
                                _hasFailedMedia = true;
                            }
                        }
                        else if (property.EqualsCaseless("LNOBJ"))
                        {
                            _longNoteBMSID = data;
                        }
                        else if (property.EqualsCaseless("LNMODE"))
                        {
                            switch (targetComputing.LongNoteModeDate)
                            {
                                case Component.LongNoteModeDate._1_0_0:
                                    targetComputing.IsAutoLongNote = false;
                                    break;
                                case Component.LongNoteModeDate._1_14_20:
                                case Component.LongNoteModeDate._1_16_4:
                                    switch (Utility.ToInt32(data))
                                    {
                                        case 1:
                                            targetComputing.IsAutoLongNote = true;
                                            break;
                                        case 2:
                                        case 3:
                                            targetComputing.IsAutoLongNote = false;
                                            break;
                                    }
                                    break;
                            }
                        }
                        else if (property.EqualsCaseless("PREVIEW"))
                        {
                            targetComputing.TrailerAudioName = data;
                        }
                    }
                    else
                    {
                        if (property.EqualsCaseless("4K"))
                        {
                            _is4K = true;
                        }
                        else if (property.EqualsCaseless("6K"))
                        {
                            _is6K = true;
                        }
                    }
                }
            }
            var earlyBMSInputItemSet = new Dictionary<string, SortedSet<EarlyBMSInputItem>>();
            var earlyBMSLongInputItemSet = new Dictionary<string, SortedSet<EarlyBMSLongInputItem>>();
            ComponentValue = new Component(targetComputing.LevyingBPM);
            var bmsPositionSet = new SortedSet<double>();
            var title = string.Empty;
            var titleAssister = string.Empty;
            var artist = string.Empty;
            var artistAssister = string.Empty;
            var audioTargets = new List<string>();
            var audioValues = new HashSet<string>();
            foreach (var line in _lines)
            {
                SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                var lineAt1 = line[1..];
                var delimited = lineAt1.Split(_delimiters, 2);
                if (IsMainBMSData(lineAt1))
                {
                    var property = delimited.Length > 1 ? delimited[0] : delimited[0].Substring(0, 5);
                    property = property.PadLeft(5, '0');
                    var noteVariety0 = property[3];
                    var noteVariety1 = property[4];
                    var data = delimited.Length > 1 ? delimited[1] : delimited[0][5..];
                    var dataCount = data.Length - data.Length % 2;
                    var meter = Utility.ToInt32(property.Substring(0, 3));
                    for (var meterCount = 0; meterCount < dataCount; meterCount += 2)
                    {
                        var bmsID = data.Substring(meterCount, 2);
                        if (bmsID != "00")
                        {
                            var bmsPosition = (double)meterCount / dataCount + meter;
                            var rawInput = property[3..5];
                            switch (noteVariety0)
                            {
                                case '0':
                                    switch (noteVariety1)
                                    {
                                        case '1':
                                            bmsPositionSet.Add(bmsPosition);
                                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                                            break;
                                        case '4':
                                        case '7':
                                            bmsPositionSet.Add(bmsPosition);
                                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                                            _hasMedia = true;
                                            break;
                                        case '6':
                                            bmsPositionSet.Add(bmsPosition);
                                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                                            _hasMedia = true;
                                            _hasFailedMedia = true;
                                            break;
                                        case '3':
                                        case '8':
                                        case '9':
                                            bmsPositionSet.Add(bmsPosition);
                                            break;
                                    }
                                    break;
                                case '1':
                                case '2':
                                    if (noteVariety1 != '7')
                                    {
                                        if (string.IsNullOrEmpty(_longNoteBMSID))
                                        {
                                            ++targetComputing.TotalNotes;
                                            if (noteVariety1 == '6')
                                            {
                                                ++targetComputing.AutoableNotes;
                                            }
                                            PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                                        }
                                        else
                                        {
                                            earlyBMSLongInputItemSet.NewValue(rawInput, new EarlyBMSLongInputItem
                                            {
                                                BMSPosition = bmsPosition,
                                                BMSID = bmsID
                                            });
                                        }
                                    }
                                    bmsPositionSet.Add(bmsPosition);
                                    break;
                                case '3':
                                case '4':
                                case '7':
                                case '9':
                                    bmsPositionSet.Add(bmsPosition);
                                    break;
                                case '5':
                                case '6':
                                    if (noteVariety1 != '7')
                                    {
                                        if (!string.IsNullOrEmpty(_longNoteBMSID) && _longNoteBMSID.EqualsCaseless(bmsID))
                                        {
                                            earlyBMSLongInputItemSet.NewValue(rawInput, new EarlyBMSLongInputItem
                                            {
                                                BMSPosition = bmsPosition,
                                                BMSID = bmsID
                                            });
                                        }
                                        else
                                        {
                                            earlyBMSInputItemSet.NewValue(rawInput, new EarlyBMSInputItem
                                            {
                                                BMSPosition = bmsPosition
                                            });
                                        }
                                        bmsPositionSet.Add(bmsPosition);
                                    }
                                    break;
                                case 'D':
                                case 'E':
                                    if (noteVariety1 != '7')
                                    {
                                        ++targetComputing.TrapNotes;
                                        HighestPosition = Math.Max(HighestPosition, bmsPosition);
                                        bmsPositionSet.Add(bmsPosition);
                                    }
                                    break;
                                case 'S':
                                    switch (noteVariety1)
                                    {
                                        case 'C':
                                            bmsPositionSet.Add(bmsPosition);
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (delimited.Length > 1)
                    {
                        var property = delimited[0].Trim();
                        var data = delimited[1].Trim();
                        if (property.IsFrontCaselsss("WAV"))
                        {
                            if (!string.IsNullOrEmpty(data))
                            {
                                audioTargets.Add(property.Substring(3, 2));
                            }
                        }
                        else if (property.IsFrontCaselsss("STOP"))
                        {
                            _bmsIDStopMap[property.Substring(4, 2)] = Utility.ToFloat64(data);
                        }
                        else if (property.IsFrontCaselsss("SCROLL"))
                        {
                            _bmsIDMultiplierMap[property.Substring(6, 2)] = Utility.ToFloat64(data);
                        }
                        else if (property.EqualsCaseless("TITLE"))
                        {
                            title = data;
                        }
                        else if (property.EqualsCaseless("SUBTITLE"))
                        {
                            titleAssister = data;
                        }
                        else if (property.EqualsCaseless("ARTIST"))
                        {
                            artist = data;
                        }
                        else if (property.EqualsCaseless("SUBARTIST"))
                        {
                            artistAssister = data;
                        }
                        else if (property.EqualsCaseless("GENRE"))
                        {
                            targetComputing.Genre = data;
                        }
                        else if (property.EqualsCaseless("PLAYLEVEL"))
                        {
                            if (Utility.ToFloat64(data, out var levelTextValue))
                            {
                                targetComputing.LevelTextValue = levelTextValue;
                                levelTextValue = Math.Abs(Math.Floor(levelTextValue));
                                if (levelTextValue < 100)
                                {
                                    targetComputing.LevelText = $"LV. {levelTextValue}";
                                }
                                else
                                {
                                    targetComputing.LevelText = $"LV. {levelTextValue % 100:00}";
                                }
                            }
                            else
                            {
                                targetComputing.LevelText = data;
                            }
                        }
                        else if (property.EqualsCaseless("DIFFICULTY"))
                        {
                            if (Utility.ToInt32(data, out var level))
                            {
                                targetComputing.LevelValue = (BaseNoteFile.Level)Math.Clamp(level, (int)BaseNoteFile.Level.Level0, (int)BaseNoteFile.Level.Level5);
                            }
                        }
                        else if (property.EqualsCaseless("STAGEFILE"))
                        {
                            if (string.IsNullOrEmpty(targetComputing.NoteDrawingName))
                            {
                                targetComputing.NoteDrawingName = data;
                            }
                        }
                        else if (property.EqualsCaseless("BACKBMP"))
                        {
                            if (string.IsNullOrEmpty(targetComputing.NoteDrawingName))
                            {
                                targetComputing.NoteDrawingName = data;
                            }
                        }
                        else if (property.EqualsCaseless("BANNER"))
                        {
                            targetComputing.BannerDrawingName = data;
                        }
                        else if (property.EqualsCaseless("COMMENT"))
                        {
                            targetComputing.Tag = data;
                        }
                        else if (property.EqualsCaseless("RANK"))
                        {
                            if (Utility.ToInt32(data, out var judgmentStage))
                            {
                                switch (Math.Clamp(judgmentStage, 0, 4))
                                {
                                    case 0:
                                        targetComputing.JudgmentStage = 10;
                                        break;
                                    case 1:
                                        targetComputing.JudgmentStage = 7;
                                        break;
                                    case 2:
                                        targetComputing.JudgmentStage = 5;
                                        break;
                                    case 3:
                                        targetComputing.JudgmentStage = 3;
                                        break;
                                    case 4:
                                        targetComputing.JudgmentStage = 0;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            targetComputing.Title = string.IsNullOrEmpty(titleAssister) || title.EndsWith(titleAssister) ? title : $"{title} {titleAssister}";
            targetComputing.Artist = string.IsNullOrEmpty(artistAssister) ? artist : $"{artist} / {artistAssister}";

            var isAutoLongNote = targetComputing.IsAutoLongNote;
            foreach (var (rawInput, earlyBMSLongInputItems) in earlyBMSLongInputItemSet)
            {
                EarlyBMSLongInputItem lastEarlyBMSLongInputItem = null;
                foreach (var earlyBMSLongInputItem in earlyBMSLongInputItems)
                {
                    if (earlyBMSLongInputItem.BMSID.EqualsCaseless(_longNoteBMSID))
                    {
                        if (lastEarlyBMSLongInputItem != null)
                        {
                            ++targetComputing.LongNotes;
                            targetComputing.TotalNotes += targetComputing.IsLongNoteStand1 ? 1 : 2;
                            if (rawInput[1] == '6')
                            {
                                targetComputing.AutoableNotes += targetComputing.IsLongNoteStand1 ? 1 : 2;
                            }
                            var bmsPosition = lastEarlyBMSLongInputItem.BMSPosition;
                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                            PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                            bmsPosition = earlyBMSLongInputItem.BMSPosition;
                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                            if (!isAutoLongNote)
                            {
                                PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                            }
                            lastEarlyBMSLongInputItem = null;
                            continue;
                        }
                    }
                    else
                    {
                        if (lastEarlyBMSLongInputItem != null)
                        {
                            ++targetComputing.TotalNotes;
                            if (rawInput[1] == '6')
                            {
                                ++targetComputing.AutoableNotes;
                            }
                            var bmsPosition = lastEarlyBMSLongInputItem.BMSPosition;
                            HighestPosition = Math.Max(HighestPosition, bmsPosition);
                            PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                        }
                    }
                    lastEarlyBMSLongInputItem = earlyBMSLongInputItem;
                }
                if (lastEarlyBMSLongInputItem != null)
                {
                    ++targetComputing.TotalNotes;
                    if (rawInput[1] == '6')
                    {
                        ++targetComputing.AutoableNotes;
                    }
                    var bmsPosition = lastEarlyBMSLongInputItem.BMSPosition;
                    HighestPosition = Math.Max(HighestPosition, bmsPosition);
                    PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                }
            }

            foreach (var (rawInput, earlyBMSInputItems) in earlyBMSInputItemSet)
            {
                EarlyBMSInputItem lastEarlyBMSInputItem = null;
                foreach (var earlyBMSInputItem in earlyBMSInputItems)
                {
                    if (lastEarlyBMSInputItem != null)
                    {
                        ++targetComputing.LongNotes;
                        targetComputing.TotalNotes += targetComputing.IsLongNoteStand1 ? 1 : 2;
                        if (rawInput[1] == '6')
                        {
                            targetComputing.AutoableNotes += targetComputing.IsLongNoteStand1 ? 1 : 2;
                        }
                        var bmsPosition = lastEarlyBMSInputItem.BMSPosition;
                        HighestPosition = Math.Max(HighestPosition, bmsPosition);
                        PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                        bmsPosition = earlyBMSInputItem.BMSPosition;
                        HighestPosition = Math.Max(HighestPosition, bmsPosition);
                        if (!isAutoLongNote)
                        {
                            PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                        }
                        lastEarlyBMSInputItem = null;
                    }
                    else
                    {
                        lastEarlyBMSInputItem = earlyBMSInputItem;
                    }
                }
                if (lastEarlyBMSInputItem != null)
                {
                    ++targetComputing.TotalNotes;
                    if (rawInput[1] == '6')
                    {
                        ++targetComputing.AutoableNotes;
                    }
                    var bmsPosition = lastEarlyBMSInputItem.BMSPosition;
                    HighestPosition = Math.Max(HighestPosition, bmsPosition);
                    PositionStandNoteCountMap[bmsPosition] = PositionStandNoteCountMap.GetValueOrDefault(bmsPosition) + 1;
                }
            }

            var inputSet = new HashSet<int>();
            foreach (var line in _lines)
            {
                SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                var lineAt1 = line[1..];
                var delimited = lineAt1.Split(_delimiters, 2);
                if (IsMainBMSData(lineAt1))
                {
                    var property = delimited.Length > 1 ? delimited[0] : delimited[0].Substring(0, 5);
                    property = property.PadLeft(5, '0');
                    var data = delimited.Length > 1 ? delimited[1] : delimited[0][5..];
                    var dataCount = data.Length - data.Length % 2;
                    var meter = Utility.ToInt32(property.Substring(0, 3));
                    for (var meterCount = 0; meterCount < dataCount; meterCount += 2)
                    {
                        var bmsID = data.Substring(meterCount, 2);
                        if (bmsID != "00")
                        {
                            var bmsPosition = (double)meterCount / dataCount + meter;
                            var noteVariety0 = property[3];
                            var noteVariety1 = property[4];
                            switch (noteVariety0)
                            {
                                case '0':
                                    switch (noteVariety1)
                                    {
                                        case '3':
                                            PositionBPMMap[bmsPosition] = Convert.ToInt32(bmsID, 16);
                                            break;
                                        case '8':
                                            if (bmsIDBPMMap.TryGetValue(bmsID, out var bpm))
                                            {
                                                PositionBPMMap[bmsPosition] = bpm;
                                            }
                                            break;
                                        case '9':
                                            if (_bmsIDStopMap.TryGetValue(bmsID, out var stop))
                                            {
                                                if (stop < 0.0)
                                                {
                                                    throw new ArgumentException(LanguageSystem.Instance.NegativeStopFault);
                                                }
                                                bmsPositionStopMap[bmsPosition] = stop;
                                            }
                                            break;
                                    }
                                    break;
                                case '1':
                                case '2':
                                case '5':
                                case '6':
                                    if (audioTargets.Contains(bmsID))
                                    {
                                        audioValues.Add(bmsID);
                                    }
                                    break;
                                case 'S':
                                    switch (noteVariety1)
                                    {
                                        case 'C':
                                            if (_bmsIDMultiplierMap.TryGetValue(bmsID, out var multiplier))
                                            {
                                                _bmsPositionMultiplierMap[bmsPosition] = multiplier;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            inputSet.Add(GetBMSInput(noteVariety0, noteVariety1, Component.InputMode._14_2));
                        }
                    }
                }
                else
                {
                    if (delimited.Length > 1)
                    {
                        var property = delimited[0].Trim();
                        var data = delimited[1].Trim();
                        if (property.EqualsCaseless("TOTAL"))
                        {
                            if (Utility.ToFloat64(data, out var value))
                            {
                                if (value > 0.0)
                                {
                                    targetComputing.HitPointsValue = value / (100 * targetComputing.TotalNotes);
                                }
                            }
                        }
                    }
                }
            }
            InputMode = GetInputMode(inputSet);

            for (var i = 0; i <= _highestMeter + 1; ++i)
            {
                bmsPositionSet.Add(i);
            }
            var lastBMSPosition = 0.0;
            var lastLogicalY = 0.0;
            var lastWait = 0.0;
            var lastMultiplier = 1.0;
            foreach (var bmsPosition in bmsPositionSet)
            {
                var meterMultiplier = meterMeterMultiplierMap.GetValueOrDefault((int)lastBMSPosition, 1.0);
                lastLogicalY -= (bmsPosition - lastBMSPosition) * lastMultiplier * meterMultiplier * ComponentValue.LogicalYMeter;
                _bmsPositionLogicalYMap[bmsPosition] = lastLogicalY;
                lastWait += ComponentValue.MillisMeter * meterMultiplier * (bmsPosition - lastBMSPosition);
                _bmsPositionWaitMap[bmsPosition] = lastWait;
                if (PositionBPMMap.TryGetValue(bmsPosition, out var bpm))
                {
                    ComponentValue.SetBPM(bpm);
                }
                if (bmsPositionStopMap.TryGetValue(bmsPosition, out var stop))
                {
                    lastWait += ComponentValue.MillisMeter * stop / Component.StandardMeter;
                }
                if (_bmsPositionMultiplierMap.TryGetValue(bmsPosition, out var multiplier))
                {
                    lastMultiplier = multiplier;
                }
                lastBMSPosition = bmsPosition;
            }
            targetComputing.IsBanned = audioValues.Count < 2 || targetComputing.TotalNotes == 0;
            targetComputing.IsHellBPM = targetComputing.TrapNotes > 0;
        }

        public override void CompileImpl(DefaultCompute defaultComputer, byte[] noteFileContents, bool loadParallelItems)
        {
            var bmsIDAudioItemMap = new ConcurrentDictionary<string, AudioItem?>();
            var bmsIDHandledItemMap = new ConcurrentDictionary<string, IHandledItem>();
            var parallelItems = new ConcurrentBag<Action>();
            try
            {
                var noteDrawingPath = Utility.GetFilePath(defaultComputer.NoteDrawingPath, Utility.FileFormatFlag.Drawing);
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
            var isBanalMedia = (!_hasMedia || defaultComputer.AlwaysBanalMedia) && defaultComputer.BanalMedia;
            var isBanalFailedMedia = (!_hasFailedMedia || defaultComputer.AlwaysBanalFailedMedia) && defaultComputer.BanalFailedMedia;
            defaultComputer.LoadBanalMedia(isBanalMedia, isBanalFailedMedia, parallelItems);
            foreach (var line in _lines)
            {
                SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                var lineAt1 = line[1..];
                if (!IsMainBMSData(lineAt1))
                {
                    var delimited = lineAt1.Split(_delimiters, 2);
                    var property = delimited[0].Trim();
                    if (delimited.Length > 1)
                    {
                        var data = delimited[1].Trim();
                        if (property.IsFrontCaselsss("BMP"))
                        {
                            if (defaultComputer.LoadedMedia)
                            {
                                property = property.Substring(3, 2);
                                if ((property != "00" || !isBanalFailedMedia) && !isBanalMedia)
                                {
                                    parallelItems.Add(() =>
                                    {
                                        try
                                        {
                                            if (_mediaMap.TryGetValue(NoteFile.GetNoteID512(), out var media))
                                            {
                                                data = media;
                                            }
                                            var mediaFilePath = Utility.GetFilePath(Path.Combine(NoteFile.EntryItem.EntryPath, data), Utility.FileFormatFlag.Drawing | Utility.FileFormatFlag.Media);
                                            if (!string.IsNullOrEmpty(mediaFilePath))
                                            {
                                                bmsIDHandledItemMap[property] = Utility.GetFileFormat(mediaFilePath) switch
                                                {
                                                    Utility.FileFormatFlag.Drawing => new HandledDrawingItem
                                                    {
                                                        Drawing = DrawingSystem.Instance.LoadBMS(mediaFilePath, defaultComputer),
                                                        DefaultDrawing = DrawingSystem.Instance.LoadDefaultBMS(mediaFilePath, defaultComputer)
                                                    },
                                                    Utility.FileFormatFlag.Media => MediaSystem.Instance.Load(Utility.GetFiles(Path.GetDirectoryName(mediaFilePath), $"{Path.GetFileNameWithoutExtension(mediaFilePath)}.*")
                                                        .Where(targetFile => targetFile.IsTailCaselsss(".mp4"))
                                                        .FirstOrDefault() ?? mediaFilePath, defaultComputer, false),
                                                    _ => null as IHandledItem,
                                                };
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    });
                                }
                            }
                        }
                        else if (property.IsFrontCaselsss("WAV"))
                        {
                            parallelItems.Add(() =>
                            {
                                try
                                {
                                    var audioFilePath = Utility.GetFilePath(Path.Combine(NoteFile.EntryItem.EntryPath, data), Utility.FileFormatFlag.Audio);
                                    if (!string.IsNullOrEmpty(audioFilePath))
                                    {
                                        bmsIDAudioItemMap[property.Substring(3, 2)] = AudioSystem.Instance.Load(audioFilePath, defaultComputer, 1F, data);
                                    }
                                }
                                catch
                                {
                                }
                            });
                        }
                    }
                }
            }

            if (loadParallelItems)
            {
                var endStatus = parallelItems.Count;
                var status = 0;
                Utility.HandleLowlyParallelly(parallelItems, Configure.Instance.CompilingBin, parallelItem =>
                {
                    parallelItem();
                    defaultComputer.SetCompilingStatus((double)Interlocked.Increment(ref status) / endStatus);
                }, SetCancelCompiler?.Token);
            }

            if (bmsIDHandledItemMap.TryGetValue("00", out var mediaItem))
            {
                defaultComputer.WaitMediaNoteMap.NewValue(0.0, new MediaNote
                {
                    MediaMode = MediaNote.Mode.Failed,
                    HasContents = defaultComputer.LoadedMedia,
                    MediaItem = mediaItem
                });
            }
            Notes.AddRange(Enumerable.Range(0, _highestMeter + 1).Select(i =>
            {
                var logicalY = ComponentValue.LevyingHeight + _bmsPositionLogicalYMap[i];
                var wait = _bmsPositionWaitMap[i];
                defaultComputer.MeterWaitMap[i] = wait;
                return new MeterNote(logicalY, wait, i);
            }));
            var inputCount = Component.InputCounts[(int)InputMode];
            var bmsInputItemSets = new SortedSet<BMSInputItem>[inputCount + 1];
            for (var i = inputCount; i > 0; --i)
            {
                bmsInputItemSets[i] = new();
            }
            var bmsLongInputItemSets = new SortedSet<BMSLongInputItem>[inputCount + 1];
            for (var i = inputCount; i > 0; --i)
            {
                bmsLongInputItemSets[i] = new();
            }
            foreach (var line in _lines)
            {
                SetCancelCompiler?.Token.ThrowIfCancellationRequested();
                var lineAt1 = line[1..];
                if (IsMainBMSData(lineAt1))
                {
                    var delimited = lineAt1.Split(_delimiters, 2);
                    var property = delimited.Length > 1 ? delimited[0] : delimited[0].Substring(0, 5);
                    property = property.PadLeft(5, '0');
                    var noteVariety0 = property[3];
                    var noteVariety1 = property[4];
                    if (noteVariety0 != '0' || noteVariety1 != '2')
                    {
                        var data = delimited.Length > 1 ? delimited[1] : delimited[0][5..];
                        var dataCount = data.Length - data.Length % 2;
                        var meter = Utility.ToInt32(property.Substring(0, 3));
                        for (var meterCount = 0; meterCount < dataCount; meterCount += 2)
                        {
                            var bmsID = data.Substring(meterCount, 2);
                            if (bmsID != "00")
                            {
                                var input = GetBMSInput(noteVariety0, noteVariety1, InputMode);
                                var bmsPosition = (double)meterCount / dataCount + meter;
                                var logicalY = ComponentValue.LevyingHeight + _bmsPositionLogicalYMap[bmsPosition];
                                var wait = _bmsPositionWaitMap[bmsPosition];
                                bmsIDAudioItemMap.TryGetValue(bmsID, out var audioItem);
                                var audioNote = new AudioNote
                                {
                                    AudioItem = audioItem
                                };
                                bmsIDHandledItemMap.TryGetValue(bmsID, out mediaItem);
                                switch (noteVariety0)
                                {
                                    case '0':
                                        switch (noteVariety1)
                                        {
                                            case '1':
                                                defaultComputer.WaitAudioNoteMap.NewValue(wait, audioNote);
                                                break;
                                            case '3':
                                            case '8':
                                                if (PositionBPMMap.TryGetValue(bmsPosition, out var bpm))
                                                {
                                                    WaitBPMMap[wait] = bpm;
                                                }
                                                break;
                                            case '4' when !isBanalMedia:
                                                defaultComputer.WaitMediaNoteMap.NewValue(wait, new MediaNote
                                                {
                                                    MediaMode = MediaNote.Mode.Default,
                                                    MediaItem = mediaItem,
                                                    HasContents = defaultComputer.LoadedMedia
                                                });
                                                break;
                                            case '6' when !isBanalMedia && !isBanalFailedMedia:
                                                defaultComputer.WaitMediaNoteMap.NewValue(wait, new MediaNote
                                                {
                                                    MediaMode = MediaNote.Mode.Failed,
                                                    MediaItem = mediaItem,
                                                    HasContents = defaultComputer.LoadedMedia
                                                });
                                                break;
                                            case '7' when !isBanalMedia:
                                                defaultComputer.WaitMediaNoteMap.NewValue(wait, new MediaNote
                                                {
                                                    MediaMode = MediaNote.Mode.Layer,
                                                    MediaItem = mediaItem,
                                                    HasContents = defaultComputer.LoadedMedia
                                                });
                                                break;
                                            case '9':
                                                WaitStopMap[wait] = _bmsIDStopMap.GetValueOrDefault(bmsID) / Component.StandardMeter;
                                                break;
                                        }
                                        break;
                                    case '1':
                                    case '2':
                                        if (input != default)
                                        {
                                            var inputNote = new InputNote(logicalY, wait, new[] { audioNote }, input);
                                            if (string.IsNullOrEmpty(_longNoteBMSID))
                                            {
                                                Notes.Add(inputNote);
                                            }
                                            else
                                            {
                                                bmsLongInputItemSets[input].Add(new BMSLongInputItem
                                                {
                                                    InputNote = inputNote,
                                                    BMSID = bmsID
                                                });
                                            }
                                        }
                                        break;
                                    case '3':
                                    case '4':
                                        if (input != default)
                                        {
                                            Notes.Add(new VoidNote(logicalY, wait, new[] { audioNote }, input));
                                        }
                                        break;
                                    case '5':
                                    case '6':
                                        if (input != default)
                                        {
                                            var inputNote = new InputNote(logicalY, wait, new[] { audioNote }, input);
                                            if (!string.IsNullOrEmpty(_longNoteBMSID) && _longNoteBMSID.EqualsCaseless(bmsID))
                                            {
                                                bmsLongInputItemSets[input].Add(new BMSLongInputItem
                                                {
                                                    InputNote = inputNote,
                                                    BMSID = bmsID
                                                });
                                            }
                                            else
                                            {
                                                bmsInputItemSets[input].Add(new BMSInputItem
                                                {
                                                    InputNote = inputNote
                                                });
                                            }
                                        }
                                        break;
                                    case 'D':
                                    case 'E':
                                        if (input != default)
                                        {
                                            Notes.Add(new TrapNote(logicalY, wait, Array.Empty<AudioNote>(), input));
                                        }
                                        break;
                                    case 'S':
                                        switch (noteVariety1)
                                        {
                                            case 'C' when _bmsPositionMultiplierMap.TryGetValue(bmsPosition, out var multiplier):
                                                WaitMultiplierMap[wait] = multiplier;
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            for (var i = inputCount; i > 0; --i)
            {
                BMSLongInputItem lastBMSLongInputItem = null;
                foreach (var bmsLongInputItem in bmsLongInputItemSets[i])
                {
                    if (bmsLongInputItem.BMSID.EqualsCaseless(_longNoteBMSID))
                    {
                        if (lastBMSLongInputItem != null)
                        {
                            var lastInputNote = lastBMSLongInputItem.InputNote;
                            var lastWait = lastInputNote.Wait;
                            var targetWait = bmsLongInputItem.InputNote.Wait;
                            Notes.RemoveAll((Predicate<BaseNote>)(note =>
                            {
                                var wait = note.Wait;
                                return lastWait <= wait && wait <= targetWait && note.LevyingInput == i;
                            }));
                            Notes.Add(new LongNote(lastInputNote.LogicalY, lastWait, lastInputNote.AudioNotes, lastInputNote.LevyingInput, targetWait - lastWait, lastInputNote.LogicalY - bmsLongInputItem.InputNote.LogicalY));
                            lastBMSLongInputItem = null;
                            continue;
                        }
                    }
                    else
                    {
                        if (lastBMSLongInputItem != null)
                        {
                            Notes.Add(lastBMSLongInputItem.InputNote);
                        }
                    }
                    lastBMSLongInputItem = bmsLongInputItem;
                }
                if (lastBMSLongInputItem != null)
                {
                    Notes.Add(lastBMSLongInputItem.InputNote);
                }
            }
            for (var i = inputCount; i > 0; --i)
            {
                BMSInputItem lastBMSInputItem = null;
                foreach (var bmsInputItem in bmsInputItemSets[i])
                {
                    if (lastBMSInputItem != null)
                    {
                        var lastInputNote = lastBMSInputItem.InputNote;
                        var lastWait = lastInputNote.Wait;
                        var targetWait = bmsInputItem.InputNote.Wait;
                        Notes.RemoveAll((Predicate<BaseNote>)(note =>
                        {
                            var wait = note.Wait;
                            return lastWait <= wait && wait <= targetWait && note.LevyingInput == i;
                        }));
                        Notes.Add(new LongNote(lastInputNote.LogicalY, lastWait, lastInputNote.AudioNotes, i, targetWait - lastWait, lastInputNote.LogicalY - bmsInputItem.InputNote.LogicalY));
                        lastBMSInputItem = null;
                    }
                    else
                    {
                        lastBMSInputItem = bmsInputItem;
                    }
                }
                if (lastBMSInputItem != null)
                {
                    Notes.Add(lastBMSInputItem.InputNote);
                }
            }
        }

        public virtual Component.InputMode GetInputMode(ICollection<int> inputSet)
        {
            if (_is4K)
            {
                return Component.InputMode._4;
            }
            if (_is6K)
            {
                return Component.InputMode._6;
            }
            if (inputSet.All(input => input == 0))
            {
                return Component.InputMode._5_1;
            }
            var isMode71 = inputSet.Contains(7) || inputSet.Contains(8);
            var isMode102 = inputSet.Contains(9) || inputSet.Contains(10) || inputSet.Contains(11) || inputSet.Contains(12) || inputSet.Contains(13);
            var isMode142 = (isMode71 && isMode102) || inputSet.Contains(14) || inputSet.Contains(15);
            if (isMode142)
            {
                return Component.InputMode._14_2;
            }
            if (isMode102)
            {
                return Component.InputMode._10_2;
            }
            if (isMode71)
            {
                return Component.InputMode._7_1;
            }
            return Component.InputMode._5_1;
        }

        static bool IsMainBMSData(string lineAt1)
        {
            if (lineAt1.Length >= 5)
            {
                var lineAt13 = lineAt1[3];
                var lineAt14 = lineAt1[4];
                return char.IsDigit(lineAt1[0]) && char.IsDigit(lineAt1[1]) && char.IsDigit(lineAt1[2]) && (char.IsDigit(lineAt13) || ((lineAt13 == 'D' || lineAt13 == 'E') && char.IsDigit(lineAt14)) || (lineAt13 == 'S' && lineAt14 == 'C'));
            }
            else
            {
                return false;
            }
        }

        static int GetBMSInput(char noteVariety0, char noteVariety1, Component.InputMode inputMode)
        {
            switch (noteVariety0)
            {
                case '1':
                case '3':
                case '5':
                case 'D':
                    switch (inputMode)
                    {
                        case Component.InputMode._4:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                    return noteVariety1 - '0';
                                case '4':
                                case '5':
                                    return noteVariety1 - '1';
                            }
                            break;
                        case Component.InputMode._5_1:
                        case Component.InputMode._10_2:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0' + 1;
                                case '6':
                                    return 1;
                            }
                            break;
                        case Component.InputMode._6:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                    return noteVariety1 - '0';
                                case '5':
                                    return noteVariety1 - '1';
                                case '8':
                                case '9':
                                    return noteVariety1 - '3';
                            }
                            break;
                        case Component.InputMode._7_1:
                        case Component.InputMode._14_2:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0' + 1;
                                case '6':
                                    return 1;
                                case '8':
                                case '9':
                                    return noteVariety1 - '1';
                            }
                            break;
                        case Component.InputMode._9:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0';
                            }
                            break;
                    }
                    break;
                case '2':
                case '4':
                case '6':
                case 'E':
                    switch (inputMode)
                    {
                        case Component.InputMode._10_2:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0' + 6;
                                case '6':
                                    return 12;
                            }
                            break;
                        case Component.InputMode._14_2:
                            switch (noteVariety1)
                            {
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0' + 8;
                                case '6':
                                    return 16;
                                case '8':
                                case '9':
                                    return noteVariety1 - '2' + 8;
                            }
                            break;
                        case Component.InputMode._9:
                            switch (noteVariety1)
                            {
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                    return noteVariety1 - '0' + 4;
                            }
                            break;
                    }
                    break;
            }
            return default;
        }
    }
}