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
        public const int HighestUIConfigure = 16;
        public const int HighestNoteID = 64;
        public const int HighestPaintPropertyID = 256;

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

        readonly Dictionary<string, AudioItem?> _audioItemMap = new();

        /// <summary>
        /// UI, BaseUI가 로드됨을 보장하는 락
        /// </summary>
        public object ContentsCSX { get; } = new();

        public XamlUIConfigure[] XamlUIConfigures { get; set; } = Array.Empty<XamlUIConfigure>();

        public string[] LoadedConfigures { get; } = new string[8];

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

        public double DefaultHeight => Component.StandardHeight;

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

        public PaintProperty[] PaintPropertyValues { get; } = new PaintProperty[HighestPaintPropertyID];

        public DrawingItem?[][] JudgmentDrawings { get; } = new DrawingItem?[11][];

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

        public DrawingItem?[] HitPointsDrawings { get; } = new DrawingItem?[8];

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

        public bool HandleAudio(string audioFileName, string defaultFileName, PausableAudioHandler pausableAudioHandler, double fadeInLength)
        {
            lock (ContentsCSX)
            {
                if (!_audioItemMap.TryGetValue(audioFileName, out var audioItem) && defaultFileName != null)
                {
                    _audioItemMap.TryGetValue(defaultFileName, out audioItem);
                }
                if (audioItem != null)
                {
                    AudioSystem.Instance.Handle(new AudioNote
                    {
                        AudioLevyingPosition = pausableAudioHandler?.GetAudioPosition() ?? 0U,
                        AudioItem = audioItem
                    }, AudioSystem.SEAudio, 1.0, false, pausableAudioHandler, fadeInLength);
                    return true;
                }
                return false;
            }
        }

        public string FaultText { get; set; }

        public UI() => Init();

        void LoadUIImpl(UIItem src, UIItem target)
        {
            #region COMPATIBLE
            Compatible.Compatible.UI(QwilightComponent.UIEntryPath, target.GetYamlFilePath(), target.YamlName, target.UIEntry);
            #endregion

            var mainViewModel = ViewModels.Instance.MainValue;
            try
            {
                Init();

                var drawingMap = new int[HighestNoteID + 1][];
                drawingMap[0] = new int[] { 0 };
                var noteHitDrawings = new DrawingItem?[HighestNoteID + 1][];
                var longNoteHitDrawings = new DrawingItem?[HighestNoteID + 1][];
                var inputDrawings = new DrawingItem?[HighestNoteID + 1][];
                var noteDrawings = new DrawingItem?[HighestNoteID + 1][][][];
                var mainDrawings = new DrawingItem?[HighestNoteID + 1][];
                var autoInputDrawings = new DrawingItem?[HighestNoteID + 1];
                var judgmentMainDrawings = new DrawingItem?[HighestNoteID + 1][];
                var mainJudgmentMeterDrawings = new Dictionary<int, DrawingItem?>[HighestNoteID + 1];
                string zipName;

                var lsCaller = new Script();

                var parallelItems = new ConcurrentBag<Action>();

                var ys = new YamlStream();
                using (var sr = File.OpenText(target.GetYamlFilePath()))
                {
                    ys.Load(sr);

                    var valueNode = ys.Documents[0].RootNode;
                    var formatNode = valueNode[new YamlScalarNode("format")];
                    var funcNode = valueNode[new YamlScalarNode("func")];
                    var frameNode = valueNode[new YamlScalarNode("frame")];
                    var pointNode = valueNode[new YamlScalarNode("point")];
                    (valueNode as YamlMappingNode).Children.TryGetValue(new YamlScalarNode("paint"), out var paintNode);
                    (valueNode as YamlMappingNode).Children.TryGetValue(new YamlScalarNode("font"), out var fontNode);

                    zipName = Utility.GetText(formatNode, "zip", target.YamlName);

                    XamlUIConfigures = Enumerable.Range(0, HighestUIConfigure).Select(i =>
                    {
                        var configures = (Utility.GetText(funcNode, $"configure-{i}-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(funcNode, $"configure-{i}"))?.Split(',')?.Select(configure => configure.Trim())?.ToArray();
                        if (configures != null)
                        {
                            LoadedConfigures[i] = Configure.Instance.UIConfigureValue.UIConfiguresV2[i] ??= configures.FirstOrDefault();
                            return new XamlUIConfigure
                            {
                                Position = i,
                                Configures = configures,
                                ConfigureComment = Utility.GetText(funcNode, $"configure-comment-{i}-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(funcNode, $"configure-comment-{i}")
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

                    var setPaintPipelines = Utility.ToBool(Utility.GetText(funcNode, "set-paint-pinelines", bool.FalseString));
                    foreach (var pipeline in GetCalledText(Utility.GetText(funcNode, "pipeline")).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var pipeline) ? pipeline : 0))
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
                    SaveInt(funcNode, "drawingInputModeSystem", 0);

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
                    SaveValueMapAsDefaultID("stopPointDrawingLength", "binLength");

                    SaveSplitValueMap("floatingNotePosition0", 0.0);
                    SaveSplitValueMap("floatingNoteLength", 0.0);
                    SaveSplitValueMap("slashNotePosition0", 0.0);

                    SaveValueMap(pointNode, "judgmentMainPosition");

                    SaveValueMap(pointNode, "mediaPosition0", 0.0);
                    SaveValueMap(pointNode, "mediaPosition1", 0.0);
                    SaveValueMap(pointNode, "mediaLength", 1280.0);
                    SaveValueMap(pointNode, "mediaHeight", 720.0);
                    SaveAltMap("alt-media", 0);

                    SaveValueMap(pointNode, "titlePosition0");
                    SaveValueMap(pointNode, "titlePosition1");
                    SaveValueMap(pointNode, "titleLength");
                    SaveValueMap(pointNode, "titleHeight");
                    SaveIntMap(pointNode, "titleSystem0");
                    SaveIntMap(pointNode, "titleSystem1", 2);
                    SaveAltMap("alt-title", 0);

                    SaveValueMap(pointNode, "artistPosition0");
                    SaveValueMap(pointNode, "artistPosition1");
                    SaveValueMap(pointNode, "artistLength");
                    SaveValueMap(pointNode, "artistHeight");
                    SaveIntMap(pointNode, "artistSystem0");
                    SaveIntMap(pointNode, "artistSystem1", 2);
                    SaveAltMap("alt-artist", 0);

                    SaveValueMap(pointNode, "genrePosition0");
                    SaveValueMap(pointNode, "genrePosition1");
                    SaveValueMap(pointNode, "genreLength");
                    SaveValueMap(pointNode, "genreHeight");
                    SaveIntMap(pointNode, "genreSystem0");
                    SaveIntMap(pointNode, "genreSystem1");
                    SaveAltMap("alt-genre", 0);

                    SaveValueMap(pointNode, "levelTextPosition0");
                    SaveValueMap(pointNode, "levelTextPosition1");
                    SaveValueMap(pointNode, "levelTextLength");
                    SaveValueMap(pointNode, "levelTextHeight");
                    SaveIntMap(pointNode, "levelTextSystem0");
                    SaveIntMap(pointNode, "levelTextSystem1");
                    SaveAltMap("alt-level-text", 0);

                    SaveValueMap(pointNode, "wantLevelPosition0");
                    SaveValueMap(pointNode, "wantLevelPosition1");
                    SaveValueMap(pointNode, "wantLevelLength");
                    SaveValueMap(pointNode, "wantLevelHeight");
                    SaveIntMap(pointNode, "wantLevelSystem0");
                    SaveIntMap(pointNode, "wantLevelSystem1");
                    SaveAltMap("alt-want-level", 0);

                    SaveValueMap(pointNode, "audioVisualizerPosition0", 0.0);
                    SaveValueMap(pointNode, "audioVisualizerPosition1", 0.0);
                    SaveValueMap(pointNode, "audioVisualizerLength", 1280.0);
                    SaveValueMap(pointNode, "audioVisualizerHeight", 720.0);

                    DrawingSystem.Instance.SetFontLevel(TitleFont, Utility.ToFloat32(Utility.GetText(fontNode, "titleLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                    DrawingSystem.Instance.SetFontLevel(ArtistFont, Utility.ToFloat32(Utility.GetText(fontNode, "artistLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                    DrawingSystem.Instance.SetFontLevel(GenreFont, Utility.ToFloat32(Utility.GetText(fontNode, "genreLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                    DrawingSystem.Instance.SetFontLevel(LevelTextFont, Utility.ToFloat32(Utility.GetText(fontNode, "levelTextLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));
                    DrawingSystem.Instance.SetFontLevel(WantLevelFont, Utility.ToFloat32(Utility.GetText(fontNode, "wantLevelLevel", QwilightComponent.GetBuiltInFloat64As("FontLevel0"))));

                    DefaultLength = Utility.ToFloat64(Utility.GetText(formatNode, "defaultLength", "1280"));

                    SaveValueMap(pointNode, "mainWall0Length");
                    SaveValueMap(pointNode, "mainWall1Length");
                    SaveValueMap(pointNode, "mainWall0Position1");
                    SaveValueMap(pointNode, "mainWall0Height", 720.0);
                    SaveValueMap(pointNode, "mainWall1Position1");
                    SaveValueMap(pointNode, "mainWall1Height", 720.0);
                    SaveAltMap("alt-wall-0", 2);
                    SaveAltMap("alt-wall-1", 2);

                    SaveIntMap(frameNode, "main-frame");
                    SaveValueMap(frameNode, "main-framerate");
                    SaveValueMap(pointNode, "mainPosition1");
                    SaveValueMap(pointNode, "mainHeight", 720.0);

                    SaveSplitValueMap("autoInputPosition1");
                    SaveSplitValueMap("autoInputHeight", 720.0);

                    SaveIntMap(pointNode, "hitPointsSystem");
                    SaveValueMap(pointNode, "hitPointsPosition0");
                    SaveValueMap(pointNode, "hitPointsPosition1");
                    SaveValueMap(pointNode, "hitPointsLength");
                    SaveValueMap(pointNode, "hitPointsHeight");
                    SaveAltMap("alt-hit-points", 2);

                    SaveIntMap(frameNode, "note-frame");
                    SaveValueMap(frameNode, "note-framerate");
                    SaveSplitValueMap("noteLength");
                    SaveSplitValueMap("noteHeight");
                    SaveSplitValueMap("noteHeightJudgment");
                    SaveSplitValueMap("longNoteTailEdgeHeight");
                    SaveSplitValueMap("longNoteFrontEdgeHeight");
                    SaveSplitValueMap("longNoteTailEdgePosition");
                    SaveSplitValueMap("longNoteFrontEdgePosition");
                    SaveSplitValueMap("longNoteTailContentsHeight");
                    SaveSplitValueMap("longNoteFrontContentsHeight");
                    MaintainLongNoteFrontEdge = Utility.ToBool(Utility.GetText(funcNode, "maintainLongNoteFrontEdge", bool.FalseString));
                    MaintainAutoInput = Utility.ToBool(Utility.GetText(funcNode, "maintainAutoInput", bool.TrueString));
                    LoopingMain = Utility.ToInt32(Utility.GetText(funcNode, "loopingMain", 0.ToString()));
                    LoopingInput = Utility.ToInt32(Utility.GetText(funcNode, "loopingInput", 0.ToString()));
                    SetJudgmentMainPosition = Utility.ToBool(Utility.GetText(funcNode, "setJudgmentMainPosition", bool.FalseString));
                    SetMainPosition = Utility.ToBool(Utility.GetText(funcNode, "setMainPosition", bool.FalseString));
                    SetNoteLength = Utility.ToBool(Utility.GetText(funcNode, "setNoteLength", bool.FalseString));
                    SetNoteHeight = Utility.ToBool(Utility.GetText(funcNode, "setNoteHeight", bool.FalseString));
                    SetBandPosition = Utility.ToBool(Utility.GetText(funcNode, "setBandPosition", bool.FalseString));

                    SaveIntMap(funcNode, "judgmentPaintComposition");
                    SaveIntMap(funcNode, "hitNotePaintComposition", (int)CanvasComposite.Add);

                    SaveIntMap(frameNode, "input-frame");
                    SaveValueMap(frameNode, "input-framerate");
                    SaveSplitValueMap("inputPosition0");
                    SaveSplitValueMapAsDefaultID("inputPosition1", "judgmentMainPosition");
                    SaveSplitValueMap("inputLength");
                    SaveSplitValueMapAsDefaultID("inputHeight", "judgmentMainPosition", value => 720.0 - value);

                    SaveIntMap(frameNode, "level-frame");
                    SaveValueMap(frameNode, "level-framerate");
                    SaveValueMap(pointNode, "levelPosition0");
                    SaveValueMap(pointNode, "levelPosition1");
                    SaveValueMap(pointNode, "levelLength");
                    SaveValueMap(pointNode, "levelHeight");
                    SaveAltMap("alt-level");

                    SaveIntMap(pointNode, "bandSystem");
                    SaveValueMap(pointNode, "bandPosition0");
                    SaveValueMap(pointNode, "bandPosition1");
                    SaveIntMap(frameNode, "band-frame");
                    SaveValueMap(frameNode, "band-framerate");
                    SaveValueMapAsDefaultID("binBandLength", "binLength");
                    SaveValueMapAsDefaultID("binBandHeight", "binHeight");
                    SaveValueMap(pointNode, "enlargeBand");
                    SaveAltMap("alt-band", 2);

                    SaveIntMap(pointNode, "judgmentMeterSystem");
                    SaveValueMap(pointNode, "judgmentMeterPosition0");
                    SaveValueMap(pointNode, "judgmentMeterPosition1");
                    SaveValueMapAsDefaultID("binJudgmentMeterLength", "binLength");
                    SaveValueMapAsDefaultID("binJudgmentMeterHeight", "binHeight");
                    SaveValueMapAsDefaultID("judgmentMeterFrontDrawingLength", "binJudgmentMeterLength");
                    SaveValueMapAsDefaultID("judgmentMeterUnitDrawingLength", "binJudgmentMeterLength");
                    SaveAltMap("alt-judgment-meter", 3);

                    SaveIntMap(pointNode, "standSystem");
                    SaveValueMap(pointNode, "standPosition0");
                    SaveValueMap(pointNode, "standPosition1");
                    SaveValueMapAsDefaultID("binStandLength", "binLength");
                    SaveValueMapAsDefaultID("binStandHeight", "binHeight");
                    SaveValueMapAsDefaultID("standCommaDrawingLength", "binLength");
                    SaveAltMap("alt-stand");

                    SaveIntMap(pointNode, "pointSystem");
                    SaveValueMap(pointNode, "pointPosition0");
                    SaveValueMap(pointNode, "pointPosition1");
                    SaveValueMapAsDefaultID("binPointLength", "binLength");
                    SaveValueMapAsDefaultID("binPointHeight", "binHeight");
                    SaveValueMapAsDefaultID("pointStopPointDrawingLength", "stopPointDrawingLength");
                    SaveValueMapAsDefaultID("pointUnitDrawingLength", "binPointLength");
                    SaveAltMap("alt-point");

                    SaveIntMap(pointNode, "bpmSystem");
                    SaveValueMap(pointNode, "bpmPosition0");
                    SaveValueMap(pointNode, "bpmPosition1");
                    SaveValueMapAsDefaultID("binBPMLength", "binLength");
                    SaveValueMapAsDefaultID("binBPMHeight", "binHeight");
                    SaveValueMapAsDefaultID("bpmUnitDrawingLength", "binBPMLength");
                    SaveAltMap("alt-bpm");

                    SaveIntMap(pointNode, "multiplierSystem");
                    SaveValueMap(pointNode, "multiplierPosition0");
                    SaveValueMap(pointNode, "multiplierPosition1");
                    SaveValueMapAsDefaultID("binMultiplierLength", "binLength");
                    SaveValueMapAsDefaultID("binMultiplierHeight", "binHeight");
                    SaveValueMapAsDefaultID("multiplierStopPointDrawingLength", "stopPointDrawingLength");
                    SaveValueMapAsDefaultID("multiplierUnitDrawingLength", "binMultiplierLength");
                    SaveAltMap("alt-multiplier");

                    SaveIntMap(frameNode, "note-hit-frame");
                    SaveValueMap(frameNode, "note-hit-framerate");
                    SaveIntMap(frameNode, "long-note-hit-frame");
                    SaveValueMap(frameNode, "long-note-hit-framerate");
                    SaveIntMap(frameNode, "judgment-frame");
                    SaveValueMap(frameNode, "judgment-framerate");
                    SaveIntMap(pointNode, "judgmentSystem");
                    SaveValueMap(pointNode, "judgmentPosition0");
                    SaveValueMap(pointNode, "judgmentPosition1");
                    SaveValueMap(pointNode, "judgmentLength");
                    SaveValueMap(pointNode, "judgmentHeight");
                    SaveSplitValueMap("hitNotePaintLength");
                    SaveSplitValueMap("hitNotePaintHeight");
                    SaveSplitValueMap("hitLongNotePaintLength");
                    SaveSplitValueMap("hitLongNotePaintHeight");
                    SaveSplitValueMapAsDefaultID("hitNotePaintPosition0", "hitNotePaintLength1", value => -value / 2);
                    SaveSplitValueMapAsDefaultID("hitNotePaintPosition1", "hitNotePaintHeight1", value => -value / 2);
                    SaveSplitValueMapAsDefaultID("hitLongNotePaintPosition0", "hitLongNotePaintLength1", value => -value / 2);
                    SaveSplitValueMapAsDefaultID("hitLongNotePaintPosition1", "hitLongNotePaintHeight1", value => -value / 2);
                    SaveIntMap(frameNode, "hit-input-paint-frame");
                    SaveValueMap(frameNode, "hit-input-paint-framerate");
                    SaveSplitValueMap("hitInputPaintPosition0");
                    SaveSplitValueMap("hitInputPaintPosition1");
                    SaveSplitValueMap("hitInputPaintLength");
                    SaveSplitValueMap("hitInputPaintHeight");
                    SaveIntMap(frameNode, "long-note-hit-loop-frame");
                    SaveIntMap(frameNode, "last-enlarged-band-loop-frame");
                    SaveIntMap(frameNode, "last-frame");
                    SaveValueMap(frameNode, "last-framerate");
                    SaveIntMap(pointNode, "lastSystem");
                    SaveValueMap(pointNode, "lastPosition0");
                    SaveValueMap(pointNode, "lastPosition1");
                    SaveValueMap(pointNode, "lastLength");
                    SaveValueMap(pointNode, "lastHeight");
                    SaveAltMap("alt-last", 2);
                    SaveIntMap(frameNode, "band!-frame");
                    SaveValueMap(frameNode, "band!-framerate");
                    SaveIntMap(pointNode, "band!System");
                    SaveValueMap(pointNode, "band!Position0");
                    SaveValueMap(pointNode, "band!Position1");
                    SaveValueMap(pointNode, "band!Length");
                    SaveValueMap(pointNode, "band!Height");
                    SaveAltMap("alt-band!", 2);

                    SaveIntMap(pointNode, "netSystem");
                    SaveValueMap(pointNode, "netPosition0");
                    SaveValueMap(pointNode, "netPosition1");
                    SaveAltMap("alt-net");

                    SaveIntMap(frameNode, "auto-main-frame");
                    SaveValueMap(frameNode, "auto-main-framerate");
                    SaveIntMap(pointNode, "autoMainSystem");
                    SaveValueMap(pointNode, "autoMainPosition0");
                    SaveValueMap(pointNode, "autoMainPosition1");
                    SaveValueMap(pointNode, "autoMainLength");
                    SaveValueMap(pointNode, "autoMainHeight");
                    SaveAltMap("alt-auto-main");

                    SaveIntMap(frameNode, "pause-frame", 1);
                    SaveIntMap(pointNode, "pauseSystem");
                    SaveValueMap(pointNode, "pausePosition0");
                    SaveValueMap(pointNode, "pausePosition1");
                    SaveValueMap(pointNode, "pauseLength");
                    SaveValueMap(pointNode, "pauseHeight");
                    SaveAltMap("alt-pause", 2);

                    SaveIntMap(pointNode, "statusSystem");
                    SaveValueMap(pointNode, "statusPosition0");
                    SaveValueMap(pointNode, "statusPosition1");
                    SaveValueMap(pointNode, "statusLength");
                    SaveValueMap(pointNode, "statusHeight");
                    SaveAltMap("alt-status");

                    SaveIntMap(pointNode, "statusSliderSystem");
                    SaveValueMap(pointNode, "statusSliderPosition0");
                    SaveValueMap(pointNode, "statusSliderPosition1");
                    SaveValueMap(pointNode, "statusSliderLength");
                    SaveValueMap(pointNode, "statusSliderHeight");
                    SaveValueMap(pointNode, "statusSliderContentsLength");
                    SaveValueMap(pointNode, "statusSliderContentsHeight");
                    SaveAltMap("alt-status-slider");

                    SaveIntMap(pointNode, "hmsSystem");
                    SaveValueMap(pointNode, "hmsPosition0");
                    SaveValueMap(pointNode, "hmsPosition1");
                    SaveValueMapAsDefaultID("binHmsLength", "binLength");
                    SaveValueMapAsDefaultID("binHmsHeight", "binHeight");
                    SaveValueMapAsDefaultID("hmsColonDrawingLength", "binLength");
                    SaveValueMapAsDefaultID("hmsSlashDrawingLength", "binLength");
                    SaveAltMap("alt-hms");

                    SaveIntMap(pointNode, "judgmentPointsSystem");
                    SaveValueMap(pointNode, "judgmentPointsPosition0");
                    SaveValueMap(pointNode, "judgmentPointsPosition1");
                    SaveValueMap(pointNode, "judgmentPointsLength");
                    SaveValueMap(pointNode, "judgmentPointsHeight");
                    SaveAltMap("alt-judgment-points");

                    SaveIntMap(frameNode, "judgment-main-frame");
                    SaveValueMap(frameNode, "judgment-main-framerate");
                    SaveSplitValueMap("judgmentMainPosition1");
                    SaveSplitValueMap("judgmentMainHeight");

                    SaveIntMap(frameNode, "main-judgment-meter-frame");
                    SaveValueMap(frameNode, "main-judgment-meter-framerate");
                    SaveSplitValueMap("mainJudgmentMeterPosition1");
                    SaveSplitValueMap("mainJudgmentMeterHeight");

                    SaveIntMap(pointNode, "audioMultiplierSystem");
                    SaveValueMap(pointNode, "audioMultiplierPosition0");
                    SaveValueMap(pointNode, "audioMultiplierPosition1");
                    SaveValueMapAsDefaultID("binAudioMultiplierLength", "binLength");
                    SaveValueMapAsDefaultID("binAudioMultiplierHeight", "binHeight");
                    SaveValueMapAsDefaultID("audioMultiplierStopPointDrawingLength", "stopPointDrawingLength");
                    SaveValueMapAsDefaultID("audioMultiplierUnitDrawingLength", "binAudioMultiplierLength");
                    SaveAltMap("alt-audio-multiplier");

                    SaveIntMap(pointNode, "hitPointsVisualizerSystem");
                    SaveValueMap(pointNode, "hitPointsVisualizerPosition0");
                    SaveValueMap(pointNode, "hitPointsVisualizerPosition1");
                    SaveValueMapAsDefaultID("binHitPointsVisualizerLength", "binLength");
                    SaveValueMapAsDefaultID("binHitPointsVisualizerHeight", "binHeight");
                    SaveValueMap(pointNode, "hitPointsVisualizerUnitDrawingLength");
                    SaveAltMap("alt-hit-points-visualizer");

                    SaveIntMap(pointNode, "highestJudgmentValueSystem");
                    SaveValueMap(pointNode, "highestJudgmentValuePosition0");
                    SaveValueMap(pointNode, "highestJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binHighestJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binHighestJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-highest-judgment-value");

                    SaveIntMap(pointNode, "higherJudgmentValueSystem");
                    SaveValueMap(pointNode, "higherJudgmentValuePosition0");
                    SaveValueMap(pointNode, "higherJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binHigherJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binHigherJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-higher-judgment-value");

                    SaveIntMap(pointNode, "highJudgmentValueSystem");
                    SaveValueMap(pointNode, "highJudgmentValuePosition0");
                    SaveValueMap(pointNode, "highJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binHighJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binHighJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-high-judgment-value");

                    SaveIntMap(pointNode, "lowJudgmentValueSystem");
                    SaveValueMap(pointNode, "lowJudgmentValuePosition0");
                    SaveValueMap(pointNode, "lowJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binLowJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binLowJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-low-judgment-value");

                    SaveIntMap(pointNode, "lowerJudgmentValueSystem");
                    SaveValueMap(pointNode, "lowerJudgmentValuePosition0");
                    SaveValueMap(pointNode, "lowerJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binLowerJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binLowerJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-lower-judgment-value");

                    SaveIntMap(pointNode, "lowestJudgmentValueSystem");
                    SaveValueMap(pointNode, "lowestJudgmentValuePosition0");
                    SaveValueMap(pointNode, "lowestJudgmentValuePosition1");
                    SaveValueMapAsDefaultID("binLowestJudgmentValueLength", "binLength");
                    SaveValueMapAsDefaultID("binLowestJudgmentValueHeight", "binHeight");
                    SaveAltMap("alt-lowest-judgment-value");

                    SaveIntMap(pointNode, "highestBandSystem");
                    SaveValueMap(pointNode, "highestBandPosition0");
                    SaveValueMap(pointNode, "highestBandPosition1");
                    SaveValueMapAsDefaultID("binHighestBandLength", "binLength");
                    SaveValueMapAsDefaultID("binHighestBandHeight", "binHeight");
                    SaveAltMap("alt-highest-band");

                    SaveValueMapAsDefaultID("limiterPosition1", "mainPosition1");
                    SaveValueMap(pointNode, "limiterLength", 1.0);
                    SaveValueMapAsDefaultID("limiterHeight", "mainHeight");

                    SaveIntMap(pointNode, "judgmentVisualizerSystem");
                    SaveValueMap(pointNode, "judgmentVisualizerPosition0");
                    SaveValueMap(pointNode, "judgmentVisualizerPosition1");
                    SaveValueMap(pointNode, "judgmentVisualizerLength");
                    SaveValueMap(pointNode, "judgmentVisualizerHeight");
                    SaveValueMap(pointNode, "judgmentVisualizerContentsLength");
                    SaveValueMap(pointNode, "judgmentVisualizerContentsHeight");
                    SaveAltMap("alt-judgment-visualizer");

                    SaveIntMap(pointNode, "hunterSystem");
                    SaveValueMap(pointNode, "hunterPosition0");
                    SaveValueMap(pointNode, "hunterPosition1");
                    SaveValueMapAsDefaultID("binHunterLength", "binLength");
                    SaveValueMapAsDefaultID("binHunterHeight", "binHeight");
                    SaveValueMapAsDefaultID("hunterFrontDrawingLength", "binHunterLength");
                    SaveAltMap("alt-hunter");

                    SaveIntMap(pointNode, "inputVisualizerSystem");
                    SaveValueMap(pointNode, "inputVisualizerPosition0");
                    SaveValueMap(pointNode, "inputVisualizerPosition1");
                    SaveValueMapAsDefaultID("binInputVisualizerLength", "binLength");
                    SaveValueMapAsDefaultID("binInputVisualizerHeight", "binHeight");
                    SaveAltMap("alt-input-visualizer");

                    SaveIntMap(pointNode, "earlyValueSystem");
                    SaveValueMap(pointNode, "earlyValuePosition0");
                    SaveValueMap(pointNode, "earlyValuePosition1");
                    SaveValueMapAsDefaultID("binEarlyValueLength", "binLength");
                    SaveValueMapAsDefaultID("binEarlyValueHeight", "binHeight");
                    SaveAltMap("alt-early-value");

                    SaveIntMap(pointNode, "lateValueSystem");
                    SaveValueMap(pointNode, "lateValuePosition0");
                    SaveValueMap(pointNode, "lateValuePosition1");
                    SaveValueMapAsDefaultID("binLateValueLength", "binLength");
                    SaveValueMapAsDefaultID("binLateValueHeight", "binHeight");
                    SaveAltMap("alt-late-value");

                    SaveIntMap(pointNode, "judgmentVSVisualizerSystem");
                    SaveValueMap(pointNode, "judgmentVSVisualizerPosition0");
                    SaveValueMap(pointNode, "judgmentVSVisualizerPosition1");
                    SaveValueMapAsDefaultID("binJudgmentVSVisualizerLength", "binLength");
                    SaveValueMapAsDefaultID("binJudgmentVSVisualizerHeight", "binHeight");
                    SaveValueMapAsDefaultID("judgmentVSVisualizerStopPointDrawingLength", "stopPointDrawingLength");
                    SaveAltMap("alt-judgment-vs-visualizer");

                    SaveValueMap(pointNode, "judgmentInputVisualizerPosition0");
                    SaveValueMap(pointNode, "judgmentInputVisualizerPosition1");
                    SaveValueMap(pointNode, "judgmentInputVisualizerLength");
                    SaveValueMap(pointNode, "judgmentInputVisualizerHeight");
                    SaveAltMap("alt-judgment-input-visualizer");

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

                    SaveValueMap(pointNode, "assistTextPosition1", DefaultHeight / 2);
                    SaveValueMap(pointNode, "inputAssistTextPosition1", 2 * DefaultHeight / 3);

                    for (var i = HighestNoteID; i > 0; --i)
                    {
                        drawingMap[i] = GetCalledText(Utility.GetText(funcNode, $"drawing{i}", i.ToString())).Split(',').Select(value => Utility.ToInt32(value, out var drawingPipeline) ? drawingPipeline : 0).Where(drawingPipeline => 0 < drawingPipeline && drawingPipeline < HighestNoteID).ToArray();
                    }

                    DrawingInputModeMap[(int)Component.InputMode.InputMode4] = GetDrawingInputMode((int)Component.InputMode.InputMode4, "2, 3, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode5] = GetDrawingInputMode((int)Component.InputMode.InputMode5, "2, 3, 4, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode6] = GetDrawingInputMode((int)Component.InputMode.InputMode6, "2, 3, 2, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode7] = GetDrawingInputMode((int)Component.InputMode.InputMode7, "2, 3, 2, 5, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode8] = GetDrawingInputMode((int)Component.InputMode.InputMode8, "2, 3, 2, 3, 3, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode9] = GetDrawingInputMode((int)Component.InputMode.InputMode9, "2, 3, 2, 3, 4, 3, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode51] = GetDrawingInputMode((int)Component.InputMode.InputMode51, "1, 2, 3, 4, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode71] = GetDrawingInputMode((int)Component.InputMode.InputMode71, "1, 2, 3, 2, 5, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode102] = GetDrawingInputMode2P((int)Component.InputMode.InputMode102, "1, 2, 3, 4, 3, 2, 2, 3, 4, 3, 2, 10");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode142] = GetDrawingInputMode2P((int)Component.InputMode.InputMode142, "1, 2, 3, 2, 5, 2, 3, 2, 2, 3, 2, 5, 2, 3, 2, 10");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode10] = GetDrawingInputMode((int)Component.InputMode.InputMode10, "2, 3, 2, 3, 2, 2, 3, 2, 3, 2");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode242] = GetDrawingInputMode((int)Component.InputMode.InputMode242, "6, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 11");
                    DrawingInputModeMap[(int)Component.InputMode.InputMode484] = GetDrawingInputMode((int)Component.InputMode.InputMode484, "6, 6, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 9, 8, 7, 8, 7, 9, 8, 7, 8, 7, 8, 7, 11, 11");
                    if (DrawingInputModeMap[(int)Component.InputMode.InputMode484].Length == 51)
                    {
                        DrawingInputModeMap[(int)Component.InputMode.InputMode484] = new int[] { default }.Append(DrawingInputModeMap[(int)Component.InputMode.InputMode484][1]).Concat(DrawingInputModeMap[(int)Component.InputMode.InputMode484].Skip(1)).Append(DrawingInputModeMap[(int)Component.InputMode.InputMode484][50]).ToArray();
                    }

                    DrawingPipeline.AddRange(GetCalledText(Utility.GetText(funcNode, "drawingPipeline", string.Join(", ", Enumerable.Range(0, HighestNoteID)))).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var drawingPipeline) ? drawingPipeline : 0).Where(drawingPipeline => drawingPipeline < HighestNoteID));

                    int[] GetDrawingInputMode(int mode, string defaultValue)
                    {
                        return new int[] { default }.Concat(GetCalledText(Utility.GetText(funcNode, $"drawingInputMode{mode}", defaultValue)).Split(',').Select(value => Utility.ToInt32(value.Trim(), out var drawingPipeline) ? drawingPipeline : 0).Where(drawingPipeline => 0 < drawingPipeline && drawingPipeline < HighestNoteID)).ToArray();
                    }
                    int[] GetDrawingInputMode2P(int mode, string defaultValue)
                    {
                        var drawingInputModeMap = GetDrawingInputMode(mode, defaultValue).Skip(1).ToArray();
                        return IntMap["drawingInputModeSystem"] switch
                        {
                            0 => new int[] { default }.Concat(drawingInputModeMap).Concat(drawingInputModeMap.Skip(1)).Append(drawingInputModeMap.First()).ToArray(),
                            1 => new int[] { default }.Concat(drawingInputModeMap).Concat(drawingInputModeMap.Reverse()).ToArray(),
                            2 => new int[] { default }.Concat(drawingInputModeMap).ToArray(),
                            _ => throw new ArgumentException("drawingInputModeSystem")
                        };
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
                    void SaveSplitValueMap(string target, double defaultValue = default)
                    {
                        var data = GetCalledData(pointNode, target);
                        if (data != null)
                        {
                            string lastData = null;
                            for (var i = 0; i < HighestNoteID; ++i)
                            {
                                var t = data.ElementAtOrDefault(i) ?? "~";
                                var s = t != "~" ? t : lastData;
                                SaveValueMapImpl(s, $"{target}{i + 1}", ValueMap, ValueCallMap);
                                lastData = s;
                            }
                        }
                        else
                        {
                            for (var i = HighestNoteID - 1; i >= 0; --i)
                            {
                                ValueMap[$"{target}{i + 1}"] = defaultValue;
                            }
                        }
                    }
                    void SaveSplitValueMapAsDefaultID(string target, string defaultValueID = null, Func<double, double> valueMapping = null)
                    {
                        var data = GetCalledData(pointNode, target);
                        if (data != null)
                        {
                            string lastData = null;
                            for (var i = 0; i < HighestNoteID; ++i)
                            {
                                var t = data.ElementAtOrDefault(i) ?? "~";
                                var s = t != "~" ? t : lastData;
                                SaveValueMapImpl(s, $"{target}{i + 1}", ValueMap, ValueCallMap);
                                lastData = s;
                            }
                        }
                        else
                        {
                            for (var i = HighestNoteID - 1; i >= 0; --i)
                            {
                                if (ValueMap.TryGetValue(defaultValueID, out var defaultValue))
                                {
                                    ValueMap[$"{target}{i + 1}"] = valueMapping?.Invoke(defaultValue) ?? defaultValue;
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
                    void SaveValueMapAsDefaultID(string target, string defaultValueID, Func<double, double> valueMapping = null)
                    {
                        var text = Utility.GetText(pointNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            if (ValueMap.TryGetValue(defaultValueID, out var defaultValue))
                            {
                                ValueMap[target] = valueMapping?.Invoke(defaultValue) ?? defaultValue;
                            }
                        }
                        else
                        {
                            SaveValueMapImpl(text, target, ValueMap, ValueCallMap);
                        }
                    }
                    void SaveAltMap(string target, int defaultValue = default)
                    {
                        var text = Utility.GetText(funcNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            AltMap[target] = defaultValue;
                        }
                        else
                        {
                            SaveAltImpl(text, target, AltMap, AltCallMap);
                        }
                    }
                    for (var i = PaintPropertyValues.Length - 1; i >= 0; --i)
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
                            PaintPropertyValues[i] = paintProperty;
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
                var getNotePaint = new Func<int[], string>(args => "N");
                var getLongNotePaint = new Func<int[], string>(args => "L");
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

                SetFunc("_GetNote", ref getNote);
                SetFunc("_GetNotePaint", ref getNotePaint);
                SetFunc("_GetLongNotePaint", ref getLongNotePaint);
                SetFunc("_GetJudgmentPaint", ref getJudgmentPaint);
                SetFunc("_GetMain", ref getMain);
                SetFunc("_GetWall", ref getWall);
                SetFunc("_GetAutoInput", ref getAutoInput);
                SetFunc("_GetAutoMain", ref getAutoMain);
                SetFunc("_GetJudgmentMain", ref getJudgmentMain);
                SetFunc("_GetMainJudgmentMeter", ref getMainJudgmentMeter);
                SetFunc("_GetInput", ref getInput);
                SetFunc("_GetPaintProperty", ref getPaintProperty);
                SetFunc("_GetHighestBandBin", ref getHighestBandBin);
                SetFunc("_GetStandBin", ref getStandBin);

                void SetFunc(string funcName, ref Func<int[], string> value)
                {
                    var funcValue = lsCaller.Globals[funcName];
                    if (funcValue != null)
                    {
                        value = new Func<int[], string>(args =>
                        {
                            try
                            {
                                return lsCaller.Call(funcValue, args).String;
                            }
                            catch
                            {
                                throw new ArgumentException($"{funcName}([{string.Join(", ", args)}])");
                            }
                        });
                    }
                }

                foreach (var (toCallID, values) in IntCallMap)
                {
                    if (!IntMap.ContainsKey(toCallID))
                    {
                        var funcName = values[0];
                        var funcValue = lsCaller.Globals[funcName];
                        if (funcValue != null)
                        {
                            try
                            {
                                IntMap[toCallID] = (int)lsCaller.Call(funcValue, values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                            }
                            catch
                            {
                                // 여기서는 불가능한 연산들
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"{funcName}({string.Join(", ", values.Skip(1))})");
                        }
                    }
                }
                foreach (var paintPropertyValue in PaintPropertyValues.Where(paintPropertyValue => paintPropertyValue != null))
                {
                    foreach (var (toCallID, values) in paintPropertyValue.IntCallMap)
                    {
                        if (!paintPropertyValue.IntMap.ContainsKey(toCallID))
                        {
                            var funcName = values[0];
                            var funcValue = lsCaller.Globals[funcName];
                            if (funcValue != null)
                            {
                                try
                                {
                                    paintPropertyValue.IntMap[toCallID] = (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                                }
                                catch
                                {
                                    // 여기서는 불가능한 연산들
                                }
                            }
                            else
                            {
                                throw new ArgumentException($"{funcName}({string.Join(", ", values.Skip(1))})");
                            }
                        }
                    }
                    switch (paintPropertyValue.IntMap[PaintProperty.ID.Mode])
                    {
                        case 0:
                        case 2:
                            paintPropertyValue.Drawings = new DrawingItem?[paintPropertyValue.IntMap[PaintProperty.ID.Frame]];
                            break;
                        case 1:
                            paintPropertyValue.Drawings = new DrawingItem?[paintPropertyValue.IntMap[PaintProperty.ID.Frame] + 1];
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
                IntMap.TryGetValue("band!-frame", out var band1Frame);
                JudgmentDrawings[JudgmentPaint.Band1] = new DrawingItem?[band1Frame];
                IntMap.TryGetValue("last-frame", out var lastFrame);
                JudgmentDrawings[JudgmentPaint.Last] = new DrawingItem?[lastFrame];
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
                        SetDrawing($"Paused/{pausedFileName}", File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Assets", "Paused", pausedFileName)));
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
                                        audioValues[fileName] = AudioSystem.Instance.Load(s, this, 1F, null, QwilightComponent.GetLoopingAudioComputer().IsMatch(justFileName));
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
                            switch (justFileName)
                            {
                                case "Veil":
                                    NewHandledDrawing(s);
                                    break;
                                default:
                                    NewDrawing(s);
                                    break;
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
                Utility.HandleHMP(parallelItems, Configure.Instance.UIBin, parallelItem => parallelItem());

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
                            if (fileNameContents[0] == getNotePaint(new[] { main, frame }))
                            {
                                switch (fileNameContents.Length)
                                {
                                    case 2:
                                        for (var i = HighestNoteID; i > 0; --i)
                                        {
                                            noteHitDrawings.SetValue(i, main, drawingItem);
                                        }
                                        break;
                                    case 3:
                                        foreach (var drawingMapValue in drawingMap[main])
                                        {
                                            noteHitDrawings.SetValue(drawingMapValue, frame, drawingItem);
                                        }
                                        break;
                                }
                            }
                            else if (fileNameContents[0] == getLongNotePaint(new[] { main, frame }))
                            {
                                switch (fileNameContents.Length)
                                {
                                    case 2:
                                        for (var i = HighestNoteID; i > 0; --i)
                                        {
                                            longNoteHitDrawings.SetValue(i, main, drawingItem);
                                        }
                                        break;
                                    case 3:
                                        foreach (var drawingMapValue in drawingMap[main])
                                        {
                                            longNoteHitDrawings.SetValue(drawingMapValue, frame, drawingItem);
                                        }
                                        break;
                                }
                            }
                            else if (fileNameContents[0] == getJudgmentPaint(new[] { main, frame }))
                            {
                                JudgmentDrawings.SetValue(main, frame, drawingItem);
                            }
                            break;
                        case "Drawing":
                            fileNameContents = justFileName.Split(' ');
                            Utility.ToInt32(fileNameContents.ElementAtOrDefault(1), out var value1);
                            Utility.ToInt32(fileNameContents.ElementAtOrDefault(2), out var value2);
                            if (fileNameContents[0] == getPaintProperty(new[] { value1, value2 }))
                            {
                                Utility.GetValue(PaintPropertyValues, value1)?.Drawings?.SetValue(fileNameContents.Length >= 3 ? value2 : 0, drawingItem);
                            }
                            else if (fileNameContents[0] == getAutoMain(new[] { value1 }))
                            {
                                AutoMainDrawings[value1] = drawingItem;
                            }
                            else if (fileNameContents[0] == getPause(new[] { value1, value2 }))
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
                            if (fileNameContents[0] == getNote(new[] { main, frame, text, longNoteContents }))
                            {
                                var status = fileNameContents.Length > 4 ? longNoteContents : LongNote.LongNoteBefore;
                                foreach (var drawingMapValue in drawingMap[main])
                                {
                                    noteDrawings.SetValue(drawingMapValue, frame, text, status, drawingItem);
                                }
                            }
                            break;
                        case "Main":
                            fileNameContents = justFileName.Split(' ');
                            main = Utility.ToInt32(fileNameContents.ElementAtOrDefault(1));
                            frame = Utility.ToInt32(fileNameContents.ElementAtOrDefault(2));
                            if (fileNameContents[0] == getMain(new[] { main, frame }))
                            {
                                foreach (var drawingMapValue in drawingMap[main])
                                {
                                    mainDrawings.SetValue(drawingMapValue, frame, drawingItem);
                                }
                            }
                            else if (fileNameContents[0] == getWall(new[] { main, frame }))
                            {
                                MainWalls.SetValue(main, drawingItem);
                            }
                            else if (fileNameContents[0] == getAutoInput(new[] { main, frame }))
                            {
                                foreach (var drawingMapValue in drawingMap[main])
                                {
                                    autoInputDrawings.SetValue(drawingMapValue, drawingItem);
                                }
                            }
                            else if (fileNameContents[0] == getJudgmentMain(new[] { main, frame }))
                            {
                                foreach (var drawingMapValue in drawingMap[main])
                                {
                                    judgmentMainDrawings.SetValue(drawingMapValue, frame, drawingItem);
                                }
                            }
                            else if (fileNameContents[0] == getMainJudgmentMeter(new[] { main, frame }))
                            {
                                if (main > 0)
                                {
                                    foreach (var drawingMapValue in drawingMap[main])
                                    {
                                        mainJudgmentMeterDrawings[drawingMapValue][frame] = drawingItem;
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
                            if (fileNameContents[0] == getInput(new[] { main, frame }))
                            {
                                foreach (var drawingMapValue in drawingMap[main])
                                {
                                    if (drawingMapValue > 0)
                                    {
                                        inputDrawings.SetValue(drawingMapValue, frame, drawingItem);
                                    }
                                }
                            }
                            break;
                        case "Level":
                            fileNameContents = justFileName.Split(' ');
                            main = Utility.ToInt32(fileNameContents[1]);
                            frame = Utility.ToInt32(fileNameContents[2]);
                            if (fileNameContents[0] == getLevel(new[] { main, frame }))
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
                                    else if (fileNameContents[0] == "C")
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
                                    else if (fileNameContents[0] == getHighestBandBin(new[] { value1 }))
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
                                    else if (fileNameContents[0] == getStandBin(new[] { value1 }))
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
                            switch (justFileName)
                            {
                                case "Veil":
                                    VeilDrawing = handledDrawingItem;
                                    break;
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
                for (var i = 1; i < band1Frame; ++i)
                {
                    JudgmentDrawings[JudgmentPaint.Band1][i] ??= JudgmentDrawings[JudgmentPaint.Band1][i - 1];
                }
                for (var i = 1; i < lastFrame; ++i)
                {
                    JudgmentDrawings[JudgmentPaint.Last][i] ??= JudgmentDrawings[JudgmentPaint.Last][i - 1];
                }

                for (var i = noteDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = noteFrame - 1; j >= 0; --j)
                    {
                        noteDrawings[i][j][TrapNote.TrapNoteContents][LongNote.LongNoteBefore] ??= noteDrawings[0][j][InputNote.InputNoteContents][LongNote.LongNoteBefore];
                        for (var m = InputNote.NeutralPostableItemNoteContents; m >= TrapNote.TrapNoteContents; --m)
                        {
                            noteDrawings[i][j][m][LongNote.LongNoteBefore] ??= noteDrawings[0][j][m][LongNote.LongNoteBefore];
                        }
                    }
                }
                for (var i = noteDrawings.Length - 1; i > 0; --i)
                {
                    for (var j = noteFrame - 1; j >= 0; --j)
                    {
                        for (var m = InputNote.NeutralPostableItemNoteContents; m >= InputNote.InputNoteContents; --m)
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
                        for (var m = InputNote.NeutralPostableItemNoteContents; m >= InputNote.InputNoteContents; --m)
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
                for (var i = HitPointsDrawings.Length - 1; i >= 0; --i)
                {
                    HitPointsDrawings[i] ??= HitPointsDrawings[(int)ModeComponent.HitPointsMode.Default];
                }
                for (var i = (int)Component.InputMode.InputMode484; i >= (int)Component.InputMode.InputMode4; --i)
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

                mainViewModel.GetHandlingComputer()?.SetUIMap();
                FaultText = null;
            }
            finally
            {
                ViewModels.Instance.NotifyWindowViewModels();
                mainViewModel.NotifyModel();
                mainViewModel.IsUILoading = false;
            }
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
            _audioItemMap.Clear();
            ValueCallMap.Clear();
            ValueMap.Clear();
            IntMap.Clear();
            IntCallMap.Clear();
            AltMap.Clear();
            PaintPipelineValues.Clear();
            DrawingPipeline.Clear();
            DefaultLength = 1280.0;
            DrawingSystem.Instance.SetFontLevel(TitleFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(ArtistFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(GenreFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(LevelTextFont, Levels.FontLevel0Float32);
            DrawingSystem.Instance.SetFontLevel(WantLevelFont, Levels.FontLevel0Float32);
        }

        public void SetConfigures(Script lsCaller)
        {
            lsCaller.Globals["configures"] = Enumerable.Range(0, HighestUIConfigure).Select(i =>
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
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpeningUIFileContents);
                    Task.Run(() =>
                    {
                        lock (ContentsCSX)
                        {
                            try
                            {
                                LoadUIImpl(src, target);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpenedUIFileContents);
                            }
                            catch (YamlException e)
                            {
                                FaultText = string.Format(LanguageSystem.Instance.YAMLCompileFault, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText);
                            }
                            catch (InterpreterException e)
                            {
                                FaultText = string.Format(LanguageSystem.Instance.LSCompileFault, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText);
                            }
                            catch (Exception e)
                            {
                                FaultText = string.Format(LanguageSystem.Instance.UIFaultText, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText);
                            }
                        }
                    });
                }
                else
                {
                    lock (ContentsCSX)
                    {
                        try
                        {
                            LoadUIImpl(src, target);
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
                    }
                }
            }
        }
    }
}