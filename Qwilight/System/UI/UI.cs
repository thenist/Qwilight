using Ionic.Zip;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using MoonSharp.Interpreter;
using Qwilight.Note;
using Qwilight.PaintComponent;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Windows.UI;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Qwilight
{
    public sealed class UI : Model, IDrawingContainer, IAudioContainer
    {
        public const int MaxUIConfigure = 16;
        public const int MaxNoteID = 64;
        public const int MaxPaintPropertyID = 256;

        public static readonly UI Instance = QwilightComponent.GetBuiltInData<UI>(nameof(UI));

        public static void ZipUIFile(ZipFile zipFile, UIItem value, string entryPath)
        {
            var yamlFilePath = value.GetYamlFilePath();
            if (File.Exists(yamlFilePath))
            {
                zipFile.AddFile(yamlFilePath, Path.Combine(entryPath, Path.GetRelativePath(QwilightComponent.UIEntryPath, Path.GetDirectoryName(yamlFilePath))));
                var ys = new YamlStream();
                using (var sr = File.OpenText(yamlFilePath))
                {
                    ys.Load(sr);
                }
                var formatNode = ys.Documents[0].RootNode[new YamlScalarNode("format")];
                var zipFilePath = Path.Combine(QwilightComponent.UIEntryPath, value.UIEntry, Path.ChangeExtension(Utility.GetText(formatNode, "zip", value.YamlName), "zip"));
                if (File.Exists(zipFilePath))
                {
                    zipFile.AddFile(zipFilePath, Path.Combine(entryPath, Path.GetRelativePath(QwilightComponent.UIEntryPath, Path.GetDirectoryName(zipFilePath))));
                }
                var luaFilePath = Path.Combine(QwilightComponent.UIEntryPath, value.UIEntry, Path.ChangeExtension(Utility.GetText(formatNode, "lua", value.YamlName), "lua"));
                if (File.Exists(luaFilePath))
                {
                    zipFile.AddFile(luaFilePath, Path.Combine(entryPath, Path.GetRelativePath(QwilightComponent.UIEntryPath, Path.GetDirectoryName(luaFilePath))));
                }
            }
        }

        readonly Dictionary<string, AudioItem> _audioItemMap = new();

        /// <summary>
        /// UI, BaseUI가 로드됨을 보장하는 락
        /// </summary>
        public object LoadedCSX { get; } = new();

        public XamlUIConfigure[] XamlUIConfigures { get; set; }

        public string[] LoadedConfigures { get; } = new string[MaxUIConfigure];

        public bool HasMain => PaintPipelineValues.Contains(PaintPipelineID.Main);

        public bool HasJudgmentMain => PaintPipelineValues.Contains(PaintPipelineID.JudgmentMain);

        public bool HasJudgmentCount => PaintPipelineValues.Contains(PaintPipelineID.HighestJudgment) ||
            PaintPipelineValues.Contains(PaintPipelineID.HigherJudgment) ||
            PaintPipelineValues.Contains(PaintPipelineID.HighJudgment) ||
            PaintPipelineValues.Contains(PaintPipelineID.LowJudgment) ||
            PaintPipelineValues.Contains(PaintPipelineID.LowerJudgment) ||
            PaintPipelineValues.Contains(PaintPipelineID.LowestJudgment);

        public bool HasJudgmentMeter => PaintPipelineValues.Contains(PaintPipelineID.JudgmentMeter);

        public bool HasJudgmentVisualizer => PaintPipelineValues.Contains(PaintPipelineID.JudgmentVisualizer);

        public bool HasJudgmentPaint => PaintPipelineValues.Contains(PaintPipelineID.JudgmentPaint);

        public bool HasHitNotePaint => PaintPipelineValues.Contains(PaintPipelineID.HitNotePaint);

        public bool HasBPM => PaintPipelineValues.Contains(PaintPipelineID.BPM);

        public bool HasNet => PaintPipelineValues.Contains(PaintPipelineID.Net);

        public bool HasJudgmentInputVisualizer => PaintPipelineValues.Contains(PaintPipelineID.JudgmentInputVisualizer);

        public bool HasHunter => PaintPipelineValues.Contains(PaintPipelineID.Hunter);

        public bool HasMediaInput => PaintPipelineValues.Contains(PaintPipelineID.MediaInput);

        public bool HasMainJudgmentMeter => PaintPipelineValues.Contains(PaintPipelineID.MainJudgmentMeter);

        public double DefaultLength { get; set; }

        public double DefaultHeight { get; set; }

        public ObservableCollection<UIItem> UIItems { get; } = new();

        public Dictionary<string, string[]> IntCallMap { get; } = new();

        public Dictionary<string, string[]> ValueCallMap { get; } = new();

        public Dictionary<string, string[]> AltCallMap { get; } = new();

        public Dictionary<string, double> ValueMap { get; } = new();

        public Dictionary<string, int> IntMap { get; } = new();

        public Dictionary<string, int> AltMap { get; } = new();

        public List<PaintPipelineID> PaintPipelineValues { get; } = new();

        public Color TitleColor { get; set; }

        public Color ArtistColor { get; set; }

        public Color GenreColor { get; set; }

        public Color WantLevelIDColor { get; set; }

        public ICanvasBrush[] NetTextPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] NetWallPaints { get; } = new ICanvasBrush[101];

        public List<int> DrawingPipeline { get; } = new();

        public int[][] DrawingInputModeMap { get; } = new int[17][];

        public DrawingItem?[][] PauseDrawings { get; } = new DrawingItem?[3][];

        public DrawingItem?[][][][][] NoteDrawings { get; } = new DrawingItem?[17][][][][];

        public DrawingItem?[][][] NoteHitDrawings { get; } = new DrawingItem?[17][][];

        public DrawingItem?[][][] LongNoteHitDrawings { get; } = new DrawingItem?[17][][];

        public DrawingItem?[][][] MainDrawings { get; } = new DrawingItem?[17][][];

        public DrawingItem?[][][] JudgmentMainDrawings { get; } = new DrawingItem?[17][][];

        public DrawingItem?[][][] InputDrawings { get; } = new DrawingItem?[17][][];

        public DrawingItem?[] MainWalls { get; } = new DrawingItem?[2];

        public DrawingItem?[][] AutoInputDrawings { get; } = new DrawingItem?[17][];

        public DrawingItem?[] BinStandMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinPointMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinHitPointsVisualizerMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinHmsMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinAudioMultiplierMap { get; } = new DrawingItem?[10];

        public DrawingItem?[][] BinJudgmentValueMap { get; } = new DrawingItem?[6][];

        public DrawingItem?[] BinBPMMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinMultiplierMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinHighestBandMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinInputVisualizerMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinJudgmentMeterMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinHunterMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinEarlyValueMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinLateValueMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] BinJudgmentVSVisualizerMap { get; } = new DrawingItem?[10];

        public DrawingItem?[] JudgmentPointsDrawings { get; } = new DrawingItem?[6];

        public DrawingItem?[] JudgmentInputDrawings { get; } = new DrawingItem?[6];

        public PaintProperty[] PaintProperties { get; } = new PaintProperty[MaxPaintPropertyID];

        public DrawingItem?[][] JudgmentDrawings { get; } = new DrawingItem?[12][];

        public DrawingItem?[][] LevelDrawings { get; } = new DrawingItem?[6][];

        public DrawingItem?[] AutoMainDrawings { get; set; }

        public Dictionary<int, DrawingItem?>[][] MainJudgmentMeterDrawings { get; } = new Dictionary<int, DrawingItem?>[17][];

        public CanvasTextFormat TitleFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat ArtistFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat GenreFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LevelTextFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat WantLevelFont { get; } = DrawingSystem.Instance.GetFont();

        public string UILS { get; set; } = string.Empty;

        public bool MaintainLongNoteFrontEdge { get; set; }

        public bool MaintainAutoInput { get; set; }

        public int LoopingMain { get; set; }

        public int LoopingInput { get; set; }

        public bool SetJudgmentMainPosition { get; set; }

        public bool SetMainPosition { get; set; }

        public bool SetNoteLength { get; set; }

        public bool SetNoteHeight { get; set; }

        public bool SetBandPosition { get; set; }

        public bool SetJudgmentPaintPosition { get; set; }

        public bool SetHitNotePaintArea { get; set; }

        public bool SetJudgmentVisualizerPosition { get; set; }

        public DrawingItem?[] HitPointsDrawings { get; } = new DrawingItem?[9];

        public HandledDrawingItem? VeilDrawing { get; set; }

        public DrawingItem? StatusDrawing { get; set; }

        public DrawingItem? StatusSliderDrawing { get; set; }

        public DrawingItem? CommaDrawing { get; set; }

        public DrawingItem? SlashDrawing { get; set; }

        public DrawingItem? ColonDrawing { get; set; }

        public DrawingItem? AudioMultiplierStopPointDrawing { get; set; }

        public DrawingItem? MultiplierStopPointDrawing { get; set; }

        public DrawingItem? PointStopPointDrawing { get; set; }

        public DrawingItem? JudgmentVSVisualizerStopPointDrawing { get; set; }

        public DrawingItem? PointUnitDrawing { get; set; }

        public DrawingItem? HitPointsVisualizerUnitDrawing { get; set; }

        public DrawingItem? BPMUnitDrawing { get; set; }

        public DrawingItem? JudgmentMererHigherDrawing { get; set; }

        public DrawingItem? JudgmentMeterLowerDrawing { get; set; }

        public DrawingItem? HunterHigherDrawing { get; set; }

        public DrawingItem? HunterLowerDrawing { get; set; }

        public DrawingItem? JudgmentMeterUnitDrawing { get; set; }

        public DrawingItem? AudioMultiplierUnitDrawing { get; set; }

        public DrawingItem? MultiplierUnitDrawing { get; set; }

        public DrawingItem?[,] BinBandMap { get; set; }

        public DrawingItem?[] PausedUnpauseDrawings { get; } = new DrawingItem?[2];

        public DrawingItem?[] PausedStopDrawings { get; } = new DrawingItem?[2];

        public DrawingItem?[] PausedUndoDrawings { get; } = new DrawingItem?[2];

        public DrawingItem?[] PausedConfigureDrawings { get; } = new DrawingItem?[2];

        public bool HandleAudio(string audioFileName, string defaultFileName = null, PausableAudioHandler pausableAudioHandler = null, double fadeInLength = 0.0, int audioVariety = AudioSystem.SEAudio)
        {
            lock (LoadedCSX)
            {
                var wasHandled = false;
                if (_audioItemMap.TryGetValue(audioFileName, out var audioItem))
                {
                    wasHandled = true;
                }
                else if (defaultFileName != null)
                {
                    wasHandled = _audioItemMap.TryGetValue(defaultFileName, out audioItem);
                }
                if (wasHandled)
                {
                    AudioSystem.Instance.Handle(new()
                    {
                        AudioLevyingPosition = pausableAudioHandler?.GetAudioPosition() ?? 0U,
                        AudioItem = audioItem
                    }, audioVariety, 1.0, false, pausableAudioHandler, fadeInLength);
                }
                return wasHandled;
            }
        }

        public string FaultText { get; set; }

        public UI() => Init();

        void LoadUIImpl(UIItem src, UIItem target)
        {
            #region COMPATIBLE
            Compatible.Compatible.UI(QwilightComponent.UIEntryPath, target.GetYamlFilePath(), target.YamlName, target.UIEntry);
            #endregion

            Init();

            var drawingMap = new int[MaxNoteID + 1][];
            for (var i = drawingMap.Length - 1; i >= 0; --i)
            {
                drawingMap[i] = [i];
            }
            var noteHitDrawings = new DrawingItem?[MaxNoteID + 1][];
            var longNoteHitDrawings = new DrawingItem?[MaxNoteID + 1][];
            var inputDrawings = new DrawingItem?[MaxNoteID + 1][];
            var noteDrawings = new DrawingItem?[MaxNoteID + 1][][][];
            var mainDrawings = new DrawingItem?[MaxNoteID + 1][];
            var autoInputDrawings = new DrawingItem?[MaxNoteID + 1];
            var judgmentMainDrawings = new DrawingItem?[MaxNoteID + 1][];
            var mainJudgmentMeterDrawings = new Dictionary<int, DrawingItem?>[MaxNoteID + 1];
            string zipName;

            var lsCaller = new Script();

            var parallelItems = new ConcurrentBag<Action>();

            var ys = new YamlStream();
            using (var sr = File.OpenText(target.GetYamlFilePath()))
            {
                ys.Load(sr);

                var valueNode = ys.Documents[0].RootNode;
                var formatNode = valueNode[new YamlScalarNode("format")];
                var lambdaNode = valueNode[new YamlScalarNode("lambda")];
                var frameNode = valueNode[new YamlScalarNode("frame")];
                var pointNode = valueNode[new YamlScalarNode("point")];
                (valueNode as YamlMappingNode).Children.TryGetValue(new YamlScalarNode("paint"), out var paintNode);
                (valueNode as YamlMappingNode).Children.TryGetValue(new YamlScalarNode("font"), out var fontNode);

                zipName = Utility.GetText(formatNode, "zip", target.YamlName);

                XamlUIConfigures = Enumerable.Range(0, MaxUIConfigure).Select(i =>
                {
                    var configures = (Utility.GetText(lambdaNode, $"configure-{i}-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(lambdaNode, $"configure-{i}"))?.Split(',')?.Select(configure => configure.Trim())?.ToArray();
                    if (configures != null)
                    {
                        LoadedConfigures[i] = Configure.Instance.UIConfigureValue.UIConfiguresV2[i] ??= configures.FirstOrDefault();
                        return new XamlUIConfigure
                        {
                            Position = i,
                            Configures = configures,
                            ConfigureComment = Utility.GetText(lambdaNode, $"configure-comment-{i}-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(lambdaNode, $"configure-comment-{i}")
                        };
                    }
                    else
                    {
                        return null;
                    }
                }).Where(value => value != null).ToArray();
                SetConfigures(lsCaller);

                UILS = File.ReadAllText(Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, Path.ChangeExtension(Utility.GetText(formatNode, "lua", target.YamlName), "lua")), Encoding.UTF8);
                Script.RunString(UILS);
                lsCaller.DoString(UILS);

                DefaultLength = GetCalledValue(formatNode, "defaultLength", Component.StandardLength.ToString());
                DefaultHeight = GetCalledValue(formatNode, "defaultHeight", Component.StandardHeight.ToString());

                for (var i = drawingMap.Length - 1; i > 0; --i)
                {
                    drawingMap[i] = GetCalledText(Utility.GetText(lambdaNode, $"drawing{i}", i.ToString())).Split(',').Select(value => Utility.ToInt32(value, out var main) ? main : 0).Where(main => 0 < main && main < MaxNoteID).ToArray();
                }

                var setPaintPipelines = Utility.ToBool(Utility.GetText(lambdaNode, "set-paint-pinelines", bool.FalseString));
                foreach (var pipeline in GetCalledText(Utility.GetText(lambdaNode, "pipeline")).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var pipeline) ? pipeline : 0))
                {
                    var paintPipeline = (PaintPipelineID)pipeline;
                    PaintPipelineValues.Add(paintPipeline);
                    if (paintPipeline == PaintPipelineID.JudgmentPaint && !setPaintPipelines)
                    {
                        PaintPipelineValues.Add(PaintPipelineID.HitNotePaint);
                    }
                }
                if (!PaintPipelineValues.Contains(PaintPipelineID.Media))
                {
                    PaintPipelineValues.Insert(0, PaintPipelineID.Media);
                }
                if (!PaintPipelineValues.Contains(PaintPipelineID.MainAreaFaint))
                {
                    var mainPosition = PaintPipelineValues.IndexOf(PaintPipelineID.Main);
                    if (mainPosition >= 0)
                    {
                        PaintPipelineValues.Insert(mainPosition, PaintPipelineID.MainAreaFaint);
                    }
                }
                if (!PaintPipelineValues.Contains(PaintPipelineID.Limiter))
                {
                    var mainPosition = PaintPipelineValues.IndexOf(PaintPipelineID.Main);
                    if (mainPosition >= 0)
                    {
                        PaintPipelineValues.Insert(mainPosition + 1, PaintPipelineID.Limiter);
                    }
                }
                if (!PaintPipelineValues.Contains(PaintPipelineID.VeilDrawing))
                {
                    var notePosition = PaintPipelineValues.IndexOf(PaintPipelineID.Note);
                    if (notePosition >= 0)
                    {
                        PaintPipelineValues.Insert(notePosition + 1, PaintPipelineID.VeilDrawing);
                    }
                }
                PaintPipelineValues.Add(PaintPipelineID.MediaInput);
                SaveInt(lambdaNode, "drawingInputModeSystem", 0);

                TitleColor = Utility.GetText(paintNode, "title", nameof(Colors.White)).GetColor();
                ArtistColor = Utility.GetText(paintNode, "artist", nameof(Colors.White)).GetColor();
                GenreColor = Utility.GetText(paintNode, "genre", nameof(Colors.White)).GetColor();
                WantLevelIDColor = Utility.GetText(paintNode, "wantLevelID", nameof(Colors.White)).GetColor();
                parallelItems.Add(() => DrawingSystem.Instance.SetFaintPaints(this, NetTextPaints, Utility.GetText(paintNode, "netText", nameof(Colors.White)).GetColor()));
                parallelItems.Add(() => DrawingSystem.Instance.SetFaintPaints(this, NetWallPaints, Utility.GetText(paintNode, "netWall", nameof(Colors.Black)).GetColor()));

                SaveValueMap(pointNode, "mainPosition");
                SaveValueMap(pointNode, "p2Position");

                SaveValueMap(pointNode, "binLength");
                SaveValueMap(pointNode, "binHeight");
                SaveValueMapAsDefaultID(pointNode, "stopPointDrawingLength", "binLength");

                SaveSplitValueMap(pointNode, "floatingNotePosition0", 0.0);
                SaveSplitValueMap(pointNode, "floatingNoteLength", 0.0);
                SaveSplitValueMap(pointNode, "slashNotePosition0", 0.0);

                SaveValueMap(pointNode, "judgmentMainPosition");

                SaveValueMap(pointNode, "mediaPosition0", 0.0);
                SaveValueMap(pointNode, "mediaPosition1", 0.0);
                SaveValueMap(pointNode, "mediaLength", Component.StandardLength);
                SaveValueMap(pointNode, "mediaHeight", Component.StandardHeight);
                SaveAltMap(lambdaNode, "alt-media", 0);

                SaveValueMap(pointNode, "titlePosition0");
                SaveValueMap(pointNode, "titlePosition1");
                SaveValueMap(pointNode, "titleLength");
                SaveValueMap(pointNode, "titleHeight");
                SaveIntMap(pointNode, "titleSystem0");
                SaveIntMap(pointNode, "titleSystem1", 2);
                SaveAltMap(lambdaNode, "alt-title", 0);

                SaveValueMap(pointNode, "artistPosition0");
                SaveValueMap(pointNode, "artistPosition1");
                SaveValueMap(pointNode, "artistLength");
                SaveValueMap(pointNode, "artistHeight");
                SaveIntMap(pointNode, "artistSystem0");
                SaveIntMap(pointNode, "artistSystem1", 2);
                SaveAltMap(lambdaNode, "alt-artist", 0);

                SaveValueMap(pointNode, "genrePosition0");
                SaveValueMap(pointNode, "genrePosition1");
                SaveValueMap(pointNode, "genreLength");
                SaveValueMap(pointNode, "genreHeight");
                SaveIntMap(pointNode, "genreSystem0");
                SaveIntMap(pointNode, "genreSystem1");
                SaveAltMap(lambdaNode, "alt-genre", 0);

                SaveValueMap(pointNode, "levelTextPosition0");
                SaveValueMap(pointNode, "levelTextPosition1");
                SaveValueMap(pointNode, "levelTextLength");
                SaveValueMap(pointNode, "levelTextHeight");
                SaveIntMap(pointNode, "levelTextSystem0");
                SaveIntMap(pointNode, "levelTextSystem1");
                SaveAltMap(lambdaNode, "alt-level-text", 0);

                SaveValueMap(pointNode, "wantLevelPosition0");
                SaveValueMap(pointNode, "wantLevelPosition1");
                SaveValueMap(pointNode, "wantLevelLength");
                SaveValueMap(pointNode, "wantLevelHeight");
                SaveIntMap(pointNode, "wantLevelSystem0");
                SaveIntMap(pointNode, "wantLevelSystem1");
                SaveAltMap(lambdaNode, "alt-want-level", 0);

                SaveValueMap(pointNode, "audioVisualizerPosition0", 0.0);
                SaveValueMap(pointNode, "audioVisualizerPosition1", 0.0);
                SaveValueMap(pointNode, "audioVisualizerLength", Component.StandardLength);
                SaveValueMap(pointNode, "audioVisualizerHeight", Component.StandardHeight);

                DrawingSystem.Instance.SetFontLevel(TitleFont, Utility.ToFloat32(Utility.GetText(fontNode, "titleLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                DrawingSystem.Instance.SetFontLevel(ArtistFont, Utility.ToFloat32(Utility.GetText(fontNode, "artistLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                DrawingSystem.Instance.SetFontLevel(GenreFont, Utility.ToFloat32(Utility.GetText(fontNode, "genreLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                DrawingSystem.Instance.SetFontLevel(LevelTextFont, Utility.ToFloat32(Utility.GetText(fontNode, "levelTextLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                DrawingSystem.Instance.SetFontLevel(WantLevelFont, Utility.ToFloat32(Utility.GetText(fontNode, "wantLevelLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));

                SaveValueMap(pointNode, "mainWall0Length");
                SaveValueMap(pointNode, "mainWall1Length");
                SaveValueMap(pointNode, "mainWall0Position1");
                SaveValueMap(pointNode, "mainWall0Height", 720.0);
                SaveValueMap(pointNode, "mainWall1Position1");
                SaveValueMap(pointNode, "mainWall1Height", 720.0);
                SaveAltMap(lambdaNode, "alt-wall-0", 2);
                SaveAltMap(lambdaNode, "alt-wall-1", 2);

                SaveIntMap(frameNode, "main-frame");
                SaveValueMap(frameNode, "main-framerate");
                SaveValueMap(pointNode, "mainPosition1");
                SaveValueMap(pointNode, "mainHeight", 720.0);

                SaveSplitValueMap(pointNode, "autoInputPosition1");
                SaveSplitValueMap(pointNode, "autoInputHeight", 720.0);

                SaveIntMap(pointNode, "hitPointsSystem");
                SaveValueMap(pointNode, "hitPointsPosition0");
                SaveValueMap(pointNode, "hitPointsPosition1");
                SaveValueMap(pointNode, "hitPointsLength");
                SaveValueMap(pointNode, "hitPointsHeight");
                SaveAltMap(lambdaNode, "alt-hit-points", 2);

                SaveIntMap(frameNode, "note-frame");
                SaveValueMap(frameNode, "note-framerate");
                SaveSplitValueMap(pointNode, "noteLength");
                SaveSplitValueMap(pointNode, "noteHeight");
                SaveSplitValueMap(pointNode, "noteHeightJudgment");
                SaveSplitValueMap(pointNode, "longNoteTailEdgeHeight");
                SaveSplitValueMap(pointNode, "longNoteFrontEdgeHeight");
                SaveSplitValueMap(pointNode, "longNoteTailEdgePosition");
                SaveSplitValueMap(pointNode, "longNoteFrontEdgePosition");
                SaveSplitValueMap(pointNode, "longNoteTailContentsHeight");
                SaveSplitValueMap(pointNode, "longNoteFrontContentsHeight");
                MaintainLongNoteFrontEdge = Utility.ToBool(Utility.GetText(lambdaNode, "maintainLongNoteFrontEdge", bool.FalseString));
                MaintainAutoInput = Utility.ToBool(Utility.GetText(lambdaNode, "maintainAutoInput", bool.TrueString));
                LoopingMain = Utility.ToInt32(Utility.GetText(lambdaNode, "loopingMain", 0.ToString()));
                LoopingInput = Utility.ToInt32(Utility.GetText(lambdaNode, "loopingInput", 0.ToString()));
                SetJudgmentMainPosition = Utility.ToBool(Utility.GetText(lambdaNode, "setJudgmentMainPosition", bool.FalseString));
                SetMainPosition = Utility.ToBool(Utility.GetText(lambdaNode, "setMainPosition", bool.FalseString));
                SetNoteLength = Utility.ToBool(Utility.GetText(lambdaNode, "setNoteLength", bool.FalseString));
                SetNoteHeight = Utility.ToBool(Utility.GetText(lambdaNode, "setNoteHeight", bool.FalseString));
                SetBandPosition = Utility.ToBool(Utility.GetText(lambdaNode, "setBandPosition", bool.FalseString));
                SetJudgmentPaintPosition = Utility.ToBool(Utility.GetText(lambdaNode, "setJudgmentPaintPosition", bool.FalseString));
                SetJudgmentVisualizerPosition = Utility.ToBool(Utility.GetText(lambdaNode, "setJudgmentVisualizerPosition", bool.FalseString));
                SetHitNotePaintArea = Utility.ToBool(Utility.GetText(lambdaNode, "setHitNotePaintArea", bool.FalseString));

                SaveIntMap(lambdaNode, "judgmentPaintComposition");
                SaveIntMap(lambdaNode, "hitNotePaintComposition", (int)CanvasComposite.Add);

                SaveIntMap(frameNode, "input-frame");
                SaveValueMap(frameNode, "input-framerate");
                SaveSplitValueMap(pointNode, "inputPosition0");
                SaveSplitValueMapAsDefaultID(pointNode, "inputPosition1", "judgmentMainPosition");
                SaveSplitValueMap(pointNode, "inputLength");
                SaveSplitValueMapAsDefaultID(pointNode, "inputHeight", "judgmentMainPosition", value => 720.0 - value);

                SaveIntMap(frameNode, "level-frame");
                SaveValueMap(frameNode, "level-framerate");
                SaveValueMap(pointNode, "levelPosition0");
                SaveValueMap(pointNode, "levelPosition1");
                SaveValueMap(pointNode, "levelLength");
                SaveValueMap(pointNode, "levelHeight");
                SaveAltMap(lambdaNode, "alt-level");

                SaveIntMap(pointNode, "bandSystem");
                SaveValueMap(pointNode, "bandPosition0");
                SaveValueMap(pointNode, "bandPosition1");
                SaveIntMap(frameNode, "band-frame");
                SaveValueMap(frameNode, "band-framerate");
                SaveValueMapAsDefaultID(pointNode, "binBandLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binBandHeight", "binHeight");
                SaveValueMap(pointNode, "enlargeBand");
                SaveAltMap(lambdaNode, "alt-band", 2);

                SaveIntMap(pointNode, "judgmentMeterSystem");
                SaveValueMap(pointNode, "judgmentMeterPosition0");
                SaveValueMap(pointNode, "judgmentMeterPosition1");
                SaveValueMapAsDefaultID(pointNode, "binJudgmentMeterLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binJudgmentMeterHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "judgmentMeterFrontDrawingLength", "binJudgmentMeterLength");
                SaveValueMapAsDefaultID(pointNode, "judgmentMeterUnitDrawingLength", "binJudgmentMeterLength");
                SaveAltMap(lambdaNode, "alt-judgment-meter", 3);

                SaveIntMap(pointNode, "standSystem");
                SaveValueMap(pointNode, "standPosition0");
                SaveValueMap(pointNode, "standPosition1");
                SaveValueMapAsDefaultID(pointNode, "binStandLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binStandHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "standCommaDrawingLength", "binLength");
                SaveAltMap(lambdaNode, "alt-stand");

                SaveIntMap(pointNode, "pointSystem");
                SaveValueMap(pointNode, "pointPosition0");
                SaveValueMap(pointNode, "pointPosition1");
                SaveValueMapAsDefaultID(pointNode, "binPointLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binPointHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "pointStopPointDrawingLength", "stopPointDrawingLength");
                SaveValueMapAsDefaultID(pointNode, "pointUnitDrawingLength", "binPointLength");
                SaveAltMap(lambdaNode, "alt-point");

                SaveIntMap(pointNode, "bpmSystem");
                SaveValueMap(pointNode, "bpmPosition0");
                SaveValueMap(pointNode, "bpmPosition1");
                SaveValueMapAsDefaultID(pointNode, "binBPMLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binBPMHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "bpmUnitDrawingLength", "binBPMLength");
                SaveAltMap(lambdaNode, "alt-bpm");

                SaveIntMap(pointNode, "multiplierSystem");
                SaveValueMap(pointNode, "multiplierPosition0");
                SaveValueMap(pointNode, "multiplierPosition1");
                SaveValueMapAsDefaultID(pointNode, "binMultiplierLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binMultiplierHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "multiplierStopPointDrawingLength", "stopPointDrawingLength");
                SaveValueMapAsDefaultID(pointNode, "multiplierUnitDrawingLength", "binMultiplierLength");
                SaveAltMap(lambdaNode, "alt-multiplier");

                SaveIntMap(frameNode, "note-hit-frame");
                SaveValueMap(frameNode, "note-hit-framerate");
                SaveIntMap(frameNode, "long-note-hit-frame");
                SaveValueMap(frameNode, "long-note-hit-framerate");
                SaveIntMap(frameNode, "judgment-frame");
                SaveValueMap(frameNode, "judgment-framerate");
                SaveIntMap(pointNode, "judgmentPaintSystem");
                SaveValueMap(pointNode, "judgmentPaintPosition0");
                SaveValueMap(pointNode, "judgmentPaintPosition1");
                SaveValueMap(pointNode, "judgmentPaintLength");
                SaveValueMap(pointNode, "judgmentPaintHeight");
                SaveSplitValueMap(pointNode, "hitNotePaintLength");
                SaveSplitValueMap(pointNode, "hitNotePaintHeight");
                SaveSplitValueMap(pointNode, "hitLongNotePaintLength");
                SaveSplitValueMap(pointNode, "hitLongNotePaintHeight");
                SaveSplitValueMapAsDefaultID(pointNode, "hitNotePaintPosition0", "hitNotePaintLength1", value => -value / 2);
                SaveSplitValueMapAsDefaultID(pointNode, "hitNotePaintPosition1", "hitNotePaintHeight1", value => -value / 2);
                SaveSplitValueMapAsDefaultID(pointNode, "hitLongNotePaintPosition0", "hitLongNotePaintLength1", value => -value / 2);
                SaveSplitValueMapAsDefaultID(pointNode, "hitLongNotePaintPosition1", "hitLongNotePaintHeight1", value => -value / 2);
                SaveIntMap(frameNode, "hit-input-paint-frame");
                SaveValueMap(frameNode, "hit-input-paint-framerate");
                SaveSplitValueMap(pointNode, "hitInputPaintPosition0");
                SaveSplitValueMap(pointNode, "hitInputPaintPosition1");
                SaveSplitValueMap(pointNode, "hitInputPaintLength");
                SaveSplitValueMap(pointNode, "hitInputPaintHeight");
                SaveIntMap(frameNode, "long-note-hit-loop-frame");
                SaveIntMap(frameNode, "last-enlarged-band-loop-frame");
                SaveIntMap(frameNode, "last-frame");
                SaveValueMap(frameNode, "last-framerate");
                SaveIntMap(pointNode, "lastSystem");
                SaveValueMap(pointNode, "lastPosition0");
                SaveValueMap(pointNode, "lastPosition1");
                SaveValueMap(pointNode, "lastLength");
                SaveValueMap(pointNode, "lastHeight");
                SaveAltMap(lambdaNode, "alt-last", 2);
                SaveIntMap(frameNode, "band!-frame");
                SaveValueMap(frameNode, "band!-framerate");
                SaveIntMap(pointNode, "band!System");
                SaveValueMap(pointNode, "band!Position0");
                SaveValueMap(pointNode, "band!Position1");
                SaveValueMap(pointNode, "band!Length");
                SaveValueMap(pointNode, "band!Height");
                SaveAltMap(lambdaNode, "alt-band!", 2);
                SaveIntMapAsDefaultID(frameNode, "yell!-frame", "band!-frame");
                SaveValueMapAsDefaultID(frameNode, "yell!-framerate", "band!-framerate");
                SaveIntMapAsDefaultID(pointNode, "yell!System", "band!System");
                SaveValueMapAsDefaultID(pointNode, "yell!Position0", "band!Position0");
                SaveValueMapAsDefaultID(pointNode, "yell!Position1", "band!Position1");
                SaveValueMapAsDefaultID(pointNode, "yell!Length", "band!Length");
                SaveValueMapAsDefaultID(pointNode, "yell!Height", "band!Height");
                SaveAltMapAsDefaultID(lambdaNode, "alt-yell!", "alt-band!");

                SaveIntMap(pointNode, "netSystem");
                SaveValueMap(pointNode, "netPosition0");
                SaveValueMap(pointNode, "netPosition1");
                SaveAltMap(lambdaNode, "alt-net");

                SaveIntMap(frameNode, "auto-main-frame");
                SaveValueMap(frameNode, "auto-main-framerate");
                SaveIntMap(pointNode, "autoMainSystem");
                SaveValueMap(pointNode, "autoMainPosition0");
                SaveValueMap(pointNode, "autoMainPosition1");
                SaveValueMap(pointNode, "autoMainLength");
                SaveValueMap(pointNode, "autoMainHeight");
                SaveAltMap(lambdaNode, "alt-auto-main");

                SaveIntMap(frameNode, "pause-frame", 1);
                SaveIntMap(pointNode, "pauseSystem");
                SaveValueMap(pointNode, "pausePosition0");
                SaveValueMap(pointNode, "pausePosition1");
                SaveValueMap(pointNode, "pauseLength");
                SaveValueMap(pointNode, "pauseHeight");
                SaveAltMap(lambdaNode, "alt-pause", 2);

                SaveIntMap(pointNode, "statusSystem");
                SaveValueMap(pointNode, "statusPosition0");
                SaveValueMap(pointNode, "statusPosition1");
                SaveValueMap(pointNode, "statusLength");
                SaveValueMap(pointNode, "statusHeight");
                SaveAltMap(lambdaNode, "alt-status");

                SaveIntMap(pointNode, "statusSliderSystem");
                SaveValueMap(pointNode, "statusSliderPosition0");
                SaveValueMap(pointNode, "statusSliderPosition1");
                SaveValueMap(pointNode, "statusSliderLength");
                SaveValueMap(pointNode, "statusSliderHeight");
                SaveValueMap(pointNode, "statusSliderContentsLength");
                SaveValueMap(pointNode, "statusSliderContentsHeight");
                SaveAltMap(lambdaNode, "alt-status-slider");

                SaveIntMap(pointNode, "hmsSystem");
                SaveValueMap(pointNode, "hmsPosition0");
                SaveValueMap(pointNode, "hmsPosition1");
                SaveValueMapAsDefaultID(pointNode, "binHmsLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHmsHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "hmsColonDrawingLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "hmsSlashDrawingLength", "binLength");
                SaveAltMap(lambdaNode, "alt-hms");

                SaveIntMap(pointNode, "judgmentPointsSystem");
                SaveValueMap(pointNode, "judgmentPointsPosition0");
                SaveValueMap(pointNode, "judgmentPointsPosition1");
                SaveValueMap(pointNode, "judgmentPointsLength");
                SaveValueMap(pointNode, "judgmentPointsHeight");
                SaveAltMap(lambdaNode, "alt-judgment-points");

                SaveIntMap(frameNode, "judgment-main-frame");
                SaveValueMap(frameNode, "judgment-main-framerate");
                SaveSplitValueMap(pointNode, "judgmentMainPosition1");
                SaveSplitValueMap(pointNode, "judgmentMainHeight");

                SaveIntMap(frameNode, "main-judgment-meter-frame");
                SaveValueMap(frameNode, "main-judgment-meter-framerate");
                SaveSplitValueMap(pointNode, "mainJudgmentMeterPosition1");
                SaveSplitValueMap(pointNode, "mainJudgmentMeterHeight");

                SaveIntMap(pointNode, "audioMultiplierSystem");
                SaveValueMap(pointNode, "audioMultiplierPosition0");
                SaveValueMap(pointNode, "audioMultiplierPosition1");
                SaveValueMapAsDefaultID(pointNode, "binAudioMultiplierLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binAudioMultiplierHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "audioMultiplierStopPointDrawingLength", "stopPointDrawingLength");
                SaveValueMapAsDefaultID(pointNode, "audioMultiplierUnitDrawingLength", "binAudioMultiplierLength");
                SaveAltMap(lambdaNode, "alt-audio-multiplier");

                SaveIntMap(pointNode, "hitPointsVisualizerSystem");
                SaveValueMap(pointNode, "hitPointsVisualizerPosition0");
                SaveValueMap(pointNode, "hitPointsVisualizerPosition1");
                SaveValueMapAsDefaultID(pointNode, "binHitPointsVisualizerLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHitPointsVisualizerHeight", "binHeight");
                SaveValueMap(pointNode, "hitPointsVisualizerUnitDrawingLength");
                SaveAltMap(lambdaNode, "alt-hit-points-visualizer");

                SaveIntMap(pointNode, "highestJudgmentValueSystem");
                SaveValueMap(pointNode, "highestJudgmentValuePosition0");
                SaveValueMap(pointNode, "highestJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binHighestJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHighestJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-highest-judgment-value");

                SaveIntMap(pointNode, "higherJudgmentValueSystem");
                SaveValueMap(pointNode, "higherJudgmentValuePosition0");
                SaveValueMap(pointNode, "higherJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binHigherJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHigherJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-higher-judgment-value");

                SaveIntMap(pointNode, "highJudgmentValueSystem");
                SaveValueMap(pointNode, "highJudgmentValuePosition0");
                SaveValueMap(pointNode, "highJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binHighJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHighJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-high-judgment-value");

                SaveIntMap(pointNode, "lowJudgmentValueSystem");
                SaveValueMap(pointNode, "lowJudgmentValuePosition0");
                SaveValueMap(pointNode, "lowJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binLowJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binLowJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-low-judgment-value");

                SaveIntMap(pointNode, "lowerJudgmentValueSystem");
                SaveValueMap(pointNode, "lowerJudgmentValuePosition0");
                SaveValueMap(pointNode, "lowerJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binLowerJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binLowerJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-lower-judgment-value");

                SaveIntMap(pointNode, "lowestJudgmentValueSystem");
                SaveValueMap(pointNode, "lowestJudgmentValuePosition0");
                SaveValueMap(pointNode, "lowestJudgmentValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binLowestJudgmentValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binLowestJudgmentValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-lowest-judgment-value");

                SaveIntMap(pointNode, "highestBandSystem");
                SaveValueMap(pointNode, "highestBandPosition0");
                SaveValueMap(pointNode, "highestBandPosition1");
                SaveValueMapAsDefaultID(pointNode, "binHighestBandLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHighestBandHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-highest-band");

                SaveValueMapAsDefaultID(pointNode, "limiterPosition1", "mainPosition1");
                SaveValueMap(pointNode, "limiterLength", 1.0);
                SaveValueMapAsDefaultID(pointNode, "limiterHeight", "mainHeight");

                SaveIntMap(pointNode, "judgmentVisualizerSystem");
                SaveValueMap(pointNode, "judgmentVisualizerPosition0");
                SaveValueMap(pointNode, "judgmentVisualizerPosition1");
                SaveValueMap(pointNode, "judgmentVisualizerLength");
                SaveValueMap(pointNode, "judgmentVisualizerHeight");
                SaveValueMap(pointNode, "judgmentVisualizerContentsLength");
                SaveValueMap(pointNode, "judgmentVisualizerContentsHeight");
                SaveAltMap(lambdaNode, "alt-judgment-visualizer");

                SaveIntMap(pointNode, "hunterSystem");
                SaveValueMap(pointNode, "hunterPosition0");
                SaveValueMap(pointNode, "hunterPosition1");
                SaveValueMapAsDefaultID(pointNode, "binHunterLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binHunterHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "hunterFrontDrawingLength", "binHunterLength");
                SaveAltMap(lambdaNode, "alt-hunter");

                SaveIntMap(pointNode, "inputVisualizerSystem");
                SaveValueMap(pointNode, "inputVisualizerPosition0");
                SaveValueMap(pointNode, "inputVisualizerPosition1");
                SaveValueMapAsDefaultID(pointNode, "binInputVisualizerLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binInputVisualizerHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-input-visualizer");

                SaveIntMap(pointNode, "earlyValueSystem");
                SaveValueMap(pointNode, "earlyValuePosition0");
                SaveValueMap(pointNode, "earlyValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binEarlyValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binEarlyValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-early-value");

                SaveIntMap(pointNode, "lateValueSystem");
                SaveValueMap(pointNode, "lateValuePosition0");
                SaveValueMap(pointNode, "lateValuePosition1");
                SaveValueMapAsDefaultID(pointNode, "binLateValueLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binLateValueHeight", "binHeight");
                SaveAltMap(lambdaNode, "alt-late-value");

                SaveIntMap(pointNode, "judgmentVSVisualizerSystem");
                SaveValueMap(pointNode, "judgmentVSVisualizerPosition0");
                SaveValueMap(pointNode, "judgmentVSVisualizerPosition1");
                SaveValueMapAsDefaultID(pointNode, "binJudgmentVSVisualizerLength", "binLength");
                SaveValueMapAsDefaultID(pointNode, "binJudgmentVSVisualizerHeight", "binHeight");
                SaveValueMapAsDefaultID(pointNode, "judgmentVSVisualizerStopPointDrawingLength", "stopPointDrawingLength");
                SaveAltMap(lambdaNode, "alt-judgment-vs-visualizer");

                SaveValueMap(pointNode, "judgmentInputVisualizerPosition0");
                SaveValueMap(pointNode, "judgmentInputVisualizerPosition1");
                SaveValueMap(pointNode, "judgmentInputVisualizerLength");
                SaveValueMap(pointNode, "judgmentInputVisualizerHeight");
                SaveAltMap(lambdaNode, "alt-judgment-input-visualizer");

                SaveValueMap(pointNode, "pausedUnpausePosition0", 539);
                SaveValueMap(pointNode, "pausedUnpausePosition1", 239);
                SaveValueMap(pointNode, "pausedUnpauseLength", 202);
                SaveValueMap(pointNode, "pausedUnpauseHeight", 53);

                SaveValueMap(pointNode, "pausedConfigurePosition0", 539);
                SaveValueMap(pointNode, "pausedConfigurePosition1", 302);
                SaveValueMap(pointNode, "pausedConfigureLength", 202);
                SaveValueMap(pointNode, "pausedConfigureHeight", 53);

                SaveValueMap(pointNode, "pausedUndoPosition0", 539);
                SaveValueMap(pointNode, "pausedUndoPosition1", 365);
                SaveValueMap(pointNode, "pausedUndoLength", 202);
                SaveValueMap(pointNode, "pausedUndoHeight", 53);

                SaveValueMap(pointNode, "pausedStopPosition0", 539);
                SaveValueMap(pointNode, "pausedStopPosition1", 428);
                SaveValueMap(pointNode, "pausedStopLength", 202);
                SaveValueMap(pointNode, "pausedStopHeight", 53);

                SaveValueMap(pointNode, "assistTextPosition1", 360.0);
                SaveValueMap(pointNode, "inputAssistTextPosition1", 480.0);

                DrawingInputModeMap[(int)Component.InputMode._4] = GetDrawingInputMode((int)Component.InputMode._4, "2, 3, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._5] = GetDrawingInputMode((int)Component.InputMode._5, "2, 3, 4, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._6] = GetDrawingInputMode((int)Component.InputMode._6, "2, 3, 2, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._7] = GetDrawingInputMode((int)Component.InputMode._7, "2, 3, 2, 5, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._8] = GetDrawingInputMode((int)Component.InputMode._8, "2, 3, 2, 3, 3, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._9] = GetDrawingInputMode((int)Component.InputMode._9, "2, 3, 2, 3, 4, 3, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._5_1] = GetDrawingInputMode((int)Component.InputMode._5_1, "1, 2, 3, 4, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._7_1] = GetDrawingInputMode((int)Component.InputMode._7_1, "1, 2, 3, 2, 5, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._10_2] = GetDrawingInputMode2P((int)Component.InputMode._10_2, "1, 2, 3, 4, 3, 2, 2, 3, 4, 3, 2, 10");
                DrawingInputModeMap[(int)Component.InputMode._14_2] = GetDrawingInputMode2P((int)Component.InputMode._14_2, "1, 2, 3, 2, 5, 2, 3, 2, 2, 3, 2, 5, 2, 3, 2, 10");
                DrawingInputModeMap[(int)Component.InputMode._10] = GetDrawingInputMode((int)Component.InputMode._10, "2, 3, 2, 3, 2, 2, 3, 2, 3, 2");
                DrawingInputModeMap[(int)Component.InputMode._24_2] = GetDrawingInputMode((int)Component.InputMode._24_2, "6, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 11");
                DrawingInputModeMap[(int)Component.InputMode._48_4] = GetDrawingInputMode((int)Component.InputMode._48_4, "6, 6, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 11, 11");
                if (DrawingInputModeMap[(int)Component.InputMode._48_4].Length == 51)
                {
                    DrawingInputModeMap[(int)Component.InputMode._48_4] = new[] { 0 }.Append(DrawingInputModeMap[(int)Component.InputMode._48_4][1]).Concat(DrawingInputModeMap[(int)Component.InputMode._48_4].Skip(1)).Append(DrawingInputModeMap[(int)Component.InputMode._48_4][50]).ToArray();
                }

                DrawingPipeline.AddRange(GetCalledText(Utility.GetText(lambdaNode, "drawingPipeline", string.Join(", ", Enumerable.Range(0, MaxNoteID)))).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var drawingPipeline) ? drawingPipeline : 0).Where(drawingPipeline => drawingPipeline < MaxNoteID));

                int[] GetDrawingInputMode(int mode, string defaultValue)
                {
                    return new[] { 0 }.Concat(GetCalledText(Utility.GetText(lambdaNode, $"drawingInputMode{mode}", defaultValue)).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var drawingPipeline) ? drawingPipeline : 0).Where(drawingPipeline => 0 < drawingPipeline && drawingPipeline < MaxNoteID)).ToArray();
                }

                int[] GetDrawingInputMode2P(int mode, string defaultValue)
                {
                    var drawingInputModeMap = GetDrawingInputMode(mode, defaultValue).Skip(1).ToArray();
                    return IntMap["drawingInputModeSystem"] switch
                    {
                        0 => new[] { 0 }.Concat(drawingInputModeMap).Concat(drawingInputModeMap.Skip(1)).Append(drawingInputModeMap.First()).ToArray(),
                        1 => new[] { 0 }.Concat(drawingInputModeMap).Concat(drawingInputModeMap.Reverse()).ToArray(),
                        2 => new[] { 0 }.Concat(drawingInputModeMap).ToArray(),
                        _ => throw new ArgumentException("drawingInputModeSystem")
                    };
                }

                double GetCalledValue(YamlNode yamlNode, string target, string defaultValue = null)
                {
                    var text = Utility.GetText(yamlNode, target, defaultValue);
                    if (Utility.ToFloat64(text, out var r))
                    {
                        return r;
                    }
                    else if (QwilightComponent.GetCallComputer().IsMatch(text))
                    {
                        var values = text.Split("(").Select(value => value.Trim()).ToArray();
                        return lsCaller.Call(lsCaller.Globals[values[0]], values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                    }
                    else
                    {
                        throw new ArgumentException(target.ToString());
                    }
                }

                string GetCalledText(string text)
                {
                    if (QwilightComponent.GetCallComputer().IsMatch(text))
                    {
                        var values = text.Split("(");
                        text = lsCaller.Call(lsCaller.Globals[values[0]], values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => Utility.ToInt32(value.Trim(), out var arg) ? (object)arg : value as object).ToArray()).String;
                    }
                    return text;
                }

                static string[] GetCalledData(YamlNode yamlNode, string target, string[] defaultValues = null)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        return defaultValues;
                    }
                    else
                    {
                        return text.Split(',').Select(value => value.Trim()).ToArray();
                    }
                }

                void SaveInt(YamlNode yamlNode, string target, int defaultValue = default)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (!string.IsNullOrEmpty(text) && Utility.ToInt32(text, out var value))
                    {
                        IntMap[target] = value;
                    }
                    else
                    {
                        IntMap[target] = defaultValue;
                    }
                }
                void SaveIntMap(YamlNode yamlNode, string target, int defaultValue = default)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        IntMap[target] = defaultValue;
                    }
                    else
                    {
                        SaveIntMapImpl(text, target, IntMap, IntCallMap);
                    }
                }
                void SaveIntMapAsDefaultID(YamlNode yamlNode, string target, string defaultID)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        if (IntMap.TryGetValue(defaultID, out var defaultValue))
                        {
                            IntMap[target] = defaultValue;
                        }
                        if (IntCallMap.TryGetValue(defaultID, out var defaultCall))
                        {
                            IntCallMap[target] = defaultCall;
                        }
                    }
                    else
                    {
                        SaveIntMapImpl(text, target, IntMap, IntCallMap);
                    }
                }
                void SaveSplitValueMap(YamlNode yamlNode, string target, double defaultValue = default)
                {
                    var data = GetCalledData(yamlNode, target);
                    if (data != null)
                    {
                        string lastData = null;
                        for (var i = 0; i < MaxNoteID; ++i)
                        {
                            var t = data.ElementAtOrDefault(i) ?? "~";
                            var s = t != "~" ? t : lastData;
                            SaveValueMapImpl(s, $"{target}{i + 1}", ValueMap, ValueCallMap);
                            lastData = s;
                        }
                    }
                    else
                    {
                        for (var i = MaxNoteID - 1; i >= 0; --i)
                        {
                            ValueMap[$"{target}{i + 1}"] = defaultValue;
                        }
                    }
                }
                void SaveSplitValueMapAsDefaultID(YamlNode yamlNode, string target, string defaultID = null, Func<double, double> valueMapping = null)
                {
                    var data = GetCalledData(yamlNode, target);
                    if (data != null)
                    {
                        string lastData = null;
                        for (var i = 0; i < MaxNoteID; ++i)
                        {
                            var t = data.ElementAtOrDefault(i) ?? "~";
                            var s = t != "~" ? t : lastData;
                            SaveValueMapImpl(s, $"{target}{i + 1}", ValueMap, ValueCallMap);
                            lastData = s;
                        }
                    }
                    else
                    {
                        for (var i = MaxNoteID - 1; i >= 0; --i)
                        {
                            if (ValueMap.TryGetValue(defaultID, out var defaultValue))
                            {
                                ValueMap[$"{target}{i + 1}"] = valueMapping?.Invoke(defaultValue) ?? defaultValue;
                            }
                            if (ValueCallMap.TryGetValue(defaultID, out var defaultCall))
                            {
                                ValueCallMap[$"{target}{i + 1}"] = defaultCall;
                            }
                        }
                    }
                }
                void SaveValueMap(YamlNode yamlNode, string target, double defaultValue = default)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        ValueMap[target] = defaultValue;
                    }
                    else
                    {
                        SaveValueMapImpl(text, target, ValueMap, ValueCallMap);
                    }
                }
                void SaveValueMapAsDefaultID(YamlNode yamlNode, string target, string defaultID, Func<double, double> valueMapping = null)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        if (ValueMap.TryGetValue(defaultID, out var defaultValue))
                        {
                            ValueMap[target] = valueMapping?.Invoke(defaultValue) ?? defaultValue;
                        }
                        if (ValueCallMap.TryGetValue(defaultID, out var defaultCall))
                        {
                            ValueCallMap[target] = defaultCall;
                        }
                    }
                    else
                    {
                        SaveValueMapImpl(text, target, ValueMap, ValueCallMap);
                    }
                }
                void SaveAltMap(YamlNode yamlNode, string target, int defaultValue = default)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        AltMap[target] = defaultValue;
                    }
                    else
                    {
                        SaveAltImpl(text, target, AltMap, AltCallMap);
                    }
                }
                void SaveAltMapAsDefaultID(YamlNode yamlNode, string target, string defaultID)
                {
                    var text = Utility.GetText(yamlNode, target);
                    if (string.IsNullOrEmpty(text))
                    {
                        if (AltMap.TryGetValue(defaultID, out var defaultValue))
                        {
                            AltMap[target] = defaultValue;
                        }
                        if (AltCallMap.TryGetValue(defaultID, out var defaultCall))
                        {
                            AltCallMap[target] = defaultCall;
                        }
                    }
                    else
                    {
                        SaveAltImpl(text, target, AltMap, AltCallMap);
                    }
                }

                for (var i = PaintProperties.Length - 1; i >= 0; --i)
                {
                    var data = GetCalledData(pointNode, $"paintProperty{i}");
                    if (data?.Length > 4)
                    {
                        var paintProperty = new PaintProperty();
                        SavePaintMap(PaintProperty.ID.Position0, 0);
                        SavePaintMap(PaintProperty.ID.Position1, 1);
                        SavePaintMap(PaintProperty.ID.Length, 2);
                        SavePaintMap(PaintProperty.ID.Height, 3);
                        SavePaintAlt(PaintProperty.ID.Alt, 4);
                        SavePaintInt(PaintProperty.ID.Frame, 5, 1);
                        SavePaintValue(PaintProperty.ID.Framerate, 6);
                        SavePaintInt(PaintProperty.ID.Mode, 7);
                        SavePaintInt(PaintProperty.ID.Pipeline, 8, -1);
                        SavePaintInt(PaintProperty.ID.Composition, 9);
                        PaintProperties[i] = paintProperty;
                        void SavePaintMap(PaintProperty.ID target, int paintPosition, double defaultValue = default)
                        {
                            if (paintPosition < data.Length)
                            {
                                SaveValueMapImpl(data[paintPosition], target, paintProperty.ValueMap, paintProperty.ValueCallMap);
                            }
                            else
                            {
                                paintProperty.ValueMap[target] = defaultValue;
                            }
                        }
                        void SavePaintAlt(PaintProperty.ID target, int paintPosition, int defaultValue = default)
                        {
                            if (paintPosition < data.Length)
                            {
                                SaveAltImpl(data[paintPosition], target, paintProperty.AltMap, paintProperty.AltCallMap);
                            }
                            else
                            {
                                paintProperty.AltMap[target] = defaultValue;
                            }
                        }
                        void SavePaintValue(PaintProperty.ID target, int paintPosition, double defaultValue = default)
                        {
                            var text = data.ElementAtOrDefault(paintPosition);
                            if (!string.IsNullOrEmpty(text) && Utility.ToFloat64(data[paintPosition], out var value))
                            {
                                paintProperty.ValueMap[target] = value;
                            }
                            else
                            {
                                paintProperty.ValueMap[target] = defaultValue;
                            }
                        }
                        void SavePaintInt(PaintProperty.ID target, int paintPosition, int defaultValue = default)
                        {
                            if (paintPosition < data.Length)
                            {
                                SaveIntMapImpl(data[paintPosition], target, paintProperty.IntMap, paintProperty.IntCallMap);
                            }
                            else
                            {
                                paintProperty.IntMap[target] = defaultValue;
                            }
                        }
                    }
                }
                static void SaveIntMapImpl<T>(string text, T target, Dictionary<T, int> toIntMap, Dictionary<T, string[]> toCallMap)
                {
                    if (Utility.ToInt32(text, out var r))
                    {
                        toIntMap[target] = r;
                    }
                    else if (QwilightComponent.GetCallComputer().IsMatch(text))
                    {
                        var data = new List<string>();
                        var values = text.Split("(");
                        data.Add(values[0]);
                        data.AddRange(values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()));
                        toCallMap[target] = data.ToArray();
                    }
                    else
                    {
                        throw new ArgumentException(target.ToString());
                    }
                }
                static void SaveValueMapImpl<T>(string text, T target, Dictionary<T, double> toValueMap, Dictionary<T, string[]> toCallMap)
                {
                    if (Utility.ToFloat64(text, out var r))
                    {
                        toValueMap[target] = r;
                    }
                    else if (QwilightComponent.GetCallComputer().IsMatch(text))
                    {
                        var data = new List<string>();
                        var values = text.Split("(");
                        data.Add(values[0]);
                        data.AddRange(values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()));
                        toCallMap[target] = data.ToArray();
                    }
                    else
                    {
                        throw new ArgumentException(target.ToString());
                    }
                }
                static void SaveAltImpl<T>(string text, T target, Dictionary<T, int> altMap, Dictionary<T, string[]> toCallMap)
                {
                    switch (text)
                    {
                        case "0":
                            altMap[target] = 0;
                            break;
                        case "1":
                            altMap[target] = 3;
                            break;
                        case "2":
                            altMap[target] = 2;
                            break;
                        default:
                            if (QwilightComponent.GetCallComputer().IsMatch(text))
                            {
                                var data = new List<string>();
                                var values = text.Split("(");
                                data.Add(values[0]);
                                data.AddRange(values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()));
                                toCallMap[target] = data.ToArray();
                            }
                            else
                            {
                                throw new ArgumentException(target.ToString());
                            }
                            break;
                    }
                }
            }

            var getNote = new Func<int[], string>(args => "N");
            var getHitNotePaint = new Func<int[], string>(args => "N");
            var getHitLongNotePaint = new Func<int[], string>(args => "L");
            var getJudgmentPaint = new Func<int[], string>(args => "J");
            var getMain = new Func<int[], string>(args => "M");
            var getWall = new Func<int[], string>(args => "W");
            var get = new Func<int[], string>(args => "W");
            var getAutoInput = new Func<int[], string>(args => "A");
            var getAutoMain = new Func<int[], string>(args => "AM");
            var getPause = new Func<int[], string>(args => "PS");
            var getJudgmentMain = new Func<int[], string>(args => "J");
            var getMainJudgmentMeter = new Func<int[], string>(args => "JM");
            var getInput = new Func<int[], string>(args => "I");
            var getLevel = new Func<int[], string>(args => "L");
            var getPaintProperty = new Func<int[], string>(args => "P");
            var getHighestBandBin = new Func<int[], string>(args => "HC");
            var getStandBin = new Func<int[], string>(args => "S");
            var getBandBin = new Func<int[], string>(args => "C");
            var getVeil = new Func<int[], string>(args => "Veil");

            SetLambda("_GetNote", ref getNote);
            SetLambda("_GetHitNotePaint", ref getHitNotePaint);
            SetLambda("_GetHitLongNotePaint", ref getHitLongNotePaint);
            SetLambda("_GetJudgmentPaint", ref getJudgmentPaint);
            SetLambda("_GetMain", ref getMain);
            SetLambda("_GetWall", ref getWall);
            SetLambda("_GetAutoInput", ref getAutoInput);
            SetLambda("_GetAutoMain", ref getAutoMain);
            SetLambda("_GetJudgmentMain", ref getJudgmentMain);
            SetLambda("_GetMainJudgmentMeter", ref getMainJudgmentMeter);
            SetLambda("_GetInput", ref getInput);
            SetLambda("_GetPaintProperty", ref getPaintProperty);
            SetLambda("_GetHighestBandBin", ref getHighestBandBin);
            SetLambda("_GetStandBin", ref getStandBin);
            SetLambda("_GetBandBin", ref getBandBin);
            SetLambda("_GetVeil", ref getVeil);

            void SetLambda(string lambdaName, ref Func<int[], string> lambda)
            {
                var value = lsCaller.Globals[lambdaName];
                if (value != null)
                {
                    lambda = new Func<int[], string>(args =>
                    {
                        try
                        {
                            return lsCaller.Call(value, args).String;
                        }
                        catch
                        {
                            throw new ArgumentException($"{lambdaName}([{string.Join(", ", args)}])");
                        }
                    });
                }
            }

            foreach (var (toCallID, values) in IntCallMap)
            {
                if (!IntMap.ContainsKey(toCallID))
                {
                    var lambdaName = values[0];
                    var value = lsCaller.Globals[lambdaName];
                    if (value != null)
                    {
                        try
                        {
                            IntMap[toCallID] = (int)lsCaller.Call(value, values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                        }
                        catch
                        {
                            // 여기서는 불가능한 연산들
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"{lambdaName}({string.Join(", ", values.Skip(1))})");
                    }
                }
            }
            foreach (var paintProperty in PaintProperties.Where(paintProperty => paintProperty != null))
            {
                foreach (var (toCallID, values) in paintProperty.IntCallMap)
                {
                    if (!paintProperty.IntMap.ContainsKey(toCallID))
                    {
                        var lambdaName = values[0];
                        var lambdaValue = lsCaller.Globals[lambdaName];
                        if (lambdaValue != null)
                        {
                            try
                            {
                                paintProperty.IntMap[toCallID] = (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                            }
                            catch
                            {
                                // 여기서는 불가능한 연산들
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"{lambdaName}({string.Join(", ", values.Skip(1))})");
                        }
                    }
                }
                switch (paintProperty.IntMap[PaintProperty.ID.Mode])
                {
                    case 0:
                    case 2:
                        paintProperty.Drawings = new DrawingItem?[paintProperty.IntMap[PaintProperty.ID.Frame]];
                        break;
                    case 1:
                        paintProperty.Drawings = new DrawingItem?[paintProperty.IntMap[PaintProperty.ID.Frame] + 1];
                        break;
                }
            }

            for (var i = BinJudgmentValueMap.Length - 1; i >= 0; --i)
            {
                BinJudgmentValueMap[i] = new DrawingItem?[10];
            }
            IntMap.TryGetValue("judgment-frame", out var judgmentFrame);
            for (var i = JudgmentDrawings.Length - 1; i >= 0; --i)
            {
                JudgmentDrawings[i] = new DrawingItem?[judgmentFrame];
            }
            IntMap.TryGetValue("last-frame", out var lastFrame);
            JudgmentDrawings[JudgmentPaint.Last] = new DrawingItem?[lastFrame];
            IntMap.TryGetValue("band!-frame", out var band1Frame);
            JudgmentDrawings[JudgmentPaint.Band1] = new DrawingItem?[band1Frame];
            IntMap.TryGetValue("yell!-frame", out var yell1Frame);
            JudgmentDrawings[JudgmentPaint.Yell1] = new DrawingItem?[yell1Frame];
            if (!IntMap.TryGetValue("band-frame", out var bandFrame))
            {
                bandFrame = 1;
                IntMap["band-frame"] = bandFrame;
            }
            BinBandMap = new DrawingItem?[10, bandFrame];
            IntMap.TryGetValue("note-hit-frame", out var noteHitFrame);
            for (var i = noteHitDrawings.Length - 1; i > 0; --i)
            {
                noteHitDrawings[i] = new DrawingItem?[noteHitFrame];
            }
            IntMap.TryGetValue("long-note-hit-frame", out var longNoteHitFrame);
            for (var i = longNoteHitDrawings.Length - 1; i > 0; --i)
            {
                longNoteHitDrawings[i] = new DrawingItem?[longNoteHitFrame];
            }
            if (!IntMap.TryGetValue("long-note-hit-loop-frame", out var longNoteHitLoopFrame))
            {
                longNoteHitLoopFrame = longNoteHitFrame;
                IntMap["long-note-hit-loop-frame"] = longNoteHitLoopFrame;
            }
            if (!IntMap.TryGetValue("last-enlarged-band-loop-frame", out var lastEnlargedBandLoopFrame))
            {
                lastEnlargedBandLoopFrame = longNoteHitLoopFrame;
                IntMap["last-enlarged-band-loop-frame"] = lastEnlargedBandLoopFrame;
            }
            IntMap.TryGetValue("main-frame", out var mainFrame);
            for (var i = mainDrawings.Length - 1; i > 0; --i)
            {
                mainDrawings[i] = new DrawingItem?[mainFrame + 1];
            }
            IntMap.TryGetValue("judgment-main-frame", out var judgmentMainFrame);
            for (var i = judgmentMainDrawings.Length - 1; i > 0; --i)
            {
                judgmentMainDrawings[i] = new DrawingItem?[judgmentMainFrame + 1];
            }
            IntMap.TryGetValue("input-frame", out var inputFrame);
            for (var i = inputDrawings.Length - 1; i > 0; --i)
            {
                inputDrawings[i] = new DrawingItem?[inputFrame + 1];
            }
            IntMap.TryGetValue("level-frame", out var levelFrame);
            for (var i = LevelDrawings.Length - 1; i >= 0; --i)
            {
                LevelDrawings[i] = new DrawingItem?[levelFrame];
            }
            IntMap.TryGetValue("note-frame", out var noteFrame);
            for (var i = noteDrawings.Length - 1; i >= 0; --i)
            {
                noteDrawings[i] = new DrawingItem?[noteFrame][][];
                for (var j = noteDrawings[i].Length - 1; j >= 0; --j)
                {
                    noteDrawings[i][j] = new DrawingItem?[18][];
                    for (var m = noteDrawings[i][j].Length - 1; m >= 0; --m)
                    {
                        noteDrawings[i][j][m] = new DrawingItem?[3];
                    }
                }
            }
            IntMap.TryGetValue("auto-main-frame", out var autoMainFrame);
            AutoMainDrawings = new DrawingItem?[autoMainFrame];
            IntMap.TryGetValue("pause-frame", out var pauseFrame);
            for (var i = PauseDrawings.Length - 1; i >= 0; --i)
            {
                PauseDrawings[i] = new DrawingItem?[pauseFrame];
            }
            IntMap.TryGetValue("main-judgment-meter-frame", out var mainJudgmentMeterFrame);
            for (var i = mainJudgmentMeterDrawings.Length - 1; i > 0; --i)
            {
                mainJudgmentMeterDrawings[i] = new Dictionary<int, DrawingItem?>();
            }
            DrawingItem? pointUnitDrawing = null;
            DrawingItem? multiplierUnitDrawing = null;
            DrawingItem? stopPointDrawing = null;
            DrawingItem? lowerDrawing = null;
            DrawingItem? higherDrawing = null;
            var binMap = new DrawingItem?[10];
            var audioValues = new ConcurrentDictionary<string, AudioItem>();
            var drawingValues = new ConcurrentDictionary<string, DrawingItem>();
            var handledDrawingValues = new ConcurrentDictionary<string, HandledDrawingItem>();
            var zipFilePath = Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, Path.ChangeExtension(zipName, "zip"));
            var fileNames = new HashSet<string>();
            if (File.Exists(zipFilePath))
            {
                using var zipFile = new ZipFile(zipFilePath);
                foreach (var zipEntry in zipFile)
                {
                    if (!zipEntry.IsDirectory)
                    {
                        var rms = PoolSystem.Instance.GetDataFlow((int)zipEntry.UncompressedSize);
                        zipEntry.Extract(rms);
                        SetDrawing(zipEntry.FileName, rms);
                    }
                }
            }
            foreach (var pausedFileName in new[] { "Configure 0.png", "Configure 1.png", "Stop 0.png", "Stop 1.png", "Undo 0.png", "Undo 1.png", "Unpause 0.png", "Unpause 1.png" })
            {
                if (!fileNames.Contains($"Paused/{pausedFileName}"))
                {
                    SetDrawing($"Paused/{pausedFileName}", File.OpenRead(Path.Combine(QwilightComponent.AssetsEntryPath, "Paused", pausedFileName)));
                }
            }
            void SetDrawing(string fileName, Stream s)
            {
                var justFileName = Path.GetFileNameWithoutExtension(fileName);
                fileNames.Add(fileName);
                switch (Path.GetDirectoryName(fileName))
                {
                    case "Audio":
                        parallelItems.Add(() =>
                        {
                            try
                            {
                                using (s)
                                {
                                    audioValues[fileName] = AudioSystem.Instance.Load(s, this, 1F, QwilightComponent.GetLoopingAudioComputer().IsMatch(justFileName));
                                }
                            }
                            catch
                            {
                            }
                        });
                        break;
                    case "Note":
                    case "Main":
                        NewDrawing(s, true);
                        break;
                    case "Drawing":
                        if (justFileName == getVeil([]))
                        {
                            NewHandledDrawing(s);
                        }
                        else
                        {
                            NewDrawing(s);
                        }
                        break;
                    default:
                        NewDrawing(s);
                        break;
                }
                void NewDrawing(Stream s, bool setAverage = false) => parallelItems.Add(() =>
                {
                    try
                    {
                        using (s)
                        {
                            drawingValues[fileName] = DrawingSystem.Instance.Load(s, this, setAverage);
                        }
                    }
                    catch
                    {
                    }
                });
                void NewHandledDrawing(Stream s)
                {
                    parallelItems.Add(() =>
                    {
                        using (s)
                        {
                            try
                            {
                                handledDrawingValues[fileName] = new HandledDrawingItem
                                {
                                    Drawing = DrawingSystem.Instance.Load(s, this),
                                    DefaultDrawing = DrawingSystem.Instance.LoadDefault(s, this)
                                };
                            }
                            catch
                            {
                            }
                        }
                    });
                }
            }

            if (src != target)
            {
                AudioSystem.Instance.Close(this);
                DrawingSystem.Instance.Close(this);
            }
            Utility.HandleLowestlyParallelly(parallelItems, Configure.Instance.UIBin, parallelItem => parallelItem());

            foreach (var fileName in audioValues.Keys)
            {
                var justFileName = Path.GetFileNameWithoutExtension(fileName);
                var audioItem = audioValues[fileName];
                if (Path.GetDirectoryName(fileName) == "Audio")
                {
                    _audioItemMap[justFileName] = audioItem;
                }
            }
            foreach (var (fileName, drawingItem) in drawingValues.OrderBy(drawingValue => drawingValue.Key))
            {
                var justFileName = Path.GetFileNameWithoutExtension(fileName);
                switch (Path.GetDirectoryName(fileName))
                {
                    case "Paint":
                        var fileNameContents = justFileName.Split(' ');
                        var main = Utility.ToInt32(fileNameContents.ElementAtOrDefault(1));
                        var frame = Utility.ToInt32(fileNameContents.ElementAtOrDefault(2));
                        if (fileNameContents[0] == getHitNotePaint([main, frame]))
                        {
                            switch (fileNameContents.Length)
                            {
                                case 2:
                                    for (var i = MaxNoteID; i > 0; --i)
                                    {
                                        noteHitDrawings.SetValue(i, main, drawingItem);
                                    }
                                    break;
                                case 3:
                                    foreach (var drawing in drawingMap[main])
                                    {
                                        noteHitDrawings.SetValue(drawing, frame, drawingItem);
                                    }
                                    break;
                            }
                        }
                        else if (fileNameContents[0] == getHitLongNotePaint([main, frame]))
                        {
                            switch (fileNameContents.Length)
                            {
                                case 2:
                                    for (var i = MaxNoteID; i > 0; --i)
                                    {
                                        longNoteHitDrawings.SetValue(i, main, drawingItem);
                                    }
                                    break;
                                case 3:
                                    foreach (var drawing in drawingMap[main])
                                    {
                                        longNoteHitDrawings.SetValue(drawing, frame, drawingItem);
                                    }
                                    break;
                            }
                        }
                        else if (fileNameContents[0] == getJudgmentPaint([main, frame]))
                        {
                            JudgmentDrawings.SetValue(main, frame, drawingItem);
                        }
                        break;
                    case "Drawing":
                        fileNameContents = justFileName.Split(' ');
                        Utility.ToInt32(fileNameContents.ElementAtOrDefault(1), out var value1);
                        Utility.ToInt32(fileNameContents.ElementAtOrDefault(2), out var value2);
                        if (fileNameContents[0] == getPaintProperty([value1, value2]))
                        {
                            Utility.GetValue(PaintProperties, value1)?.Drawings?.SetValue(fileNameContents.Length >= 3 ? value2 : 0, drawingItem);
                        }
                        else if (fileNameContents[0] == getAutoMain([value1]))
                        {
                            AutoMainDrawings[value1] = drawingItem;
                        }
                        else if (fileNameContents[0] == getPause([value1, value2]))
                        {
                            PauseDrawings.SetValue(value1, value2, drawingItem);
                        }
                        else if (justFileName.IsFrontCaselsss("JI"))
                        {
                            if (Utility.ToInt32(justFileName.Split(' ')[1], out var i))
                            {
                                JudgmentInputDrawings.SetValue(i, drawingItem);
                            }
                        }
                        else if (justFileName.IsFrontCaselsss("J"))
                        {
                            if (Utility.ToInt32(justFileName.Split(' ')[1], out var i))
                            {
                                JudgmentPointsDrawings.SetValue(i, drawingItem);
                            }
                        }
                        else if (justFileName.IsFrontCaselsss("HP"))
                        {
                            if (Utility.ToInt32(justFileName.Split(' ')[1], out var i))
                            {
                                HitPointsDrawings.SetValue(i, drawingItem);
                            }
                        }
                        else
                        {
                            switch (justFileName)
                            {
                                case "Status":
                                    StatusDrawing = drawingItem;
                                    break;
                                case "Status Slider":
                                    StatusSliderDrawing = drawingItem;
                                    break;
                            }
                        }
                        break;
                    case "Note":
                        fileNameContents = justFileName.Split(' ');
                        main = Utility.ToInt32(fileNameContents.ElementAtOrDefault(1));
                        frame = Utility.ToInt32(fileNameContents.ElementAtOrDefault(2));
                        var text = Utility.ToInt32(fileNameContents.ElementAtOrDefault(3));
                        var longNoteContents = Utility.ToInt32(fileNameContents.ElementAtOrDefault(4));
                        if (fileNameContents[0] == getNote([main, frame, text, longNoteContents]))
                        {
                            var longNoteStatus = fileNameContents.Length > 4 ? longNoteContents : LongNote.LongNoteBefore;
                            foreach (var drawing in drawingMap[main])
                            {
                                noteDrawings.SetValue(drawing, frame, text, longNoteStatus, drawingItem);
                            }
                        }
                        break;
                    case "Main":
                        fileNameContents = justFileName.Split(' ');
                        main = Utility.ToInt32(fileNameContents.ElementAtOrDefault(1));
                        frame = Utility.ToInt32(fileNameContents.ElementAtOrDefault(2));
                        if (fileNameContents[0] == getMain([main, frame]))
                        {
                            foreach (var drawing in drawingMap[main])
                            {
                                mainDrawings.SetValue(drawing, frame, drawingItem);
                            }
                        }
                        else if (fileNameContents[0] == getWall([main, frame]))
                        {
                            MainWalls.SetValue(main, drawingItem);
                        }
                        else if (fileNameContents[0] == getAutoInput([main, frame]))
                        {
                            foreach (var drawing in drawingMap[main])
                            {
                                autoInputDrawings.SetValue(drawing, drawingItem);
                            }
                        }
                        else if (fileNameContents[0] == getJudgmentMain([main, frame]))
                        {
                            foreach (var drawing in drawingMap[main])
                            {
                                judgmentMainDrawings.SetValue(drawing, frame, drawingItem);
                            }
                        }
                        else if (fileNameContents[0] == getMainJudgmentMeter([main, frame]))
                        {
                            if (main > 0)
                            {
                                foreach (var drawing in drawingMap[main])
                                {
                                    mainJudgmentMeterDrawings[drawing][frame] = drawingItem;
                                }
                            }
                            else
                            {
                                for (var i = drawingMap.Length - 1; i > 0; --i)
                                {
                                    mainJudgmentMeterDrawings[i][frame] = drawingItem;
                                }
                            }
                        }
                        break;
                    case "Input":
                        fileNameContents = justFileName.Split(' ');
                        main = Utility.ToInt32(fileNameContents[1]);
                        frame = Utility.ToInt32(fileNameContents[2]);
                        if (fileNameContents[0] == getInput([main, frame]))
                        {
                            foreach (var drawing in drawingMap[main])
                            {
                                if (drawing > 0)
                                {
                                    inputDrawings.SetValue(drawing, frame, drawingItem);
                                }
                            }
                        }
                        break;
                    case "Level":
                        fileNameContents = justFileName.Split(' ');
                        main = Utility.ToInt32(fileNameContents[1]);
                        frame = Utility.ToInt32(fileNameContents[2]);
                        if (fileNameContents[0] == getLevel([main, frame]))
                        {
                            LevelDrawings.SetValue(main, frame, drawingItem);
                        }
                        break;
                    case "Paused":
                        fileNameContents = justFileName.Split(' ');
                        switch (fileNameContents[0])
                        {
                            case "Unpause":
                                PausedUnpauseDrawings[Utility.ToInt32(fileNameContents[1])] = drawingItem;
                                break;
                            case "Stop":
                                PausedStopDrawings[Utility.ToInt32(fileNameContents[1])] = drawingItem;
                                break;
                            case "Undo":
                                PausedUndoDrawings[Utility.ToInt32(fileNameContents[1])] = drawingItem;
                                break;
                            case "Configure":
                                PausedConfigureDrawings[Utility.ToInt32(fileNameContents[1])] = drawingItem;
                                break;
                        }
                        break;
                    case "Bin":
                        switch (justFileName)
                        {
                            case "%":
                                pointUnitDrawing = drawingItem;
                                break;
                            case ",":
                                CommaDrawing = drawingItem;
                                break;
                            case ".":
                                stopPointDrawing = drawingItem;
                                break;
                            case "..":
                                ColonDrawing = drawingItem;
                                break;
                            case "+":
                                higherDrawing = drawingItem;
                                break;
                            case "-":
                                lowerDrawing = drawingItem;
                                break;
                            case "BPM":
                                BPMUnitDrawing = drawingItem;
                                break;
                            case "Millis":
                                JudgmentMeterUnitDrawing = drawingItem;
                                break;
                            case "Slash":
                                SlashDrawing = drawingItem;
                                break;
                            case "X":
                                multiplierUnitDrawing = drawingItem;
                                break;
                            default:
                                fileNameContents = justFileName.Split(' ');
                                Utility.ToInt32(fileNameContents.ElementAtOrDefault(1), out value1);
                                Utility.ToInt32(fileNameContents.ElementAtOrDefault(2), out value2);
                                if (Utility.ToInt32(justFileName, out var bin))
                                {
                                    binMap.SetValue(bin, drawingItem);
                                }
                                else if (fileNameContents[0].IsFrontCaselsss("A"))
                                {
                                    switch (justFileName)
                                    {
                                        case "AW":
                                            AudioMultiplierUnitDrawing = drawingItem;
                                            break;
                                        case "A .":
                                            AudioMultiplierStopPointDrawing = drawingItem;
                                            break;
                                        default:
                                            BinAudioMultiplierMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "B")
                                {
                                    BinBPMMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0] == getBandBin([value1, value2]))
                                {
                                    switch (fileNameContents.Length)
                                    {
                                        case 2:
                                            BinBandMap.SetValue(value1, 0, drawingItem);
                                            break;
                                        case 3:
                                            BinBandMap.SetValue(value1, value2, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "E")
                                {
                                    BinEarlyValueMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0] == getHighestBandBin([value1]))
                                {
                                    BinHighestBandMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0] == "HU")
                                {
                                    switch (justFileName)
                                    {
                                        case "HU +":
                                            HunterHigherDrawing = drawingItem;
                                            break;
                                        case "HU -":
                                            HunterLowerDrawing = drawingItem;
                                            break;
                                        default:
                                            BinHunterMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "HH")
                                {
                                    switch (justFileName)
                                    {
                                        case "HH .":
                                            JudgmentVSVisualizerStopPointDrawing = drawingItem;
                                            break;
                                        default:
                                            BinJudgmentVSVisualizerMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "HP")
                                {
                                    switch (justFileName)
                                    {
                                        case "HP %":
                                            HitPointsVisualizerUnitDrawing = drawingItem;
                                            break;
                                        default:
                                            BinHitPointsVisualizerMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "IS")
                                {
                                    BinInputVisualizerMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0] == "JM")
                                {
                                    switch (justFileName)
                                    {
                                        case "JM -":
                                            JudgmentMeterLowerDrawing = drawingItem;
                                            break;
                                        case "JM +":
                                            JudgmentMererHigherDrawing = drawingItem;
                                            break;
                                        default:
                                            BinJudgmentMeterMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "J")
                                {
                                    BinJudgmentValueMap.SetValue(value1, value2, drawingItem);
                                }
                                else if (fileNameContents[0] == "L")
                                {
                                    BinLateValueMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0].IsFrontCaselsss("M"))
                                {
                                    switch (justFileName)
                                    {
                                        case "MW":
                                            MultiplierUnitDrawing = drawingItem;
                                            break;
                                        case "M .":
                                            MultiplierStopPointDrawing = drawingItem;
                                            break;
                                        default:
                                            BinMultiplierMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == "P")
                                {
                                    switch (justFileName)
                                    {
                                        case "P %":
                                            PointUnitDrawing = drawingItem;
                                            break;
                                        case "P .":
                                            PointStopPointDrawing = drawingItem;
                                            break;
                                        default:
                                            BinPointMap.SetValue(value1, drawingItem);
                                            break;
                                    }
                                }
                                else if (fileNameContents[0] == getStandBin([value1]))
                                {
                                    BinStandMap.SetValue(value1, drawingItem);
                                }
                                else if (fileNameContents[0] == "W")
                                {
                                    BinHmsMap.SetValue(value1, drawingItem);
                                }
                                break;
                        }
                        break;
                }
            }
            foreach (var (fileName, handledDrawingItem) in handledDrawingValues.OrderBy(handledDrawingValue => handledDrawingValue.Key))
            {
                var justFileName = Path.GetFileNameWithoutExtension(fileName);
                switch (Path.GetDirectoryName(fileName))
                {
                    case "Drawing":
                        if (justFileName == getVeil([]))
                        {
                            VeilDrawing = handledDrawingItem;
                        }
                        break;
                }
            }

            for (var i = (int)Component.Judged.Lowest; i >= (int)Component.Judged.Highest; --i)
            {
                for (var j = 1; j < judgmentFrame; ++j)
                {
                    JudgmentDrawings[i][j] ??= JudgmentDrawings[i][j - 1];
                }
            }
            for (var i = 1; i < lastFrame; ++i)
            {
                JudgmentDrawings[JudgmentPaint.Last][i] ??= JudgmentDrawings[JudgmentPaint.Last][i - 1];
            }
            for (var i = 1; i < band1Frame; ++i)
            {
                JudgmentDrawings[JudgmentPaint.Band1][i] ??= JudgmentDrawings[JudgmentPaint.Band1][i - 1];
            }
            for (var i = 1; i < yell1Frame; ++i)
            {
                JudgmentDrawings[JudgmentPaint.Yell1][i] ??= JudgmentDrawings[JudgmentPaint.Yell1][i - 1];
            }
            for (var i = 0; i < yell1Frame; ++i)
            {
                JudgmentDrawings[JudgmentPaint.Yell1][i] ??= JudgmentDrawings[JudgmentPaint.Band1][i];
            }

            for (var i = noteDrawings.Length - 1; i > 0; --i)
            {
                for (var j = noteFrame - 1; j >= 0; --j)
                {
                    noteDrawings[i][j][TrapNote.TrapNoteContents][LongNote.LongNoteBefore] ??= noteDrawings[0][j][InputNote.InputNoteContents][LongNote.LongNoteBefore];
                    for (var m = VoidNote.VoidNoteContents; m >= TrapNote.TrapNoteContents; --m)
                    {
                        noteDrawings[i][j][m][LongNote.LongNoteBefore] ??= noteDrawings[0][j][m][LongNote.LongNoteBefore];
                    }
                }
            }
            for (var i = noteDrawings.Length - 1; i > 0; --i)
            {
                for (var j = noteFrame - 1; j >= 0; --j)
                {
                    for (var m = VoidNote.VoidNoteContents; m >= InputNote.InputNoteContents; --m)
                    {
                        for (var o = LongNote.LongNoteFailed; o > LongNote.LongNoteBefore; --o)
                        {
                            noteDrawings[i][j][m][o] ??= noteDrawings[i][j][m][LongNote.LongNoteBefore];
                        }
                    }
                }
            }
            for (var i = noteDrawings.Length - 1; i >= 0; --i)
            {
                for (var j = noteFrame - 1; j >= 0; --j)
                {
                    for (var m = LongNote.LongNoteFailed; m >= LongNote.LongNoteBefore; --m)
                    {
                        noteDrawings[i][j][VoidNote.VoidNoteContents][m] ??= noteDrawings[i][j][TrapNote.TrapNoteContents][m];
                    }
                }
            }
            for (var i = noteDrawings.Length - 1; i >= 0; --i)
            {
                for (var j = 1; j < noteFrame; ++j)
                {
                    for (var m = VoidNote.VoidNoteContents; m >= InputNote.InputNoteContents; --m)
                    {
                        for (var o = LongNote.LongNoteFailed; o >= LongNote.LongNoteBefore; --o)
                        {
                            noteDrawings[i][j][m][o] ??= noteDrawings[i][j - 1][m][o];
                        }
                    }
                }
            }

            var lowestHitFrame = Math.Min(longNoteHitFrame, noteHitFrame);
            if (lowestHitFrame > 0)
            {
                for (var i = longNoteHitDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = lowestHitFrame - 1; j >= 0; --j)
                    {
                        longNoteHitDrawings[i][j] ??= noteHitDrawings[i][j];
                    }
                }
            }

            if (mainFrame > 0)
            {
                for (var i = mainDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = 1; j <= mainFrame; ++j)
                    {
                        mainDrawings[i][j] ??= mainDrawings[i][j - 1];
                    }
                }
            }

            if (judgmentMainFrame > 0)
            {
                for (var i = judgmentMainDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = 1; j <= judgmentMainFrame; ++j)
                    {
                        judgmentMainDrawings[i][j] ??= judgmentMainDrawings[i][j - 1];
                    }
                }
            }

            if (mainJudgmentMeterFrame > 0)
            {
                for (var i = mainJudgmentMeterDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = 1; j <= mainJudgmentMeterFrame; ++j)
                    {
                        if (mainJudgmentMeterDrawings[i].ContainsKey(j - 1))
                        {
                            mainJudgmentMeterDrawings[i][j] ??= mainJudgmentMeterDrawings[i][j - 1];
                        }
                    }
                    for (var j = -1; j >= -mainJudgmentMeterFrame; --j)
                    {
                        if (mainJudgmentMeterDrawings[i].ContainsKey(j + 1))
                        {
                            mainJudgmentMeterDrawings[i][j] ??= mainJudgmentMeterDrawings[i][j + 1];
                        }
                    }
                }
            }

            if (inputFrame > 0)
            {
                for (var i = inputDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = 1; j <= inputFrame; ++j)
                    {
                        inputDrawings[i][j] ??= inputDrawings[i][j - 1];
                    }
                }
            }

            if (levelFrame > 0)
            {
                for (var i = LevelDrawings.Length - 1; i >= 0; --i)
                {
                    for (var j = 1; j < levelFrame; ++j)
                    {
                        LevelDrawings[i][j] ??= LevelDrawings[i][j - 1];
                    }
                }
            }

            if (autoMainFrame > 0)
            {
                for (var i = 1; i < autoMainFrame; ++i)
                {
                    AutoMainDrawings[i] ??= AutoMainDrawings[i - 1];
                }
            }

            if (pauseFrame > 0)
            {
                for (var i = PauseDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = 1; j < pauseFrame; ++j)
                    {
                        PauseDrawings[i][j] ??= PauseDrawings[i][j - 1];
                    }
                }
            }

            for (var i = 9; i >= 0; --i)
            {
                BinBandMap[i, 0] ??= binMap[i];
                for (var j = 0; j < bandFrame; ++j)
                {
                    if (BinBandMap[i, j] == null)
                    {
                        for (var m = j - 1; m >= 0; --m)
                        {
                            if (BinBandMap[i, m] != null)
                            {
                                BinBandMap[i, j] ??= BinBandMap[i, m];
                                break;
                            }
                        }
                    }
                }
                for (var j = (int)Component.Judged.Lowest; j >= (int)Component.Judged.Highest; --j)
                {
                    BinJudgmentValueMap[j][i] ??= binMap[i];
                }
                BinStandMap[i] ??= binMap[i];
                BinPointMap[i] ??= binMap[i];
                BinHmsMap[i] ??= binMap[i];
                BinHitPointsVisualizerMap[i] ??= binMap[i];
                BinJudgmentMeterMap[i] ??= binMap[i];
                BinHighestBandMap[i] ??= binMap[i];
                BinInputVisualizerMap[i] ??= binMap[i];
                BinAudioMultiplierMap[i] ??= binMap[i];
                BinMultiplierMap[i] ??= binMap[i];
                BinBPMMap[i] ??= binMap[i];
                BinHunterMap[i] ??= binMap[i];
                BinEarlyValueMap[i] ??= binMap[i];
                BinLateValueMap[i] ??= binMap[i];
                BinJudgmentVSVisualizerMap[i] ??= binMap[i];
            }
            HitPointsVisualizerUnitDrawing ??= pointUnitDrawing;
            PointUnitDrawing ??= pointUnitDrawing;
            PointStopPointDrawing ??= stopPointDrawing;
            MultiplierStopPointDrawing ??= stopPointDrawing;
            AudioMultiplierStopPointDrawing ??= stopPointDrawing;
            JudgmentVSVisualizerStopPointDrawing ??= stopPointDrawing;
            MultiplierUnitDrawing ??= multiplierUnitDrawing;
            AudioMultiplierUnitDrawing ??= multiplierUnitDrawing;
            JudgmentMeterLowerDrawing ??= lowerDrawing;
            JudgmentMererHigherDrawing ??= higherDrawing;
            HunterLowerDrawing ??= lowerDrawing;
            HunterHigherDrawing ??= higherDrawing;
            HitPointsDrawings[(int)ModeComponent.HitPointsMode.Yell] ??= HitPointsDrawings[(int)ModeComponent.HitPointsMode.Failed];
            for (var i = HitPointsDrawings.Length - 1; i >= 0; --i)
            {
                HitPointsDrawings[i] ??= HitPointsDrawings[(int)ModeComponent.HitPointsMode.Default];
            }
            for (var i = (int)Component.InputMode._48_4; i >= (int)Component.InputMode._4; --i)
            {
                var inputCount = Component.InputCounts[i];
                AutoInputDrawings[i] = new DrawingItem?[inputCount + 1];
                NoteHitDrawings[i] = new DrawingItem?[inputCount + 1][];
                LongNoteHitDrawings[i] = new DrawingItem?[inputCount + 1][];
                MainDrawings[i] = new DrawingItem?[inputCount + 1][];
                JudgmentMainDrawings[i] = new DrawingItem?[inputCount + 1][];
                InputDrawings[i] = new DrawingItem?[inputCount + 1][];
                NoteDrawings[i] = new DrawingItem?[inputCount + 1][][][];
                MainJudgmentMeterDrawings[i] = new Dictionary<int, DrawingItem?>[inputCount + 1];
                for (var j = inputCount; j > 0; --j)
                {
                    AutoInputDrawings[i][j] = autoInputDrawings[DrawingInputModeMap[i][j]];
                    NoteHitDrawings[i][j] = noteHitDrawings[DrawingInputModeMap[i][j]];
                    LongNoteHitDrawings[i][j] = longNoteHitDrawings[DrawingInputModeMap[i][j]];
                    MainDrawings[i][j] = mainDrawings[DrawingInputModeMap[i][j]];
                    JudgmentMainDrawings[i][j] = judgmentMainDrawings[DrawingInputModeMap[i][j]];
                    InputDrawings[i][j] = inputDrawings[DrawingInputModeMap[i][j]];
                    NoteDrawings[i][j] = noteDrawings[DrawingInputModeMap[i][j]];
                    MainJudgmentMeterDrawings[i][j] = mainJudgmentMeterDrawings[DrawingInputModeMap[i][j]];
                }
            }

            FaultText = null;
        }

        public void SetFontFamily()
        {
            DrawingSystem.Instance.SetFontFamily(TitleFont);
            DrawingSystem.Instance.SetFontFamily(ArtistFont);
            DrawingSystem.Instance.SetFontFamily(GenreFont);
            DrawingSystem.Instance.SetFontFamily(LevelTextFont);
            DrawingSystem.Instance.SetFontFamily(WantLevelFont);
        }

        public void LoadUIFiles() => Utility.SetUICollection(UIItems, Utility.GetEntry(QwilightComponent.UIEntryPath).Prepend(QwilightComponent.UIEntryPath).SelectMany(targetEntryPath => Utility.GetFiles(targetEntryPath).Where(targetFilePath => !Path.GetFileName(targetFilePath).StartsWith('@') && targetFilePath.IsTailCaselsss(".yaml")).Select(yamlFilePath => new UIItem
        {
            UIEntry = Path.GetRelativePath(QwilightComponent.UIEntryPath, targetEntryPath),
            YamlName = Path.GetFileNameWithoutExtension(yamlFilePath)
        })).ToArray());

        void Init()
        {
            foreach (var value in typeof(UI).GetProperties())
            {
                if (value.CanWrite)
                {
                    value.SetValue(this, default);
                }
                else if (value.PropertyType.IsArray)
                {
                    var data = value.GetValue(this) as Array;
                    Array.Clear(data, 0, data.Length);
                }
            }
            DefaultLength = Component.StandardLength;
            DefaultHeight = Component.StandardHeight;
            XamlUIConfigures = Array.Empty<XamlUIConfigure>();
            _audioItemMap.Clear();
            ValueCallMap.Clear();
            ValueMap.Clear();
            IntMap.Clear();
            IntCallMap.Clear();
            AltMap.Clear();
            PaintPipelineValues.Clear();
            DrawingPipeline.Clear();
            DrawingSystem.Instance.SetFontLevel(TitleFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(ArtistFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(GenreFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(LevelTextFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(WantLevelFont, Levels.FontLevel0Float32);
        }

        public void SetConfigures(Script lsCaller)
        {
            lsCaller.Globals["configures"] = Enumerable.Range(0, MaxUIConfigure).Select(i =>
            {
                return Math.Max(0, Array.IndexOf(XamlUIConfigures.SingleOrDefault(value => value.Position == i)?.Configures ?? Array.Empty<string>(), Configure.Instance.UIConfigureValue.UIConfiguresV2[i]));
            }).ToArray();
        }

        public void LoadUI(UIItem src, UIItem target, bool isParallel = true)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsUILoading)
            {
                mainViewModel.IsUILoading = true;
                if (isParallel)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpeningUIFileContents, true, null, null, NotifySystem.LoadUIID);
                    Task.Run(() =>
                    {
                        try
                        {
                            lock (LoadedCSX)
                            {
                                LoadUIImpl(src, target);
                            }
                            OnLoaded();
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpenedUIFileContents, true, null, null, NotifySystem.LoadUIID);
                        }
                        catch (YamlException e)
                        {
                            FaultText = string.Format(LanguageSystem.Instance.YAMLCompileFault, e.Message);
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText, true, null, null, NotifySystem.LoadUIID);
                        }
                        catch (InterpreterException e)
                        {
                            FaultText = string.Format(LanguageSystem.Instance.LSCompileFault, e.Message);
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText, true, null, null, NotifySystem.LoadUIID);
                        }
                        catch (Exception e)
                        {
                            FaultText = string.Format(LanguageSystem.Instance.UIFaultText, e.Message);
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText, true, null, null, NotifySystem.LoadUIID);
                        }
                        finally
                        {
                            mainViewModel.IsUILoading = false;
                        }
                    });
                }
                else
                {
                    try
                    {
                        lock (LoadedCSX)
                        {
                            LoadUIImpl(src, target);
                        }
                        OnLoaded();
                    }
                    catch (YamlException e)
                    {
                        FaultText = string.Format(LanguageSystem.Instance.YAMLCompileFault, e.Message, true);
                    }
                    catch (InterpreterException e)
                    {
                        FaultText = string.Format(LanguageSystem.Instance.LSCompileFault, e.Message, true);
                    }
                    catch (Exception e)
                    {
                        FaultText = string.Format(LanguageSystem.Instance.UIFaultText, e.Message, true);
                    }
                    finally
                    {
                        mainViewModel.IsUILoading = false;
                    }
                }

                void OnLoaded()
                {
                    mainViewModel.GetHandlingComputer()?.SetUIMap();
                    DrawingSystem.Instance.OnModified();
                    DrawingSystem.Instance.LoadVeilDrawing();
                    AvatarTitleSystem.Instance.WipeAvatarTitles();
                    ViewModels.Instance.NotifyWindowViewModels();
                    ViewModels.Instance.ModifyModeComponentValue.SetModeComponentItems();
                    mainViewModel.NotifyModel();
                }
            }
        }
    }
}