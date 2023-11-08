using FMOD;
using Ionic.Zip;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using MoonSharp.Interpreter;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using Windows.UI;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using Brush = System.Windows.Media.Brush;
using ImageSource = System.Windows.Media.ImageSource;
using Stretch = System.Windows.Media.Stretch;

namespace Qwilight
{
    public sealed class BaseUI : Model, IAudioContainer, IDrawingContainer, IMediaContainer, IMediaHandler
    {
        public const int HighestBaseUIConfigure = 16;
        public const int HighestPaintPropertyID = 256;

        public static readonly BaseUI Instance = QwilightComponent.GetBuiltInData<BaseUI>(nameof(BaseUI));

        public enum EventItem
        {
            LevelUp = 100, LevelClear, SignIn, NotSignIn, ModifyEntryItem, ModifyNoteFile, NewTitle, AbilityUp
        }

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
                var zipFilePath = Path.Combine(QwilightComponent.UIEntryPath, value.UIEntry, Path.ChangeExtension($"@{Utility.GetText(formatNode, "zip")}", "zip"));
                if (File.Exists(zipFilePath))
                {
                    zipFile.AddFile(zipFilePath, Path.Combine(entryPath, Path.GetRelativePath(QwilightComponent.UIEntryPath, Path.GetDirectoryName(zipFilePath))));
                }
                var luaFilePath = Path.Combine(QwilightComponent.UIEntryPath, value.UIEntry, Path.ChangeExtension($"@{Utility.GetText(formatNode, "lua")}", "lua"));
                if (File.Exists(luaFilePath))
                {
                    zipFile.AddFile(luaFilePath, Path.Combine(entryPath, Path.GetRelativePath(QwilightComponent.UIEntryPath, Path.GetDirectoryName(luaFilePath))));
                }
            }
        }

        readonly ConcurrentDictionary<string, AudioItem?> _audioItemMap = new();
        readonly ConcurrentDictionary<string, Channel> _audioChannelMap = new();

        public bool HandleAudio(string audioFileName, string defaultFileName = null, PausableAudioHandler pausableAudioHandler = null, double fadeInLength = 0.0)
        {
            lock (UI.Instance.ContentsCSX)
            {
                if (!_audioItemMap.TryGetValue(audioFileName, out var audioItem) && defaultFileName != null)
                {
                    _audioItemMap.TryGetValue(defaultFileName, out audioItem);
                }
                if (audioItem != null)
                {
                    if (QwilightComponent.GetStopLastAudioComputer().IsMatch(audioFileName) && _audioChannelMap.TryGetValue(audioFileName, out var audioChannel))
                    {
                        AudioSystem.Instance.Stop(audioChannel);
                    }
                    _audioChannelMap[audioFileName] = AudioSystem.Instance.Handle(new AudioNote
                    {
                        AudioLevyingPosition = pausableAudioHandler?.GetAudioPosition() ?? 0U,
                        AudioItem = audioItem
                    }, AudioSystem.SEAudio, 1.0, false, pausableAudioHandler, fadeInLength);
                    return true;
                }
                return false;
            }
        }

        public XamlBaseUIConfigure[] XamlBaseUIConfigures { get; set; } = Array.Empty<XamlBaseUIConfigure>();

        public string[] LoadedConfigures { get; } = new string[HighestBaseUIConfigure];

        public string FaultText { get; set; }

        public ObservableCollection<UIItem> UIItems { get; } = new();

        public double DefaultLength { get; set; }

        public double DefaultHeight { get; set; }

        public HandledDrawingItem? DefaultDrawing { get; set; }

        public HandledDrawingItem?[][] QuitDrawings { get; } = new HandledDrawingItem?[8][];

        public HandledDrawingItem?[] InputModeDrawings { get; } = new HandledDrawingItem?[17];

        public ImageSource[] InputModeWindowDrawings { get; } = new ImageSource[17];

        public ImageSource[] HandledWallDrawings { get; } = new ImageSource[7];

        public ImageSource[] DefaultEntryDrawings { get; } = new ImageSource[5];

        public ImageSource[] SaltAutoDrawings { get; } = new ImageSource[2];

        public ImageSource[] AvatarConfigureDrawings { get; } = new ImageSource[6];

        public ImageSource[] SiteSituationDrawings { get; } = new ImageSource[3];

        public ImageSource[] SignInDrawings { get; } = new ImageSource[2];

        public ImageSource[] SiteCipherDrawings { get; } = new ImageSource[2];

        public ImageSource[] SiteConfigureDrawings { get; } = new ImageSource[3];

        public ImageSource[] StopAutoDrawings { get; } = new ImageSource[2];

        public ImageSource[] SiteMediaDrawings { get; } = new ImageSource[3];

        public ImageSource[] SiteAudioDrawings { get; } = new ImageSource[3];

        public ImageSource FileDrawing { get; set; }

        public ImageSource TotalNotesDrawing { get; set; }

        public ImageSource TotalNotesFitDrawing { get; set; }

        public ImageSource TotalNotesNetSiteDrawing { get; set; }

        public ImageSource JudgmentStageDrawing { get; set; }

        public ImageSource HighestInputCountDrawing { get; set; }

        public ImageSource LengthDrawing { get; set; }

        public ImageSource BPMDrawing { get; set; }

        public ImageSource BPM1Drawing { get; set; }

        public ImageSource InputModeDrawing { get; set; }

        public DrawingItem? JudgmentStageQuitDrawing { get; set; }

        public DrawingItem? HighestInputCountQuitDrawing { get; set; }

        public DrawingItem? LengthQuitDrawing { get; set; }

        public DrawingItem? BPMQuitDrawing { get; set; }

        public DrawingItem? InputModeQuitDrawing { get; set; }

        public ImageSource JudgmentStageNetSiteDrawing { get; set; }

        public ImageSource HighestInputCountFitDrawing { get; set; }

        public ImageSource AverageInputCountFitDrawing { get; set; }

        public ImageSource HighestInputCountNetSiteDrawing { get; set; }

        public ImageSource EntryPathFitDrawing { get; set; }

        public ImageSource LengthFitDrawing { get; set; }

        public ImageSource LengthNetSiteDrawing { get; set; }

        public ImageSource BPMFitDrawing { get; set; }

        public ImageSource BPMNetSiteDrawing { get; set; }

        public ImageSource BPM1NetSiteDrawing { get; set; }

        public ImageSource FileViewerDrawing { get; set; }

        public ImageSource AssistFileViewerDrawing { get; set; }

        public ImageSource LevelTextValueFitDrawing { get; set; }

        public ImageSource TitleFitDrawing { get; set; }

        public ImageSource ArtistFitDrawing { get; set; }

        public ImageSource ModifiedDateFitDrawing { get; set; }

        public ImageSource HandledCountFitDrawing { get; set; }

        public ImageSource LatestDateFitDrawing { get; set; }

        public ImageSource CommentDrawing { get; set; }

        public ImageSource ConfigureDrawing { get; set; }

        public ImageSource AudioInputDrawing { get; set; }

        public ImageSource SaltDrawing { get; set; }

        public ImageSource DefaultEntryConfigureDrawing { get; set; }

        public DrawingItem? TotalNotesJudgmentDrawing { get; set; }

        public HandledDrawingItem?[] JudgmentDrawings { get; } = new HandledDrawingItem?[6];

        public HandledDrawingItem?[] NotifyDrawings { get; } = new HandledDrawingItem?[7];

        public Brush[] JudgmentPaints { get; } = new Brush[6];

        public ICanvasBrush[][] D2DJudgmentPaints { get; } = new ICanvasBrush[6][];

        public System.Windows.Media.Color SiteDateColor { get; set; }

        public System.Windows.Media.Color SiteEnterColor { get; set; }

        public System.Windows.Media.Color SiteQuitColor { get; set; }

        public System.Windows.Media.Color SiteHrefColor { get; set; }

        public System.Windows.Media.Color SiteTitleColor { get; set; }

        public System.Windows.Media.Color SiteArtistColor { get; set; }

        public System.Windows.Media.Color SiteGenreColor { get; set; }

        public System.Windows.Media.Color SiteStandColor { get; set; }

        public System.Windows.Media.Color FileColor { get; set; }

        public System.Windows.Media.Color EventNoteNameColor { get; set; }

        public System.Windows.Media.Color TitleColor { get; set; }

        public System.Windows.Media.Color ArtistColor { get; set; }

        public System.Windows.Media.Color FittedTextColor { get; set; }

        public System.Windows.Media.Color GenreColor { get; set; }

        public System.Windows.Media.Color WantLevelIDColor { get; set; }

        public System.Windows.Media.Color JudgmentStageColor { get; set; }

        public System.Windows.Media.Color TotalNotesColor { get; set; }

        public System.Windows.Media.Color HighestInputCountColor { get; set; }

        public System.Windows.Media.Color LengthColor { get; set; }

        public System.Windows.Media.Color BPMColor { get; set; }

        public System.Windows.Media.Color CommentDateColor { get; set; }

        public System.Windows.Media.Color CommentNameColor { get; set; }

        public System.Windows.Media.Color CommentPointColor { get; set; }

        public System.Windows.Media.Color CommentStandColor { get; set; }

        public System.Windows.Media.Color[] LevelColors { get; } = new System.Windows.Media.Color[6];

        public Brush[] LevelPaints { get; } = new Brush[6];

        public Color[] D2DLevelColors { get; } = new Color[6];

        public ICanvasBrush[][] D2DHitPointsPaints { get; } = new ICanvasBrush[8][];

        public Color[] HitPointsColor { get; } = new Color[8];

        public Brush[] HitPointsPaints { get; } = new Brush[8];

        public Color StatusHandlingColor { get; set; }

        public Brush StatusHandlingPaint { get; set; }

        public Color StatusPausedColor { get; set; }

        public Brush StatusPausedPaint { get; set; }

        public Color StatusLoadingNoteFileColor { get; set; }

        public Brush StatusLoadingNoteFilePaint { get; set; }

        public Color StatusLoadingDefaultEntryColor { get; set; }

        public Brush StatusLoadingDefaultEntryPaint { get; set; }

        public System.Windows.Media.Color[] QuitColors { get; } = new System.Windows.Media.Color[7];

        public Brush[] CommentViewPaints { get; } = new Brush[7];

        public Thickness NoteFileMargin { get; set; }

        public Thickness EntryViewTitleMargin { get; set; }

        public object[] SignInPoint { get; set; }

        public object[] ConfigurePoint { get; set; }

        public object[] CommentPoint { get; set; }

        public bool HasCommentPoint { get; set; }

        public object[] StopAutoPoint { get; set; }

        public object[] SaltAutoPoint { get; set; }

        public double[] CommentViewPoint { get; set; }

        public double[] EntryViewPoint { get; set; }

        public double[] InputNoteCountViewPoint { get; set; }

        public object[] AssistViewPoint { get; set; }

        public double HandledWallLength { get; set; }

        public double EntryItemHeight { get; set; }

        public double CommentItemHeight { get; set; }

        public double CommentItemAvatarHeight { get; set; }

        public object[] AutoModePoint { get; set; }

        public object[] NoteSaltModePoint { get; set; }

        public object[] FaintNoteModePoint { get; set; }

        public object[] JudgmentModePoint { get; set; }

        public object[] HitPointsModePoint { get; set; }

        public object[] NoteMobilityModePoint { get; set; }

        public object[] InputFavorModePoint { get; set; }

        public object[] LongNoteModePoint { get; set; }

        public object[] NoteModifyModePoint { get; set; }

        public object[] BPMModePoint { get; set; }

        public object[] WaveModePoint { get; set; }

        public object[] SetNoteModePoint { get; set; }

        public object[] LowestJudgmentConditionModePoint { get; set; }

        public object[] FilePoint { get; set; }

        public object[] FileContentsPoint { get; set; }

        public object[] FileViewerPoint { get; set; }

        public object[] AssistFileViewerPoint { get; set; }

        public object[] JudgmentStagePoint { get; set; }

        public object[] JudgmentStageContentsPoint { get; set; }

        public object[] TotalNotesPoint { get; set; }

        public object[] TotalNotesContentsPoint { get; set; }

        public object[] HighestInputCountPoint { get; set; }

        public object[] HighestInputCountContentsPoint { get; set; }

        public object[] LengthPoint { get; set; }

        public object[] LengthContentsPoint { get; set; }

        public object[] BPMPoint { get; set; }

        public object[] BPMContentsPoint { get; set; }

        public object[] InputModePoint { get; set; }

        public object[] InputModeContentsPoint { get; set; }

        public double[] StatusPoint { get; set; }

        public double[] StatusDefaultEntryPoint { get; set; }

        public double EventNoteNameFontLevel { get; set; }

        public double TitleFontLevel { get; set; }

        public double ArtistFontLevel { get; set; }

        public double FittedTextFontLevel { get; set; }

        public double GenreFontLevel { get; set; }

        public double LevelFontLevel { get; set; }

        public double WantLevelIDFontLevel { get; set; }

        public double EntryItemPositionFontLevel { get; set; }

        public double CommentDateFontLevel { get; set; }

        public double CommentViewPointFontLevel { get; set; }

        public double CommentAvatarNameFontLevel { get; set; }

        public double CommentStandFontLevel { get; set; }

        public float[] TitleQuitPoint { get; set; }

        public float[] ArtistQuitPoint { get; set; }

        public float[] GenreQuitPoint { get; set; }

        public float[] LevelQuitPoint { get; set; }

        public float[] WantLevelIDQuitPoint { get; set; }

        public float[] TotalNotesJudgmentQuitPoint { get; set; }

        public float[] TotalNotesJudgmentContentsQuitPoint { get; set; }

        public float[] HighestJudgmentQuitPoint { get; set; }

        public float[] HighestJudgmentContentsQuitPoint { get; set; }

        public float[] HigherJudgmentQuitPoint { get; set; }

        public float[] HigherJudgmentContentsQuitPoint { get; set; }

        public float[] HighJudgmentQuitPoint { get; set; }

        public float[] HighJudgmentContentsQuitPoint { get; set; }

        public float[] LowJudgmentQuitPoint { get; set; }

        public float[] LowJudgmentContentsQuitPoint { get; set; }

        public float[] LowerJudgmentQuitPoint { get; set; }

        public float[] LowerJudgmentContentsQuitPoint { get; set; }

        public float[] LowestJudgmentQuitPoint { get; set; }

        public float[] LowestJudgmentContentsQuitPoint { get; set; }

        public float[] AutoModeQuitPoint { get; set; }

        public float[] NoteSaltModeQuitPoint { get; set; }

        public float[] FaintNoteModeQuitPoint { get; set; }

        public float[] JudgmentModeQuitPoint { get; set; }

        public float[] HitPointsModeQuitPoint { get; set; }

        public float[] NoteMobilityModeQuitPoint { get; set; }

        public float[] InputFavorModeQuitPoint { get; set; }

        public float[] LongNoteModeQuitPoint { get; set; }

        public float[] NoteModifyModeQuitPoint { get; set; }

        public float[] BPMModeQuitPoint { get; set; }

        public float[] WaveModeQuitPoint { get; set; }

        public float[] SetNoteModeQuitPoint { get; set; }

        public float[] LowestJudgmentConditionModeQuitPoint { get; set; }

        public float[] JudgmentStageQuitPoint { get; set; }

        public float[] JudgmentStageContentsQuitPoint { get; set; }

        public float[] HighestInputCountQuitPoint { get; set; }

        public float[] HighestInputCountContentsQuitPoint { get; set; }

        public float[] LengthQuitPoint { get; set; }

        public float[] LengthContentsQuitPoint { get; set; }

        public float[] BPMQuitPoint { get; set; }

        public float[] BPMContentsQuitPoint { get; set; }

        public float[] InputModeQuitPoint { get; set; }

        public float[] InputModeContentsQuitPoint { get; set; }

        public float[] QuitDrawingPoint { get; set; }

        public float[] JudgmentMeterViewPoint { get; set; }

        public float[] StatusViewPoint { get; set; }

        public float[] ViewCommentPoint { get; set; }

        public float[] HandleUndoPoint { get; set; }

        public float[] QuitMove0Point { get; set; }

        public float[] QuitMove1Point { get; set; }

        public float[] StandQuitPoint { get; set; }

        public float[] PointQuitPoint { get; set; }

        public float[] BandQuitPoint { get; set; }

        public float[] StandContentsQuitPoint { get; set; }

        public float[] PointContentsQuitPoint { get; set; }

        public float[] BandContentsQuitPoint { get; set; }

        public float[] CommentPlace0Point { get; } = new float[4];

        public float[] CommentPlace1Point { get; } = new float[4];

        public HandledDrawingItem?[][] ModeComponentDrawings { get; } = new HandledDrawingItem?[13][];

        public DrawingItem? ViewCommentDrawing { get; set; }

        public DrawingItem? HandleUndoDrawing { get; set; }

        public DrawingItem? QuitMove0Drawing { get; set; }

        public DrawingItem? QuitMove1Drawing { get; set; }

        public DrawingItem? StandDrawing { get; set; }

        public DrawingItem? HighestBandDrawing { get; set; }

        public DrawingItem? PointDrawing { get; set; }

        public DrawingItem? NewStandDrawing { get; set; }

        public DrawingItem?[] NetPositionDrawings { get; } = new DrawingItem?[8];

        public Color TitleQuitColor { get; set; }

        public Color ArtistQuitColor { get; set; }

        public Color GenreQuitColor { get; set; }

        public Color WantLevelIDQuitColor { get; set; }

        public Color JudgmentStageQuitColor { get; set; }

        public Color HighestInputCountQuitColor { get; set; }

        public Color LengthQuitColor { get; set; }

        public Color BPMQuitColor { get; set; }

        public Color TotalNotesJudgmentQuitColor { get; set; }

        public Color[] JudgmentQuitColors { get; } = new Color[6];

        public Color[] JudgmentColors { get; } = new Color[6];

        public Color StandQuitColor { get; set; }

        public Color PointQuitColor { get; set; }

        public Color BandQuitColor { get; set; }

        public Color CommentPlaceColor { get; set; }

        public Color StandStatusViewColor { get; set; }

        public Color PointStatusViewColor { get; set; }

        public Color BandStatusViewColor { get; set; }

        public Color HitPointsStatusViewColor { get; set; }

        public CanvasTextFormat TitleQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat ArtistQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat GenreQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LevelQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat WantLevelIDQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat TotalNotesQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat HighestJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat HigherJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat HighJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LowJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LowerJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LowestJudgmentQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat JudgmentStageQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat HighestInputCountQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat LengthQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat BPMQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat StandQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat PointQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat BandQuitFont { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat CommentPlace0Font { get; } = DrawingSystem.Instance.GetFont();

        public CanvasTextFormat CommentPlace1Font { get; } = DrawingSystem.Instance.GetFont();

        public BasePaintProperty[] PaintPropertyValues { get; } = new BasePaintProperty[HighestPaintPropertyID];

        public ConcurrentDictionary<EventItem, List<BasePaintProperty>> EventItems { get; } = new();

        public FadingProperty[][] FadingPropertyValues { get; } = new FadingProperty[4][];

        public MediaModifier MediaModifierValue { get; }

        public bool IsCounterWave => false;

        public double LoopingCounter { get; }

        public bool IsPausing => false;

        public double AudioMultiplier => 1.0;

        public BaseUI()
        {
            Init();
        }

        public void InitEvents()
        {
            foreach (var eventItem in EventItems)
            {
                foreach (var paintPropertyValue in eventItem.Value)
                {
                    paintPropertyValue.DrawingFrame = -1;
                }
            }
        }

        public void HandleEvent(EventItem eventItem)
        {
            if (EventItems.TryGetValue(eventItem, out var paintPropertyValues))
            {
                foreach (var paintPropertyValue in paintPropertyValues)
                {
                    paintPropertyValue.DrawingMillis = 0.0;
                    paintPropertyValue.DrawingFrame = 0;
                }
            }
        }

        public void SetFontFamily()
        {
            DrawingSystem.Instance.SetFontFamily(TitleQuitFont);
            DrawingSystem.Instance.SetFontFamily(ArtistQuitFont);
            DrawingSystem.Instance.SetFontFamily(GenreQuitFont);
            DrawingSystem.Instance.SetFontFamily(LevelQuitFont);
            DrawingSystem.Instance.SetFontFamily(WantLevelIDQuitFont);
            DrawingSystem.Instance.SetFontFamily(TotalNotesQuitFont);
            DrawingSystem.Instance.SetFontFamily(HighestJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(HigherJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(HighJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(LowJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(LowerJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(LowestJudgmentQuitFont);
            DrawingSystem.Instance.SetFontFamily(JudgmentStageQuitFont);
            DrawingSystem.Instance.SetFontFamily(HighestInputCountQuitFont);
            DrawingSystem.Instance.SetFontFamily(LengthQuitFont);
            DrawingSystem.Instance.SetFontFamily(BPMQuitFont);
            DrawingSystem.Instance.SetFontFamily(StandQuitFont);
            DrawingSystem.Instance.SetFontFamily(PointQuitFont);
            DrawingSystem.Instance.SetFontFamily(BandQuitFont);
            DrawingSystem.Instance.SetFontFamily(CommentPlace0Font);
            DrawingSystem.Instance.SetFontFamily(CommentPlace1Font);
        }

        void LoadBaseUIImpl(UIItem src, UIItem target)
        {
            #region COMPATIBLE
            Compatible.Compatible.BaseUI(QwilightComponent.UIEntryPath, target.GetYamlFilePath(), target.YamlName, target.UIEntry);
            #endregion

            var mainViewModel = ViewModels.Instance.MainValue;
            try
            {
                Init();
                string zipName;

                var lsCaller = new Script();

                var parallelItems = new ConcurrentBag<Action>();

                var ys = new YamlStream();
                using (var sr = File.OpenText(target.GetYamlFilePath()))
                {
                    ys.Load(sr);

                    var valueNode = ys.Documents[0].RootNode;
                    var formatNode = valueNode[new YamlScalarNode("format")];
                    var paintNode = valueNode[new YamlScalarNode("paint")];
                    var pointNode = valueNode[new YamlScalarNode("point")];
                    var fontNode = valueNode[new YamlScalarNode("font")];
                    (valueNode as YamlMappingNode).Children.TryGetValue(new YamlScalarNode("func"), out var funcNode);

                    zipName = $"@{Utility.GetText(formatNode, "zip")}";

                    XamlBaseUIConfigures = Enumerable.Range(0, HighestBaseUIConfigure).Select(i =>
                    {
                        var configures = (Utility.GetText(funcNode, $"configure-{i}-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(funcNode, $"configure-{i}"))?.Split(',')?.Select(configure => configure.Trim())?.ToArray();
                        if (configures != null)
                        {
                            LoadedConfigures[i] = Configure.Instance.BaseUIConfigureValue.UIConfigures[i] ??= configures.FirstOrDefault();
                            return new XamlBaseUIConfigure
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

                    var luaFilePath = Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, Path.ChangeExtension($"@{Utility.GetText(formatNode, "lua")}", "lua"));
                    if (File.Exists(luaFilePath))
                    {
                        lsCaller.DoString(File.ReadAllText(luaFilePath, Encoding.UTF8));
                    }

                    DefaultLength = GetCalledValue(formatNode, "defaultLength", "1280");
                    DefaultHeight = GetCalledValue(formatNode, "defaultHeight", "720");

                    SiteDateColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteDate", nameof(Colors.White))).GetColor());
                    SiteEnterColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteEnter", nameof(Colors.White))).GetColor());
                    SiteQuitColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteQuit", nameof(Colors.White))).GetColor());
                    SiteHrefColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteHref", nameof(Colors.White))).GetColor());
                    SiteTitleColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteTitle", nameof(Colors.White))).GetColor());
                    SiteArtistColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteArtist", nameof(Colors.White))).GetColor());
                    SiteGenreColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteGenre", nameof(Colors.White))).GetColor());
                    SiteStandColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "siteStand", nameof(Colors.White))).GetColor());

                    EventNoteNameColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "eventNoteName", nameof(Colors.White))).GetColor());
                    TitleColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "title", nameof(Colors.White))).GetColor());
                    ArtistColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "artist", nameof(Colors.White))).GetColor());
                    GenreColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "genre", nameof(Colors.White))).GetColor());
                    WantLevelIDColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "wantLevelID", nameof(Colors.White))).GetColor());
                    FittedTextColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "fittedText", nameof(Colors.White))).GetColor());

                    FileColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "file", nameof(Colors.White))).GetColor());
                    JudgmentStageColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "judgmentStage", nameof(Colors.White))).GetColor());
                    TotalNotesColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "totalNotes", nameof(Colors.White))).GetColor());
                    HighestInputCountColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "highestInputCount", nameof(Colors.White))).GetColor());
                    LengthColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "length", nameof(Colors.White))).GetColor());
                    BPMColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "bpm", nameof(Colors.White))).GetColor());

                    CommentDateColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "commentDate", nameof(Colors.White))).GetColor());
                    CommentNameColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "commentName", nameof(Colors.White))).GetColor());
                    CommentPointColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "commentPoint", nameof(Colors.White))).GetColor());
                    CommentStandColor = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "commentStand", nameof(Colors.White))).GetColor());

                    foreach (var pair in new[] {
                        ("level0", (int)BaseNoteFile.Level.Level0),
                        ("level1", (int)BaseNoteFile.Level.Level1),
                        ("level2", (int)BaseNoteFile.Level.Level2),
                        ("level3", (int)BaseNoteFile.Level.Level3),
                        ("level4", (int)BaseNoteFile.Level.Level4),
                        ("level5", (int)BaseNoteFile.Level.Level5)
                    })
                    {
                        LevelColors[pair.Item2] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, pair.Item1, nameof(Colors.White))).GetColor());
                        LevelPaints[pair.Item2] = DrawingSystem.Instance.GetDefaultPaint(Utility.ModifyColor(LevelColors[pair.Item2]));
                        D2DLevelColors[pair.Item2] = Utility.ModifyColor(LevelColors[pair.Item2]);
                    }

                    TitleQuitColor = GetCalledText(Utility.GetText(paintNode, "titleQuit", nameof(Colors.White))).GetColor();
                    ArtistQuitColor = GetCalledText(Utility.GetText(paintNode, "artistQuit", nameof(Colors.White))).GetColor();
                    GenreQuitColor = GetCalledText(Utility.GetText(paintNode, "genreQuit", nameof(Colors.White))).GetColor();
                    WantLevelIDQuitColor = GetCalledText(Utility.GetText(paintNode, "wantLevelIDQuit", nameof(Colors.White))).GetColor();
                    JudgmentStageQuitColor = GetCalledText(Utility.GetText(paintNode, "judgmentStageQuit", nameof(Colors.White))).GetColor();
                    HighestInputCountQuitColor = GetCalledText(Utility.GetText(paintNode, "highestInputCountQuit", nameof(Colors.White))).GetColor();
                    LengthQuitColor = GetCalledText(Utility.GetText(paintNode, "lengthQuit", nameof(Colors.White))).GetColor();
                    BPMQuitColor = GetCalledText(Utility.GetText(paintNode, "bpmQuit", nameof(Colors.White))).GetColor();
                    StandQuitColor = GetCalledText(Utility.GetText(paintNode, "standQuit", nameof(Colors.White))).GetColor();
                    PointQuitColor = GetCalledText(Utility.GetText(paintNode, "pointQuit", nameof(Colors.White))).GetColor();
                    BandQuitColor = GetCalledText(Utility.GetText(paintNode, "bandQuit", nameof(Colors.White))).GetColor();
                    CommentPlaceColor = GetCalledText(Utility.GetText(paintNode, "commentPlace", nameof(Colors.White))).GetColor();

                    foreach (var pair in new[] {
                        ("lowestHitPoints", ModeComponent.HitPointsMode.Lowest),
                        ("lowerHitPoints", ModeComponent.HitPointsMode.Lower),
                        ("defaultHitPoints", ModeComponent.HitPointsMode.Default),
                        ("higherHitPoints", ModeComponent.HitPointsMode.Higher),
                        ("highestHitPoints", ModeComponent.HitPointsMode.Highest),
                        ("failedHitPoints", ModeComponent.HitPointsMode.Failed),
                        ("favorHitPoints", ModeComponent.HitPointsMode.Favor),
                        ("testHitPoints", ModeComponent.HitPointsMode.Test)
                    })
                    {
                        parallelItems.Add(() =>
                        {
                            HitPointsColor[(int)pair.Item2] = GetCalledText(Utility.GetText(paintNode, pair.Item1, nameof(Colors.Red))).GetColor();
                            HitPointsPaints[(int)pair.Item2] = DrawingSystem.Instance.GetDefaultPaint(HitPointsColor[(int)pair.Item2]);
                            DrawingSystem.Instance.SetFaintPaints(this, D2DHitPointsPaints[(int)pair.Item2], HitPointsColor[(int)pair.Item2]);
                        });
                    }

                    StandStatusViewColor = GetCalledText(Utility.GetText(paintNode, "standStatusView", nameof(Colors.White))).GetColor();
                    PointStatusViewColor = GetCalledText(Utility.GetText(paintNode, "pointStatusView", nameof(Colors.White))).GetColor();
                    BandStatusViewColor = GetCalledText(Utility.GetText(paintNode, "bandStatusView", nameof(Colors.White))).GetColor();
                    HitPointsStatusViewColor = GetCalledText(Utility.GetText(paintNode, "hitPointsStatusView", nameof(Colors.White))).GetColor();

                    foreach (var pair in new[] {
                        ("highestJudgmentV2", Component.Judged.Highest),
                        ("higherJudgmentV2", Component.Judged.Higher),
                        ("highJudgmentV2", Component.Judged.High),
                        ("lowJudgmentV2", Component.Judged.Low),
                        ("lowerJudgmentV2", Component.Judged.Lower),
                        ("lowestJudgmentV2", Component.Judged.Lowest)
                    })
                    {
                        var inputMode = (int)(Component.InputMode)pair.Item2;
                        JudgmentColors[(int)inputMode] = GetCalledText(Utility.GetText(paintNode, pair.Item1, nameof(Colors.White))).GetColor();
                        JudgmentPaints[(int)inputMode] = DrawingSystem.Instance.GetDefaultPaint(JudgmentColors[(int)inputMode]);
                        parallelItems.Add(() => DrawingSystem.Instance.SetFaintPaints(this, D2DJudgmentPaints[(int)inputMode], JudgmentColors[(int)inputMode]));
                    }

                    TotalNotesJudgmentQuitColor = GetCalledText(Utility.GetText(paintNode, "totalNotesJudgmentQuit", nameof(Colors.White))).GetColor();
                    foreach (var pair in new[] {
                        ("highestJudgmentQuit", Component.Judged.Highest),
                        ("higherJudgmentQuit", Component.Judged.Higher),
                        ("highJudgmentQuit", Component.Judged.High),
                        ("lowJudgmentQuit", Component.Judged.Low),
                        ("lowerJudgmentQuit", Component.Judged.Lower),
                        ("lowestJudgmentQuit", Component.Judged.Lowest)
                    })
                    {
                        JudgmentQuitColors[(int)pair.Item2] = GetCalledText(Utility.GetText(paintNode, pair.Item1, nameof(Colors.White))).GetColor();
                    }

                    StatusHandlingColor = GetCalledText(Utility.GetText(paintNode, "statusHandling", "#7F008000")).GetColor();
                    StatusHandlingPaint = DrawingSystem.Instance.GetDefaultPaint(StatusHandlingColor);

                    StatusPausedColor = GetCalledText(Utility.GetText(paintNode, "statusPaused", "#7FFFFF00")).GetColor();
                    StatusPausedPaint = DrawingSystem.Instance.GetDefaultPaint(StatusPausedColor);

                    StatusLoadingNoteFileColor = GetCalledText(Utility.GetText(paintNode, "statusLoadingNoteFile", "#7FFF0000")).GetColor();
                    StatusLoadingNoteFilePaint = DrawingSystem.Instance.GetDefaultPaint(StatusLoadingNoteFileColor);

                    StatusLoadingDefaultEntryColor = GetCalledText(Utility.GetText(paintNode, "statusLoadingDefaultEntry", "#7F0000FF")).GetColor();
                    StatusLoadingDefaultEntryPaint = DrawingSystem.Instance.GetDefaultPaint(StatusLoadingDefaultEntryColor);

                    QuitColors[(int)DefaultCompute.QuitStatus.SPlus] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit0")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.S] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit1")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.APlus] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit2")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.A] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit3")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.B] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit4")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.C] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit5")).GetColor());
                    QuitColors[(int)DefaultCompute.QuitStatus.D] = Utility.ModifyColor(GetCalledText(Utility.GetText(paintNode, "quit6")).GetColor());

                    for (var i = (int)DefaultCompute.QuitStatus.D; i >= (int)DefaultCompute.QuitStatus.SPlus; --i)
                    {
                        CommentViewPaints[i] = DrawingSystem.Instance.GetDefaultPaint(Utility.ModifyColor(QuitColors[i]));
                    }

                    NoteFileMargin = new(GetCalledValue(pointNode, "noteFileMargin", "31.5"), 1.0, 0.0, 1.0);
                    var entryViewTitleMargin = GetLayerPoint(pointNode, "entryViewTitleMargin", new[] { 58.0, 0.0, 0.0, 0.0 });
                    EntryViewTitleMargin = new(entryViewTitleMargin[0], entryViewTitleMargin[1], entryViewTitleMargin[2], entryViewTitleMargin[3]);

                    SignInPoint = GetDrawingPoint(pointNode, "signIn");
                    ConfigurePoint = GetDrawingPoint(pointNode, "configure");
                    CommentPoint = GetDrawingPoint(pointNode, "comment", new object[] { 0.0, 0.0, 0.0, 0.0, HorizontalAlignment.Center, VerticalAlignment.Center, Stretch.Uniform });
                    HasCommentPoint = (double)CommentPoint[2] > 0.0 && (double)CommentPoint[3] > 0.0;

                    SaltAutoPoint = GetDrawingPoint(pointNode, "saltAuto");
                    StopAutoPoint = GetDrawingPoint(pointNode, "stopAuto");

                    HandledWallLength = GetCalledValue(pointNode, "handledWallLength", "10");
                    EntryItemHeight = GetCalledValue(pointNode, "entryItemHeight", "60");
                    CommentItemHeight = GetCalledValue(pointNode, "commentItemHeight", "48");
                    CommentItemAvatarHeight = CommentItemHeight + 2 * CommentItemHeight * Levels.EdgeXY + 2 * 1.0;

                    CommentViewPoint = GetLayerPoint(pointNode, "commentView");
                    EntryViewPoint = GetLayerPoint(pointNode, "entryView");
                    InputNoteCountViewPoint = GetLayerPoint(pointNode, "inputNoteCountView", new double[] { 0.0, 0.0, 0.0, 0.0 });
                    AssistViewPoint = GetPanelPoint(pointNode, "assistView", new object[] { 0.0, 0.0, 0.0, 0.0, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, Colors.Transparent, Colors.Transparent });

                    AutoModePoint = GetDrawingPoint(pointNode, "autoMode");
                    NoteSaltModePoint = GetDrawingPoint(pointNode, "noteSaltMode");
                    FaintNoteModePoint = GetDrawingPoint(pointNode, "faintNoteMode");
                    JudgmentModePoint = GetDrawingPoint(pointNode, "judgmentMode");
                    HitPointsModePoint = GetDrawingPoint(pointNode, "hitPointsMode");
                    NoteMobilityModePoint = GetDrawingPoint(pointNode, "noteMobilityMode");
                    InputFavorModePoint = GetDrawingPoint(pointNode, "inputFavorMode");
                    LongNoteModePoint = GetDrawingPoint(pointNode, "longNoteMode");
                    NoteModifyModePoint = GetDrawingPoint(pointNode, "noteModifyMode");
                    BPMModePoint = GetDrawingPoint(pointNode, "bpmMode");
                    WaveModePoint = GetDrawingPoint(pointNode, "waveMode");
                    SetNoteModePoint = GetDrawingPoint(pointNode, "setNoteMode");
                    LowestJudgmentConditionModePoint = GetDrawingPoint(pointNode, "lowestJudgmentConditionMode");

                    FilePoint = GetDrawingPoint(pointNode, "file");
                    FileContentsPoint = GetTextPoint(pointNode, "fileContents");
                    FileViewerPoint = GetDrawingPoint(pointNode, "fileViewer");
                    AssistFileViewerPoint = GetDrawingPoint(pointNode, "assistFileViewer");
                    JudgmentStagePoint = GetDrawingPoint(pointNode, "judgmentStage");
                    JudgmentStageContentsPoint = GetTextPoint(pointNode, "judgmentStageContents");
                    JudgmentStagePoint = GetDrawingPoint(pointNode, "judgmentStage");
                    JudgmentStageContentsPoint = GetTextPoint(pointNode, "judgmentStageContents");
                    TotalNotesPoint = GetDrawingPoint(pointNode, "totalNotes");
                    TotalNotesContentsPoint = GetTextPoint(pointNode, "totalNotesContents");
                    HighestInputCountPoint = GetDrawingPoint(pointNode, "highestInputCount");
                    HighestInputCountContentsPoint = GetTextPoint(pointNode, "highestInputCountContents");
                    LengthPoint = GetDrawingPoint(pointNode, "length");
                    LengthContentsPoint = GetTextPoint(pointNode, "lengthContents");
                    BPMPoint = GetDrawingPoint(pointNode, "bpm");
                    BPMContentsPoint = GetTextPoint(pointNode, "bpmContents");
                    InputModePoint = GetDrawingPoint(pointNode, "inputMode");
                    InputModeContentsPoint = GetDrawingPoint(pointNode, "inputModeContents");
                    StatusPoint = GetLayerPoint(pointNode, "status");
                    StatusDefaultEntryPoint = GetLayerPoint(pointNode, "statusDefaultEntry");

                    EventNoteNameFontLevel = GetCalledValue(fontNode, "eventNoteNameLevel", Levels.FontLevel1.ToString());
                    TitleFontLevel = GetCalledValue(fontNode, "titleLevel", Levels.FontLevel1.ToString());
                    ArtistFontLevel = GetCalledValue(fontNode, "artistLevel", Levels.FontLevel0.ToString());
                    FittedTextFontLevel = GetCalledValue(fontNode, "fittedTextLevel", Levels.FontLevel0.ToString());
                    GenreFontLevel = GetCalledValue(fontNode, "genreLevel", Levels.FontLevel0.ToString());
                    LevelFontLevel = GetCalledValue(fontNode, "levelLevel", Levels.FontLevel1.ToString());
                    WantLevelIDFontLevel = GetCalledValue(fontNode, "wantLevelIDLevel", Levels.FontLevel1.ToString());
                    EntryItemPositionFontLevel = GetCalledValue(fontNode, "entryItemPositionLevel", Levels.FontLevel0.ToString());

                    CommentDateFontLevel = GetCalledValue(fontNode, "commentDateLevel", Levels.FontLevel0.ToString());
                    CommentViewPointFontLevel = GetCalledValue(fontNode, "commentPointLevel", Levels.FontLevel0.ToString());
                    CommentAvatarNameFontLevel = GetCalledValue(fontNode, "commentAvatarNameLevel", Levels.FontLevel1.ToString());
                    CommentStandFontLevel = GetCalledValue(fontNode, "commentStandLevel", Levels.FontLevel1.ToString());

                    TitleQuitPoint = GetQuitPoint(pointNode, "titleQuit");
                    ArtistQuitPoint = GetQuitPoint(pointNode, "artistQuit");
                    GenreQuitPoint = GetQuitPoint(pointNode, "genreQuit");
                    LevelQuitPoint = GetQuitPoint(pointNode, "levelQuit");
                    WantLevelIDQuitPoint = GetQuitPoint(pointNode, "wantLevelIDQuit");

                    TotalNotesJudgmentQuitPoint = GetQuitPoint(pointNode, "totalNotesJudgmentQuit");
                    TotalNotesJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "totalNotesJudgmentContentsQuit");
                    HighestJudgmentQuitPoint = GetQuitPoint(pointNode, "highestJudgmentQuit");
                    HighestJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "highestJudgmentContentsQuit");
                    HigherJudgmentQuitPoint = GetQuitPoint(pointNode, "higherJudgmentQuit");
                    HigherJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "higherJudgmentContentsQuit");
                    HighJudgmentQuitPoint = GetQuitPoint(pointNode, "highJudgmentQuit");
                    HighJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "highJudgmentContentsQuit");
                    LowJudgmentQuitPoint = GetQuitPoint(pointNode, "lowJudgmentQuit");
                    LowJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "lowJudgmentContentsQuit");
                    LowerJudgmentQuitPoint = GetQuitPoint(pointNode, "lowerJudgmentQuit");
                    LowerJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "lowerJudgmentContentsQuit");
                    LowestJudgmentQuitPoint = GetQuitPoint(pointNode, "lowestJudgmentQuit");
                    LowestJudgmentContentsQuitPoint = GetQuitPoint(pointNode, "lowestJudgmentContentsQuit");

                    AutoModeQuitPoint = GetQuitPoint(pointNode, "autoModeQuit");
                    NoteSaltModeQuitPoint = GetQuitPoint(pointNode, "noteSaltModeQuit");
                    FaintNoteModeQuitPoint = GetQuitPoint(pointNode, "faintNoteModeQuit");
                    JudgmentModeQuitPoint = GetQuitPoint(pointNode, "judgmentModeQuit");
                    HitPointsModeQuitPoint = GetQuitPoint(pointNode, "hitPointsModeQuit");
                    NoteMobilityModeQuitPoint = GetQuitPoint(pointNode, "noteMobilityModeQuit");
                    InputFavorModeQuitPoint = GetQuitPoint(pointNode, "inputFavorModeQuit");
                    LongNoteModeQuitPoint = GetQuitPoint(pointNode, "longNoteModeQuit");
                    NoteModifyModeQuitPoint = GetQuitPoint(pointNode, "noteModifyModeQuit");
                    BPMModeQuitPoint = GetQuitPoint(pointNode, "bpmModeQuit");
                    WaveModeQuitPoint = GetQuitPoint(pointNode, "waveModeQuit");
                    SetNoteModeQuitPoint = GetQuitPoint(pointNode, "setNoteModeQuit");
                    LowestJudgmentConditionModeQuitPoint = GetQuitPoint(pointNode, "lowestJudgmentConditionModeQuit");

                    JudgmentStageQuitPoint = GetQuitPoint(pointNode, "judgmentStageQuit");
                    JudgmentStageContentsQuitPoint = GetQuitPoint(pointNode, "judgmentStageContentsQuit");
                    HighestInputCountQuitPoint = GetQuitPoint(pointNode, "highestInputCountQuit");
                    HighestInputCountContentsQuitPoint = GetQuitPoint(pointNode, "highestInputCountContentsQuit");
                    LengthQuitPoint = GetQuitPoint(pointNode, "lengthQuit");
                    LengthContentsQuitPoint = GetQuitPoint(pointNode, "lengthContentsQuit");
                    BPMQuitPoint = GetQuitPoint(pointNode, "bpmQuit");
                    BPMContentsQuitPoint = GetQuitPoint(pointNode, "bpmContentsQuit");
                    InputModeQuitPoint = GetQuitPoint(pointNode, "inputModeQuit");
                    InputModeContentsQuitPoint = GetQuitPoint(pointNode, "inputModeContentsQuit");

                    QuitDrawingPoint = GetQuitPoint(pointNode, "quitDrawingV2");
                    JudgmentMeterViewPoint = GetQuitPoint(pointNode, "judgmentMeterView");
                    StatusViewPoint = GetQuitPoint(pointNode, "statusView");

                    ViewCommentPoint = GetQuitPoint(pointNode, "viewComment");
                    HandleUndoPoint = GetQuitPoint(pointNode, "handleUndo");

                    QuitMove0Point = GetQuitPoint(pointNode, "quitMove0");
                    QuitMove1Point = GetQuitPoint(pointNode, "quitMove1");

                    StandQuitPoint = GetQuitPoint(pointNode, "standQuit");
                    PointQuitPoint = GetQuitPoint(pointNode, "pointQuit");
                    BandQuitPoint = GetQuitPoint(pointNode, "bandQuit");
                    StandContentsQuitPoint = GetQuitPoint(pointNode, "standContentsQuit");
                    PointContentsQuitPoint = GetQuitPoint(pointNode, "pointContentsQuit");
                    BandContentsQuitPoint = GetQuitPoint(pointNode, "bandContentsQuit");

                    var commentPlacePoint = GetQuitPoint(pointNode, "commentPlace", new float[] { 0F, 0F, 0F, 0F, Levels.FontLevel1Float32 });
                    CommentPlace0Point[0] = commentPlacePoint[0];
                    CommentPlace0Point[1] = commentPlacePoint[1];
                    CommentPlace0Point[2] = commentPlacePoint[2] / 2;
                    CommentPlace0Point[3] = commentPlacePoint[3];
                    DrawingSystem.Instance.SetFontLevel(CommentPlace0Font, (float)commentPlacePoint[4]);
                    DrawingSystem.Instance.SetFontSystem(CommentPlace0Font, (int)CanvasHorizontalAlignment.Left, (int)CanvasVerticalAlignment.Bottom);
                    CommentPlace1Point[0] = CommentPlace0Point[0] + CommentPlace0Point[2];
                    CommentPlace1Point[1] = CommentPlace0Point[1];
                    CommentPlace1Point[2] = CommentPlace0Point[2];
                    CommentPlace1Point[3] = CommentPlace0Point[3];
                    DrawingSystem.Instance.SetFontLevel(CommentPlace1Font, (float)commentPlacePoint[4] / 2);
                    DrawingSystem.Instance.SetFontSystem(CommentPlace1Font, (int)CanvasHorizontalAlignment.Right, (int)CanvasVerticalAlignment.Bottom);

                    SetQuitFont(TitleQuitPoint, TitleQuitFont);
                    SetQuitFont(ArtistQuitPoint, ArtistQuitFont);
                    SetQuitFont(GenreQuitPoint, GenreQuitFont);
                    SetQuitFont(LevelQuitPoint, LevelQuitFont);
                    SetQuitFont(WantLevelIDQuitPoint, WantLevelIDQuitFont);

                    SetQuitFont(JudgmentStageContentsQuitPoint, JudgmentStageQuitFont);
                    SetQuitFont(HighestInputCountContentsQuitPoint, HighestInputCountQuitFont);
                    SetQuitFont(LengthContentsQuitPoint, LengthQuitFont);
                    SetQuitFont(BPMContentsQuitPoint, BPMQuitFont);

                    SetQuitFont(TotalNotesJudgmentContentsQuitPoint, TotalNotesQuitFont);
                    SetQuitFont(HighestJudgmentContentsQuitPoint, HighestJudgmentQuitFont);
                    SetQuitFont(HigherJudgmentContentsQuitPoint, HigherJudgmentQuitFont);
                    SetQuitFont(HighJudgmentContentsQuitPoint, HighJudgmentQuitFont);
                    SetQuitFont(LowJudgmentContentsQuitPoint, LowJudgmentQuitFont);
                    SetQuitFont(LowerJudgmentContentsQuitPoint, LowerJudgmentQuitFont);
                    SetQuitFont(LowestJudgmentContentsQuitPoint, LowestJudgmentQuitFont);

                    SetQuitFont(StandContentsQuitPoint, StandQuitFont);
                    SetQuitFont(PointContentsQuitPoint, PointQuitFont);
                    SetQuitFont(BandContentsQuitPoint, BandQuitFont);

                    static void SetQuitFont(float[] point, CanvasTextFormat font)
                    {
                        DrawingSystem.Instance.SetFontLevel(font, (float)point[4]);
                        DrawingSystem.Instance.SetFontSystem(font, (int)point[5], (int)point[6]);
                    }

                    for (var i = PaintPropertyValues.Length - 1; i >= 0; --i)
                    {
                        PaintPropertyValues[i]?.Dispose();
                        var text = Utility.GetText(pointNode, $"paintProperty{i}");
                        if (!string.IsNullOrEmpty(text))
                        {
                            var data = text.Split(",").Select(value => GetCalledText(value.Trim())).ToArray();
                            var frame = Utility.ToInt32(data[4]);
                            var drawingVariety = Utility.ToInt32(data[7]);
                            var paintPropertyValue = new BasePaintProperty
                            {
                                PaintBound = new(Utility.ToFloat64(data[0]), Utility.ToFloat64(data[1]), Utility.ToFloat64(data[2]), Utility.ToFloat64(data[3])),
                                HandledDrawingItems = new HandledDrawingItem?[frame],
                                Frame = frame,
                                Framerate = Utility.ToFloat64(data[5]),
                                Layer = Utility.ToInt32(data[6]),
                                DrawingVariety = drawingVariety,
                                Etc = data.ElementAtOrDefault(8),
                                Mode = Utility.ToInt32(data.ElementAtOrDefault(9) ?? "0")
                            };
                            switch (paintPropertyValue.DrawingVariety)
                            {
                                case 3:
                                case 4:
                                case 5:
                                case 7:
                                case 13:
                                    var etcColor = paintPropertyValue.Etc.GetColor();
                                    paintPropertyValue.EtcPaint = DrawingSystem.Instance.GetDefaultPaint(etcColor);
                                    paintPropertyValue.EtcColor = etcColor;
                                    break;
                            }
                            switch (paintPropertyValue.DrawingVariety)
                            {
                                case 7:
                                case 12:
                                    paintPropertyValue.Font = DrawingSystem.Instance.GetFont();
                                    DrawingSystem.Instance.SetFontFamily(paintPropertyValue.Font);
                                    DrawingSystem.Instance.SetFontLevel(paintPropertyValue.Font, (float)paintPropertyValue.Framerate);
                                    break;
                            }
                            if (drawingVariety >= 100)
                            {
                                paintPropertyValue.DrawingFrame = -1;
                                EventItems.Into((EventItem)drawingVariety, paintPropertyValue);
                            }
                            PaintPropertyValues[i] = paintPropertyValue;
                        }
                    }

                    for (var i = FadingPropertyValues.Length - 1; i >= 0; --i)
                    {
                        for (var j = FadingPropertyValues[i].Length - 1; j >= 0; --j)
                        {
                            var text = Utility.GetText(pointNode, $"fadingProperty{i}{j}");
                            if (!string.IsNullOrEmpty(text))
                            {
                                var data = text.Split(",").Select(value => GetCalledText(value.Trim())).ToArray();
                                var frame = Utility.ToInt32(data[0]);
                                var framerate = Utility.ToFloat64(data[1]);
                                FadingPropertyValues[i][j] = new()
                                {
                                    HandledDrawingItems = new HandledDrawingItem?[frame],
                                    Frame = frame,
                                    Framerate = framerate,
                                    Millis = 1000.0 * frame / framerate,
                                    DrawingStatus = Utility.ToFloat64(data[2])
                                };
                            }
                        }
                    }

                    object[] GetDrawingPoint(YamlNode yamlNode, string target, object[] defaultValues = null)
                    {
                        var text = Utility.GetText(yamlNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            return defaultValues;
                        }
                        else
                        {
                            return text.Split(",").Select(value => GetCalledText(value.Trim())).Select((data, i) => i switch
                            {
                                4 => Utility.ToInt32(data) switch
                                {
                                    0 => HorizontalAlignment.Left,
                                    1 => HorizontalAlignment.Center,
                                    2 => HorizontalAlignment.Right,
                                    _ => throw new ArgumentException(target),
                                },
                                5 => Utility.ToInt32(data) switch
                                {
                                    0 => VerticalAlignment.Top,
                                    1 => VerticalAlignment.Center,
                                    2 => VerticalAlignment.Bottom,
                                    _ => throw new ArgumentException(target),
                                },
                                6 => Utility.ToInt32(data) switch
                                {
                                    0 => Stretch.Uniform,
                                    1 => Stretch.Fill,
                                    _ => throw new ArgumentException(target),
                                },
                                _ => Utility.ToFloat64(data) as object,
                            }).ToArray();
                        }
                    }

                    object[] GetPanelPoint(YamlNode yamlNode, string target, object[] defaultValues = null)
                    {
                        var text = Utility.GetText(yamlNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            return defaultValues;
                        }
                        else
                        {
                            return text.Split(",").Select(value => GetCalledText(value.Trim())).Select((data, i) => i switch
                            {
                                4 => Utility.ToInt32(data) switch
                                {
                                    0 => HorizontalAlignment.Left,
                                    1 => HorizontalAlignment.Center,
                                    2 => HorizontalAlignment.Right,
                                    _ => throw new ArgumentException(target),
                                },
                                5 => Utility.ToInt32(data) switch
                                {
                                    0 => VerticalAlignment.Top,
                                    1 => VerticalAlignment.Center,
                                    2 => VerticalAlignment.Bottom,
                                    _ => throw new ArgumentException(target),
                                },
                                6 => DrawingSystem.Instance.GetDefaultPaint(data.GetColor(), 100),
                                7 => DrawingSystem.Instance.GetDefaultPaint(data.GetColor(), 100),
                                _ => Utility.ToFloat64(data) as object,
                            }).ToArray();
                        }
                    }

                    double[] GetLayerPoint(YamlNode yamlNode, string target, double[] defaultValues = null)
                    {
                        var text = Utility.GetText(yamlNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            return defaultValues;
                        }
                        else
                        {
                            return text.Split(",").Select(value => GetCalledText(value.Trim())).Select(data => Utility.ToFloat64(data)).ToArray();
                        }
                    }

                    float[] GetQuitPoint(YamlNode yamlNode, string target, float[] defaultValues = null)
                    {
                        var text = Utility.GetText(yamlNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            return defaultValues;
                        }
                        else
                        {
                            return text.Split(",").Select(value => GetCalledText(value.Trim())).Select(data => Utility.ToFloat32(data)).ToArray();
                        }
                    }

                    object[] GetTextPoint(YamlNode yamlNode, string target, object[] defaultValues = null)
                    {
                        var text = Utility.GetText(yamlNode, target);
                        if (string.IsNullOrEmpty(text))
                        {
                            return defaultValues;
                        }
                        else
                        {
                            return text.Split(",").Select(value => GetCalledText(value.Trim())).Select((data, i) => i switch
                            {
                                4 => Utility.ToInt32(data) switch
                                {
                                    0 => HorizontalAlignment.Left,
                                    1 => HorizontalAlignment.Center,
                                    2 => HorizontalAlignment.Right,
                                    _ => throw new ArgumentException(target),
                                },
                                5 => Utility.ToInt32(data) switch
                                {
                                    0 => VerticalAlignment.Top,
                                    1 => VerticalAlignment.Center,
                                    2 => VerticalAlignment.Bottom,
                                    _ => throw new ArgumentException(target),
                                },
                                _ => Utility.ToFloat64(data) as object,
                            }).ToArray();
                        }
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
                        if (!string.IsNullOrEmpty(text) && QwilightComponent.GetCallComputer().IsMatch(text))
                        {
                            var values = text.Split("(").Select(value => value.Trim()).ToArray();
                            return lsCaller.Call(lsCaller.Globals[values[0]], values[1][0..^1].Split(',').Where(value => !string.IsNullOrEmpty(value)).Select(value => Utility.ToFloat64(value) as object).ToArray()).ToObject().ToString();
                        }
                        else
                        {
                            return text;
                        }
                    }
                }

                var getPaintProperty = new Func<int[], string>(args => "P");
                var getTransition = new Func<int[], string>(args => "T");

                SetFunc("_GetPaintProperty", ref getPaintProperty);
                SetFunc("_GetTransition", ref getTransition);

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

                var handledMediaValues = new ConcurrentDictionary<string, HandledMediaItem>();
                var defaultDrawingValues = new ConcurrentDictionary<string, ImageSource>();
                var drawingValues = new ConcurrentDictionary<string, DrawingItem>();
                var handledDrawingValues = new ConcurrentDictionary<string, HandledDrawingItem>();
                var zipFilePath = Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, Path.ChangeExtension(zipName, "zip"));
                if (File.Exists(zipFilePath))
                {
                    using var zipFile = ZipFile.Read(zipFilePath);
                    foreach (var zipEntry in zipFile)
                    {
                        if (!zipEntry.IsDirectory)
                        {
                            var rms = PoolSystem.Instance.GetDataFlow((int)zipEntry.UncompressedSize);
                            zipEntry.Extract(rms);
                            var fileName = zipEntry.FileName;
                            var justFileName = Path.GetFileNameWithoutExtension(fileName);
                            switch (Path.GetDirectoryName(fileName))
                            {
                                case "Audio":
                                    parallelItems.Add(() =>
                                    {
                                        try
                                        {
                                            using (rms)
                                            {
                                                _audioItemMap[justFileName] = AudioSystem.Instance.Load(rms, this, 1F, null, QwilightComponent.GetLoopingAudioComputer().IsMatch(justFileName));
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    });
                                    break;
                                case "Media":
                                    parallelItems.Add(() =>
                                    {
                                        try
                                        {
                                            using (rms)
                                            {
                                                var hash = Utility.GetID128s(rms);
                                                var hashMediaFilePath = Path.Combine(QwilightComponent.MediaEntryPath, $"{hash}{Path.GetExtension(fileName)}");
                                                if (!File.Exists(hashMediaFilePath))
                                                {
                                                    using var fs = File.OpenWrite(hashMediaFilePath);
                                                    rms.WriteTo(fs);
                                                }
                                                handledMediaValues[Path.GetFileName(fileName)] = MediaSystem.Instance.Load(hashMediaFilePath, this);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    });
                                    break;
                                case "Drawing":
                                    var fileNameContents = justFileName.Split(' ');
                                    Utility.ToInt32(fileNameContents.ElementAtOrDefault(1), out var value1);
                                    Utility.ToInt32(fileNameContents.ElementAtOrDefault(2), out var value2);
                                    if (fileNameContents[0] == getPaintProperty(new[] { value1, value2 }))
                                    {
                                        NewHandledDrawing(rms);
                                    }
                                    else if (fileNameContents[0] == getTransition(new[] { value1, value2 }))
                                    {
                                        NewHandledDrawing(rms);
                                    }
                                    break;
                                case "Input":
                                    fileNameContents = justFileName.Split(" ");
                                    if (fileNameContents[0] == "I")
                                    {
                                        NewHandledDrawing(rms);
                                    }
                                    break;
                                case "Judgment":
                                    if (justFileName == "Total Notes")
                                    {
                                        NewDrawing(rms);
                                    }
                                    else
                                    {
                                        fileNameContents = justFileName.Split(" ");
                                        if (fileNameContents[0] == "J")
                                        {
                                            NewHandledDrawing(rms);
                                        }
                                    }
                                    break;
                                case "Mode":
                                    fileNameContents = justFileName.Split(" ");
                                    if (fileNameContents[0] == "M")
                                    {
                                        NewHandledDrawing(rms);
                                    }
                                    break;
                                case "Net Position":
                                    fileNameContents = justFileName.Split(" ");
                                    if (fileNameContents[0] == "NP")
                                    {
                                        NewDrawing(rms);
                                    }
                                    break;
                                case "Notify":
                                    fileNameContents = justFileName.Split(" ");
                                    if (fileNameContents[0] == "N")
                                    {
                                        NewHandledDrawing(rms);
                                    }
                                    break;
                                case "Quit v2":
                                    switch (justFileName)
                                    {
                                        case "S+":
                                        case "S":
                                        case "S FC":
                                        case "A+":
                                        case "A+ FC":
                                        case "A":
                                        case "A FC":
                                        case "B":
                                        case "B FC":
                                        case "C":
                                        case "C FC":
                                        case "D":
                                        case "D FC":
                                        case "F":
                                            NewHandledDrawing(rms);
                                            break;
                                    }
                                    break;
                                case "Quit Mode":
                                    NewDrawing(rms);
                                    break;
                                case "":
                                    switch (justFileName)
                                    {
                                        case "File":
                                        case "Total Notes":
                                        case "BPM!":
                                        case "File Viewer":
                                        case "Assist File Viewer":
                                        case "Configure":
                                        case "Comment":
                                        case "Audio Input":
                                        case "Want":
                                        case "Salt":
                                        case "Default Entry Configure":
                                        case "Judgment Stage":
                                        case "Highest Input Count":
                                        case "Length":
                                        case "BPM":
                                        case "Input Mode":
                                            NewDefaultDrawing(rms);
                                            break;
                                        case "Default":
                                            NewHandledDrawing(rms);
                                            break;
                                    }
                                    break;
                                default:
                                    NewDefaultDrawing(rms);
                                    break;
                            }
                            void NewDrawing(Stream s) => parallelItems.Add(() =>
                            {
                                try
                                {
                                    using (s)
                                    {
                                        drawingValues[fileName] = DrawingSystem.Instance.Load(s, this);
                                    }
                                }
                                catch
                                {
                                }
                            });
                            void NewDefaultDrawing(Stream s) => parallelItems.Add(() =>
                            {
                                try
                                {
                                    using (s)
                                    {
                                        defaultDrawingValues[fileName] = DrawingSystem.Instance.LoadDefault(s, this);
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
                                            handledDrawingValues[fileName] = new()
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
                    }
                }

                if (src != target)
                {
                    AudioSystem.Instance.Close(this);
                    DrawingSystem.Instance.Close(this);
                    MediaSystem.Instance.Close(this, this);
                }
                else
                {
                    MediaSystem.Instance.Stop(this);
                }
                Utility.HandleHMP(parallelItems, Configure.Instance.UIBin, parallelItem => parallelItem());

                foreach (var paintPropertyValue in PaintPropertyValues)
                {
                    if (paintPropertyValue?.DrawingVariety == 11)
                    {
                        var mode = paintPropertyValue.Mode;
                        var isAvailable = mode == 1;
                        var isDefaultAvailable = mode == 0;
                        if (handledMediaValues.TryGetValue(paintPropertyValue.Etc, out var handledMediaItem))
                        {
                            paintPropertyValue.MediaHandlerItemValue = MediaSystem.Instance.Handle(handledMediaItem, this, isAvailable, isDefaultAvailable);
                        }
                        else
                        {
                            try
                            {
                                paintPropertyValue.MediaHandlerItemValue = MediaSystem.Instance.Handle(MediaSystem.Instance.Load(Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, paintPropertyValue.Etc), this), this, isAvailable, isDefaultAvailable);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                foreach (var (fileName, drawingItem) in drawingValues)
                {
                    var justFileName = Path.GetFileNameWithoutExtension(fileName);
                    switch (Path.GetDirectoryName(fileName))
                    {
                        case "Judgment":
                            if (justFileName == "Total Notes")
                            {
                                TotalNotesJudgmentDrawing = drawingItem;
                            }
                            break;
                        case "Net Position":
                            var fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "NP")
                            {
                                NetPositionDrawings[Utility.ToInt32(fileNameContents[1])] = drawingItem;
                            }
                            break;
                        case "Quit Mode":
                            switch (justFileName)
                            {
                                case "Stand":
                                    StandDrawing = drawingItem;
                                    break;
                                case "Highest Band":
                                    HighestBandDrawing = drawingItem;
                                    break;
                                case "Point":
                                    PointDrawing = drawingItem;
                                    break;
                                case "New Stand":
                                    NewStandDrawing = drawingItem;
                                    break;
                                case "View Comment":
                                    ViewCommentDrawing = drawingItem;
                                    break;
                                case "Handle Undo":
                                    HandleUndoDrawing = drawingItem;
                                    break;
                                case "Move 0":
                                    QuitMove0Drawing = drawingItem;
                                    break;
                                case "Move 1":
                                    QuitMove1Drawing = drawingItem;
                                    break;
                                case "Judgment Stage":
                                    JudgmentStageQuitDrawing = drawingItem;
                                    break;
                                case "Highest Input Count":
                                    HighestInputCountQuitDrawing = drawingItem;
                                    break;
                                case "Length":
                                    LengthQuitDrawing = drawingItem;
                                    break;
                                case "BPM":
                                    BPMQuitDrawing = drawingItem;
                                    break;
                                case "Input Mode":
                                    InputModeQuitDrawing = drawingItem;
                                    break;
                            }
                            break;
                    }
                }
                foreach (var (fileName, defaultDrawing) in defaultDrawingValues)
                {
                    var justFileName = Path.GetFileNameWithoutExtension(fileName);
                    switch (Path.GetDirectoryName(fileName))
                    {
                        case "Default Entry":
                            var fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "DE")
                            {
                                DefaultEntryDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Salt Auto":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SA")
                            {
                                SaltAutoDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Site Situation":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SS")
                            {
                                SiteSituationDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Input Window":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "IW")
                            {
                                InputModeWindowDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Handled":
                            fileNameContents = justFileName.Split(" ");
                            switch (fileNameContents[0])
                            {
                                case "W":
                                    HandledWallDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                                    break;
                            }
                            break;
                        case "Avatar Configure":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "AC")
                            {
                                AvatarConfigureDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Net Site":
                            switch (justFileName)
                            {
                                case "BPM":
                                    BPMNetSiteDrawing = defaultDrawing;
                                    break;
                                case "BPM!":
                                    BPM1NetSiteDrawing = defaultDrawing;
                                    break;
                                case "Total Notes":
                                    TotalNotesNetSiteDrawing = defaultDrawing;
                                    break;
                                case "Highest Input Count":
                                    HighestInputCountNetSiteDrawing = defaultDrawing;
                                    break;
                                case "Length":
                                    LengthNetSiteDrawing = defaultDrawing;
                                    break;
                                case "Judgment Stage":
                                    JudgmentStageNetSiteDrawing = defaultDrawing;
                                    break;
                            }
                            break;
                        case "Sign in":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "S")
                            {
                                SignInDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Site Cipher":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SC")
                            {
                                SiteCipherDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Site Configure":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SC")
                            {
                                SiteConfigureDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Stop Auto":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SA")
                            {
                                StopAutoDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Site Media":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SM")
                            {
                                SiteMediaDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Site Audio":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "SA")
                            {
                                SiteAudioDrawings[Utility.ToInt32(fileNameContents[1])] = defaultDrawing;
                            }
                            break;
                        case "Fit":
                            switch (justFileName)
                            {
                                case "Title":
                                    TitleFitDrawing = defaultDrawing;
                                    break;
                                case "Artist":
                                    ArtistFitDrawing = defaultDrawing;
                                    break;
                                case "Level Text Value":
                                    LevelTextValueFitDrawing = defaultDrawing;
                                    break;
                                case "Modified Date":
                                    ModifiedDateFitDrawing = defaultDrawing;
                                    break;
                                case "Handled Count":
                                    HandledCountFitDrawing = defaultDrawing;
                                    break;
                                case "Latest Date":
                                    LatestDateFitDrawing = defaultDrawing;
                                    break;
                                case "BPM":
                                    BPMFitDrawing = defaultDrawing;
                                    break;
                                case "Total Notes":
                                    TotalNotesFitDrawing = defaultDrawing;
                                    break;
                                case "Highest Input Count":
                                    HighestInputCountFitDrawing = defaultDrawing;
                                    break;
                                case "Average Input Count":
                                    AverageInputCountFitDrawing = defaultDrawing;
                                    break;
                                case "Length":
                                    LengthFitDrawing = defaultDrawing;
                                    break;
                                case "Entry Path":
                                    EntryPathFitDrawing = defaultDrawing;
                                    break;
                            }
                            break;
                        case "":
                            switch (justFileName)
                            {
                                case "File":
                                    FileDrawing = defaultDrawing;
                                    break;
                                case "Total Notes":
                                    TotalNotesDrawing = defaultDrawing;
                                    break;
                                case "File Viewer":
                                    FileViewerDrawing = defaultDrawing;
                                    break;
                                case "Assist File Viewer":
                                    AssistFileViewerDrawing = defaultDrawing;
                                    break;
                                case "Comment":
                                    CommentDrawing = defaultDrawing;
                                    break;
                                case "Configure":
                                    ConfigureDrawing = defaultDrawing;
                                    break;
                                case "Audio Input":
                                    AudioInputDrawing = defaultDrawing;
                                    break;
                                case "Salt":
                                    SaltDrawing = defaultDrawing;
                                    break;
                                case "Default Entry Configure":
                                    DefaultEntryConfigureDrawing = defaultDrawing;
                                    break;
                                case "Judgment Stage":
                                    JudgmentStageDrawing = defaultDrawing;
                                    break;
                                case "Highest Input Count":
                                    HighestInputCountDrawing = defaultDrawing;
                                    break;
                                case "Length":
                                    LengthDrawing = defaultDrawing;
                                    break;
                                case "BPM":
                                    BPMDrawing = defaultDrawing;
                                    break;
                                case "BPM!":
                                    BPM1Drawing = defaultDrawing;
                                    break;
                                case "Input Mode":
                                    InputModeDrawing = defaultDrawing;
                                    break;
                            }
                            break;
                    }
                }
                foreach (var (fileName, handledDrawingItem) in handledDrawingValues)
                {
                    var justFileName = Path.GetFileNameWithoutExtension(fileName);
                    switch (Path.GetDirectoryName(fileName))
                    {
                        case "Drawing":
                            var fileNameContents = justFileName.Split(' ');
                            Utility.ToInt32(fileNameContents.ElementAtOrDefault(1), out var value1);
                            Utility.ToInt32(fileNameContents.ElementAtOrDefault(2), out var value2);
                            if (fileNameContents[0] == getPaintProperty(new[] { value1, value2 }))
                            {
                                PaintPropertyValues[value1].HandledDrawingItems.SetValue(value2, handledDrawingItem);
                            }
                            else if (fileNameContents[0] == getTransition(new[] { value1, value2 }))
                            {
                                FadingPropertyValues[value1][value2].HandledDrawingItems.SetValue(Utility.ToInt32(fileNameContents[3]), handledDrawingItem);
                            }
                            break;
                        case "Input":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "I")
                            {
                                InputModeDrawings[Utility.ToInt32(fileNameContents[1])] = handledDrawingItem;
                            }
                            break;
                        case "Judgment":
                            if (justFileName != "Total Notes")
                            {
                                fileNameContents = justFileName.Split(" ");
                                if (fileNameContents[0] == "J")
                                {
                                    JudgmentDrawings[Utility.ToInt32(justFileName.Split(" ")[1])] = handledDrawingItem;
                                }
                            }
                            break;
                        case "Mode":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "M")
                            {
                                ModeComponentDrawings[Utility.ToInt32(fileNameContents[1])][Utility.ToInt32(fileNameContents[2])] = handledDrawingItem;
                            }
                            break;
                        case "Notify":
                            fileNameContents = justFileName.Split(" ");
                            if (fileNameContents[0] == "N")
                            {
                                NotifyDrawings[Utility.ToInt32(fileNameContents[1])] = handledDrawingItem;
                            }
                            break;
                        case "Quit v2":
                            switch (justFileName)
                            {
                                case "S+":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.SPlus][0] = handledDrawingItem;
                                    break;
                                case "S":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.S][0] = handledDrawingItem;
                                    break;
                                case "S FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.S][1] = handledDrawingItem;
                                    break;
                                case "A+":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.APlus][0] = handledDrawingItem;
                                    break;
                                case "A+ FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.APlus][1] = handledDrawingItem;
                                    break;
                                case "A":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.A][0] = handledDrawingItem;
                                    break;
                                case "A FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.A][1] = handledDrawingItem;
                                    break;
                                case "B":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.B][0] = handledDrawingItem;
                                    break;
                                case "B FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.B][1] = handledDrawingItem;
                                    break;
                                case "C":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.C][0] = handledDrawingItem;
                                    break;
                                case "C FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.C][1] = handledDrawingItem;
                                    break;
                                case "D":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.D][0] = handledDrawingItem;
                                    break;
                                case "D FC":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.D][1] = handledDrawingItem;
                                    break;
                                case "F":
                                    QuitDrawings[(int)DefaultCompute.QuitStatus.F][0] = handledDrawingItem;
                                    break;
                            }
                            break;
                        case "":
                            switch (justFileName)
                            {
                                case "Default":
                                    DefaultDrawing = handledDrawingItem;
                                    break;
                            }
                            break;
                    }
                }

                for (var i = (int)Component.InputMode.InputMode484; i >= (int)Component.InputMode.InputMode4; --i)
                {
                    InputModeWindowDrawings[i] ??= InputModeDrawings[i]?.DefaultDrawing;
                }

                for (var i = (int)DefaultCompute.QuitStatus.F; i >= (int)DefaultCompute.QuitStatus.SPlus; --i)
                {
                    QuitDrawings[i][1] ??= QuitDrawings[i][0];
                }

                for (var i = PaintPropertyValues.Length - 1; i >= 0; --i)
                {
                    var frame = PaintPropertyValues[i]?.Frame ?? 0;
                    for (var j = 1; j < frame; ++j)
                    {
                        PaintPropertyValues[i].HandledDrawingItems[j] ??= PaintPropertyValues[i].HandledDrawingItems[j - 1];
                    }
                }

                for (var i = FadingPropertyValues.Length - 1; i >= 0; --i)
                {
                    for (var j = FadingPropertyValues[i].Length - 1; j >= 0; --j)
                    {
                        var frame = FadingPropertyValues[i][j]?.Frame ?? 0;
                        for (var m = 1; m < frame; ++m)
                        {
                            FadingPropertyValues[i][j].HandledDrawingItems[m] ??= FadingPropertyValues[i][j].HandledDrawingItems[m - 1];
                        }
                    }
                    for (var j = FadingPropertyValues[i].Length - 1; j >= 2; --j)
                    {
                        FadingPropertyValues[i][j] ??= FadingPropertyValues[i][1];
                    }
                    for (var m = (FadingPropertyValues[i][1]?.Frame ?? 0) - 1; m >= 0; --m)
                    {
                        FadingPropertyValues[i][1].HandledDrawingItems[m] ??= FadingPropertyValues[i][0].HandledDrawingItems.ElementAtOrDefault(m);
                    }
                    for (var j = FadingPropertyValues[i].Length - 1; j >= 2; --j)
                    {
                        for (var m = (FadingPropertyValues[i][j]?.Frame ?? 0) - 1; m >= 1; --m)
                        {
                            FadingPropertyValues[i][j].HandledDrawingItems[m] ??= FadingPropertyValues[i][1].HandledDrawingItems.ElementAtOrDefault(m);
                        }
                    }
                }

                FaultText = null;
            }
            finally
            {
                DrawingSystem.Instance.LoadDefaultDrawing();
                DrawingSystem.Instance.LoadVeilDrawing();
                ViewModels.Instance.NotifyWindowViewModels();
                mainViewModel.NotifyModel();
                mainViewModel.IsUILoading = false;
            }
        }

        void SetConfigures(Script lsCaller)
        {
            lsCaller.Globals["configures"] = Enumerable.Range(0, HighestBaseUIConfigure).Select(i =>
            {
                return Math.Max(0, Array.IndexOf(XamlBaseUIConfigures.SingleOrDefault(value => value.Position == i)?.Configures ?? Array.Empty<string>(), Configure.Instance.BaseUIConfigureValue.UIConfigures[i]));
            }).ToArray();
        }

        public void LoadUIFiles() => Utility.SetUICollection(UIItems, Utility.GetEntry(QwilightComponent.UIEntryPath).Prepend(QwilightComponent.UIEntryPath).SelectMany(targetEntryPath => Utility.GetFiles(targetEntryPath).Where(targetFilePath => Path.GetFileName(targetFilePath).StartsWith('@') && targetFilePath.IsTailCaselsss(".yaml")).Select(yamlFilePath => new UIItem
        {
            UIEntry = Path.GetRelativePath(QwilightComponent.UIEntryPath, targetEntryPath),
            YamlName = Path.GetFileNameWithoutExtension(yamlFilePath)
        })).ToArray());

        void Init()
        {
            foreach (var value in typeof(BaseUI).GetProperties())
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
            for (var i = D2DHitPointsPaints.Length - 1; i >= 0; --i)
            {
                D2DHitPointsPaints[i] = new ICanvasBrush[101];
            }
            for (var i = D2DJudgmentPaints.Length - 1; i >= 0; --i)
            {
                D2DJudgmentPaints[i] = new ICanvasBrush[101];
            }
            _audioItemMap.Clear();
            EventItems.Clear();
            FadingPropertyValues[(int)MainViewModel.Mode.NoteFile] = new FadingProperty[4];
            FadingPropertyValues[1] = new FadingProperty[2];
            FadingPropertyValues[(int)MainViewModel.Mode.Computing] = new FadingProperty[4];
            FadingPropertyValues[(int)MainViewModel.Mode.Quit] = new FadingProperty[2];
            for (var i = (int)DefaultCompute.QuitStatus.F; i >= (int)DefaultCompute.QuitStatus.SPlus; --i)
            {
                QuitDrawings[i] = new HandledDrawingItem?[2];
            }
            ModeComponentDrawings[ModifyModeComponentViewModel.AutoModeVariety] = new HandledDrawingItem?[2];
            ModeComponentDrawings[ModifyModeComponentViewModel.NoteSaltModeVariety] = new HandledDrawingItem?[15];
            ModeComponentDrawings[ModifyModeComponentViewModel.FaintNoteModeVariety] = new HandledDrawingItem?[4];
            ModeComponentDrawings[ModifyModeComponentViewModel.JudgmentModeVariety] = new HandledDrawingItem?[6];
            ModeComponentDrawings[ModifyModeComponentViewModel.HitPointsModeVariety] = new HandledDrawingItem?[8];
            ModeComponentDrawings[ModifyModeComponentViewModel.NoteMobilityModeVariety] = new HandledDrawingItem?[6];
            ModeComponentDrawings[ModifyModeComponentViewModel.LongNoteModeVariety] = new HandledDrawingItem?[4];
            ModeComponentDrawings[ModifyModeComponentViewModel.InputFavorModeVariety] = new HandledDrawingItem?[17];
            ModeComponentDrawings[ModifyModeComponentViewModel.NoteModifyModeVariety] = new HandledDrawingItem?[3];
            ModeComponentDrawings[ModifyModeComponentViewModel.BPMModeVariety] = new HandledDrawingItem?[2];
            ModeComponentDrawings[ModifyModeComponentViewModel.WaveModeVariety] = new HandledDrawingItem?[2];
            ModeComponentDrawings[ModifyModeComponentViewModel.SetNoteModeVariety] = new HandledDrawingItem?[4];
            ModeComponentDrawings[ModifyModeComponentViewModel.LowestJudgmentConditionModeVariety] = new HandledDrawingItem?[2];
            DefaultLength = 1280.0;
            DefaultHeight = 720.0;
            EventNoteNameFontLevel = Levels.FontLevel0;
            TitleFontLevel = Levels.FontLevel0;
            ArtistFontLevel = Levels.FontLevel0;
            FittedTextFontLevel = Levels.FontLevel0;
            GenreFontLevel = Levels.FontLevel0;
            LevelFontLevel = Levels.FontLevel0;
            WantLevelIDFontLevel = Levels.FontLevel0;
            EntryItemPositionFontLevel = Levels.FontLevel0;
            CommentDateFontLevel = Levels.FontLevel0;
            CommentViewPointFontLevel = Levels.FontLevel0;
            CommentAvatarNameFontLevel = Levels.FontLevel0;
            CommentStandFontLevel = Levels.FontLevel0;
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
                        lock (UI.Instance.ContentsCSX)
                        {
                            try
                            {
                                LoadBaseUIImpl(src, target);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpenedUIFileContents);
                            }
                            catch (YamlException e)
                            {
                                FaultText = string.Format(LanguageSystem.Instance.YAMLCompileFault, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText);
                                LoadDefaultBaseUI();
                            }
                            catch (Exception e)
                            {
                                FaultText = string.Format(LanguageSystem.Instance.UIFaultText, e.Message);
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, FaultText);
                                LoadDefaultBaseUI();
                            }
                        }
                    });
                }
                else
                {
                    lock (UI.Instance.ContentsCSX)
                    {
                        try
                        {
                            LoadBaseUIImpl(src, target);
                        }
                        catch (YamlException e)
                        {
                            FaultText = string.Format(LanguageSystem.Instance.YAMLCompileFault, e.Message, true);
                            LoadDefaultBaseUI();
                        }
                        catch (Exception e)
                        {
                            FaultText = string.Format(LanguageSystem.Instance.UIFaultText, e.Message, true);
                            LoadDefaultBaseUI();
                        }
                    }
                }

                void LoadDefaultBaseUI()
                {
                    var defaultUIItem = new UIItem
                    {
                        UIEntry = "@Default",
                        YamlName = "@Default"
                    };
                    if (target != defaultUIItem)
                    {
                        LoadUI(null, defaultUIItem, isParallel);
                    }
                }
            }
        }
    }
}