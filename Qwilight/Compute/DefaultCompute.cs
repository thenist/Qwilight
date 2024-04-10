using Google.Protobuf;
using Ionic.Zip;
using Microsoft.Graphics.Canvas;
using MoonSharp.Interpreter;
using Qwilight.Compiler;
using Qwilight.Note;
using Qwilight.NoteFile;
using Qwilight.PaintComponent;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using Qwilight.XOR;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Channel = FMOD.Channel;

namespace Qwilight.Compute
{
    public class DefaultCompute : Computing, IAudioHandler, IAudioContainer, IDrawingContainer, IMediaContainer, IMediaHandler
    {
        public static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(DefaultCompute));

        public enum WaitingTwilight
        {
            Default, Net, WaitIO, CallIO
        }

        public enum QuitStatus
        {
            SPlus, S, APlus, A, B, C, D, F
        }

        [Flags]
        public enum InputFlag
        {
            Not = 0,
            DefaultController = 1,
            Controller = 2,
            MIDI = 4,
            Pointer = 8
        }

        public sealed class PostableItemStatus
        {
            public string AvatarName { get; set; }

            public bool IsLevyed { get; set; }

            public bool IsHandling { get; set; }

            public double Wait { get; set; }

            public double TotalWait { get; set; }

            public void Init(PostableItem postableItem, string avatarName, double wait)
            {
                AvatarName = avatarName;
                TotalWait = wait;
                Wait = wait;
                IsLevyed = false;
                IsHandling = true;
            }

            public void Stop()
            {
                Wait = 0.0;
            }

            public void Halt()
            {
                Stop();
                IsHandling = false;
            }

            public bool Elapse(double millisLoopUnit)
            {
                Wait = Math.Max(0.0, Wait - millisLoopUnit);
                IsLevyed = true;
                var isElapsed = IsHandling && Wait == 0.0;
                IsHandling = Wait > 0.0;
                return isElapsed;
            }
        }

        public FrozenDictionary<PostableItem, PostableItemStatus> PostableItemStatusMap { get; }

        public Dictionary<int, double> PostedItemFaints { get; } = new()
        {
            { -1, 0.0 },
            { 0, 0.0 },
            { 1, 0.0 }
        };

        readonly Action _lazyInit;
        readonly double[] _paintPropertyMillis = new double[UI.MaxPaintPropertyID];
        readonly double[] _hitLongNotePaintMillis = new double[53];
        readonly double[] _lastEnlargedBandPaintMillis = new double[53];
        readonly int[] _targetMainFrames = new int[53];
        readonly int[] _targetInputFrames = new int[53];
        readonly bool _isLevyingComputer;
        readonly BaseCompiler _targetCompiler;
        readonly Thread _targetCompilerHandler;
        readonly CancellationTokenSource _setCancelCompiler = new();
        readonly Component.HitPointsModeDate _hitPointsModeDate;
        readonly Component.HitPointsMapDate _hitPointsMapDate;
        readonly Component.PointMapDate _pointMapDate;
        readonly Component.StandMapDate _standMapDate;
        readonly Component.StandModeDate _standModeDate;
        readonly Component.JudgmentModeDate _judgmentModeDate;
        readonly Component.JudgmentMapDate _judgmentMapDate;
        readonly Component.LongNoteAssistDate _longNoteAssistDate;
        readonly Component.TrapNoteJudgmentDate _trapNoteJudgmentDate;
        readonly Component.PaintEventsDate _paintEventsDate;
        readonly Component.TooLongLongNoteDate _tooLongLongNoteDate;
        readonly List<AudioNote>[] _lastIIDXInputAudioNoteMap = new List<AudioNote>[53];
        readonly List<Comment> _comments = new();
        readonly List<NetItem> _netItems = new();
        readonly Dictionary<MediaNote.Mode, IHandlerItem> MediaCollection = new();
        readonly Stopwatch _loopingHandler = new();
        readonly ConcurrentQueue<(int, byte)> _rawInputQueue = new();
        readonly List<PaintEvent>[] _paintEventsGAS = new List<PaintEvent>[8];
        readonly object _targetHandlerCSX = new();
        readonly List<BaseNote> _handlingNotes = new();
        readonly Queue<double> _eventPositions = new();
        readonly List<string> _ioAvatarIDs = new();
        readonly List<string> _pendingIOAvatarIDs = new();
        readonly List<string> _sentIOAvatarIDs = new();
        readonly List<double> _noteWaits = new();
        readonly XORFloat64[] _hitPointsGAS = new XORFloat64[6];
        Component _valueComponent;
        bool _isPausing;
        bool _isPausingWindowOpened;
        double _bandPaintMillis;
        double _autoMainMillis;
        double _mainMillis;
        double _inputMillis;
        double _levelMillis;
        double _judgmentMainPaintMillis;
        double _mainJudgmentMeterPaintMillis;
        double _pauseMillis;
        int _lastPaintedBand;
        Thread _targetHandler;
        double _noteMillis;
        double _failedDrawingMillis;
        bool _wasNetItems;
        bool _wasGetNetItems;
        bool _wasGetNetComments;
        int _validJudgedNotes;
        CompilerStatus _targetCompilerStatus;
        double _standMultiplier;
        bool _isPassable;
        bool _isEscapable;
        LastStatus _lastStatus;
        bool _paintEnlargedLastBand;
        double _millisMeter;
        double _millisStandardMeter;
        bool _isPaused;
        InputFlag _inputFlags;
        int _totalComments;

        public readonly DrawingComponent DrawingComponentValue = new();

        public PostableItem[] PostableItems { get; } = new PostableItem[2];

        public PostableItem[] LastPostableItems { get; } = new PostableItem[2];

        public MoveValue<double>[] PostableItemFaints { get; } = new MoveValue<double>[2];

        public string PostedItemText { get; set; }

        public int PostedItemVariety { get; set; }

        bool IsInEvents => _eventPositions.Count > 0;

        public string SiteID { get; set; }

        public int ValidNetMode { get; set; }

        public bool IsPostableItemMode => ValidNetMode > 0;

        public int PostableItemBand { get; set; }

        public int AvatarsCount { get; set; }

        public PostableItem[] AllowedPostableItems { get; set; }

        public HandlableAudioHandler TrailerAudioHandler { get; } = new();

        public List<BaseNote> PaintedNotes { get; } = new();

        public Comment EventComment { get; set; }

        public double GetIIDXMultiplierMillis(ModeComponent modeComponent) => Math.Max(0.0, (DrawingComponentValue.judgmentMainPosition - Configure.Instance.VeilDrawingHeight) / (modeComponent.Multiplier * modeComponent.AudioMultiplier * modeComponent.ComponentValue.LogicalYMillis));

        public WwwLevelData WwwLevelDataValue { get; set; }

        public JSON.TwilightQuitNet.QuitNetItem[] PendingQuitNetItems { get; set; }

        public Comment[] PendingQuitNetComments { get; set; }

        public string JudgmentMeterText { get; set; } = string.Empty;

        public string EarlyLateText { get; set; } = string.Empty;

        public int EarlyValue { get; set; }

        public int LateValue { get; set; }

        public float HighestNetLength { get; set; }

        public float HighestNetHeight { get; set; }

        public MoveValue<double> VeilDrawingHeight { get; } = new();

        public Component.Judged LastJudged { get; set; } = Component.Judged.Not;

        public List<NetItem> NetItems { get; set; } = new();

        public List<BaseNote> Notes { get; } = new();

        public int[] MainFrames { get; } = new int[53];

        public int[] InputFrames { get; } = new int[53];

        public int LevelFrame { get; set; }

        public int[] JudgmentMainFrames { get; } = new int[53];

        public int[] MainJudgmentMeterFrames { get; } = new int[53];

        public int[] PaintPropertyFrames { get; } = new int[UI.MaxPaintPropertyID];

        public int AutoMainFrame { get; set; }

        public string MeterText { get; set; }

        public int[] PauseFrames { get; } = new int[3];

        /// <summary>
        /// 실시간 랭킹 시작 포함 인덱스
        /// </summary>
        public int LevyingNetPosition { get; set; }

        /// <summary>
        /// 실시간 랭킹 마지막 포함 인덱스
        /// </summary>
        public int QuitNetPosition { get; set; }

        public WaitingTwilight WaitingTwilightLevel { get; set; }

        public double[] JudgmentMeters { get; } = new double[2];

        public Queue<JudgmentVisualizer>[] JudgmentVisualizerValues { get; } = new Queue<JudgmentVisualizer>[2];

        public Dictionary<int, JudgmentPaint> JudgmentPaints { get; } = new();

        public Dictionary<int, HitNotePaint> HitNotePaints { get; } = new();

        public Dictionary<int, HitLongNotePaint> HitLongNotePaints { get; } = new();

        public Dictionary<int, double> BandEnlargedMap { get; } = new();

        public Dictionary<int, int> BandDrawingFrames { get; } = new();

        public List<KeyValuePair<double, double>>[] JudgmentMeterEventValues { get; } = new List<KeyValuePair<double, double>>[6];

        public object EventValuesCSX { get; } = new();

        public List<KeyValuePair<double, double>> HitPointsEventValues { get; } = new();

        public List<KeyValuePair<double, double>> StandEventValues { get; } = new();

        public List<KeyValuePair<double, double>> PointEventValues { get; } = new();

        public List<KeyValuePair<double, double>> BandEventValues { get; } = new();

        public Component.InputMapping InputMappingValue { get; set; }

        public BaseNoteFile NoteFile => NoteFiles[LevyingComputingPosition];

        public string AvatarID => AvatarIDs[LevyingComputingPosition];

        public string AvatarName => AvatarNames[LevyingComputingPosition];

        public string UbuntuID { get; }

        public virtual string TotalNotesInQuit => NoteFiles.Length > 1 ? $"{TotalNotes} / {InheritedTotalNotes}" : $"{TotalNotes}";

        public virtual string HighestJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.HighestJudgment} / {InheritedHighestJudgment}" : $"{Comment.HighestJudgment}";

        public virtual string HigherJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.HigherJudgment} / {InheritedHigherJudgment}" : $"{Comment.HigherJudgment}";

        public virtual string HighJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.HighJudgment} / {InheritedHighJudgment}" : $"{Comment.HighJudgment}";

        public virtual string LowJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.LowJudgment} / {InheritedLowJudgment}" : $"{Comment.LowJudgment}";

        public virtual string LowerJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.LowerJudgment} / {InheritedLowerJudgment}" : $"{Comment.LowerJudgment}";

        public virtual string LowestJudgmentInQuit => NoteFiles.Length > 1 ? $"{Comment.LowestJudgment} / {InheritedLowestJudgment}" : $"{Comment.LowestJudgment}";

        public string BPMText => Utility.GetBPMText(BPM, LevyingMultiplier, LevyingAudioMultiplier);

        public string LengthText => Utility.GetLengthText(Length);

        public byte[] NoteFileContents { get; set; }

        public BaseNoteFile[] MyNoteFiles { get; set; }

        public BaseNoteFile[] NoteFiles { get; set; }

        public ModeComponent[] ModeComponentValues { get; set; }

        public string[] AvatarIDs { get; set; }

        public string[] AvatarNames { get; set; }

        public EntryItem EventNoteEntryItem { get; }

        public ModeComponent ModeComponentValue => ModeComponentValues[LevyingComputingPosition];

        /// <summary>
        /// 게임 끝난 후 이것으로 복원
        /// </summary>
        public ModeComponent DefaultModeComponentValue { get; }

        /// <summary>
        /// 미리듣기 게임 모드 호환 검사
        /// </summary>
        public ModeComponent CompatibleModeComponentValue { get; }

        public Comment Comment => Comments[LevyingComputingPosition];

        public Comment[] Comments { get; set; }

        public string HandlerID { get; }

        public ConcurrentQueue<double> MultiplierQueue { get; } = new();

        public ConcurrentQueue<double> AudioMultiplierQueue { get; } = new();

        public ConcurrentQueue<JSON.TwilightCompiledIO> TwilightCompiledIOQueue { get; } = new();

        public ConcurrentQueue<int> PostItemQueue { get; } = new();

        public List<string> IOAvatarNames { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIOInput>> TwilightIOInputQueue { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIOJudge>> TwilightIOJudgeQueue { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIOJudgmentMeter>> TwilightIOJudgmentMeterQueue { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIONoteVisibility>> TwilightIONoteVisibilityQueue { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIOMultiplier>> TwilightIOMultiplierQueue { get; } = new();

        public Queue<Elapsable<Event.Types.TwilightIOAudioMultiplier>> TwilightIOAudioMultiplierQueue { get; } = new();

        public Queue<Event.Types.TwilightPostItem> TwilightPostItemQueue { get; } = new();

        public Action IOLazyInit { get; set; }

        public double IOMillis { get; set; }

        public ConcurrentQueue<double> InputCountQueue { get; } = new();

        public bool IsHandling { get; set; }

        public double LowestJudgmentMillis { get; set; }

        public double HighestJudgmentMillis { get; set; }

        public MoveValue<int?> Hunter { get; } = new();

        public double AudioMultiplier => Math.Round(ModeComponentValue.AudioMultiplier, 2);

        public bool IsCounterWave => ModeComponentValue.WaveModeValue == ModeComponent.WaveMode.Counter;

        public override BaseNoteFile.NoteVariety NoteVarietyValue => NoteFile.NoteVarietyValue;

        public Queue<Event.Types.NetDrawing> NetDrawings { get; set; }

        public bool IsValidNetDrawings { get; set; } = true;

        public void NewNetDrawing(bool isValidNetDrawings, Event.Types.NetDrawing.Types.Variety drawingVariety, uint param, double position0, double position1, double length, double height)
        {
            if (!isValidNetDrawings)
            {
                NetDrawings.Enqueue(new()
                {
                    DrawingVariety = drawingVariety,
                    Param = param,
                    Position0 = position0,
                    Position1 = position1,
                    Length = length,
                    Height = height
                });
            }
        }

        public Event.Types.DrawingComponent NetDrawingComponentValue { get; set; }

        public virtual bool CanPause => WwwLevelDataValue?.AllowPause != false;

        public virtual bool CanUndo => WwwLevelDataValue?.AllowPause != false;

        public int FadingViewLayer { get; set; } = 1;

        public bool LoadedMedia { get; } = Configure.Instance.LoadedMedia;

        public bool IsFailMode { get; set; } = Configure.Instance.IsFailMode;

        public virtual Configure.InputAudioVariety InputAudioVarietyValue => Configure.Instance.InputAudioVarietyValue;

        public virtual string PlatformVarietyContents => IsAutoMode ? LanguageSystem.Instance.PlatformAutoComputing : LanguageSystem.Instance.PlatformDefaultComputing;

        public virtual bool CanSetPosition => false;

        public virtual bool CanModifyAutoMode => false;

        public virtual bool CanIO => !IsPostableItemMode;

        public virtual bool IsPassable => _isPassable;

        public virtual bool IsEscapable => _isEscapable;

        public virtual bool IsMeterVisible => false;

        /// <summary>
        /// 멀티방에서 실시간 랭킹을 받은건지 여부
        /// </summary>
        public bool IsTwilightNetItems { get; set; }

        public object IsTwilightNetItemsCSX { get; } = new();

        public int InheritedHighestJudgment { get; set; }

        public int InheritedHigherJudgment { get; set; }

        public int InheritedHighJudgment { get; set; }

        public int InheritedLowJudgment { get; set; }

        public int InheritedLowerJudgment { get; set; }

        public int InheritedLowestJudgment { get; set; }

        public int LevyingComputingPosition { get; set; }

        public bool HasFailedJudgment { get; set; }

        public double TotalPoint { get; set; }

        public double SavedPoint { get; set; }

        public int LastStand { get; set; }

        public double SavedStand { get; set; }

        public double LevyingWait { get; set; } = double.NaN;

        public int LevyingMeter { get; set; } = -1;

        public int HighestComputingPosition { get; set; }

        public int InheritTotalStand { get; }

        public int InheritedTotalNotes { get; set; }

        public int InputCount1P { get; set; }

        public bool[] IsIn2P { get; set; }

        public bool Has2P { get; set; }

        public bool IsNewStand { get; set; }

        public int NetPosition { get; set; } = -1;

        public string CommentPlace0Text { get; set; }

        public string CommentPlace1Text { get; set; }

        public MoveValue<XORInt32> Band { get; } = new();

        public MoveValue<XORFloat64> Point => Points[LevyingComputingPosition];

        public MoveValue<XORInt32> Stand => Stands[LevyingComputingPosition];

        public MoveValue<XORFloat64> HitPoints => HitPointsValues[LevyingComputingPosition];

        public Primitive<bool> IsF => IsFs[LevyingComputingPosition];

        bool CanGAS => !IsPostableItemMode && ModeComponentValue.CanGAS && WwwLevelDataValue == null;

        public double DefaultHitPoints => CanGAS ? _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] : HitPoints.TargetValue;

        public MoveValue<XORFloat64>[] Points { get; set; }

        public MoveValue<XORInt32>[] Stands { get; set; }

        public MoveValue<XORFloat64>[] HitPointsValues { get; set; }

        public Primitive<bool>[] IsFs { get; set; }

        public XORInt32 HighestBand { get; set; }

        public bool IsMediaHandling { get; set; }

        public bool AlwaysNotP2Position { get; set; }

        public bool SetStop { get; set; }

        public double HandlingBPM { get; set; }

        public double NoteMobilityCosine { get; set; }

        public double NoteMobilityValue { get; set; }

        public double FaintCosine { get; set; }

        public double FaintLayered { get; set; }

        public double LengthLayered { get; set; }

        public bool IsPausing
        {
            get => _isPausing;

            set
            {
                _isPausing = value;
                ViewModels.Instance.MainValue.NotifyIsPausing();
            }
        }

        public bool SetPass { get; set; }

        public bool SetEscape { get; set; }

        public bool IsSilent { get; set; }

        public int NoteFrame { get; set; }

        public HandledDrawingItem? NoteDrawing { get; set; }

        public double Status { get; set; }

        public double LoopingCounter { get; set; }

        public int PauseCount => (int)Math.Ceiling(_pauseMillis / 1000.0);

        public bool SetUndo { get; set; }

        public double LevyingMultiplier { get; set; }

        public double LevyingAudioMultiplier { get; set; }

        public double TotallyLevyingMultiplier { get; set; }

        public double TotallyLevyingAudioMultiplier { get; set; }

        public bool SetPause { get; set; }

        public void SetCompilingStatus(double value)
        {
            Status = value;
            Point.Value = value;
        }

        public bool IsPausingWindowOpened
        {
            get => _isPausingWindowOpened;

            set
            {
                if (_isPausingWindowOpened != value)
                {
                    _isPausingWindowOpened = value;
                    Utility.HandleUIAudio(value ? "Window 1" : "Window 0");
                }
            }
        }

        public void Migrate(DefaultCompute targetMigrateComputer)
        {
            AudioSystem.Instance.Migrate(TrailerAudioHandler, targetMigrateComputer.TrailerAudioHandler);
            targetMigrateComputer.TrailerAudioHandler.IsHandling = TrailerAudioHandler.IsHandling;

            AudioSystem.Instance.Migrate(this as IAudioContainer, targetMigrateComputer as IAudioContainer);
            MediaSystem.Instance.Migrate(this, targetMigrateComputer);
            DrawingSystem.Instance.Migrate(this, targetMigrateComputer);
        }

        public void PaintDefaultMedia(DrawingContext targetSession, ref Bound r, int defaultMediaFaint)
        {
            if (defaultMediaFaint < 100)
            {
                targetSession.PushOpacity(defaultMediaFaint / 100.0);
            }

            if (defaultMediaFaint > 0)
            {
                targetSession.DrawRectangle(Paints.Paint0, null, r);
                if (Configure.Instance.Media && IsMediaHandling)
                {
                    var defaultHandlerItem = GetHandlerItem(MediaNote.Mode.Default);
                    var layerHandlerItem = GetHandlerItem(MediaNote.Mode.Layer);
                    if ((defaultHandlerItem == null || defaultHandlerItem is DrawingHandlerItem) && (layerHandlerItem == null || layerHandlerItem is DrawingHandlerItem))
                    {
                        targetSession.DrawRectangle(Paints.Paint0, null, r);
                    }
                    HandleHandlerItem(defaultHandlerItem, ref r);
                    HandleHandlerItem(layerHandlerItem, ref r);

                    void HandleHandlerItem(IHandlerItem handler, ref Bound r)
                    {
                        switch (handler)
                        {
                            case DrawingHandlerItem drawingHandlerItem:
                                HandleDrawing(ref r, drawingHandlerItem.HandledDrawingItem);
                                break;
                            case MediaHandlerItem mediaHandlerItem:
                                var defaultMedia = mediaHandlerItem.HandledMediaItem.DefaultMedia;
                                if (defaultMedia != null)
                                {
                                    Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, defaultMedia.NaturalVideoWidth, defaultMedia.NaturalVideoHeight, r.Position0, r.Position1, r.Length, r.Height);
                                    targetSession.DrawVideo(defaultMedia, r);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    HandleDrawing(ref r, NoteDrawing ?? DrawingSystem.Instance.DefaultDrawing);
                }
            }

            if (defaultMediaFaint < 100)
            {
                targetSession.Pop();
            }

            void HandleDrawing(ref Bound r, HandledDrawingItem? drawingComputingValue)
            {
                var defaultDrawing = drawingComputingValue?.DefaultDrawing;
                if (defaultDrawing != null)
                {
                    Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, defaultDrawing.Width, defaultDrawing.Height, r.Position0, r.Position1, r.Length, r.Height);
                    targetSession.DrawImage(defaultDrawing, r);
                }
            }
        }

        public void PaintMedia(CanvasDrawingSession targetSession, ref Bound r, float mediaFaint)
        {
            if (mediaFaint < 1F)
            {
                targetSession.FillRectangle(r, DrawingSystem.Instance.FaintFilledPaints[(int)(100 * mediaFaint)]);
            }

            if (mediaFaint > 0F)
            {
                if (Configure.Instance.MediaInput && !Configure.Instance.FavorMediaInput)
                {
                    MediaInputSystem.Instance.PaintMediaInput(targetSession, ref r, mediaFaint);
                }
                else
                {
                    lock (LoadedCSX)
                    {
                        if (HasContents)
                        {
                            if (Configure.Instance.Media && IsMediaHandling)
                            {
                                var defaultHandlerItem = GetHandlerItem(MediaNote.Mode.Default);
                                var layerHandlerItem = GetHandlerItem(MediaNote.Mode.Layer);
                                HandleHandlerItem(defaultHandlerItem, ref r);
                                HandleHandlerItem(layerHandlerItem, ref r);

                                void HandleHandlerItem(IHandlerItem handler, ref Bound r)
                                {
                                    switch (handler)
                                    {
                                        case DrawingHandlerItem drawingHandlerItem:
                                            HandleDrawing(ref r, drawingHandlerItem.HandledDrawingItem);
                                            break;
                                        case MediaHandlerItem mediaHandlerItem:
                                            var mediaFrame = mediaHandlerItem.MediaFrame;
                                            if (mediaFrame != null)
                                            {
                                                var mediaFrameBound = mediaFrame.Bounds;
                                                Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, mediaFrameBound.Width, mediaFrameBound.Height, r.Position0, r.Position1, r.Length, r.Height);
                                                targetSession.PaintDrawing(ref r, mediaFrame, mediaFaint);
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                HandleDrawing(ref r, NoteDrawing ?? DrawingSystem.Instance.DefaultDrawing);
                            }

                            void HandleDrawing(ref Bound r, HandledDrawingItem? drawingComputingValue)
                            {
                                var drawing = drawingComputingValue?.Drawing;
                                if (drawing.HasValue)
                                {
                                    var drawingValue = drawing.Value;
                                    var drawingBound = drawingValue.DrawingBound;
                                    Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, drawingBound.Length, drawingBound.Height, r.Position0, r.Position1, r.Length, r.Height);
                                    targetSession.PaintDrawing(ref r, drawingValue, mediaFaint);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void SetCommentPlaceText()
        {
            if (NetItems.Count > 0)
            {
                var i = NetItems.Single(netItem => netItem.IsMyNetItem).TargetPosition;
                CommentPlace0Text = i > 50 ? $"＃＞{i}" : $"＃{i + 1}";
                CommentPlace1Text = $"／{_totalComments + 1}";
            }
            else
            {
                CommentPlace0Text = string.Empty;
                CommentPlace1Text = string.Empty;
            }
        }

        public void NotifyCompute(int delta = 0)
        {
            LevyingComputingPosition += delta;
            Title = NoteFile.Title;
            Artist = NoteFile.Artist;
            Genre = NoteFile.Genre;
            LevelValue = NoteFile.LevelValue;
            LevelText = NoteFile.LevelText;
            PlatformText = NoteFile.PlatformText;
            JudgmentStage = NoteFile.JudgmentStage;
            HitPointsValue = NoteFile.HitPointsValue;
            HighestInputCount = NoteFile.HighestInputCount;
            Length = NoteFile.Length;
            BPM = NoteFile.BPM;
            InputMode = NoteFile.InputMode;
            TotalNotes = NoteFile.TotalNotes;
            if (PendingQuitNetItems != null)
            {
                var quitNetItem = PendingQuitNetItems[LevyingComputingPosition];
                InheritedTotalNotes = TotalNotes;
                InheritedLowestJudgment = Comment.LowestJudgment;
                IsF.SetValue(quitNetItem.isF);
                HighestBand = quitNetItem.highestBand;
                LevyingMultiplier = quitNetItem.multiplier;
                LevyingAudioMultiplier = quitNetItem.audioMultiplier;
                NetPosition = quitNetItem.netPosition;
            }
            SetCommentPlaceText();
            var date = Version.Parse(Comment.Date);
            if (!Utility.IsLowerDate(date, 1, 7, 0))
            {
                LevyingMultiplier = Comment.LevyingMultiplier;
                LevyingAudioMultiplier = Comment.LevyingAudioMultiplier;
            }

            lock (JudgmentMeterEventValues)
            {
                foreach (var judgmentMeterEventValues in JudgmentMeterEventValues)
                {
                    judgmentMeterEventValues.Clear();
                }
                foreach (var judgmentMeterEvent in Comment.JudgmentMeters)
                {
                    var judgmentMeter = judgmentMeterEvent.JudgmentMeter;
                    var loopingCounter = judgmentMeterEvent.Wait;
                    if (Utility.IsLowerDate(date, 1, 3, 11))
                    {
                        loopingCounter -= judgmentMeter;
                    }
                    judgmentMeter = Math.Clamp(judgmentMeter, Component.GetJudgmentMillis(Component.Judged.Lowest, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, 0, (Component.JudgmentAssist)judgmentMeterEvent.Assist), Component.GetJudgmentMillis(Component.Judged.Lowest, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, 1, (Component.JudgmentAssist)judgmentMeterEvent.Assist));
                    JudgmentMeterEventValues[(int)Component.GetJudged(judgmentMeter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, (Component.JudgmentAssist)judgmentMeterEvent.Assist)].Add(KeyValuePair.Create(loopingCounter, judgmentMeter));
                }
                var judgmentMeters = Comment.JudgmentMeters.Select(judgmentMeterEvent => judgmentMeterEvent.JudgmentMeter).ToArray();
                var judgmentMetersLength = judgmentMeters.Length;
                if (judgmentMetersLength > 0)
                {
                    var average = judgmentMeters.Sum(judgmentMeter => judgmentMeter) / judgmentMetersLength;
                    var absAverage = judgmentMeters.Sum(judgmentMeter => Math.Abs(judgmentMeter)) / judgmentMetersLength;
                    JudgmentMeterText = $"AVG: {average:0.###} ms, |AVG|: {absAverage:0.###} ms, STD: {Math.Sqrt(judgmentMeters.Sum(judgmentMeter => Math.Pow(average - judgmentMeter, 2)) / judgmentMetersLength):0.###} ms";
                    var lowJudgmentMeters = judgmentMeters.Where(judgmentMeter => Math.Abs(judgmentMeter) >= Configure.Instance.JudgmentMeterMillis).ToArray();
                    EarlyLateText = $"Early: {lowJudgmentMeters.Count(lowJudgmentMeter => lowJudgmentMeter < 0.0)}, Late: {lowJudgmentMeters.Count(lowJudgmentMeter => lowJudgmentMeter > 0.0)}";
                }
                else
                {
                    JudgmentMeterText = string.Empty;
                    EarlyLateText = string.Empty;
                }
            }

            var paintEvents = (_paintEventsDate == Component.PaintEventsDate._1_0_0 ? Comment.Paints.Select((paintEvent, i) =>
            {
                paintEvent.Wait = i * Length / (Comment.Paints.Count - 1);
                return paintEvent;
            }) : Comment.Paints).Where(paintEvent => 0.0 <= paintEvent.Wait && paintEvent.Wait <= Length).ToArray();
            lock (EventValuesCSX)
            {
                HitPointsEventValues.Clear();
                PointEventValues.Clear();
                StandEventValues.Clear();
                BandEventValues.Clear();
                if (paintEvents.Length > 0)
                {
                    for (var i = 100; i >= 0; --i)
                    {
                        var wait = Length * i / 100;
                        var lastPaintEvent = paintEvents.LastOrDefault(paintEvent => paintEvent.Wait <= wait) ?? new()
                        {
                            Wait = wait,
                            HitPoints = 1.0,
                            Point = 1.0
                        };
                        HitPointsEventValues.Add(KeyValuePair.Create(wait / Length, 1.0 - lastPaintEvent.HitPoints));
                        PointEventValues.Add(KeyValuePair.Create(wait / Length, 1.0 - lastPaintEvent.Point));
                        StandEventValues.Add(KeyValuePair.Create(wait / Length, Math.Min((double)(InheritTotalStand - lastPaintEvent.Stand) / InheritTotalStand, 1.0)));
                        BandEventValues.Add(KeyValuePair.Create(wait / Length, Math.Min((double)(InheritedTotalNotes - lastPaintEvent.Band) / InheritedTotalNotes, 1.0)));
                    }
                }
            }
            SendSituation();
        }

        public float GetPosition(int input)
        {
            var lowerInput = (int)input;
            return (float)(DrawingComponentValue.mainPosition + DrawingComponentValue.MainNoteLengthLevyingMap[lowerInput] + DrawingComponentValue.DrawingNoteLengthMap[lowerInput] * (input - lowerInput) + (IsIn2P[lowerInput] && DrawingComponentValue.MainNoteLengthMap[lowerInput] > 0F ? DrawingComponentValue.p2Position : 0F));
        }

        public void SetUIMap()
        {
            AlwaysNotP2Position = Configure.Instance.AlwaysNotP2Position;
            InputMappingValue = Configure.Instance.InputMappingValue;
            var inputCount = Component.InputCounts[(int)InputMode];
            if (AlwaysNotP2Position)
            {
                InputCount1P = inputCount;
                IsIn2P = new bool[inputCount + 1];
                Has2P = false;
            }
            else
            {
                InputCount1P = Component.InputCounts1P[(int)InputMode];
                IsIn2P = Component.IsIn2P[(int)InputMode];
                Has2P = Component.Has2P[(int)InputMode];
            }
            lock (UI.Instance.LoadedCSX)
            {
                if (string.IsNullOrEmpty(UI.Instance.FaultText))
                {
                    DrawingComponentValue.SetValue(this);

                    foreach (var note in Notes)
                    {
                        if (note.HasInput)
                        {
                            note.SetLayer(this);
                        }
                    }

                    lock (JudgmentPaints)
                    {
                        JudgmentPaints.Clear();
                    }
                    lock (HitNotePaints)
                    {
                        HitNotePaints.Clear();
                    }
                    lock (HitLongNotePaints)
                    {
                        HitLongNotePaints.Clear();
                    }
                    HighestNetLength = 0F;
                    HighestNetHeight = 0F;
                    LevelFrame = 0;
                    NoteFrame = 0;
                    AutoMainFrame = 0;
                    Array.Clear(JudgmentMainFrames, 0, JudgmentMainFrames.Length);
                    Array.Clear(MainJudgmentMeterFrames, 0, MainJudgmentMeterFrames.Length);
                    Array.Clear(PaintPropertyFrames, 0, PaintPropertyFrames.Length);
                    for (var i = BandDrawingFrames.Count - 1; i >= 0; --i)
                    {
                        BandDrawingFrames[i] = 0;
                    }
                    for (var i = BandEnlargedMap.Count - 1; i >= 0; --i)
                    {
                        BandEnlargedMap[i] = 0.0;
                    }

                    switch (UI.Instance.LoopingMain)
                    {
                        case 0:
                        case 1:
                            Array.Fill(MainFrames, 0);
                            Array.Fill(_targetMainFrames, 0);
                            break;
                        case 2:
                            var mainFrame = DrawingComponentValue.mainFrame;
                            Array.Fill(MainFrames, mainFrame);
                            Array.Fill(_targetMainFrames, mainFrame);
                            break;
                    }
                    switch (UI.Instance.LoopingInput)
                    {
                        case 0:
                        case 1:
                            Array.Fill(InputFrames, 0);
                            Array.Fill(_targetInputFrames, 0);
                            break;
                        case 2:
                            var inputFrame = DrawingComponentValue.inputFrame;
                            Array.Fill(InputFrames, inputFrame);
                            Array.Fill(_targetInputFrames, inputFrame);
                            break;
                    }

                    ViewModels.Instance.MainValue.ModeComponentValue.NotifyIIDXMultiplierMillisText();

                    DrawingSystem.Instance.SetFontSystem(UI.Instance.TitleFont, DrawingComponentValue.titleSystem0, DrawingComponentValue.titleSystem1);
                    DrawingSystem.Instance.SetFontSystem(UI.Instance.ArtistFont, DrawingComponentValue.artistSystem0, DrawingComponentValue.artistSystem1);
                    DrawingSystem.Instance.SetFontSystem(UI.Instance.GenreFont, DrawingComponentValue.genreSystem0, DrawingComponentValue.genreSystem1);
                    DrawingSystem.Instance.SetFontSystem(UI.Instance.LevelTextFont, DrawingComponentValue.levelTextSystem0, DrawingComponentValue.levelTextSystem1);
                    DrawingSystem.Instance.SetFontSystem(UI.Instance.WantLevelFont, DrawingComponentValue.wantLevelSystem0, DrawingComponentValue.wantLevelSystem1);

                    NetDrawingComponentValue = new()
                    {
                        P2BuiltLength = (float)(DrawingComponentValue.p2BuiltLength + (Has2P ? DrawingComponentValue.p2Position : 0F)),
                        JudgmentMainPosition = DrawingComponentValue.judgmentMainPosition
                    };
                }
            }
        }

        public void HandleComputer()
        {
            lock (_targetHandlerCSX)
            {
                if (IsHandling)
                {
                    _targetHandler.Interrupt();
                    _targetHandler.Join();
                }
                IsHandling = true;
                _targetHandler = Utility.HandleParallelly(HandleNotes);
            }
            SendSituation();
        }

        public object LoadedCSX { get; } = new();

        public bool HasContents { get; set; } = true;

        public void Close()
        {
            IsSilent = true;
            SetStop = true;
            SendNotCompiled();
            lock (_targetHandlerCSX)
            {
                if (IsHandling)
                {
                    _targetHandler.Interrupt();
                    _targetHandler.Join();
                }
            }
            TrailerAudioHandler.Stop();
            Task.Run(() =>
            {
                if (_targetCompiler != null)
                {
                    lock (_targetCompiler)
                    {
                        if (_targetCompilerStatus == CompilerStatus.Compiling)
                        {
                            _setCancelCompiler.Cancel();
                            _setCancelCompiler.Dispose();
                            MediaModifierValue.StopModifyMedia();
                            _targetCompilerHandler.Join();
                            _targetCompilerStatus = CompilerStatus.Close;
                        }
                    }
                }
                lock (LoadedCSX)
                {
                    if (HasContents)
                    {
                        AudioSystem.Instance.Stop(this);
                        AudioSystem.Instance.Close(this, this);
                        MediaSystem.Instance.Stop(this);
                        MediaSystem.Instance.Close(this, this);
                        DrawingSystem.Instance.Close(this);
                        HasContents = false;
                    }
                }
            });
        }

        public void LowerMultiplier()
        {
            if (IsHandling)
            {
                MultiplierQueue.Enqueue(double.NegativeInfinity);
            }
        }

        public void HigherMultiplier()
        {
            if (IsHandling)
            {
                MultiplierQueue.Enqueue(double.PositiveInfinity);
            }
        }

        public void LowerAudioMultiplier()
        {
            if (IsHandling)
            {
                AudioMultiplierQueue.Enqueue(double.NegativeInfinity);
            }
        }

        public void HigherAudioMultiplier()
        {
            if (IsHandling)
            {
                AudioMultiplierQueue.Enqueue(double.PositiveInfinity);
            }
        }

        void SetInheritedValues()
        {
            InheritedTotalNotes += TotalNotes;
            InheritedHighestJudgment += Comment.HighestJudgment;
            InheritedHigherJudgment += Comment.HigherJudgment;
            InheritedHighJudgment += Comment.HighJudgment;
            InheritedLowJudgment += Comment.LowJudgment;
            InheritedLowerJudgment += Comment.LowerJudgment;
            InheritedLowestJudgment += Comment.LowestJudgment;
        }

        public void SetQuitMode()
        {
            SetStop = true;
            SetInheritedValues();
            ViewModels.Instance.MainValue.SetQuitMode(this);
        }

        public virtual void SetNoteFileMode(string faultText = null)
        {
            SetStop = true;
            ViewModels.Instance.MainValue.SetNoteFileMode(faultText);
        }

        public virtual void AtNoteFileMode()
        {
            SetAutoNoteWait();
            Close();
        }

        public void OnStopped()
        {
            if (!IsSilent)
            {
                SetNoteFileMode();
            }
        }

        public override void OnCompiled()
        {
            if (!IsSilent)
            {
                ViewModels.Instance.MainValue.CloseAutoComputer();
                try
                {
                    SetUIMap();
                }
                catch (Exception e)
                {
                    throw new ScriptRuntimeException(e);
                }
                HandleComputer();
            }
        }

        public virtual void OnFault(ScriptRuntimeException e)
        {
            if (!IsSilent)
            {
                SetNoteFileMode(string.Format(LanguageSystem.Instance.UIFaultText, e.Message));
            }
        }

        public override void OnFault(Exception e)
        {
            if (!IsSilent)
            {
                SetNoteFileMode(string.Format(LanguageSystem.Instance.CompileFaultText, e.Message));
            }
        }

        public void InitNetComments()
        {
            _wasGetNetComments = false;
            _wasNetItems = false;
            _wasGetNetItems = false;
        }

        public void AutoPause()
        {
            if (IsHandling && CanPause && !CanSetPosition && ValidatedTotalNotes > 0 && LastStatusValue == LastStatus.Not && !QwilightComponent.IsVS)
            {
                Pause();
            }
        }

        public void Pause()
        {
            IsPausingWindowOpened = true;
            if (CanPause)
            {
                SetPause = true;
                if (ValidatedTotalNotes > 0 && _validJudgedNotes > 0 && LastStatusValue == LastStatus.Not)
                {
                    _isPaused = true;
                }
                if (_sentIOAvatarIDs.Count > 0)
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.IoPause, new
                    {
                        avatarIDs = _sentIOAvatarIDs,
                        handlerID = HandlerID,
                        isPaused = true
                    });
                }
            }
        }

        public void Unpause()
        {
            IsPausingWindowOpened = false;
            SetPause = false;
            if (_sentIOAvatarIDs.Count > 0)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.IoPause, new
                {
                    avatarIDs = _sentIOAvatarIDs,
                    handlerID = HandlerID,
                    isPaused = false
                });
            }
        }

        public bool IsSuitableAsInput(int input) => ModeComponentValue.AutoModeValue switch
        {
            ModeComponent.AutoMode.Default => true,
            ModeComponent.AutoMode.Autoable => Array.IndexOf(Component.AutoableInputs[(int)InputMode], input) == -1,
            _ => default,
        };

        public virtual bool IsSuitableAsHwInput(int input) => IsSuitableAsInput(input);

        public virtual bool IsSuitableAsAutoJudge(int input) => ModeComponentValue.AutoModeValue switch
        {
            ModeComponent.AutoMode.Default => false,
            ModeComponent.AutoMode.Autoable => Component.AutoableInputs[(int)InputMode].Contains(input),
            _ => default,
        };

        public void PostItem(int postableItemPosition)
        {
            PostItemQueue.Enqueue(postableItemPosition);
        }

        public void Input(int input, InputFlag inputFlag = InputFlag.Not, byte inputPower = byte.MaxValue)
        {
            if (IsSuitableAsHwInput(Math.Abs(input)))
            {
                HandleInput(input, inputFlag, inputPower);
            }
        }

        public void HandleInput(int input, InputFlag inputFlag = InputFlag.Not, byte inputPower = byte.MaxValue)
        {
            if (IsHandling && !IsPausing)
            {
                _rawInputQueue.Enqueue((input, inputPower));
                if (input > 0)
                {
                    _hitLongNotePaintMillis[input] = 0.0;
                }
                _inputFlags |= inputFlag;
            }
        }

        public bool IsAutoMode { get; set; }

        public MediaModifier MediaModifierValue { get; } = new();

        public void ModifyAutoMode()
        {
            if (CanModifyAutoMode && !SetPause)
            {
                IsAutoMode = !IsAutoMode;
            }
        }

        public enum CompilerStatus
        {
            Not, Compiling, Close
        }

        public enum LastStatus
        {
            Not, Last, Band1, Yell1
        }

        public double AudioLength { get; set; }

        public Component.CommentWaitDate CommentWaitDate { get; }

        public Component.NoteSaltModeDate NoteSaltModeDate { get; }

        public bool BanalMedia { get; } = Configure.Instance.BanalMedia;

        public bool BanalFailedMedia { get; } = Configure.Instance.BanalFailedMedia;

        public bool AlwaysBanalMedia { get; } = Configure.Instance.AlwaysBanalMedia;

        public bool AlwaysBanalFailedMedia { get; } = Configure.Instance.AlwaysBanalFailedMedia;

        public string BanalMediaFilePath { get; } = Configure.Instance.BanalMediaFilePath;

        public string BanalFailedMediaFilePath { get; } = Configure.Instance.BanalFailedMediaFilePath;

        public double[][] JudgmentInputValues { get; } = new double[100][];

        public SortedDictionary<double, List<AudioNote>> WaitAudioNoteMap { get; } = new();

        public SortedDictionary<double, List<AudioNote>> WaitInputAudioMap { get; } = new();

        public SortedDictionary<double, List<MediaNote>> WaitMediaNoteMap { get; } = new();

        public SortedDictionary<int, double> MeterWaitMap { get; } = new();

        public SortedDictionary<double, double> WaitBPMMap { get; } = new();

        public SortedDictionary<double, double> WaitLogicalYMap { get; } = new();

        public List<double> AutoableInputNoteCounts { get; } = new();

        public List<double> InputNoteCounts { get; } = new();

        public bool IsYell1 => Point.TargetValue == 1.0;

        public bool IsBand1 => InheritedLowestJudgment == 0;

        public virtual QuitStatus QuitStatusValue => Utility.GetQuitStatusValue(Point.TargetValue, Stand.TargetValue, IsF ? 0.0 : HitPoints.TargetValue, NoteFiles.Length);

        public virtual double HandledLength => Length;

        public virtual LastStatus LastStatusValue => _lastStatus;

        public virtual bool IsPowered => true;

        public virtual bool HandleFailedAudio => Configure.Instance.HandleFailedAudio.Data != ViewItem.Not;

        public virtual bool ViewFailedDrawing => Configure.Instance.ViewFailedDrawing.Data != ViewItem.Not;

        public virtual bool ViewLowestJudgment => Configure.Instance.ViewLowestJudgment.Data != ViewItem.Not;

        public double PassableWait { get; set; }

        public int ValidatedTotalNotes { get; set; }

        public IHandledItem LoopingBanalMedia { get; set; }

        public IHandledItem LoopingBanalFailedMedia { get; set; }

        void SendPostItem(PostableItem postableItem)
        {
            var isPositive = postableItem.IsPositive;
            TwilightSystem.Instance.SendParallel(new()
            {
                EventID = Event.Types.EventID.PostItem,
                QwilightPostItem = new()
                {
                    SiteID = SiteID,
                    HandlerID = HandlerID,
                    PostedItem = (int)postableItem.VarietyValue,
                    IsPositive = AvatarsCount > 1 && isPositive.HasValue ? isPositive.Value ? 1 : 0 : -1,
                    LowestWait = postableItem.LowestWait,
                    HighestWait = postableItem.HighestWait
                }
            });
        }

        void SendIOInput(int input, byte inputPower)
        {
            if (_sentIOAvatarIDs.Count > 0)
            {
                TwilightSystem.Instance.SendParallel(new()
                {
                    EventID = Event.Types.EventID.IoInput,
                    QwilightIOInput = new()
                    {
                        AvatarIDs =
                        {
                            _sentIOAvatarIDs
                        },
                        HandlerID = HandlerID,
                        Input = input,
                        Power = inputPower
                    }
                });
            }
        }

        void SendIOJudge(BaseNote note)
        {
            if (_sentIOAvatarIDs.Count > 0 && note.ID != -1)
            {
                TwilightSystem.Instance.SendParallel(new()
                {
                    EventID = Event.Types.EventID.IoJudge,
                    QwilightIOJudge = new()
                    {
                        AvatarIDs =
                        {
                            _sentIOAvatarIDs
                        },
                        HandlerID = HandlerID,
                        NoteID = note.ID,
                        Judged = (int)note.Judged
                    }
                });
            }
        }

        void SendIOJudgmentMeter(int input, double judgmentMeter, Component.JudgmentAssist judgmentAssist)
        {
            if (_sentIOAvatarIDs.Count > 0)
            {
                TwilightSystem.Instance.SendParallel(new()
                {
                    EventID = Event.Types.EventID.IoJudgmentMeter,
                    QwilightIOJudgmentMeter = new()
                    {
                        AvatarIDs =
                        {
                            _sentIOAvatarIDs
                        },
                        HandlerID = HandlerID,
                        Input = input,
                        JudgmentMeter = judgmentMeter,
                        Assist = (int)judgmentAssist
                    }
                });
            }
        }

        void SendIONoteVisibility(BaseNote note, bool setValidJudgedNotes, bool setNoteFailed)
        {
            if (_sentIOAvatarIDs.Count > 0 && note.ID != -1)
            {
                TwilightSystem.Instance.SendParallel(new()
                {
                    EventID = Event.Types.EventID.IoNoteVisibility,
                    QwilightIONoteVisibility = new()
                    {
                        AvatarIDs =
                        {
                            _sentIOAvatarIDs
                        },
                        HandlerID = HandlerID,
                        NoteID = note.ID,
                        SetValidJudgedNotes = setValidJudgedNotes,
                        SetNoteFailed = setNoteFailed
                    }
                });
            }
        }

        public int SetNetItems(List<NetItem> netItems)
        {
            if (Configure.Instance.HunterVarietyV2Value.Mode == HunterVariety.HunterVarietyFavor)
            {
                netItems.Add(new(string.Empty, "Qwilight", DateTime.Now)
                {
                    HitPointsModeValue = ModeComponent.HitPointsMode.Default,
                    IsFavorNetItem = true
                });
            }
            var targetNetItems = netItems.OrderByDescending(netItem => netItem.StandValue).ToList();
            for (var i = targetNetItems.Count - 1; i >= 0; --i)
            {
                targetNetItems[i].DrawingPosition = i;
                targetNetItems[i].TargetPosition = i;
            }
            NetItems = targetNetItems;
            return targetNetItems.Count;
        }

        public virtual void HandleWarning()
        {
            if (ModeComponentValue.IsAudioMultiplierWarning)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.AudioMultiplierWarning, ModeComponentValue.AudioMultiplier.ToString("×0.00")));
            }
            if (!Configure.Instance.AllowTwilightComment && !IsBanned && !IsPostableItemMode && ModeComponentValue.CanBeTwilightComment && TwilightSystem.Instance.IsLoggedIn)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.TwilightCommentWarning);
            }
            if (ModeComponentValue.HitPointsModeValue == ModeComponent.HitPointsMode.Test && string.IsNullOrEmpty(EventNoteEntryItem?.EventNoteID))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.TestHitPointsModeWarning);
            }
        }

        public DefaultCompute(BaseNoteFile[] noteFiles, Comment[] comments, ModeComponent defaultModeComponentValue, string avatarID, string avatarName, string ubuntuID = null, WwwLevelData wwwLevelDataValue = null, string handlerID = "", EntryItem eventNoteEntryItem = null, DefaultCompute lastComputer = null)
        {
            NoteFiles = noteFiles;
            MyNoteFiles = noteFiles;
            Stands = new MoveValue<XORInt32>[NoteFiles.Length];
            // Array.Fill은 같은 객체 참조를 의도한 것
            Array.Fill(Stands, new());
            Points = new MoveValue<XORFloat64>[NoteFiles.Length];
            Array.Fill(Points, new(1.0));
            HitPointsValues = new MoveValue<XORFloat64>[NoteFiles.Length];
            Array.Fill(HitPointsValues, new(1.0));
            Array.Fill(_hitPointsGAS, HitPoints.TargetValue);
            IsFs = new Primitive<bool>[NoteFiles.Length];
            Array.Fill(IsFs, new(false));
            ModeComponentValues = new ModeComponent[NoteFiles.Length];
            var modeComponent = ViewModels.Instance.MainValue.ModeComponentValue;
            CompatibleModeComponentValue = modeComponent.Clone();
            Array.Fill(ModeComponentValues, modeComponent);
            AvatarIDs = new string[NoteFiles.Length];
            Array.Fill(AvatarIDs, avatarID);
            AvatarNames = new string[NoteFiles.Length];
            Array.Fill(AvatarNames, avatarName);
            UbuntuID = ubuntuID;
            LevyingMultiplier = ModeComponentValue.Multiplier;
            LevyingAudioMultiplier = AudioMultiplier;
            InheritTotalStand = 1000000 * NoteFiles.Length;
            DefaultModeComponentValue = defaultModeComponentValue;
            _isLevyingComputer = lastComputer == null;
            if (_isLevyingComputer)
            {
                TotallyLevyingMultiplier = LevyingMultiplier;
                TotallyLevyingAudioMultiplier = LevyingAudioMultiplier;
                WwwLevelDataValue = wwwLevelDataValue;
            }
            else
            {
                _lazyInit = new(() =>
                {
                    HitPoints.TargetValue = lastComputer.HitPoints.TargetValue;
                    Point.TargetValue = lastComputer.Point.TargetValue;
                    Band.TargetValue = lastComputer.Band.TargetValue;
                    Stand.TargetValue = lastComputer.Stand.TargetValue;
                    HighestBand = lastComputer.HighestBand;
                    TotalPoint = lastComputer.TotalPoint;
                    SavedPoint = lastComputer.SavedPoint;
                    EarlyValue = lastComputer.EarlyValue;
                    LateValue = lastComputer.LateValue;
                    HasFailedJudgment = lastComputer.HasFailedJudgment;
                    IsF.SetValue(lastComputer.IsF);
                    _isPaused = lastComputer._isPaused;
                    _inputFlags = lastComputer._inputFlags;
                    Array.Copy(lastComputer._hitPointsGAS, _hitPointsGAS, _hitPointsGAS.Length);
                });
                LastStand = lastComputer.Stand.TargetValue;
                TotallyLevyingMultiplier = lastComputer.TotallyLevyingMultiplier;
                TotallyLevyingAudioMultiplier = lastComputer.TotallyLevyingAudioMultiplier;
                WwwLevelDataValue = lastComputer.WwwLevelDataValue;
                InheritedTotalNotes = lastComputer.InheritedTotalNotes;
                InheritedHighestJudgment = lastComputer.InheritedHighestJudgment;
                InheritedHigherJudgment = lastComputer.InheritedHigherJudgment;
                InheritedHighJudgment = lastComputer.InheritedHighJudgment;
                InheritedLowJudgment = lastComputer.InheritedLowJudgment;
                InheritedLowerJudgment = lastComputer.InheritedLowerJudgment;
                InheritedLowestJudgment = lastComputer.InheritedLowestJudgment;
                LevyingComputingPosition = lastComputer.LevyingComputingPosition + 1;
                HighestComputingPosition = lastComputer.HighestComputingPosition + 1;
                _comments = lastComputer._comments;
            }
            if (comments != null)
            {
                Comments = comments;
            }
            else
            {
                Comments = new Comment[NoteFiles.Length];
                for (var i = Comments.Length - 1; i >= 0; --i)
                {
                    Comments[i] = new()
                    {
                        Date = QwilightComponent.DateText,
                        LoopUnit = Configure.Instance.LoopUnit
                    };
                }
            }
            WwwLevelDataValue?.SetSatisify(this);
            EventNoteEntryItem = eventNoteEntryItem;
            HandlerID = handlerID ?? Guid.NewGuid().ToString();
            for (var i = 0; i < PostableItemFaints.Length; ++i)
            {
                PostableItemFaints[i] = new();
            }
            PostableItemStatusMap = Enumerable.Range(0, PostableItem.Values.Length).ToFrozenDictionary(i => PostableItem.Values[i], i => new PostableItemStatus());
            for (var i = _paintEventsGAS.Length - 1; i >= 0; --i)
            {
                _paintEventsGAS[i] = new();
            }
            for (var i = JudgmentVisualizerValues.Length - 1; i >= 0; --i)
            {
                JudgmentVisualizerValues[i] = new();
            }
            for (var i = (int)Component.Judged.Lowest; i >= (int)Component.Judged.Highest; --i)
            {
                JudgmentMeterEventValues[i] = new();
            }
            for (var i = JudgmentInputValues.Length - 1; i >= 0; --i)
            {
                JudgmentInputValues[i] = new double[6];
            }
            var date = Version.Parse(Comment.Date);
            _judgmentModeDate = Utility.GetDate<Component.JudgmentModeDate>(date, "1.6.7", "1.10.34", "1.10.35", "1.14.6");
            _judgmentMapDate = Utility.GetDate<Component.JudgmentMapDate>(date, "1.3.0", "1.6.7", "1.6.8", "1.10.34", "1.10.35", "1.11.0");
            _longNoteAssistDate = Utility.GetDate<Component.LongNoteAssistDate>(date, "1.6.7", "1.10.34", "1.10.35");
            _trapNoteJudgmentDate = Utility.GetDate<Component.TrapNoteJudgmentDate>(date, "1.14.6");
            _paintEventsDate = Utility.GetDate<Component.PaintEventsDate>(date, "1.14.91");
            LongNoteModeDate = Utility.GetDate<Component.LongNoteModeDate>(date, "1.14.20", "1.16.4");
            _hitPointsModeDate = Utility.GetDate<Component.HitPointsModeDate>(date, "1.2.3", "1.10.34", "1.10.35", "1.14.62");
            _hitPointsMapDate = Utility.GetDate<Component.HitPointsMapDate>(date, "1.6.7", "1.7.0", "1.13.2");
            _pointMapDate = Utility.GetDate<Component.PointMapDate>(date, "1.6.7");
            _standMapDate = Utility.GetDate<Component.StandMapDate>(date, "1.14.118");
            _standModeDate = Utility.GetDate<Component.StandModeDate>(date, "1.6.7", "1.14.118");
            _tooLongLongNoteDate = Utility.GetDate<Component.TooLongLongNoteDate>(date, "1.13.107", "1.14.20", "1.14.29");
            _targetCompiler = BaseCompiler.GetCompiler(NoteFile, _setCancelCompiler);
            _targetCompilerHandler = Utility.GetParallelHandler(() => _targetCompiler.Compile(this, true));
            CommentWaitDate = Utility.GetDate<Component.CommentWaitDate>(date, "1.3.11", "1.6.4");
            NoteSaltModeDate = Utility.GetDate<Component.NoteSaltModeDate>(date, "1.14.27", "1.16.11");
            Title = NoteFile.Title;
            Artist = NoteFile.Artist;
            Genre = NoteFile.Genre;
            LevelValue = NoteFile.LevelValue;
            LevelText = NoteFile.LevelText;
            InputMode = NoteFile.InputMode;
            HandleWarning();
        }

        void SetIIDXInputAudioVariety(BaseNote note)
        {
            if (InputAudioVarietyValue == Configure.InputAudioVariety.IIDX)
            {
                _lastIIDXInputAudioNoteMap[note.TargetInput] = note.AudioNotes;
            }
        }

        public void LoadBanalMedia(bool isBanalMedia, bool isBanalFailedMedia, IProducerConsumerCollection<Action> parallelItems)
        {
            if (LoadedMedia)
            {
                if (isBanalMedia && !string.IsNullOrEmpty(BanalMediaFilePath))
                {
                    parallelItems.TryAdd(() =>
                    {
                        try
                        {
                            switch (Utility.GetFileFormat(BanalMediaFilePath))
                            {
                                case Utility.FileFormatFlag.Drawing:
                                    LoopingBanalMedia = new HandledDrawingItem
                                    {
                                        Drawing = DrawingSystem.Instance.Load(BanalMediaFilePath, this),
                                        DefaultDrawing = DrawingSystem.Instance.LoadDefault(BanalMediaFilePath, this)
                                    };
                                    break;
                                case Utility.FileFormatFlag.Media:
                                    LoopingBanalMedia = MediaSystem.Instance.Load(BanalMediaFilePath, this, true);
                                    break;
                            }
                        }
                        catch
                        {
                        }
                    });
                }
                if (isBanalFailedMedia && !string.IsNullOrEmpty(BanalFailedMediaFilePath))
                {
                    parallelItems.TryAdd(() =>
                    {
                        try
                        {
                            switch (Utility.GetFileFormat(BanalFailedMediaFilePath))
                            {
                                case Utility.FileFormatFlag.Drawing:
                                    LoopingBanalFailedMedia = new HandledDrawingItem
                                    {
                                        Drawing = DrawingSystem.Instance.Load(BanalFailedMediaFilePath, this),
                                        DefaultDrawing = DrawingSystem.Instance.LoadDefault(BanalFailedMediaFilePath, this)
                                    };
                                    break;
                                case Utility.FileFormatFlag.Media:
                                    LoopingBanalFailedMedia = MediaSystem.Instance.Load(BanalFailedMediaFilePath, this, true);
                                    break;
                            }
                        }
                        catch
                        {
                        }
                    });
                }
            }
        }

        void HandleNotes()
        {
            var lastMultiplier = ModeComponentValue.Multiplier;
            var lastAudioMultiplier = AudioMultiplier;
            var isValidLoopingCounter = true;
            var wait100 = Length / 100;
            var rawJudgmentInputCounts = new int[100][];
            for (var i = rawJudgmentInputCounts.Length - 1; i >= 0; --i)
            {
                rawJudgmentInputCounts[i] = new int[6];
            }
            var lastJudgmentInputMaxValue = 0;
            var logicalY = 0.0;
            var judgmentMeters = new JudgmentMeter[3];
            for (var i = judgmentMeters.Length - 1; i >= 0; --i)
            {
                judgmentMeters[i] = new();
            }
            var areValidJudgmentMeters = new bool[3];
            var waitBPMMap = new Queue<KeyValuePair<double, double>>(WaitBPMMap);
            var waitAudioNoteMap = new Queue<KeyValuePair<double, List<AudioNote>>>(WaitAudioNoteMap.ToList());
            var waitInputAudioMap = new Queue<KeyValuePair<double, List<AudioNote>>>(WaitInputAudioMap.ToList());
            var waitMediaNoteMap = new Queue<KeyValuePair<double, List<MediaNote>>>(WaitMediaNoteMap.ToList());
            var endNoteID = Notes.Count - 1;
            var wasLastStatus = false;
            var handlingNoteID = 0;
            var paintedNoteID = 0;
            var audioChannelMap = new Dictionary<string, Channel>();
            var loopingHandlerMillis = 0.0;
            var commentInputID = 0;
            var commentMultiplierID = 0;
            var commentAudioMultiplierID = 0;
            var randomDefaultInputs = Component.DefaultInputs[(int)InputMode].ToArray();
            var inputCount = Component.InputCounts[(int)InputMode];
            var postableItemBand = 0;
            var postableHitPointsCount = 0;
            var postableJudgmentCount = 0;
            _valueComponent = new(NoteFile.LevyingBPM, Comment.LoopUnit);

            var handleAudioNotesImpl = new Action<double, List<AudioNote>>((waitModified, audioNotes) =>
            {
                foreach (var audioNote in audioNotes)
                {
                    var audioItem = audioNote.AudioItem;
                    if (audioItem.HasValue)
                    {
                        var audioLength = audioNote.Length;
                        var audioItemValue = audioItem.Value;
                        if (LoopingCounter <= waitModified + (audioLength ?? audioItemValue.Length))
                        {
                            var bmsID = audioNote.BMSID;
                            StopLastEqualAudioItem(bmsID);
                            lock (LoadedCSX)
                            {
                                if (HasContents)
                                {
                                    var audioChannel = AudioSystem.Instance.Handle(new()
                                    {
                                        AudioItem = audioItemValue,
                                        AudioLevyingPosition = (uint)(LoopingCounter - waitModified + audioNote.AudioLevyingPosition),
                                        Length = audioLength != null ? (uint?)(waitModified + audioLength - LoopingCounter) : null,
                                        BMSID = bmsID
                                    }, AudioSystem.MainAudio, AudioMultiplier, IsCounterWave, this);
                                    if (!string.IsNullOrEmpty(bmsID))
                                    {
                                        audioChannelMap[bmsID] = audioChannel;
                                    }
                                }
                            }
                        }
                    }
                }
            });
            var handleMediaNotesImpl = new Action<double, List<MediaNote>>((waitModified, mediaNotes) =>
            {
                foreach (var mediaNote in mediaNotes)
                {
                    if (mediaNote.HasContents)
                    {
                        var mediaMode = mediaNote.MediaMode;
                        var mediaItem = mediaNote.MediaItem;
                        if (mediaItem == null)
                        {
                            MediaCollection[mediaMode] = null;
                            IsMediaHandling = true;
                        }
                        else if (LoopingCounter < waitModified + mediaItem.Length)
                        {
                            lock (LoadedCSX)
                            {
                                if (HasContents)
                                {
                                    MediaCollection[mediaMode] = mediaItem.Handle(this, mediaItem.IsLooping ? TimeSpan.Zero : TimeSpan.FromMilliseconds(waitModified) - mediaNote.MediaLevyingPosition, mediaMode);
                                    IsMediaHandling = true;
                                }
                            }
                        }
                    }
                }
            });

            try
            {
                Init();

                if (LevyingMeter >= 0)
                {
                    LevyingWait = MeterWaitMap[Math.Min(LevyingMeter, MeterWaitMap.Keys.Max())];
                    LevyingMeter = -1;
                }
                var levyingWait = LevyingWait;
                if (!double.IsNaN(levyingWait))
                {
                    levyingWait = Math.Round(levyingWait);
                    if (-Component.LevyingWait > levyingWait || Length + Component.QuitWait <= levyingWait)
                    {
                        levyingWait = 0.0;
                    }
                    foreach (var eventPosition in Notes.Where(note => note.HasStand).Select(note => note.Wait)
                        .Concat(Notes.Where(note => note.LongWait > 0.0).Select(note => note.Wait + note.LongWait))
                        .Concat(EventComment?.Inputs?.Select(inputEvent => Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, inputEvent.Wait)) ?? Enumerable.Empty<double>())
                        .Concat(EventComment?.Multipliers?.Select(multiplierEvent => Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, multiplierEvent.Wait)) ?? Enumerable.Empty<double>())
                        .Concat(EventComment?.AudioMultipliers?.Select(audioMultiplierEvent => Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, audioMultiplierEvent.Wait)) ?? Enumerable.Empty<double>())
                        .Where(wait => wait < levyingWait)
                        .Append(levyingWait)
                        .Distinct()
                        .Order())
                    {
                        _eventPositions.Enqueue(eventPosition);
                    }
                }

                var atLoopingCounter1000 = 0.0;
                var lastLoopingCounter = LoopingCounter;

                var postedAudioMultiplier = 1.0;
                var postedTrapNotesMillis = 1.0;
                var postedAutoableNotesMillis = 1.0;
                var postedSaltNotesMillis = 1.0;

                var ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - IOMillis;
                while (!SetStop && !SetUndo)
                {
                    if (LoopingCounter > HandledLength + Component.QuitWait && isValidLoopingCounter)
                    {
                        isValidLoopingCounter = false;
                        OnHandled();
                        continue;
                    }

                    if (!IsPausing && SetPause)
                    {
                        AudioSystem.Instance.Pause(this, true);
                        MediaSystem.Instance.Pause(this, true);
                        IsPausing = true;
                    }

                    var millisLoopUnit = _valueComponent.MillisLoopUnit;

                    var longNoteHitFrame = DrawingComponentValue.longNoteHitFrame;
                    var longNoteHitFramerate = DrawingComponentValue.longNoteHitFramerate;
                    var longNoteHitLoopFrame = DrawingComponentValue.longNoteHitLoopFrame;
                    var lastEnlargedBandLoopFrame = DrawingComponentValue.lastEnlargedBandLoopFrame;
                    var judgmentFrame = DrawingComponentValue.judgmentFrame;
                    var judgmentFramerate = DrawingComponentValue.judgmentFramerate;
                    var judgmentSystem = DrawingComponentValue.judgmentPaintSystem;
                    var judgmentPosition0 = DrawingComponentValue.judgmentPaintPosition0;
                    var judgmentPosition1 = DrawingComponentValue.judgmentPaintPosition1;
                    var judgmentLength = DrawingComponentValue.judgmentPaintLength;
                    var judgmentHeight = DrawingComponentValue.judgmentPaintHeight;
                    var mainFrame = DrawingComponentValue.mainFrame;
                    var inputFrame = DrawingComponentValue.inputFrame;
                    var inputPaintFrame = DrawingComponentValue.hitInputPaintFrame;

                    if (!_loopingHandler.IsRunning && !IsInEvents)
                    {
                        TrailerAudioHandler.Stop();
                        _loopingHandler.Start();
                    }

                    if (IsPostableItemMode)
                    {
                        while (PostItemQueue.TryDequeue(out var postableItemPosition))
                        {
                            if (!IsF)
                            {
                                if (PostableItems[0] != null ^ PostableItems[1] != null)
                                {
                                    if (PostableItems[0] != null)
                                    {
                                        postableItemPosition = 0;
                                    }
                                    else if (PostableItems[1] != null)
                                    {
                                        postableItemPosition = 1;
                                    }
                                }
                                var postableItem = PostableItems[postableItemPosition];
                                if (postableItem != null)
                                {
                                    PostableItems[postableItemPosition] = null;
                                    PostableItemFaints[postableItemPosition].TargetValue = 0.0;
                                    if (postableItem.IsPositive == false)
                                    {
                                        AudioSystem.Instance.Handle(new()
                                        {
                                            AudioItem = AudioSystem.Instance.PostedItemAudio
                                        }, AudioSystem.SEAudio);
                                    }
                                    SendPostItem(postableItem);
                                }
                            }
                        }

                        lock (TwilightPostItemQueue)
                        {
                            while (TwilightPostItemQueue.TryDequeue(out var twilightPostItem))
                            {
                                var postedItem = PostableItem.Values[twilightPostItem.PostedItem];
                                var aegisItem = PostableItem.Values[(int)PostableItem.Variety.PositiveAegis];
                                var aegisItemStatus = PostableItemStatusMap[aegisItem];
                                if (postedItem.IsPositive == false && aegisItemStatus.IsHandling)
                                {
                                    aegisItemStatus.Stop();
                                    AudioSystem.Instance.Handle(new()
                                    {
                                        AudioItem = AudioSystem.Instance.PostableItemAudioMap[aegisItem]
                                    }, AudioSystem.SEAudio);
                                    PostedItemText = PoolSystem.Instance.GetFormattedText(LanguageSystem.Instance.AegisPostedItemText, twilightPostItem.AvatarName, postedItem.ToString());
                                    PostedItemVariety = 1;
                                    PostedItemFaints[PostedItemVariety] = 1.0;
                                }
                                else
                                {
                                    var wasAegis = false;
                                    switch (postedItem.VarietyValue)
                                    {
                                        case PostableItem.Variety.PositiveJudgment:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeJudgment]].Halt();
                                            break;
                                        case PostableItem.Variety.PositiveHitPoints:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeHitPoints]].Halt();
                                            break;
                                        case PostableItem.Variety.PositiveHitPointsLevel:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeHitPointsLevel]].Halt();
                                            break;
                                        case PostableItem.Variety.PositiveAegis:
                                            foreach (var (postableItem, postableItemStatus) in PostableItemStatusMap)
                                            {
                                                if (postableItem.IsPositive == false && postableItemStatus.IsHandling)
                                                {
                                                    postableItemStatus.Stop();
                                                    wasAegis = true;
                                                }
                                            }
                                            break;
                                        case PostableItem.Variety.NegativeFaint:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeFading]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeFading:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeFaint]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeJudgment:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.PositiveJudgment]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeHitPoints:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.PositiveHitPoints]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeHitPointsLevel:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.PositiveHitPointsLevel]].Halt();
                                            break;
                                        case PostableItem.Variety.Negative4D:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeZip]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeZip:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.Negative4D]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeAutoableNotes:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeSalt]].Halt();
                                            break;
                                        case PostableItem.Variety.NegativeSalt:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeAutoableNotes]].Halt();
                                            break;
                                        case PostableItem.Variety.LowerAudioMultiplier:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.HigherAudioMultiplier]].Halt();
                                            break;
                                        case PostableItem.Variety.HigherAudioMultiplier:
                                            PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.LowerAudioMultiplier]].Halt();
                                            break;
                                    }
                                    if (!wasAegis)
                                    {
                                        PostableItemStatusMap[postedItem].Init(postedItem, twilightPostItem.AvatarName, twilightPostItem.Wait);
                                    }
                                    AudioSystem.Instance.Handle(new()
                                    {
                                        AudioItem = AudioSystem.Instance.PostableItemAudioMap.GetValueOrDefault(postedItem)
                                    }, AudioSystem.SEAudio);
                                    PostedItemText = PoolSystem.Instance.GetFormattedText(postedItem.IsPositive == true ? LanguageSystem.Instance.PositivePostedItemText : LanguageSystem.Instance.NegativePostedItemText, twilightPostItem.AvatarName, postedItem.ToString());
                                    PostedItemVariety = postedItem.IsPositive.HasValue ? postedItem.IsPositive.Value ? 1 : 0 : -1;
                                    PostedItemFaints[PostedItemVariety] = 1.0;
                                }
                            }
                        }

                        foreach (var (postableItem, postableItemStatus) in PostableItemStatusMap)
                        {
                            switch (postableItem.VarietyValue)
                            {
                                case PostableItem.Variety.PositiveHitPoints when HitPoints.TargetValue == 1.0:
                                case PostableItem.Variety.NegativeHitPoints when HitPoints.TargetValue == double.Epsilon:
                                    continue;
                            }
                            if (postableItemStatus.IsHandling)
                            {
                                if (!postableItemStatus.IsLevyed)
                                {
                                    switch (postableItem.VarietyValue)
                                    {
                                        case PostableItem.Variety.PositiveHitPointsLevel:
                                            ModeComponentValue.HitPointsModeValue = ModeComponent.HitPointsMode.Lowest;
                                            break;
                                        case PostableItem.Variety.NegativeHitPointsLevel:
                                            ModeComponentValue.HitPointsModeValue = ModeComponent.HitPointsMode.Higher;
                                            break;
                                        case PostableItem.Variety.NegativeFaint:
                                            ModeComponentValue.FaintNoteModeValue = ModeComponent.FaintNoteMode.Faint;
                                            break;
                                        case PostableItem.Variety.NegativeFading:
                                            ModeComponentValue.FaintNoteModeValue = ModeComponent.FaintNoteMode.Fading;
                                            break;
                                        case PostableItem.Variety.Negative4D:
                                            ModeComponentValue.NoteMobilityModeValue = ModeComponent.NoteMobilityMode._4D;
                                            break;
                                        case PostableItem.Variety.NegativeZip:
                                            ModeComponentValue.NoteMobilityModeValue = ModeComponent.NoteMobilityMode.Zip;
                                            break;
                                        case PostableItem.Variety.LowerAudioMultiplier:
                                            postedAudioMultiplier = 0.5;
                                            break;
                                        case PostableItem.Variety.HigherAudioMultiplier:
                                            postedAudioMultiplier = 1.5;
                                            break;
                                        case PostableItem.Variety.Pause:
                                            AudioSystem.Instance.Pause(this, true);
                                            MediaSystem.Instance.Pause(this, true);
                                            SetPause = true;
                                            IsPausing = true;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (postableItem.VarietyValue)
                                    {
                                        case PostableItem.Variety.PositiveHitPoints:
                                            HitPoints.TargetValue = Math.Min(HitPoints.TargetValue + millisLoopUnit / 1000.0 / (1000.0 / 60.0), 1.0);
                                            break;
                                        case PostableItem.Variety.NegativeHitPoints:
                                            HitPoints.TargetValue = Math.Max(double.Epsilon, HitPoints.TargetValue - millisLoopUnit / 1000.0 / (1000.0 / 60.0));
                                            break;
                                    }
                                }
                                switch (postableItem.VarietyValue)
                                {
                                    case PostableItem.Variety.PositiveHitPointsLevel:
                                    case PostableItem.Variety.NegativeHitPointsLevel:
                                        while (postableHitPointsCount > 0)
                                        {
                                            --postableHitPointsCount;
                                            if (postableItemStatus.Elapse(1000.0 / 60))
                                            {
                                                ModeComponentValue.HitPointsModeValue = ModeComponent.HitPointsMode.Default;
                                                postableHitPointsCount = 0;
                                                break;
                                            }
                                        }
                                        break;
                                    case PostableItem.Variety.PositiveJudgment:
                                    case PostableItem.Variety.NegativeJudgment:
                                        while (postableJudgmentCount > 0)
                                        {
                                            --postableJudgmentCount;
                                            if (postableItemStatus.Elapse(1000.0 / 60))
                                            {
                                                postableJudgmentCount = 0;
                                                break;
                                            }
                                        }
                                        break;
                                    case PostableItem.Variety.NegativeFaint:
                                        if (postableItemStatus.Elapse(millisLoopUnit))
                                        {
                                            ModeComponentValue.FaintNoteModeValue = ModeComponent.FaintNoteMode.Default;
                                        }
                                        break;
                                    case PostableItem.Variety.LowerAudioMultiplier:
                                    case PostableItem.Variety.HigherAudioMultiplier:
                                        if (postableItemStatus.Elapse(millisLoopUnit))
                                        {
                                            postedAudioMultiplier = 1.0;
                                        }
                                        break;
                                    case PostableItem.Variety.Pause:
                                        if (postableItemStatus.Elapse(millisLoopUnit))
                                        {
                                            AudioSystem.Instance.Pause(this, false);
                                            MediaSystem.Instance.Pause(this, false);
                                            SetPause = false;
                                            IsPausing = false;
                                        }
                                        break;
                                    case PostableItem.Variety.NegativeTrapNotes:
                                        postedTrapNotesMillis += millisLoopUnit;
                                        while (postedTrapNotesMillis >= 60.0)
                                        {
                                            postedTrapNotesMillis -= 60.0;
                                            var input = Utility.GetSaltedValue(randomDefaultInputs);
                                            var wait = Math.Floor(Math.Min(LoopingCounter + 1000.0 * Random.Shared.Next(1000) / 1000, Length + Component.QuitWait));
                                            var isTrapNote = true;
                                            foreach (var handlingNote in _handlingNotes)
                                            {
                                                if (input == handlingNote.TargetInput && (handlingNote.Wait - 60.0 <= wait && wait < handlingNote.Wait + handlingNote.LongWait + 60.0))
                                                {
                                                    isTrapNote = false;
                                                }
                                            }
                                            if (isTrapNote)
                                            {
                                                var trapNote = new TrapNote(WaitLogicalYMap[wait], wait, Array.Empty<AudioNote>(), input, true)
                                                {
                                                    ID = -1
                                                };
                                                trapNote.InitY(logicalY);
                                                _handlingNotes.Add(trapNote);
                                                lock (PaintedNotes)
                                                {
                                                    PaintedNotes.Add(trapNote);
                                                }
                                                postableItemStatus.Elapse(1000.0 / 60);
                                            }
                                        }
                                        break;
                                    case PostableItem.Variety.NegativeAutoableNotes:
                                        postedAutoableNotesMillis += millisLoopUnit;
                                        while (postedAutoableNotesMillis >= 60.0)
                                        {
                                            postedAutoableNotesMillis -= 60.0;
                                            foreach (var handlingNote in _handlingNotes)
                                            {
                                                if (handlingNote.HasStand && handlingNote.Judged == Component.Judged.Not && handlingNote.IsVisibleHalf(this) && handlingNote.TargetInput == handlingNote.LevyingInput)
                                                {
                                                    var isAutoableNote = true;
                                                    foreach (var note in _handlingNotes)
                                                    {
                                                        if (note.TargetInput == 1 && handlingNote.IsCollided(note))
                                                        {
                                                            isAutoableNote = false;
                                                        }
                                                    }
                                                    if (isAutoableNote)
                                                    {
                                                        handlingNote.TargetInput = 1;
                                                        handlingNote.SetLayer(this);
                                                        postableItemStatus.Elapse(1000.0 / 60);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case PostableItem.Variety.NegativeSalt:
                                        postedSaltNotesMillis += millisLoopUnit;
                                        while (postedSaltNotesMillis >= 60.0)
                                        {
                                            postedSaltNotesMillis -= 60.0;
                                            var input = Utility.GetSaltedValue(randomDefaultInputs);
                                            foreach (var handlingNote in _handlingNotes)
                                            {
                                                if (handlingNote.HasStand && handlingNote.Judged == Component.Judged.Not && handlingNote.IsVisibleHalf(this) && handlingNote.TargetInput == handlingNote.LevyingInput)
                                                {
                                                    var isSaltedNote = true;
                                                    foreach (var note in _handlingNotes)
                                                    {
                                                        if (note.TargetInput == input && handlingNote.IsCollided(note))
                                                        {
                                                            isSaltedNote = false;
                                                        }
                                                    }
                                                    if (isSaltedNote)
                                                    {
                                                        handlingNote.TargetInput = input;
                                                        handlingNote.SetLayer(this);
                                                        postableItemStatus.Elapse(1000.0 / 60);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        postableItemStatus.Elapse(millisLoopUnit);
                                        break;
                                }
                            }
                            else
                            {
                                switch (postableItem.VarietyValue)
                                {
                                    case PostableItem.Variety.NegativeFading:
                                        if (FaintCosine == 1.0 && ModeComponentValue.FaintNoteModeValue == ModeComponent.FaintNoteMode.Fading)
                                        {
                                            ModeComponentValue.FaintNoteModeValue = ModeComponent.FaintNoteMode.Default;
                                        }
                                        break;
                                    case PostableItem.Variety.NegativeZip:
                                        if (NoteMobilityCosine == 1.0 && ModeComponentValue.NoteMobilityModeValue == ModeComponent.NoteMobilityMode.Zip)
                                        {
                                            ModeComponentValue.NoteMobilityModeValue = ModeComponent.NoteMobilityMode.Default;
                                        }
                                        break;
                                    case PostableItem.Variety.Negative4D:
                                        if (NoteMobilityValue == 0.0 && ModeComponentValue.NoteMobilityModeValue == ModeComponent.NoteMobilityMode._4D)
                                        {
                                            ModeComponentValue.NoteMobilityModeValue = ModeComponent.NoteMobilityMode.Default;
                                        }
                                        break;
                                }
                            }
                        }

                        ModeComponentValue.AudioMultiplier += Utility.GetMove(postedAudioMultiplier, ModeComponentValue.AudioMultiplier, 1000.0 / millisLoopUnit);
                        for (var i = 1; i >= -1; --i)
                        {
                            PostedItemFaints[i] += Utility.GetMove(0.0, PostedItemFaints[i], 1000.0 / millisLoopUnit);
                        }
                    }

                    if (IsPausing)
                    {
                        if (CanPause && !CanSetPosition && ValidatedTotalNotes > 0 && LastStatusValue == LastStatus.Not)
                        {
                            if (!SetPause)
                            {
                                if (_pauseMillis == 0.0)
                                {
                                    Array.Clear(PauseFrames, 0, PauseFrames.Length);
                                    _pauseMillis = 3000.0;
                                }
                                if (_pauseMillis % 1000.0 == 0.0)
                                {
                                    HandleUIAudio("Pause");
                                }
                                if ((_pauseMillis -= millisLoopUnit) <= 0.0)
                                {
                                    AudioSystem.Instance.Pause(this, false);
                                    MediaSystem.Instance.Pause(this, false);
                                    IsPausing = false;
                                }
                            }
                        }
                        else
                        {
                            if (!SetPause)
                            {
                                AudioSystem.Instance.Pause(this, false);
                                MediaSystem.Instance.Pause(this, false);
                                IsPausing = false;
                            }
                        }
                    }
                    else
                    {
                        lastLoopingCounter = LoopingCounter;
                        if (_eventPositions.TryDequeue(out var eventPosition))
                        {
                            LoopingCounter = eventPosition;
                        }
                        else
                        {
                            LoopingCounter += millisLoopUnit * AudioMultiplier;
                        }
                        _isPassable = LoopingCounter < PassableWait;

                        while (TwilightCompiledIOQueue.TryDequeue(out var twilightCompiledIO))
                        {
                            if (twilightCompiledIO.isCompiled)
                            {
                                _ioAvatarIDs.Add(twilightCompiledIO.avatarID);
                                _pendingIOAvatarIDs.Add(twilightCompiledIO.avatarID);
                                IOAvatarNames.Add(twilightCompiledIO.avatarName);
                            }
                            else
                            {
                                _ioAvatarIDs.Remove(twilightCompiledIO.avatarID);
                                _pendingIOAvatarIDs.Remove(twilightCompiledIO.avatarID);
                                _sentIOAvatarIDs.Remove(twilightCompiledIO.avatarID);
                                IOAvatarNames.Remove(twilightCompiledIO.avatarName);
                            }
                        }

                        if (_pendingIOAvatarIDs.Count > 0)
                        {
                            var data = Comment.ToByteString();
                            TwilightSystem.Instance.SendParallel(Event.Types.EventID.LevyIo, new
                            {
                                avatarIDs = _pendingIOAvatarIDs,
                                handlerID = HandlerID,
                                levyingWait = LoopingCounter,
                                lastStand = LastStand,
                                isF = (bool)IsF,
                                multiplier = ModeComponentValue.Multiplier,
                                audioMultiplier = AudioMultiplier,
                                ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                            }, data);
                            _sentIOAvatarIDs.AddRange(_pendingIOAvatarIDs);
                            _pendingIOAvatarIDs.Clear();
                        }

                        if (!IsInEvents && !IsPowered)
                        {
                            lock (TwilightIOInputQueue)
                            {
                                while (TwilightIOInputQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIOInput = elapsable.Value;
                                    HandleInput(twilightIOInput.Input, DefaultCompute.InputFlag.Not, (byte)twilightIOInput.Power);
                                    TwilightIOInputQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIOInputQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }

                            lock (TwilightIOJudgeQueue)
                            {
                                while (TwilightIOJudgeQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIOJudge = elapsable.Value;
                                    var note = Notes[twilightIOJudge.NoteID];
                                    SetNoteJudged(note, (Component.Judged)twilightIOJudge.Judged);
                                    OnNoteJudged(note);
                                    ++_validJudgedNotes;
                                    TwilightIOJudgeQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIOJudgeQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }

                            lock (TwilightIONoteVisibilityQueue)
                            {
                                while (TwilightIONoteVisibilityQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIONoteVisibility = elapsable.Value;
                                    var note = Notes[twilightIONoteVisibility.NoteID];
                                    if (twilightIONoteVisibility.SetNoteFailed)
                                    {
                                        SetNoteFailed(note, twilightIONoteVisibility.SetValidJudgedNotes);
                                    }
                                    else
                                    {
                                        WipeNote(note, twilightIONoteVisibility.SetValidJudgedNotes);
                                    }
                                    if (twilightIONoteVisibility.SetValidJudgedNotes)
                                    {
                                        ++_validJudgedNotes;
                                    }
                                    TwilightIONoteVisibilityQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIONoteVisibilityQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }

                            lock (TwilightIOJudgmentMeterQueue)
                            {
                                while (TwilightIOJudgmentMeterQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIOJudgmentMeter = elapsable.Value;
                                    SetJudgmentMeter(LoopingCounter, twilightIOJudgmentMeter.Input, twilightIOJudgmentMeter.JudgmentMeter, (Component.JudgmentAssist)twilightIOJudgmentMeter.Assist);
                                    TwilightIOJudgmentMeterQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIOJudgmentMeterQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }

                            lock (TwilightIOMultiplierQueue)
                            {
                                while (TwilightIOMultiplierQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIOMultiplier = elapsable.Value;
                                    MultiplierQueue.Enqueue(twilightIOMultiplier.Multiplier);
                                    TwilightIOMultiplierQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIOMultiplierQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }

                            lock (TwilightIOAudioMultiplierQueue)
                            {
                                while (TwilightIOAudioMultiplierQueue.TryPeek(out var elapsable) && elapsable.IsElapsed(ioMillis))
                                {
                                    var twilightIOAudioMultiplier = elapsable.Value;
                                    AudioMultiplierQueue.Enqueue(twilightIOAudioMultiplier.AudioMultiplier);
                                    TwilightIOAudioMultiplierQueue.TryDequeue(out _);
                                }
                                foreach (var elapsable in TwilightIOAudioMultiplierQueue)
                                {
                                    elapsable.Elapse(millisLoopUnit);
                                }
                            }
                        }

                        if (Length > 0.0)
                        {
                            Status = Math.Clamp(LoopingCounter / Length, 0.0, 1.0);
                        }
                        else
                        {
                            Status = LoopingCounter < 0.0 ? 0.0 : 1.0;
                        }

                        var absSin = Math.Abs(Math.Sin(LoopingCounter / 1000.0 % (2 * Math.PI)));
                        NoteMobilityCosine += Utility.GetMove(IsPostableItemMode && !PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeZip]].IsHandling ? 1.0 : absSin, NoteMobilityCosine, 1000.0 / millisLoopUnit);
                        NoteMobilityValue += Utility.GetMove(IsPostableItemMode && !PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.Negative4D]].IsHandling ? 0.0 : 1.0, NoteMobilityValue, 1000.0 / millisLoopUnit);
                        FaintCosine += Utility.GetMove(IsPostableItemMode && !PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeFading]].IsHandling ? 1.0 : absSin, FaintCosine, 1000.0 / millisLoopUnit);

                        if (LoopingCounter > LengthLayered + Component.QuitWait)
                        {
                            FaintLayered += Utility.GetMove(1.0, FaintLayered, 1000.0 / millisLoopUnit);
                        }

                        if (SetPass)
                        {
                            HandleUIAudio("Pass");
                            LevyingWait = PassableWait;
                            SetUndo = true;
                            continue;
                        }
                        else if (SetEscape)
                        {
                            OnHandled();
                            continue;
                        }

                        while (endNoteID >= handlingNoteID)
                        {
                            var note = Notes[handlingNoteID];
                            if (note.IsClose(LoopingCounter))
                            {
                                note.InitY(logicalY);
                                _handlingNotes.Add(note);
                                ++handlingNoteID;
                            }
                            else
                            {
                                break;
                            }
                        }
                        while (endNoteID >= paintedNoteID)
                        {
                            var note = Notes[paintedNoteID];
                            if (note.IsWiped)
                            {
                                ++paintedNoteID;
                                continue;
                            }
                            else if (note.IsVisible(this))
                            {
                                lock (PaintedNotes)
                                {
                                    PaintedNotes.Add(note);
                                }
                                ++paintedNoteID;
                            }
                            else
                            {
                                break;
                            }
                        }

                        var distance = Utility.GetDistance(_valueComponent, waitBPMMap, lastLoopingCounter, LoopingCounter, out var lastBPM);
                        if (!double.IsNaN(lastBPM))
                        {
                            HandlingBPM = lastBPM;
                            _millisStandardMeter = Math.Abs(60000.0 / lastBPM);
                            _millisMeter = _millisStandardMeter;
                        }
                        logicalY -= distance;

                        if (!IsInEvents)
                        {
                            Utility.LoopBefore(waitAudioNoteMap, LoopingCounter, Configure.Instance.AudioWait + Configure.Instance.BanalAudioWait, handleAudioNotesImpl);
                            if (!Configure.Instance.HandleInputAudio)
                            {
                                Utility.LoopBefore(waitInputAudioMap, LoopingCounter, Configure.Instance.AudioWait + Configure.Instance.BanalAudioWait, handleAudioNotesImpl);
                            }
                            Utility.LoopBefore(waitMediaNoteMap, LoopingCounter, Configure.Instance.MediaWait + Configure.Instance.BanalMediaWait, handleMediaNotesImpl);
                        }

                        for (var j = _handlingNotes.Count - 1; j >= 0; --j)
                        {
                            var handlingNote = _handlingNotes[j];
                            handlingNote.Move(distance);
                            handlingNote.MoveInputMillis(millisLoopUnit, this);
                            var targetInput = handlingNote.TargetInput;
                            var longWait = handlingNote.LongWait;
                            var wait = handlingNote.Wait;
                            var hasStand = handlingNote.HasStand;
                            if (HandleAutoJudged(handlingNote))
                            {
                                WipeNote(handlingNote, false);
                            }
                            else
                            {
                                if (longWait > 0.0 && wait + longWait >= LoopingCounter && handlingNote.Judged != Component.Judged.Not && !handlingNote.IsFailed)
                                {
                                    if (longNoteHitFrame > 0)
                                    {
                                        if (IsSuitableAsHitLongNotePaint(targetInput, longNoteHitLoopFrame, longNoteHitFramerate))
                                        {
                                            lock (HitLongNotePaints)
                                            {
                                                HitLongNotePaints[-targetInput] = new(this, targetInput, longNoteHitFrame, longNoteHitFramerate);
                                            }
                                        }
                                        if (IsSuitableAsEnlargedLastBandPaint(targetInput, lastEnlargedBandLoopFrame, longNoteHitFramerate))
                                        {
                                            _paintEnlargedLastBand = true;
                                        }

                                        bool IsSuitableAsHitLongNotePaint(int input, int longNoteHitLoopFrame, double longNoteHitFramerate)
                                        {
                                            _hitLongNotePaintMillis[input] += millisLoopUnit;
                                            var longNoteHitLoopFramerate = 1000.0 * longNoteHitLoopFrame / longNoteHitFramerate;
                                            if (_hitLongNotePaintMillis[input] >= longNoteHitLoopFramerate)
                                            {
                                                _hitLongNotePaintMillis[input] -= longNoteHitLoopFramerate;
                                                return true;
                                            }
                                            return false;
                                        }

                                        bool IsSuitableAsEnlargedLastBandPaint(int input, int lastEnlargedBandLoopFrame, double longNoteHitFramerate)
                                        {
                                            _lastEnlargedBandPaintMillis[input] += millisLoopUnit;
                                            var lastEnlargedBandLoopFramerate = 1000.0 * lastEnlargedBandLoopFrame / longNoteHitFramerate;
                                            if (_lastEnlargedBandPaintMillis[input] >= lastEnlargedBandLoopFramerate)
                                            {
                                                _lastEnlargedBandPaintMillis[input] -= lastEnlargedBandLoopFramerate;
                                                return true;
                                            }
                                            return false;
                                        }
                                    }
                                    var isIn2P = IsIn2P[targetInput];
                                    var paintID = isIn2P ? 1 : 0;
                                    if (judgmentFrame > 0 && IsSuitableAsInput(targetInput) && !JudgmentPaints.ContainsKey(paintID))
                                    {
                                        lock (JudgmentPaints)
                                        {
                                            JudgmentPaints[paintID] = new(this, handlingNote.Judged, isIn2P, judgmentSystem, judgmentPosition0, judgmentPosition1, judgmentFrame, judgmentFramerate, judgmentLength, judgmentHeight);
                                        }
                                    }
                                }
                                if (hasStand && (IsInEvents || IsPowered))
                                {
                                    if (handlingNote.IsFailedAsTooLate(LoopingCounter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate) && handlingNote.Judged == Component.Judged.Not)
                                    {
                                        SetNoteJudged(handlingNote, Component.Judged.Lowest);
                                        if (longWait > 0.0)
                                        {
                                            _validJudgedNotes += 2;
                                            SetNoteFailed(handlingNote, true);
                                        }
                                        else
                                        {
                                            ++_validJudgedNotes;
                                            WipeNote(handlingNote, false);
                                        }
                                        OnNoteJudged(handlingNote);
                                    }
                                    if (handlingNote.Judged != Component.Judged.Not && handlingNote.IsTooLong(LoopingCounter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate))
                                    {
                                        var isAutoLongNote = (IsAutoLongNote && ModeComponentValue.LongNoteModeValue != ModeComponent.LongNoteMode.Input) || ModeComponentValue.LongNoteModeValue == ModeComponent.LongNoteMode.Auto;
                                        if (!handlingNote.IsFailed)
                                        {
                                            if (isAutoLongNote)
                                            {
                                                switch (LongNoteModeDate)
                                                {
                                                    case Component.LongNoteModeDate._1_16_4:
                                                        SetNoteJudged(handlingNote, handlingNote.Judged);
                                                        OnNoteJudged(handlingNote);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                switch (_tooLongLongNoteDate)
                                                {
                                                    case Component.TooLongLongNoteDate._1_0_0:
                                                    case Component.TooLongLongNoteDate._1_14_20:
                                                        SetNoteJudged(handlingNote, Component.Judged.Lowest);
                                                        break;
                                                    case Component.TooLongLongNoteDate._1_13_107:
                                                    case Component.TooLongLongNoteDate._1_14_29:
                                                        SetNoteJudged(handlingNote, Component.Judged.Lower);
                                                        break;
                                                }
                                                OnNoteJudged(handlingNote);
                                            }
                                            ++_validJudgedNotes;
                                        }
                                        WipeNote(handlingNote, isAutoLongNote);
                                    }
                                    if (IsSuitableAsAutoJudge(targetInput) && !handlingNote.IsFailed)
                                    {
                                        var targetWait = wait;
                                        if (ModeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                                        {
                                            targetWait += (ModeComponentValue.FavorJudgments[(int)Component.Judged.Highest][0] + ModeComponentValue.FavorJudgments[(int)Component.Judged.Highest][1]) / 2;
                                        }
                                        if (targetWait <= LoopingCounter && handlingNote.Judged == Component.Judged.Not)
                                        {
                                            HandleInput(targetInput);
                                        }
                                        if (targetWait + longWait <= LoopingCounter)
                                        {
                                            HandleInput(-targetInput);
                                        }
                                    }
                                }
                            }

                            bool HandleAutoJudged(BaseNote note)
                            {
                                var judgedNoteData = note.AutoJudge(LoopingCounter);
                                switch (judgedNoteData?.IDValue)
                                {
                                    case JudgedNoteData.ID.Not:
                                        return true;
                                    case JudgedNoteData.ID.HandleVoid:
                                        SetIIDXInputAudioVariety(note);
                                        return true;
                                    case JudgedNoteData.ID.HandleMeter:
                                        _millisMeter = _millisStandardMeter;
                                        MeterText = judgedNoteData.Value.MeterText;
                                        return true;
                                    default:
                                        return false;
                                }
                            }
                        }

                        HandleComment(ref commentInputID, ref commentMultiplierID, ref commentAudioMultiplierID);

                        while (_rawInputQueue.TryDequeue(out var rawInput))
                        {
                            var input = rawInput.Item1;
                            var inputPower = rawInput.Item2;
                            var isInput = input > 0;
                            var absInput = Math.Abs(input);
                            if (isInput)
                            {
                                RGBSystem.Instance.InputValues[absInput] = 1.0;
                            }
                            RGBSystem.Instance.HasInputValues[absInput] = isInput;
                            if (input > 0)
                            {
                                switch (UI.Instance.LoopingMain)
                                {
                                    case 0:
                                    case 1:
                                        _targetMainFrames[absInput] = mainFrame;
                                        break;
                                    case 2:
                                        MainFrames[absInput] = 0;
                                        _targetMainFrames[absInput] = 0;
                                        break;
                                }
                                switch (UI.Instance.LoopingInput)
                                {
                                    case 0:
                                    case 1:
                                        _targetInputFrames[absInput] = inputFrame;
                                        break;
                                    case 2:
                                        InputFrames[absInput] = 0;
                                        _targetInputFrames[absInput] = 0;
                                        break;
                                }
                            }
                            else
                            {
                                switch (UI.Instance.LoopingMain)
                                {
                                    case 0:
                                        _targetMainFrames[absInput] = 0;
                                        break;
                                    case 1:
                                        MainFrames[absInput] = 0;
                                        _targetMainFrames[absInput] = 0;
                                        break;
                                    case 2:
                                        _targetMainFrames[absInput] = mainFrame;
                                        break;
                                }
                                switch (UI.Instance.LoopingInput)
                                {
                                    case 0:
                                        _targetInputFrames[absInput] = 0;
                                        break;
                                    case 1:
                                        InputFrames[absInput] = 0;
                                        _targetInputFrames[absInput] = 0;
                                        break;
                                    case 2:
                                        _targetInputFrames[absInput] = inputFrame;
                                        break;
                                }
                            }
                            if (IsSuitableAsInput(absInput) && isValidLoopingCounter)
                            {
                                Comment.Inputs.Add(new InputEvent
                                {
                                    Input = input,
                                    Wait = LoopingCounter
                                });
                            }
                            SendIOInput(input, inputPower);

                            var wasHandled = false;
                            foreach (var handlingNote in _handlingNotes)
                            {
                                if (handlingNote.TargetInput == absInput && !handlingNote.IsFailed && handlingNote.HasStand && (!isInput || handlingNote.Judged == Component.Judged.Not))
                                {
                                    var judgedNoteData = handlingNote.Judge(input, LoopingCounter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, _trapNoteJudgmentDate, IsAutoLongNote);
                                    if (judgedNoteData.HasValue)
                                    {
                                        if (isInput)
                                        {
                                            HandleInputAudio(handlingNote);
                                        }
                                        if (judgedNoteData.Value.IDValue != JudgedNoteData.ID.AutoLongNoteJudgment || !IsAutoLongNote)
                                        {
                                            InputCountQueue.Enqueue(LoopingCounter);
                                        }
                                        HandleJudged(handlingNote, judgedNoteData, LoopingCounter);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    wasHandled = true;
                                    break;
                                }
                            }
                            if (!wasHandled)
                            {
                                foreach (var handlingNote in _handlingNotes)
                                {
                                    if (handlingNote.TargetInput == absInput && !handlingNote.IsFailed && !handlingNote.HasStand)
                                    {
                                        var judgedNoteData = handlingNote.Judge(input, LoopingCounter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, _trapNoteJudgmentDate, IsAutoLongNote);
                                        if (judgedNoteData.HasValue)
                                        {
                                            HandleJudged(handlingNote, judgedNoteData, LoopingCounter);
                                            wasHandled = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!wasHandled && isInput && ModeComponentValue.LowestJudgmentConditionModeValue == ModeComponent.LowestJudgmentConditionMode.Wrong)
                            {
                                SetJudgedValue(Component.Judged.Lowest, IsIn2P[absInput], null);
                                wasHandled = true;
                            }
                            if (!wasHandled && isInput)
                            {
                                switch (InputAudioVarietyValue)
                                {
                                    case Configure.InputAudioVariety.DJMAX:
                                        foreach (var handlingNote in _handlingNotes)
                                        {
                                            if (handlingNote.TargetInput == absInput && !handlingNote.IsFailed)
                                            {
                                                HandleInputAudio(handlingNote);
                                                break;
                                            }
                                        }
                                        break;
                                    case Configure.InputAudioVariety.IIDX:
                                        if (Configure.Instance.HandleInputAudio)
                                        {
                                            foreach (var audioNote in _lastIIDXInputAudioNoteMap[absInput])
                                            {
                                                var audioItem = audioNote.AudioItem;
                                                if (audioItem.HasValue)
                                                {
                                                    var audioItemValue = audioItem.Value;
                                                    var lastEventPosition = IsInEvents ? _eventPositions.Last() : LoopingCounter;
                                                    if (lastEventPosition < LoopingCounter + audioItemValue.Length)
                                                    {
                                                        var bmsID = audioNote.BMSID;
                                                        StopLastEqualAudioItem(bmsID);
                                                        lock (LoadedCSX)
                                                        {
                                                            if (HasContents)
                                                            {
                                                                var audioChannel = AudioSystem.Instance.Handle(new()
                                                                {
                                                                    AudioItem = audioItem,
                                                                    Length = audioNote.Length,
                                                                    AudioLevyingPosition = (uint)(audioNote.AudioLevyingPosition + lastEventPosition - LoopingCounter),
                                                                    Salt = audioNote.Salt,
                                                                    BMSID = bmsID
                                                                }, AudioSystem.InputAudio, AudioMultiplier, IsCounterWave, this, 0.0, inputPower);
                                                                if (!string.IsNullOrEmpty(bmsID))
                                                                {
                                                                    audioChannelMap[bmsID] = audioChannel;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        HandleBanalAudio();
                                        break;
                                }
                            }

                            void HandleInputAudio(BaseNote note)
                            {
                                if (Configure.Instance.HandleInputAudio)
                                {
                                    foreach (var audioNote in note.AudioNotes)
                                    {
                                        var audioItem = audioNote.AudioItem;
                                        if (audioItem.HasValue)
                                        {
                                            var audioItemValue = audioItem.Value;
                                            var lastEventPosition = IsInEvents ? _eventPositions.Last() : LoopingCounter;
                                            if (lastEventPosition < LoopingCounter + (audioNote.Length ?? audioItemValue.Length))
                                            {
                                                var bmsID = audioNote.BMSID;
                                                StopLastEqualAudioItem(bmsID);
                                                lock (LoadedCSX)
                                                {
                                                    if (HasContents)
                                                    {
                                                        var audioChannel = AudioSystem.Instance.Handle(new()
                                                        {
                                                            AudioItem = audioItem,
                                                            Length = audioNote.Length,
                                                            AudioLevyingPosition = (uint)(audioNote.AudioLevyingPosition + lastEventPosition - LoopingCounter),
                                                            Salt = audioNote.Salt,
                                                            BMSID = bmsID
                                                        }, AudioSystem.InputAudio, AudioMultiplier, IsCounterWave, this, 0.0, inputPower);
                                                        note.AudioChannels.Add(audioChannel);
                                                        if (!string.IsNullOrEmpty(bmsID))
                                                        {
                                                            audioChannelMap[bmsID] = audioChannel;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                HandleBanalAudio();
                            }

                            void HandleBanalAudio()
                            {
                                if (Configure.Instance.BanalAudio && AudioSystem.Instance.BanalAudio.HasValue)
                                {
                                    var banalAudioValue = AudioSystem.Instance.BanalAudio.Value;
                                    var lastEventPosition = IsInEvents ? _eventPositions.Last() : LoopingCounter;
                                    if (lastEventPosition < LoopingCounter + banalAudioValue.Length)
                                    {
                                        StopLastEqualAudioItem(nameof(AudioSystem.Instance.BanalAudio));
                                        audioChannelMap[nameof(AudioSystem.Instance.BanalAudio)] = AudioSystem.Instance.Handle(new()
                                        {
                                            AudioItem = banalAudioValue
                                        }, AudioSystem.InputAudio, AudioMultiplier, IsCounterWave, this, 0.0, inputPower);
                                    }
                                }
                            }
                        }

                        if (HitPoints.TargetValue == 0.0)
                        {
                            if (CanGAS)
                            {
                                switch (ModeComponentValue.HandlingHitPointsModeValue)
                                {
                                    case ModeComponent.HitPointsMode.Highest:
                                        ModeComponentValue.HandlingHitPointsModeValue = ModeComponent.HitPointsMode.Higher;
                                        break;
                                    case ModeComponent.HitPointsMode.Higher:
                                        ModeComponentValue.HandlingHitPointsModeValue = ModeComponent.HitPointsMode.Default;
                                        break;
                                }
                                HitPoints.TargetValue = _hitPointsGAS[(int)ModeComponentValue.HandlingHitPointsModeValue];
                                Comment.Paints.Clear();
                                var paintEventsGAS = _paintEventsGAS[(int)ModeComponentValue.HandlingHitPointsModeValue];
                                Comment.Paints.AddRange(paintEventsGAS);
                                paintEventsGAS.Clear();
                            }
                            else
                            {
                                OnGetF();
                            }
                        }
                        if (ValidatedTotalNotes > 0 && ValidatedTotalNotes == _validJudgedNotes)
                        {
                            VeilDrawingHeight.TargetValue = Configure.Instance.FlowVeilDrawing ? DrawingComponentValue.judgmentMainPosition : Configure.Instance.VeilDrawingHeight;
                            _isEscapable = true;
                            if (LastStatusValue == LastStatus.Not)
                            {
                                _lastStatus = Comment.LowestJudgment > 0 ? LastStatus.Last : Point.TargetValue < 1.0 ? LastStatus.Band1 : LastStatus.Yell1;
                            }
                        }
                        else
                        {
                            VeilDrawingHeight.TargetValue = Configure.Instance.VeilDrawingHeight;
                        }
                        VeilDrawingHeight.Value += Utility.GetMove(VeilDrawingHeight.TargetValue, VeilDrawingHeight.Value, 500.0 / millisLoopUnit);

                        void HandleJudged(BaseNote judgedNote, JudgedNoteData? judgedNoteData, double loopingCounter)
                        {
                            if ((IsInEvents || IsPowered) && judgedNoteData.HasValue)
                            {
                                var judgedNoteDataValue = judgedNoteData.Value;
                                var targetInput = judgedNote.TargetInput;
                                var judged = judgedNoteDataValue.Judged;
                                if (judged != Component.Judged.Lowest && PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.PositiveJudgment]].IsHandling)
                                {
                                    ++postableJudgmentCount;
                                    judged = Component.Judged.Highest;
                                }
                                if (judged != Component.Judged.Highest && PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeJudgment]].IsHandling)
                                {
                                    ++postableJudgmentCount;
                                    judged = Component.Judged.Lowest;
                                }
                                switch (judgedNoteDataValue.IDValue)
                                {
                                    case JudgedNoteData.ID.NoteJudgment:
                                        SetNoteJudged(judgedNote, judged);
                                        WipeNote(judgedNote, false);
                                        if (IsSuitableAsInput(targetInput))
                                        {
                                            SetJudgmentMeter(loopingCounter, targetInput, (int)judgedNoteDataValue.JudgmentMeter, Component.JudgmentAssist.Default);
                                        }
                                        OnNoteJudged(judgedNote);
                                        ++_validJudgedNotes;
                                        break;
                                    case JudgedNoteData.ID.TrapNoteJudgment:
                                        SetNoteJudged(judgedNote, judged);
                                        WipeNote(judgedNote, false);
                                        OnNoteJudged(judgedNote);
                                        break;
                                    case JudgedNoteData.ID.QuitLongNoteJudgment:
                                        HandleQuitLongNoteJudgment();
                                        break;
                                    case JudgedNoteData.ID.LevyLongNoteJudgment:
                                        if (judged == Component.Judged.Lowest)
                                        {
                                            SetNoteFailed(judgedNote, true);
                                            _validJudgedNotes += 2;
                                        }
                                        else
                                        {
                                            ++_validJudgedNotes;
                                        }
                                        SetNoteJudged(judgedNote, judged);
                                        if (IsSuitableAsInput(targetInput))
                                        {
                                            SetJudgmentMeter(loopingCounter, targetInput, (int)judgedNoteDataValue.JudgmentMeter, Component.JudgmentAssist.Default);
                                        }
                                        OnNoteJudged(judgedNote);
                                        break;
                                    case JudgedNoteData.ID.AutoLongNoteJudgment:
                                        if (IsLongNoteStand1)
                                        {
                                            WipeNote(judgedNote, false);
                                            ++_validJudgedNotes;
                                        }
                                        else
                                        {
                                            HandleQuitLongNoteJudgment();
                                        }
                                        break;
                                    case JudgedNoteData.ID.FailedLongNoteJudgment:
                                        SetNoteFailed(judgedNote, false);
                                        SetNoteJudged(judgedNote, Component.Judged.Lowest);
                                        OnNoteJudged(judgedNote);
                                        ++_validJudgedNotes;
                                        break;
                                }

                                void HandleQuitLongNoteJudgment()
                                {
                                    SetNoteJudged(judgedNote, judged);
                                    WipeNote(judgedNote, false);
                                    if (!IsAutoLongNote && IsSuitableAsInput(targetInput))
                                    {
                                        SetJudgmentMeter(loopingCounter, targetInput, (int)judgedNoteDataValue.JudgmentMeter, Component.JudgmentAssist.LongNoteUp);
                                    }
                                    OnNoteJudged(judgedNote);
                                    ++_validJudgedNotes;
                                }
                            }
                        }

                        void SetJudgmentMeter(double loopingCounter, int input, double judgmentMeter, Component.JudgmentAssist judgmentAssist)
                        {
                            var judgmentMeterPosition = IsIn2P[input] ? 1 : 0;
                            judgmentMeters[judgmentMeterPosition].Millis = judgmentMeter;
                            judgmentMeters[judgmentMeterPosition].Judged = Component.GetJudged(judgmentMeter, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, judgmentAssist);
                            areValidJudgmentMeters[judgmentMeterPosition] = true;

                            judgmentMeters[2] = judgmentMeters[judgmentMeterPosition];
                            areValidJudgmentMeters[2] = true;

                            Comment.JudgmentMeters.Add(new JudgmentMeterEvent
                            {
                                JudgmentMeter = judgmentMeter,
                                Wait = loopingCounter - judgmentMeter,
                                Assist = (int)judgmentAssist
                            });

                            if (Math.Abs(judgmentMeter) >= Configure.Instance.JudgmentMeterMillis)
                            {
                                if (judgmentMeter > 0.0)
                                {
                                    ++LateValue;
                                    if (!IsInEvents)
                                    {
                                        MainJudgmentMeterFrames[input] = DrawingComponentValue.mainJudgmentMeterFrame;
                                    }
                                }
                                else if (judgmentMeter < 0.0)
                                {
                                    ++EarlyValue;
                                    if (!IsInEvents)
                                    {
                                        MainJudgmentMeterFrames[input] = -DrawingComponentValue.mainJudgmentMeterFrame;
                                    }
                                }
                                _noteWaits.Add(judgmentMeter);
                            }
                            if (!IsInEvents)
                            {
                                SendIOJudgmentMeter(input, judgmentMeter, judgmentAssist);
                            }
                        }

                        void WipeNote(BaseNote note, bool setValidJudgedNotes)
                        {
                            _handlingNotes.Remove(note);
                            lock (PaintedNotes)
                            {
                                PaintedNotes.Remove(note);
                            }
                            note.IsWiped = true;
                            if (note.HasStand)
                            {
                                SendIONoteVisibility(note, setValidJudgedNotes, false);
                            }
                        }
                    }

                    if (!IsInEvents)
                    {
                        switch (DrawingComponentValue.altJudgmentMeter)
                        {
                            case 0 when areValidJudgmentMeters[2]:
                                JudgmentMeters[0] = judgmentMeters[2].Millis;
                                break;
                            case 3 when areValidJudgmentMeters[2]:
                                JudgmentMeters[1] = judgmentMeters[2].Millis;
                                break;
                            case 2:
                                if (areValidJudgmentMeters[0])
                                {
                                    JudgmentMeters[0] = judgmentMeters[0].Millis;
                                }
                                if (areValidJudgmentMeters[1])
                                {
                                    JudgmentMeters[1] = judgmentMeters[1].Millis;
                                }
                                break;
                        }
                        switch (DrawingComponentValue.altJudgmentVisualizer)
                        {
                            case 2:
                                if (areValidJudgmentMeters[0])
                                {
                                    var judgmentMeter = judgmentMeters[0];
                                    var judgmentVisualizerValues = JudgmentVisualizerValues[0];
                                    lock (judgmentVisualizerValues)
                                    {
                                        judgmentVisualizerValues.Enqueue(new(Math.Clamp(0.5 + (judgmentMeter.Millis < 0.0 ? judgmentMeter.Millis / -LowestJudgmentMillis : judgmentMeter.Millis / HighestJudgmentMillis) / 2, 0.0, 1.0), judgmentMeter.Judged));
                                    }
                                }
                                if (areValidJudgmentMeters[1])
                                {
                                    var judgmentMeter = judgmentMeters[1];
                                    var judgmentVisualizerValues = JudgmentVisualizerValues[1];
                                    lock (judgmentVisualizerValues)
                                    {
                                        judgmentVisualizerValues.Enqueue(new(Math.Clamp(0.5 + (judgmentMeter.Millis < 0.0 ? judgmentMeter.Millis / -LowestJudgmentMillis : judgmentMeter.Millis / HighestJudgmentMillis) / 2, 0.0, 1.0), judgmentMeter.Judged));
                                    }
                                }
                                break;
                            case 0:
                                if (areValidJudgmentMeters[2])
                                {
                                    var judgmentMeter = judgmentMeters[2];
                                    var judgmentVisualizerValues = JudgmentVisualizerValues[0];
                                    lock (judgmentVisualizerValues)
                                    {
                                        judgmentVisualizerValues.Enqueue(new(Math.Clamp(0.5 + (judgmentMeter.Millis < 0.0 ? judgmentMeter.Millis / -LowestJudgmentMillis : judgmentMeter.Millis / HighestJudgmentMillis) / 2, 0.0, 1.0), judgmentMeter.Judged));
                                    }
                                }
                                break;
                            case 3:
                                if (areValidJudgmentMeters[2])
                                {
                                    var judgmentMeter = judgmentMeters[2];
                                    var judgmentVisualizerValues = JudgmentVisualizerValues[1];
                                    lock (judgmentVisualizerValues)
                                    {
                                        judgmentVisualizerValues.Enqueue(new(Math.Clamp(0.5 + (judgmentMeter.Millis < 0.0 ? judgmentMeter.Millis / -LowestJudgmentMillis : judgmentMeter.Millis / HighestJudgmentMillis) / 2, 0.0, 1.0), judgmentMeter.Judged));
                                    }
                                }
                                break;
                        }
                        Array.Fill(areValidJudgmentMeters, false);

                        var noteFrame = DrawingComponentValue.noteFrame;
                        if (noteFrame > 0)
                        {
                            var noteFramerate = 1000.0 / DrawingComponentValue.noteFramerate;
                            _noteMillis += millisLoopUnit;
                            while (_noteMillis >= noteFramerate)
                            {
                                _noteMillis -= noteFramerate;
                                NoteFrame = (NoteFrame + 1) % noteFrame;
                            }
                        }

                        var pauseFrame = DrawingComponentValue.pauseFrame;
                        var pauseCount = PauseCount;
                        if (pauseFrame > 0 && pauseCount > 0)
                        {
                            PauseFrames[pauseCount - 1] = pauseFrame - (int)Math.Ceiling((_pauseMillis - (1000.0 * (pauseCount - 1))) / (1000.0 / pauseFrame));
                        }

                        _mainMillis += millisLoopUnit;
                        var mainFramerate = 1000.0 / DrawingComponentValue.mainFramerate;
                        while (_mainMillis >= mainFramerate)
                        {
                            _mainMillis -= mainFramerate;
                            for (var i = inputCount; i > 0; --i)
                            {
                                MainFrames[i] += Math.Sign(_targetMainFrames[i] - MainFrames[i]);
                            }
                        }

                        _inputMillis += millisLoopUnit;
                        var inputFramerate = 1000.0 / DrawingComponentValue.inputFramerate;
                        while (_inputMillis >= inputFramerate)
                        {
                            _inputMillis -= inputFramerate;
                            for (var i = inputCount; i > 0; --i)
                            {
                                InputFrames[i] += Math.Sign(_targetInputFrames[i] - InputFrames[i]);
                            }
                        }

                        var levelFrame = DrawingComponentValue.levelFrame;
                        if (levelFrame > 0)
                        {
                            var levelFramerate = 1000.0 / DrawingComponentValue.levelFramerate;
                            _levelMillis += millisLoopUnit;
                            while (_levelMillis >= levelFramerate)
                            {
                                _levelMillis -= levelFramerate;
                                LevelFrame = (LevelFrame + 1) % levelFrame;
                            }
                        }

                        var autoMainFrame = DrawingComponentValue.autoMainFrame;
                        if (autoMainFrame > 0)
                        {
                            var autoMainFramerate = 1000.0 / DrawingComponentValue.autoMainFramerate;
                            _autoMainMillis += millisLoopUnit;
                            while (_autoMainMillis >= autoMainFramerate)
                            {
                                _autoMainMillis -= autoMainFramerate;
                                AutoMainFrame = (AutoMainFrame + 1) % autoMainFrame;
                            }
                        }

                        _judgmentMainPaintMillis += millisLoopUnit;
                        var judgmentMainFramerate = 1000.0 / DrawingComponentValue.judgmentMainFramerate;
                        while (_judgmentMainPaintMillis >= judgmentMainFramerate)
                        {
                            _judgmentMainPaintMillis -= judgmentMainFramerate;
                            for (var i = inputCount; i > 0; --i)
                            {
                                if (JudgmentMainFrames[i] > 0)
                                {
                                    --JudgmentMainFrames[i];
                                }
                            }
                        }

                        _mainJudgmentMeterPaintMillis += millisLoopUnit;
                        var mainJudgmentMeterFramerate = 1000.0 / DrawingComponentValue.mainJudgmentMeterFramerate;
                        while (_mainJudgmentMeterPaintMillis >= mainJudgmentMeterFramerate)
                        {
                            _mainJudgmentMeterPaintMillis -= mainJudgmentMeterFramerate;
                            for (var i = inputCount; i > 0; --i)
                            {
                                MainJudgmentMeterFrames[i] += Math.Sign(-MainJudgmentMeterFrames[i]);
                            }
                        }

                        lock (DrawingComponentValue.PaintPropertyCSX)
                        {
                            foreach (var paintPropertyID in DrawingComponentValue.PaintPropertyIDs)
                            {
                                var paintPropertyIntMap = DrawingComponentValue.PaintPropertyIntMap[paintPropertyID];
                                var paintFramerate = 1000.0 / DrawingComponentValue.PaintPropertyMap[paintPropertyID][PaintProperty.ID.Framerate];
                                _paintPropertyMillis[paintPropertyID] += millisLoopUnit;
                                while (_paintPropertyMillis[paintPropertyID] >= paintFramerate)
                                {
                                    _paintPropertyMillis[paintPropertyID] -= paintFramerate;
                                    switch (paintPropertyIntMap[PaintProperty.ID.Mode])
                                    {
                                        case 0:
                                            if (PaintPropertyFrames[paintPropertyID] + 1 < paintPropertyIntMap[PaintProperty.ID.Frame])
                                            {
                                                ++PaintPropertyFrames[paintPropertyID];
                                            }
                                            else
                                            {
                                                PaintPropertyFrames[paintPropertyID] = 0;
                                            }
                                            break;
                                        case 1:
                                            if (PaintPropertyFrames[paintPropertyID] > 0)
                                            {
                                                --PaintPropertyFrames[paintPropertyID];
                                            }
                                            break;
                                        case 2:
                                            if (PaintPropertyFrames[paintPropertyID] + 1 < paintPropertyIntMap[PaintProperty.ID.Frame])
                                            {
                                                ++PaintPropertyFrames[paintPropertyID];
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        var band = Band.Value;
                        if (band > 0)
                        {
                            var bandFrame = DrawingComponentValue.bandFrame;
                            if (bandFrame > 0)
                            {
                                var enlargeBand = DrawingComponentValue.enlargeBand;
                                var digit = Utility.GetDigit(band) - 1;
                                for (var i = digit; i >= 0; --i)
                                {
                                    var j = (int)Math.Pow(10, i);
                                    if ((i == 0 && _paintEnlargedLastBand) || (_lastPaintedBand / j % 10 != band / j % 10))
                                    {
                                        BandDrawingFrames[i] = bandFrame - 1;
                                        BandEnlargedMap[i] = enlargeBand;
                                        _paintEnlargedLastBand = false;
                                    }
                                }
                                _lastPaintedBand = band;
                                _bandPaintMillis += millisLoopUnit;
                                var bandFramerate = 1000.0 / DrawingComponentValue.bandFramerate;
                                var unlargeBand = DrawingComponentValue.enlargeBand / DrawingComponentValue.bandFrame;
                                while (_bandPaintMillis >= bandFramerate)
                                {
                                    _bandPaintMillis -= bandFramerate;
                                    for (var i = digit; i >= 0; --i)
                                    {
                                        if (BandDrawingFrames.TryGetValue(i, out var bandDrawingFrame) && bandDrawingFrame > 0)
                                        {
                                            --BandDrawingFrames[i];
                                        }
                                        if (BandEnlargedMap.TryGetValue(i, out var bandEnlarged) && bandEnlarged > 0)
                                        {
                                            BandEnlargedMap[i] -= unlargeBand;
                                        }
                                    }
                                }
                            }
                        }

                        lock (JudgmentPaints)
                        {
                            foreach (var (paintID, judgmentPaint) in JudgmentPaints)
                            {
                                if (judgmentPaint.IsPaintAsToo(millisLoopUnit))
                                {
                                    JudgmentPaints.Remove(paintID);
                                }
                            }
                        }
                        lock (HitNotePaints)
                        {
                            foreach (var (paintID, hitNotePaint) in HitNotePaints)
                            {
                                if (hitNotePaint.IsPaintAsToo(millisLoopUnit))
                                {
                                    HitNotePaints.Remove(paintID);
                                }
                            }
                        }
                        lock (HitLongNotePaints)
                        {
                            foreach (var (paintID, hitLongNotePaint) in HitLongNotePaints)
                            {
                                if (hitLongNotePaint.IsPaintAsToo(millisLoopUnit))
                                {
                                    HitLongNotePaints.Remove(paintID);
                                }
                            }
                        }

                        var framerate = Configure.Instance.FlowValues ? 60.0 / millisLoopUnit : 1.0;
                        foreach (var netItem in NetItems)
                        {
                            netItem.IsFailedStatus = Math.Clamp(netItem.IsFailedStatus + Utility.GetMove(0.0, netItem.IsFailedStatus, 60.0 / millisLoopUnit), 0.0, 100.0);
                            netItem.DrawingPosition += Utility.GetMove(netItem.TargetPosition, netItem.DrawingPosition, framerate);
                        }
                        Band.Value += Utility.GetMove(Band.TargetValue, Band.Value, framerate);
                        Stand.Value += Utility.GetMove(Stand.TargetValue, Stand.Value, framerate);
                        HitPoints.Value = Math.Clamp(HitPoints.Value + Utility.GetMove(HitPoints.TargetValue, HitPoints.Value, framerate), 0.0, 1.0);
                        Point.Value = Math.Clamp(Point.Value + Utility.GetMove(Point.TargetValue, Point.Value, framerate), 0.0, 1.0);
                        Hunter.Value = Hunter.TargetValue.HasValue ? (Hunter.Value ?? 0) + Utility.GetMove(Hunter.TargetValue.Value, Hunter.Value ?? 0, framerate) : null;
                        for (var i = PostableItemFaints.Length - 1; i >= 0; --i)
                        {
                            PostableItemFaints[i].Value = Math.Clamp(PostableItemFaints[i].Value + Utility.GetMove(PostableItemFaints[i].TargetValue, PostableItemFaints[i].Value, 60.0 / millisLoopUnit), 0.0, 1.0);
                        }

                        foreach (var judgmentVisualizerValues in JudgmentVisualizerValues)
                        {
                            lock (judgmentVisualizerValues)
                            {
                                foreach (var judgmentVisualizerValue in judgmentVisualizerValues)
                                {
                                    judgmentVisualizerValue.LoopingCounter -= millisLoopUnit;
                                }
                                while (judgmentVisualizerValues.TryPeek(out var judgmentVisualizerValue) && judgmentVisualizerValue.LoopingCounter <= 0.0)
                                {
                                    judgmentVisualizerValues.Dequeue();
                                }
                            }
                        }

                        var targetLoopingCounter = LoopingCounter - 1000.0 * AudioMultiplier;
                        while (InputCountQueue.TryPeek(out var loopingCounter) && loopingCounter <= targetLoopingCounter)
                        {
                            InputCountQueue.TryDequeue(out _);
                        }

                        atLoopingCounter1000 += millisLoopUnit;
                        if (atLoopingCounter1000 >= 1000.0)
                        {
                            atLoopingCounter1000 %= 1000.0;
                            MediaSystem.Instance.SetMediaPosition(this);
                            if (WaitingTwilightLevel == WaitingTwilight.CallIO)
                            {
                                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallIo, new
                                {
                                    avatarID = AvatarID,
                                    handlerID = HandlerID,
                                    ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                });
                            }
                        }

                        _millisMeter += millisLoopUnit * AudioMultiplier;
                        if (_millisMeter >= _millisStandardMeter)
                        {
                            _millisMeter %= _millisStandardMeter;
                            if (Configure.Instance.HandleMeter)
                            {
                                RGBSystem.Instance.IsMeter = true;
                                Array.Fill(JudgmentMainFrames, DrawingComponentValue.judgmentMainFrame);
                                lock (DrawingComponentValue.PaintPropertyCSX)
                                {
                                    foreach (var paintPropertyID in DrawingComponentValue.PaintPropertyIDs)
                                    {
                                        var paintPropertyIntMap = DrawingComponentValue.PaintPropertyIntMap[paintPropertyID];
                                        if (paintPropertyIntMap[PaintProperty.ID.Mode] == 1)
                                        {
                                            PaintPropertyFrames[paintPropertyID] = paintPropertyIntMap[PaintProperty.ID.Frame];
                                        }
                                    }
                                }
                            }
                        }

                        if (NetItems.Count > 1)
                        {
                            var hunterVariety = Configure.Instance.HunterVarietyV2Value.Mode;
                            if (hunterVariety == HunterVariety.HunterVarietyMe)
                            {
                                var hasNotMe = true;
                                foreach (var netItem in NetItems)
                                {
                                    if (!netItem.IsMyNetItem && netItem.AvatarID == AvatarID)
                                    {
                                        hasNotMe = false;
                                        break;
                                    }
                                }
                                if (hasNotMe)
                                {
                                    hunterVariety = HunterVariety.HunterVarietyHigher;
                                }
                            }
                            switch (hunterVariety)
                            {
                                case HunterVariety.HunterVariety1st:
                                    var value1st = int.MinValue;
                                    foreach (var netItem in NetItems)
                                    {
                                        if (!netItem.IsMyNetItem)
                                        {
                                            value1st = Math.Max(value1st, netItem.StandValue);
                                        }
                                    }
                                    Hunter.TargetValue = Stand.TargetValue - value1st;
                                    break;
                                case HunterVariety.HunterVarietyHigher:
                                    var valueLowest = int.MaxValue;
                                    var valueHighest = int.MinValue;
                                    foreach (var netItem in NetItems)
                                    {
                                        if (!netItem.IsMyNetItem && netItem.StandValue >= Stand.TargetValue)
                                        {
                                            valueLowest = Math.Min(valueLowest, netItem.StandValue);
                                        }
                                    }
                                    if (valueLowest != int.MaxValue)
                                    {
                                        Hunter.TargetValue = Stand.TargetValue - valueLowest;
                                    }
                                    else
                                    {
                                        valueHighest = int.MinValue;
                                        foreach (var netItem in NetItems)
                                        {
                                            if (!netItem.IsMyNetItem)
                                            {
                                                valueHighest = Math.Max(valueHighest, netItem.StandValue);
                                            }
                                        }
                                        Hunter.TargetValue = Stand.TargetValue - valueHighest;
                                    }
                                    break;
                                case HunterVariety.HunterVarietyLower:
                                    valueHighest = int.MinValue;
                                    foreach (var netItem in NetItems)
                                    {
                                        if (!netItem.IsMyNetItem && netItem.StandValue <= Stand.TargetValue)
                                        {
                                            valueHighest = Math.Max(valueHighest, netItem.StandValue);
                                        }
                                    }
                                    if (valueHighest != int.MinValue)
                                    {
                                        Hunter.TargetValue = Stand.TargetValue - valueHighest;
                                    }
                                    else
                                    {
                                        valueLowest = int.MaxValue;
                                        foreach (var netItem in NetItems)
                                        {
                                            if (!netItem.IsMyNetItem)
                                            {
                                                valueLowest = Math.Min(valueLowest, netItem.StandValue);
                                            }
                                        }
                                        Hunter.TargetValue = Stand.TargetValue - valueLowest;
                                    }
                                    break;
                                case HunterVariety.HunterVarietyMe:
                                    foreach (var netItem in NetItems)
                                    {
                                        if (!netItem.IsMyNetItem && netItem.AvatarID == AvatarID)
                                        {
                                            Hunter.TargetValue = Stand.TargetValue - netItem.StandValue;
                                            break;
                                        }
                                    }
                                    break;
                                case HunterVariety.HunterVarietyFavor:
                                    foreach (var netItem in NetItems)
                                    {
                                        if (netItem.IsFavorNetItem)
                                        {
                                            Hunter.TargetValue = Stand.TargetValue - netItem.StandValue;
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Hunter.TargetValue = null;
                        }

                        _failedDrawingMillis = Math.Max(0.0, _failedDrawingMillis - millisLoopUnit);

                        GetNetItems();
                        GetNetComments();
                        HandleNetItems(millisLoopUnit);
                    }

                    if (LastStatusValue != LastStatus.Not)
                    {
                        if (!wasLastStatus)
                        {
                            if (!IsF)
                            {
                                switch (LastStatusValue)
                                {
                                    case LastStatus.Last:
                                        HandleUIAudio("Last");
                                        break;
                                    case LastStatus.Band1:
                                        HandleUIAudio("Band!");
                                        break;
                                    case LastStatus.Yell1:
                                        HandleUIAudio("Yell!", "Band!");
                                        break;
                                }
                                if (judgmentFrame > 0)
                                {
                                    Component.Judged judged = 0;
                                    var altJudgment = 0.0;
                                    switch (LastStatusValue)
                                    {
                                        case LastStatus.Last:
                                            judged = Component.Judged.Last;
                                            judgmentPosition0 = DrawingComponentValue.lastPosition0;
                                            judgmentPosition1 = DrawingComponentValue.lastPosition1;
                                            judgmentSystem = DrawingComponentValue.lastSystem;
                                            judgmentFrame = DrawingComponentValue.lastFrame;
                                            judgmentFramerate = DrawingComponentValue.lastFramerate;
                                            judgmentLength = DrawingComponentValue.lastLength;
                                            judgmentHeight = DrawingComponentValue.lastHeight;
                                            altJudgment = DrawingComponentValue.altLast;
                                            break;
                                        case LastStatus.Band1:
                                            judged = Component.Judged.Band1;
                                            judgmentPosition0 = DrawingComponentValue.band1Position0;
                                            judgmentPosition1 = DrawingComponentValue.band1Position1;
                                            judgmentSystem = DrawingComponentValue.band1System;
                                            judgmentFrame = DrawingComponentValue.band1Frame;
                                            judgmentFramerate = DrawingComponentValue.band1Framerate;
                                            judgmentLength = DrawingComponentValue.band1Length;
                                            judgmentHeight = DrawingComponentValue.band1Height;
                                            altJudgment = DrawingComponentValue.altBand1;
                                            break;
                                        case LastStatus.Yell1:
                                            judged = Component.Judged.Yell1;
                                            judgmentPosition0 = DrawingComponentValue.yell1Position0;
                                            judgmentPosition1 = DrawingComponentValue.yell1Position1;
                                            judgmentSystem = DrawingComponentValue.yell1System;
                                            judgmentFrame = DrawingComponentValue.yell1Frame;
                                            judgmentFramerate = DrawingComponentValue.yell1Framerate;
                                            judgmentLength = DrawingComponentValue.yell1Length;
                                            judgmentHeight = DrawingComponentValue.yell1Height;
                                            altJudgment = DrawingComponentValue.altYell1;
                                            break;
                                    }
                                    if (!Has2P || altJudgment != 3)
                                    {
                                        lock (JudgmentPaints)
                                        {
                                            JudgmentPaints[2] = new(this, judged, false, judgmentSystem, judgmentPosition0, judgmentPosition1, judgmentFrame, judgmentFramerate, judgmentLength, judgmentHeight);
                                        }
                                    }
                                    if (Has2P && altJudgment != 0)
                                    {
                                        lock (JudgmentPaints)
                                        {
                                            JudgmentPaints[3] = new(this, judged, true, judgmentSystem, judgmentPosition0, judgmentPosition1, judgmentFrame, judgmentFramerate, judgmentLength, judgmentHeight);
                                        }
                                    }
                                }
                            }
                            wasLastStatus = true;
                        }
                    }

                    while (MultiplierQueue.TryDequeue(out var multiplier))
                    {
                        if (double.IsInfinity(multiplier))
                        {
                            if (multiplier > 0.0 ? ModeComponentValue.HigherMultiplier() : ModeComponentValue.LowerMultiplier())
                            {
                                HandleUIAudio("Multiplier");
                            }
                        }
                        else
                        {
                            if (ModeComponentValue.CanModifyMultiplier)
                            {
                                ModeComponentValue.MultiplierValue = multiplier;
                            }
                            else
                            {
                                ModeComponentValue.SentMultiplier = multiplier;
                            }
                        }
                    }
                    if (lastMultiplier != ModeComponentValue.Multiplier)
                    {
                        if (isValidLoopingCounter)
                        {
                            Comment.Multipliers.Add(new MultiplierEvent
                            {
                                Multiplier = ModeComponentValue.Multiplier,
                                Wait = LoopingCounter,
                                IsAutoEvent = false
                            });
                        }
                        if (_sentIOAvatarIDs.Count > 0)
                        {
                            TwilightSystem.Instance.SendParallel(new()
                            {
                                EventID = Event.Types.EventID.IoMultiplier,
                                QwilightIOMultiplier = new()
                                {
                                    AvatarIDs =
                                    {
                                        _sentIOAvatarIDs
                                    },
                                    HandlerID = HandlerID,
                                    Multiplier = ModeComponentValue.Multiplier
                                }
                            });
                        }
                        lastMultiplier = ModeComponentValue.Multiplier;
                    }
                    while (AudioMultiplierQueue.TryDequeue(out var audioMultiplierValue))
                    {
                        if (double.IsInfinity(audioMultiplierValue))
                        {
                            var isHigherAudioMultiplier = audioMultiplierValue > 0.0;
                            if ((WwwLevelDataValue == null || (isHigherAudioMultiplier ? Math.Round(WwwLevelDataValue.HighestAudioMultiplier - 0.01, 2) >= AudioMultiplier : AudioMultiplier >= Math.Round(WwwLevelDataValue.LowestAudioMultiplier + 0.01, 2))) && (isHigherAudioMultiplier ? ModeComponentValue.HigherAudioMultiplier() : ModeComponentValue.LowerAudioMultiplier()))
                            {
                                HandleUIAudio("Audio Multiplier");
                            }
                        }
                        else
                        {
                            ModeComponentValue.AudioMultiplier = audioMultiplierValue;
                        }
                    }
                    if (lastAudioMultiplier != AudioMultiplier)
                    {
                        if (isValidLoopingCounter)
                        {
                            Comment.AudioMultipliers.Add(new AudioMultiplierEvent
                            {
                                AudioMultiplier = AudioMultiplier,
                                Wait = LoopingCounter
                            });
                            Comment.Multipliers.Add(new MultiplierEvent
                            {
                                Multiplier = ModeComponentValue.Multiplier,
                                Wait = LoopingCounter,
                                IsAutoEvent = true
                            });
                        }
                        if (_sentIOAvatarIDs.Count > 0)
                        {
                            TwilightSystem.Instance.SendParallel(new()
                            {
                                EventID = Event.Types.EventID.IoAudioMultiplier,
                                QwilightIOAudioMultiplier = new()
                                {
                                    AvatarIDs =
                                    {
                                        _sentIOAvatarIDs
                                    },
                                    HandlerID = HandlerID,
                                    AudioMultiplier = AudioMultiplier
                                }
                            });
                        }
                        AudioSystem.Instance.SetAudioMultiplier(this, AudioMultiplier);
                        MediaSystem.Instance.SetAudioMultiplier(this);
                        SetStandMultiplier();
                        lastAudioMultiplier = AudioMultiplier;
                    }

                    if (!IsInEvents)
                    {
                        loopingHandlerMillis += millisLoopUnit;
                        var toWait = loopingHandlerMillis - _loopingHandler.GetMillis();
#if !DEBUG
                        if (toWait < -5000.0)
                        {
                            SetStop = true;
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.SlowLoopingContents);
                        }
                        else
#endif
                        if (toWait > 0.0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(toWait));
                        }
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (Exception e)
            {
                var (faultFilePath, _) = Utility.SaveFaultFile(FaultEntryPath, e);
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, e.Message, true, string.Empty, () => Utility.OpenAs(faultFilePath));
            }
            finally
            {
                if (SetUndo)
                {
                    if (_sentIOAvatarIDs.Count > 0)
                    {
                        _pendingIOAvatarIDs.AddRange(_sentIOAvatarIDs);
                        _sentIOAvatarIDs.Clear();
                    }
                    _targetHandler = Utility.HandleParallelly(HandleNotes);
                }
                else
                {
                    if (_ioAvatarIDs.Count > 0)
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.IoQuit, new
                        {
                            handlerID = HandlerID,
                            avatarIDs = _ioAvatarIDs,
                            isBanned = false
                        });
                    }
                    IsHandling = false;
                }
            }

            void SetNoteJudged(BaseNote note, Component.Judged judged)
            {
                note.Judged = judged;
                SendIOJudge(note);
            }

            void SetNoteFailed(BaseNote note, bool setValidJudgedNotes)
            {
                note.IsFailed = true;
                foreach (var audioChannel in note.AudioChannels)
                {
                    lock (LoadedCSX)
                    {
                        if (HasContents)
                        {
                            AudioSystem.Instance.Stop(audioChannel);
                        }
                    }
                }
                SendIONoteVisibility(note, setValidJudgedNotes, true);
            }

            void OnNoteJudged(BaseNote judgedNote)
            {
                var targetInput = judgedNote.TargetInput;
                if (IsSuitableAsInput(targetInput))
                {
                    SetJudgedValue(judgedNote.Judged, IsIn2P[targetInput], judgedNote);
                    SetIIDXInputAudioVariety(judgedNote);
                }
                if (!IsInEvents)
                {
                    if (judgedNote.Judged != Component.Judged.Lowest)
                    {
                        if (judgedNote.LongWait > 0.0)
                        {
                            var longNoteHitFrame = DrawingComponentValue.longNoteHitFrame;
                            if (longNoteHitFrame > 0)
                            {
                                lock (HitLongNotePaints)
                                {
                                    HitLongNotePaints[-targetInput] = new(this, targetInput, longNoteHitFrame, DrawingComponentValue.longNoteHitFramerate);
                                }
                            }
                        }
                        else
                        {
                            var noteHitFrame = DrawingComponentValue.noteHitFrame;
                            if (noteHitFrame > 0)
                            {
                                lock (HitNotePaints)
                                {
                                    HitNotePaints[targetInput] = new(this, targetInput, noteHitFrame, DrawingComponentValue.noteHitFramerate);
                                }
                            }
                        }
                    }
                }
            }

            void SetJudgedValue(Component.Judged judged, bool isIn2P, BaseNote note)
            {
                var isLowestJudgment = judged == Component.Judged.Lowest;
                _failedDrawingMillis = isLowestJudgment ? Configure.Instance.FailedDrawingMillis : 0.0;
                if (isLowestJudgment)
                {
                    if (!IsInEvents)
                    {
                        if (note is TrapNote)
                        {
                            if (note.AudioNotes.Count > 0)
                            {
                                foreach (var audioNote in note.AudioNotes)
                                {
                                    lock (LoadedCSX)
                                    {
                                        if (HasContents)
                                        {
                                            note.AudioChannels.Add(AudioSystem.Instance.Handle(audioNote, AudioSystem.InputAudio, AudioMultiplier, IsCounterWave, this));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                HandleUIAudio("Trap");
                            }
                            if (Configure.Instance.VibrationModeValue == ControllerSystem.VibrationMode.Failed)
                            {
                                ControllerSystem.Instance.Vibrate();
                            }
                        }
                        else if (Band.TargetValue >= Configure.Instance.HandleFailedAudioCount && HandleFailedAudio)
                        {
                            HandleUIAudio("Failed");
                            if (Configure.Instance.VibrationModeValue == ControllerSystem.VibrationMode.Failed)
                            {
                                ControllerSystem.Instance.Vibrate();
                            }
                        }
                    }
                    Band.TargetValue = 0;
                    postableItemBand = 0;
                    HasFailedJudgment = true;
                    ++Comment.LowestJudgment;
                    OnFailed();
                }
                else
                {
                    HighestBand = Math.Max(++Band.TargetValue, HighestBand);
                    if (!IsF && ++postableItemBand == PostableItemBand)
                    {
                        postableItemBand = 0;
                        var postableItemsLength = PostableItems.Length;
                        for (var i = 0; i < postableItemsLength; ++i)
                        {
                            if (PostableItems[i] == null)
                            {
                                var postableItem = AllowedPostableItems[Random.Shared.Next(AllowedPostableItems.Length)];
                                PostableItems[i] = postableItem;
                                LastPostableItems[i] = postableItem;
                                PostableItemFaints[i].TargetValue = 1.0;
                                AudioSystem.Instance.Handle(new()
                                {
                                    AudioItem = AudioSystem.Instance.PostableItemAudio
                                }, AudioSystem.SEAudio);
                                break;
                            }
                        }
                    }
                    switch (judged)
                    {
                        case Component.Judged.Highest:
                            ++Comment.HighestJudgment;
                            break;
                        case Component.Judged.Higher:
                            ++Comment.HigherJudgment;
                            break;
                        case Component.Judged.High:
                            ++Comment.HighJudgment;
                            break;
                        case Component.Judged.Low:
                            ++Comment.LowJudgment;
                            break;
                        case Component.Judged.Lower:
                            ++Comment.LowerJudgment;
                            break;
                        case Component.Judged.Lowest:
                            break;
                    }
                }
                if (PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.PositiveHitPointsLevel]].IsHandling || PostableItemStatusMap[PostableItem.Values[(int)PostableItem.Variety.NegativeHitPointsLevel]].IsHandling)
                {
                    ++postableHitPointsCount;
                }
                LastJudged = judged;
                Point.TargetValue = (SavedPoint += Component.PointMap[(int)_pointMapDate, (int)judged]) / (TotalPoint += Component.PointMap[(int)_pointMapDate, (int)Component.Judged.Highest]);
                switch (_standModeDate)
                {
                    case Component.StandModeDate._1_0_0:
                        Stand.TargetValue = LastStand + (int)(1000000 * (SavedStand += _standMultiplier * Point.TargetValue * Component.StandMap[(int)_standMapDate, (int)judged]) / (TotalNotes * Component.StandMap[(int)_standMapDate, (int)Component.Judged.Highest]));
                        break;
                    case Component.StandModeDate._1_6_7:
                        Stand.TargetValue = LastStand + (int)(1000000 * (SavedStand += _standMultiplier * (HasFailedJudgment ? Point.TargetValue : 1.0) * Component.StandMap[(int)_standMapDate, (int)judged]) / (TotalNotes * Component.StandMap[(int)_standMapDate, (int)Component.Judged.Highest]));
                        break;
                    case Component.StandModeDate._1_14_118:
                        Stand.TargetValue = LastStand + (int)(1000000 * (SavedStand += _standMultiplier * Component.StandMap[(int)_standMapDate, (int)judged]) / (TotalNotes * Component.StandMap[(int)_standMapDate, (int)Component.Judged.Highest]));
                        break;
                }
                if (!IsF)
                {
                    switch (_hitPointsModeDate)
                    {
                        case Component.HitPointsModeDate._1_0_0:
                            if (judged == Component.Judged.Lowest)
                            {
                                HitPoints.TargetValue -= 2.0 * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            else
                            {
                                HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            break;
                        case Component.HitPointsModeDate._1_2_3:
                            if (judged == Component.Judged.Lowest)
                            {
                                HitPoints.TargetValue -= (1.5 * HitPoints.TargetValue + 0.5) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            else
                            {
                                HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            break;
                        case Component.HitPointsModeDate._1_10_34:
                            if (judged == Component.Judged.Lowest)
                            {
                                HitPoints.TargetValue -= (1.25 * HitPoints.TargetValue + 0.75) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            else
                            {
                                HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                            }
                            break;
                        case Component.HitPointsModeDate._1_10_35:
                            switch (ModeComponentValue.HandlingHitPointsModeValue)
                            {
                                case ModeComponent.HitPointsMode.Favor:
                                    HitPoints.TargetValue += HitPointsValue * ModeComponentValue.FavorHitPoints[(int)judged][0] / 100.0 + ModeComponentValue.FavorHitPoints[(int)judged][1] / 100.0;
                                    break;
                                default:
                                    if (judged == Component.Judged.Lowest)
                                    {
                                        HitPoints.TargetValue -= (1.5 * HitPoints.TargetValue + 0.5) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    else
                                    {
                                        HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    break;
                            }
                            break;
                        case Component.HitPointsModeDate._1_14_62:
                            switch (ModeComponentValue.HandlingHitPointsModeValue)
                            {
                                case ModeComponent.HitPointsMode.Favor:
                                    HitPoints.TargetValue += HitPointsValue * ModeComponentValue.FavorHitPoints[(int)judged][0] / 100.0 + ModeComponentValue.FavorHitPoints[(int)judged][1] / 100.0;
                                    break;
                                case ModeComponent.HitPointsMode.Test:
                                    if (judged == Component.Judged.Lowest)
                                    {
                                        HitPoints.TargetValue -= (HitPoints.TargetValue + 1.0) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    else
                                    {
                                        HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    break;
                                case ModeComponent.HitPointsMode.Highest:
                                    if (judged == Component.Judged.Lowest)
                                    {
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] -= (1.5 * _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] + 0.5) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Default, (int)judged];
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher] -= (_hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher] + 1.0) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Higher, (int)judged];
                                        HitPoints.TargetValue -= 2.0 * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    else
                                    {
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Default, (int)judged];
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher] += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Higher, (int)judged];
                                        HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    break;
                                case ModeComponent.HitPointsMode.Higher:
                                    if (judged == Component.Judged.Lowest)
                                    {
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] -= (1.5 * _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] + 0.5) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Default, (int)judged];
                                        HitPoints.TargetValue -= (HitPoints.TargetValue + 1.0) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    else
                                    {
                                        _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponent.HitPointsMode.Default, (int)judged];
                                        HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    break;
                                default:
                                    if (judged == Component.Judged.Lowest)
                                    {
                                        HitPoints.TargetValue -= (1.5 * HitPoints.TargetValue + 0.5) * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    else
                                    {
                                        HitPoints.TargetValue += HitPointsValue * Component.HitPointsMap[(int)_hitPointsMapDate, (int)ModeComponentValue.HandlingHitPointsModeValue, (int)judged];
                                    }
                                    break;
                            }
                            break;
                    }
                    HitPoints.TargetValue = Math.Clamp(HitPoints.TargetValue, 0.0, 1.0);
                    _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default] = Math.Clamp(_hitPointsGAS[(int)ModeComponent.HitPointsMode.Default], 0.0, 1.0);
                    _hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher] = Math.Clamp(_hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher], 0.0, 1.0);
                    _hitPointsGAS[(int)ModeComponent.HitPointsMode.Highest] = Math.Clamp(_hitPointsGAS[(int)ModeComponent.HitPointsMode.Highest], 0.0, 1.0);

                    Comment.Paints.Add(new PaintEvent
                    {
                        HitPoints = HitPoints.TargetValue,
                        Stand = Stand.TargetValue,
                        Band = Band.TargetValue,
                        Point = Point.TargetValue,
                        Wait = LoopingCounter
                    });
                    switch (ModeComponentValue.HandlingHitPointsModeValue)
                    {
                        case ModeComponent.HitPointsMode.Highest:
                            _paintEventsGAS[(int)ModeComponent.HitPointsMode.Higher].Add(new()
                            {
                                HitPoints = _hitPointsGAS[(int)ModeComponent.HitPointsMode.Higher],
                                Stand = Stand.TargetValue,
                                Band = Band.TargetValue,
                                Point = Point.TargetValue,
                                Wait = LoopingCounter
                            });

                            _paintEventsGAS[(int)ModeComponent.HitPointsMode.Default].Add(new()
                            {
                                HitPoints = _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default],
                                Stand = Stand.TargetValue,
                                Band = Band.TargetValue,
                                Point = Point.TargetValue,
                                Wait = LoopingCounter
                            });
                            break;
                        case ModeComponent.HitPointsMode.Higher:
                            _paintEventsGAS[(int)ModeComponent.HitPointsMode.Default].Add(new()
                            {
                                HitPoints = _hitPointsGAS[(int)ModeComponent.HitPointsMode.Default],
                                Stand = Stand.TargetValue,
                                Band = Band.TargetValue,
                                Point = Point.TargetValue,
                                Wait = LoopingCounter
                            });
                            break;
                    }
                    WwwLevelDataValue?.SetSatisify(this);
                }
                if (!IsInEvents)
                {
                    if (judged != Component.Judged.Not && (ViewLowestJudgment || judged != Component.Judged.Lowest))
                    {
                        lock (JudgmentPaints)
                        {
                            JudgmentPaints[isIn2P ? 1 : 0] = new(this, judged, isIn2P, DrawingComponentValue.judgmentPaintSystem, DrawingComponentValue.judgmentPaintPosition0, DrawingComponentValue.judgmentPaintPosition1, DrawingComponentValue.judgmentFrame, DrawingComponentValue.judgmentFramerate, DrawingComponentValue.judgmentPaintLength, DrawingComponentValue.judgmentPaintHeight);
                        }
                    }
                }

                var judgmentInputPosition = (int)((note?.Wait ?? LoopingCounter) / wait100);
                var judgmentInputCounts = rawJudgmentInputCounts.ElementAtOrDefault(judgmentInputPosition);
                if (judgmentInputCounts != null)
                {
                    ++judgmentInputCounts[(int)judged];

                    var totalJudgmentInputCount = judgmentInputCounts.Sum();
                    if (lastJudgmentInputMaxValue < totalJudgmentInputCount)
                    {
                        lastJudgmentInputMaxValue = totalJudgmentInputCount;
                        for (var i = JudgmentInputValues.Length - 1; i >= 0; --i)
                        {
                            var judgmentInputValues = JudgmentInputValues[i];
                            for (var j = judgmentInputValues.Length - 1; j >= 0; --j)
                            {
                                judgmentInputValues[j] = (double)(rawJudgmentInputCounts[i][j]) / lastJudgmentInputMaxValue;
                            }
                        }
                    }
                    else
                    {
                        var judgmentInputValues = JudgmentInputValues[judgmentInputPosition];
                        for (var i = judgmentInputValues.Length - 1; i >= 0; --i)
                        {
                            judgmentInputValues[i] = (double)(judgmentInputCounts[i]) / lastJudgmentInputMaxValue;
                        }
                    }
                }
            }

            void StopLastEqualAudioItem(string bmsID)
            {
                if (Configure.Instance.StopLastEqualAudio && !string.IsNullOrEmpty(bmsID) && audioChannelMap.TryGetValue(bmsID, out var audioChannel))
                {
                    AudioSystem.Instance.Stop(audioChannel);
                }
            }
        }

        public virtual void SendSituation()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)(mainViewModel.IsQuitMode ? UbuntuItem.UbuntuSituation.QuitMode : UbuntuItem.UbuntuSituation.DefaultComputing),
                    situationText = PlatformText
                });
            }
        }

        public virtual void AtQuitMode()
        {
            Utility.HandleUIAudio(IsF ? "F" : "Quit");

            if (!IsPostableItemMode)
            {
                var date = DateTime.Now;
                var eventNoteID = EventNoteEntryItem?.EventNoteID;
                if (string.IsNullOrEmpty(eventNoteID))
                {
                    var lowestAudioMultiplier = new[] { TotallyLevyingAudioMultiplier }.Concat(Comment.AudioMultipliers.Select(audioMultiplier => audioMultiplier.AudioMultiplier)).Min();
                    if (lowestAudioMultiplier >= 1.0 && ModeComponentValue.IsDefaultHandled)
                    {
                        if (NoteFile.HandledValue != BaseNoteFile.Handled.Yell1)
                        {
                            if (IsYell1)
                            {
                                NoteFile.HandledValue = BaseNoteFile.Handled.Yell1;
                            }
                            else
                            {
                                if (NoteFile.HandledValue != BaseNoteFile.Handled.Band1)
                                {
                                    if (IsBand1)
                                    {
                                        NoteFile.HandledValue = BaseNoteFile.Handled.Band1;
                                    }
                                    else
                                    {
                                        if (IsF)
                                        {
                                            if (NoteFile.HandledValue == BaseNoteFile.Handled.Not)
                                            {
                                                NoteFile.HandledValue = BaseNoteFile.Handled.F;
                                            }
                                        }
                                        else
                                        {
                                            switch (ModeComponentValue.HandlingHitPointsModeValue)
                                            {
                                                case ModeComponent.HitPointsMode.Highest:
                                                    NoteFile.HandledValue = BaseNoteFile.Handled.HighestClear;
                                                    break;
                                                case ModeComponent.HitPointsMode.Higher when NoteFile.HandledValue != BaseNoteFile.Handled.HighestClear:
                                                    NoteFile.HandledValue = BaseNoteFile.Handled.HigherClear;
                                                    break;
                                                case ModeComponent.HitPointsMode.Default when NoteFile.HandledValue != BaseNoteFile.Handled.HigherClear && NoteFile.HandledValue != BaseNoteFile.Handled.HighestClear:
                                                    NoteFile.HandledValue = BaseNoteFile.Handled.Clear;
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (NoteFile.HandledValue == BaseNoteFile.Handled.Not || NoteFile.HandledValue == BaseNoteFile.Handled.F)
                        {
                            if (IsF)
                            {
                                NoteFile.HandledValue = BaseNoteFile.Handled.F;
                            }
                            else
                            {
                                NoteFile.HandledValue = BaseNoteFile.Handled.AssistClear;
                            }
                        }
                    }

                    DB.Instance.SetHandled(NoteFile);
                    DB.Instance.NewDate(NoteFile, default, date);
                    NoteFile.LatestDate = date;
                    ++NoteFile.HandledCount;
                }
                else
                {
                    DB.Instance.NewDate(default, eventNoteID, date);
                    EventNoteEntryItem.LatestDate = date;
                    ++EventNoteEntryItem.HandledCount;
                }

                if (!IsF)
                {
                    if (!ModeComponentValue.PutCopyNotesAvailable)
                    {
                        var commentID = Guid.NewGuid().ToString().Replace("-", string.Empty);
                        if (string.IsNullOrEmpty(eventNoteID))
                        {
                            try
                            {
                                DB.Instance.SaveComment(date, NoteFile, string.Empty, commentID, AvatarName, TotallyLevyingMultiplier, TotallyLevyingAudioMultiplier, ModeComponentValue, Stand.TargetValue, HighestBand, IsBand1, Point.TargetValue, _isPaused, _inputFlags);
                                using var fs = File.OpenWrite(Path.Combine(QwilightComponent.CommentEntryPath, commentID));
                                _comments.Single().WriteTo(fs);
                            }
                            catch (Exception e)
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SaveCommentFailed, e.Message));
                            }
                            if (!IsBanned && ModeComponentValue.CanBeTwilightComment && Configure.Instance.AllowTwilightComment)
                            {
                                if (TwilightSystem.Instance.IsLoggedIn)
                                {
                                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.Comment, new
                                    {
                                        dataID = NoteFile.DataID,
                                        multiplier = TotallyLevyingMultiplier,
                                        autoMode = (int)ModeComponentValue.AutoModeValue,
                                        noteSaltMode = (int)ModeComponentValue.NoteSaltModeValue,
                                        audioMultiplier = TotallyLevyingAudioMultiplier,
                                        faintNoteMode = (int)ModeComponentValue.FaintNoteModeValue,
                                        judgmentMode = (int)ModeComponentValue.JudgmentModeValue,
                                        hitPointsMode = (int)ModeComponentValue.HandlingHitPointsModeValue,
                                        noteMobilityMode = (int)ModeComponentValue.NoteMobilityModeValue,
                                        longNoteMode = (int)ModeComponentValue.LongNoteModeValue,
                                        inputFavorMode = (int)ModeComponentValue.InputFavorModeValue,
                                        noteModifyMode = (int)ModeComponentValue.NoteModifyModeValue,
                                        lowestJudgmentConditionMode = (int)ModeComponentValue.LowestJudgmentConditionModeValue,
                                        stand = Stand.TargetValue,
                                        band = HighestBand,
                                        point = Point.TargetValue,
                                        salt = ModeComponentValue.Salt,
                                        isPaused = _isPaused,
                                        inputFlags = _inputFlags
                                    }, UnsafeByteOperations.UnsafeWrap(NoteFileContents), Comment.ToByteString());
                                }
                                else if (QwilightComponent.IsValve)
                                {
                                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.ValveComment, new
                                    {
                                        dataID = NoteFile.DataID,
                                        stand = Stand.TargetValue,
                                        hitPointsMode = (int)ModeComponentValue.HandlingHitPointsModeValue
                                    }, UnsafeByteOperations.UnsafeWrap(NoteFileContents));
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                DB.Instance.SaveComment(date, default, eventNoteID, commentID, AvatarName, TotallyLevyingMultiplier, TotallyLevyingAudioMultiplier, ModeComponentValue, Stand.TargetValue, HighestBand, IsBand1, Point.TargetValue, _isPaused, _inputFlags);
                                using (var zipFile = new ZipFile(Path.Combine(QwilightComponent.CommentEntryPath, Path.ChangeExtension(commentID, ".zip"))))
                                {
                                    for (var i = _comments.Count - 1; i >= 0; --i)
                                    {
                                        using var rms = PoolSystem.Instance.GetDataFlow(_comments[i].CalculateSize());
                                        _comments[i].WriteTo(rms);
                                        rms.Position = 0;
                                        zipFile.AddEntry(i.ToString(), rms);
                                        zipFile.Save();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SaveCommentFailed, e.Message));
                            }
                        }
                        if (WwwLevelDataValue != null)
                        {
                            TwilightSystem.Instance.SendParallel(Event.Types.EventID.WwwLevel, new
                            {
                                noteID = eventNoteID ?? NoteFile.GetNoteID512(),
                                stand = Stand.TargetValue,
                                point = Point.TargetValue,
                                band = HighestBand,
                                judgments = new[]
                                {
                                    InheritedHighestJudgment,
                                    InheritedHigherJudgment,
                                    InheritedHighJudgment,
                                    InheritedLowJudgment,
                                    InheritedLowerJudgment,
                                    InheritedLowestJudgment
                                },
                                autoMode = (int)ModeComponentValue.AutoModeValue,
                                noteSaltMode = (int)ModeComponentValue.NoteSaltModeValue,
                                faintNoteMode = (int)ModeComponentValue.FaintNoteModeValue,
                                judgmentMode = (int)ModeComponentValue.JudgmentModeValue,
                                hitPointsMode = (int)ModeComponentValue.HandlingHitPointsModeValue,
                                noteMobilityMode = (int)ModeComponentValue.NoteMobilityModeValue,
                                longNoteMode = (int)ModeComponentValue.LongNoteModeValue,
                                inputFavorMode = (int)ModeComponentValue.InputFavorModeValue,
                                noteModifyMode = (int)ModeComponentValue.NoteModifyModeValue,
                                bpmMode = (int)ModeComponentValue.BPMModeValue,
                                waveMode = (int)ModeComponentValue.WaveModeValue,
                                setNoteMode = (int)ModeComponentValue.SetNoteModeValue,
                                lowestJudgmentConditionMode = (int)ModeComponentValue.LowestJudgmentConditionModeValue
                            });
                        }
                    }
                }
            }

            IsNewStand = Stand.TargetValue > _netItems.Where(netItem => !netItem.IsMyNetItem && netItem.AvatarID == AvatarID).DefaultIfEmpty().Max(netItem => netItem?.StandValue ?? 0);
        }

        public void HandleCompiler()
        {
            try
            {
                SetUIMap();
            }
            catch
            {
            }
            if (_targetCompiler != null)
            {
                lock (_targetCompiler)
                {
                    _targetCompilerStatus = CompilerStatus.Compiling;
                    _targetCompilerHandler.Start();
                }
            }
        }

        public void SetWait() => DB.Instance.SetWait(NoteFile, Math.Clamp(Configure.Instance.AudioWait, -1000.0, 1000.0), Math.Clamp(Configure.Instance.MediaWait, -1000.0, 1000.0), Configure.Instance.Media);

        IHandlerItem GetHandlerItem(MediaNote.Mode mode) => ViewFailedDrawing && _failedDrawingMillis > 0.0 && MediaCollection.TryGetValue(MediaNote.Mode.Failed, out var failedHandler) ? failedHandler : MediaCollection.TryGetValue(mode, out var handler) ? handler : null;

        public virtual void SendNotCompiled()
        {
        }

        public virtual void InitPassable()
        {
            LevyingMultiplier = ModeComponentValue.Multiplier;
            LevyingAudioMultiplier = AudioMultiplier;
            Comment.LevyingMultiplier = LevyingMultiplier;
            Comment.LevyingAudioMultiplier = LevyingAudioMultiplier;
            if (_isLevyingComputer)
            {
                TotallyLevyingMultiplier = LevyingMultiplier;
                TotallyLevyingAudioMultiplier = LevyingAudioMultiplier;
            }
        }

        public void Init()
        {
            _loopingHandler.Reset();
            for (var i = _lastIIDXInputAudioNoteMap.Length - 1; i > 0; --i)
            {
                _lastIIDXInputAudioNoteMap[i] = new();
            }
            NoteMobilityCosine = 1.0;
            NoteMobilityValue = 0.0;
            FaintCosine = 1.0;
            FaintLayered = 0.0;
            ModeComponentValue.HandlingHitPointsModeValue = ModeComponentValue.HitPointsModeValue;
            _failedDrawingMillis = 0.0;
            _validJudgedNotes = 0;
            _isPassable = false;
            _isEscapable = false;
            _lastStatus = LastStatus.Not;
            SetJudgmentMillis();
            SetStandMultiplier();
            HitPoints.TargetValue = 1.0;
            HitPoints.Value = 1.0;
            Array.Fill(_hitPointsGAS, HitPoints.TargetValue);
            Point.TargetValue = 1.0;
            Point.Value = 1.0;
            HandlingBPM = LevyingBPM;
            _valueComponent.SetBPM(LevyingBPM);
            _millisStandardMeter = 60000.0 / LevyingBPM;
            _millisMeter = 0.0;
            IsMediaHandling = false;
            HasFailedJudgment = false;
            _isPaused = false;
            _inputFlags = InputFlag.Not;
            AudioSystem.Instance.Stop(this);
            MediaSystem.Instance.Stop(this);
            foreach (var note in Notes)
            {
                note.Init();
            }
            foreach (var netItem in NetItems)
            {
                netItem.LastPaintEventPosition = 0;
            }
            Array.Clear(JudgmentMeters, 0, JudgmentMeters.Length);
            Array.Clear(_targetMainFrames, 0, _targetMainFrames.Length);
            Array.Clear(_targetInputFrames, 0, _targetInputFrames.Length);
            foreach (var paintEventsGAS in _paintEventsGAS)
            {
                paintEventsGAS.Clear();
            }
            if (SetPass)
            {
                SetPass = false;
            }
            else
            {
                Comment.HighestJudgment = 0;
                Comment.HigherJudgment = 0;
                Comment.HighJudgment = 0;
                Comment.LowJudgment = 0;
                Comment.LowerJudgment = 0;
                Comment.LowestJudgment = 0;
                Comment.Inputs.Clear();
                Comment.Multipliers.Clear();
                Comment.AudioMultipliers.Clear();
                Comment.JudgmentMeters.Clear();
                Comment.Paints.Clear();
                InitPassable();
            }
            IsPausing = false;
            if (SetUndo)
            {
                SetUndo = false;
                IsPausingWindowOpened = false;
                SetPause = false;
            }
            else if (!_isLevyingComputer && WwwLevelDataValue?.AllowPause == false)
            {
                Configure.Instance.DefaultSpinningModeValue = Configure.DefaultSpinningMode.Unpause;
                IsPausingWindowOpened = true;
                SetPause = true;
            }
            _noteWaits.Clear();
            EarlyValue = 0;
            LateValue = 0;
            IsF.SetValue(false);
            LoopingCounter = -Component.LevyingWait;
            Status = 0.0;
            Band.TargetValue = 0;
            Stand.TargetValue = 0;
            HighestBand = 0;
            SavedStand = 0.0;
            TotalPoint = 0.0;
            SavedPoint = 0.0;
            _bandPaintMillis = 0.0;
            _mainMillis = 0.0;
            _judgmentMainPaintMillis = 0.0;
            _inputMillis = 0.0;
            _levelMillis = 0.0;
            _pauseMillis = 0.0;
            _lastPaintedBand = 0;
            _noteMillis = 0.0;
            _eventPositions.Clear();
            _handlingNotes.Clear();
            lock (PaintedNotes)
            {
                PaintedNotes.Clear();
            }
            MediaCollection.Clear();
            foreach (var judgmentVisualizerValues in JudgmentVisualizerValues)
            {
                lock (judgmentVisualizerValues)
                {
                    judgmentVisualizerValues.Clear();
                }
            }
            InputCountQueue.Clear();
            lock (JudgmentPaints)
            {
                JudgmentPaints.Clear();
            }
            lock (HitNotePaints)
            {
                HitNotePaints.Clear();
            }
            lock (HitLongNotePaints)
            {
                HitLongNotePaints.Clear();
            }
            lock (JudgmentInputValues)
            {
                foreach (var judgmentInputValues in JudgmentInputValues)
                {
                    Array.Clear(judgmentInputValues, 0, judgmentInputValues.Length);
                }
            }
            _lazyInit?.Invoke();
            IOLazyInit?.Invoke();
        }

        public virtual void OnHandled()
        {
            if (!IsF)
            {
                SetCommentFile();
            }
            if (!IsF && LevyingComputingPosition + 1 < NoteFiles.Length)
            {
                SetStop = true;
                SetInheritedValues();
                ModeComponentValue.ComputingValue = NoteFiles[LevyingComputingPosition + 1];
                ModeComponentValue.SentMultiplier = ModeComponentValue.MultiplierValue / (ModeComponentValue.BPM * ModeComponentValue.AudioMultiplier);
                ModeComponentValue.HitPointsModeValue = ModeComponentValue.HandlingHitPointsModeValue;
                ViewModels.Instance.MainValue.SetComputingMode(this is CommentCompute ? new CommentCompute(
                    NoteFiles,
                    Comments,
                    DefaultModeComponentValue,
                    AvatarID,
                    AvatarName,
                    UbuntuID,
                    HandlerID,
                    EventNoteEntryItem,
                    this,
                    double.NaN
                ) : new DefaultCompute(
                    NoteFiles,
                    Comments,
                    DefaultModeComponentValue,
                    AvatarID,
                    AvatarName,
                    UbuntuID,
                    WwwLevelDataValue,
                    HandlerID,
                    EventNoteEntryItem,
                    this)
                );
            }
            else
            {
                FadingViewLayer = IsF ? 3 : 2;
                SetQuitMode();
            }
        }

        public void SetAutoNoteWait()
        {
            if (Configure.Instance.AutoNoteWait && _noteWaits.Count > 0)
            {
                Configure.Instance.UIConfigureValue.NoteWait += _noteWaits.Average();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SetAutoNoteWait, (int)Configure.Instance.UIConfigureValue.NoteWait));
            }
        }

        public virtual void HandleNetItems(double millisLoopUnit)
        {
            foreach (var netItem in NetItems)
            {
                if (netItem.IsMyNetItem)
                {
                    netItem.SetValue(Stand.TargetValue, Band.TargetValue, Point.TargetValue, HitPoints.TargetValue, NoteFiles.Length);
                    netItem.HitPointsModeValue = ModeComponentValue.HandlingHitPointsModeValue;
                }
                else if (netItem.IsFavorNetItem)
                {
                    var netItemStatus = (double)_validJudgedNotes / ValidatedTotalNotes;
                    netItem.SetValue((int)(Configure.Instance.FavorHunterStand * (LevyingComputingPosition + netItemStatus)), (int)(TotalNotes * netItemStatus), 1.0, 1.0, NoteFiles.Length);
                }
                else
                {
                    var paintEvents = netItem.Comment?.Paints;
                    var paintEventsCount = paintEvents?.Count ?? 0;
                    if (_wasGetNetComments && paintEventsCount > 0)
                    {
                        switch (netItem.PaintEventsDate)
                        {
                            case Component.PaintEventsDate._1_0_0:
                                switch (Status)
                                {
                                    case 0.0:
                                        netItem.SetValue(0, 0, 1.0, 1.0, NoteFiles.Length);
                                        break;
                                    case 1.0:
                                        netItem.SetValue(netItem.CommentItem.Stand, netItem.BandValue, netItem.CommentItem.Point, netItem.HitPoints, NoteFiles.Length);
                                        break;
                                    default:
                                        var paintEventEstimated = paintEvents[(int)(paintEventsCount * Status)];
                                        netItem.SetValue(paintEventEstimated.Stand, paintEventEstimated.Band, paintEventEstimated.Point, paintEventEstimated.HitPoints, NoteFiles.Length);
                                        break;
                                }
                                break;
                            case Component.PaintEventsDate._1_14_91:
                                if (LoopingCounter < paintEvents[0].Wait)
                                {
                                    netItem.SetValue(0, 0, 1.0, 1.0, NoteFiles.Length);
                                }
                                else
                                {
                                    while (netItem.LastPaintEventPosition < paintEventsCount - 1 && paintEvents[netItem.LastPaintEventPosition + 1].Wait <= LoopingCounter)
                                    {
                                        ++netItem.LastPaintEventPosition;
                                    }
                                    var paintEventClosed = paintEvents[netItem.LastPaintEventPosition];
                                    netItem.SetValue(paintEventClosed.Stand, paintEventClosed.Band, paintEventClosed.Point, paintEventClosed.HitPoints, NoteFiles.Length);
                                }
                                break;
                        }
                    }
                }
            }

            var targetPosition = SetValidNetItems();
            LevyingNetPosition = Math.Max(0, targetPosition - (Configure.Instance.NetItemCount - 1));
            QuitNetPosition = Math.Min(NetItems.Count - 1, LevyingNetPosition + Configure.Instance.NetItemCount - 1);
        }

        int SetValidNetItems()
        {
            var targetPosition = 0;
            foreach (var netItem in NetItems)
            {
                var setTargetPosition = 0;
                foreach (var targetNetItem in NetItems)
                {
                    if (targetNetItem.CompareTo(netItem) > 0)
                    {
                        ++setTargetPosition;
                    }
                }
                netItem.SetTargetPosition = setTargetPosition;
                if (netItem.IsMyNetItem)
                {
                    targetPosition = setTargetPosition;
                }
            }
            foreach (var netItem in NetItems)
            {
                netItem.TargetPosition = netItem.SetTargetPosition;
            }
            return targetPosition;
        }

        public virtual void GetNetItems()
        {
            if (!_wasNetItems)
            {
                _wasNetItems = true;
                var netItem = new NetItem(AvatarID, AvatarName, DateTime.Now)
                {
                    IsMyNetItem = true,
                    HitPointsModeValue = ModeComponentValue.HandlingHitPointsModeValue
                };
                lock (IsTwilightNetItemsCSX)
                {
                    if (!IsTwilightNetItems)
                    {
                        SetNetItems(new()
                        {
                            netItem
                        });
                    }
                }

                _ = Awaitable();
                async Task Awaitable()
                {
                    var eventNoteID = EventNoteEntryItem?.EventNoteID;
                    var netItems = new List<NetItem>();
                    switch (!string.IsNullOrEmpty(eventNoteID) || NoteFile.IsBanned ? 0 : Configure.Instance.CommentViewTabPosition)
                    {
                        case 0:
                            netItems.AddRange(GetNetItemsImpl(DB.Instance.GetCommentItems(NoteFiles[0], eventNoteID, NoteFiles.Length)));
                            _totalComments = netItems.Count;
                            break;
                        case 1:
                        case 2:
                            var twilightWwwComment = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwComment?>($"{QwilightComponent.QwilightAPI}/comment?noteID={NoteFile.GetNoteID512()}&avatarID={Configure.Instance.AvatarID}&isUbuntu={Configure.Instance.UbuntuNetItemTarget || !string.IsNullOrEmpty(UbuntuID)}&ubuntuID={UbuntuID}&viewUnit=50").ConfigureAwait(false);
                            if (twilightWwwComment.HasValue)
                            {
                                _totalComments = twilightWwwComment.Value.totalComments;
                                netItems.AddRange(GetNetItemsImpl(HandleTwilightNetItems(Utility.GetCommentItems(twilightWwwComment.Value.comments, NoteFile))));
                            }
                            break;
                    }
                    netItems.Add(netItem);
                    _netItems.AddRange(netItems);
                    lock (IsTwilightNetItemsCSX)
                    {
                        if (!IsTwilightNetItems)
                        {
                            SetNetItems(netItems);
                        }
                    }
                    _wasGetNetItems = true;

                    IEnumerable<NetItem> GetNetItemsImpl(ICollection<CommentItem> commentItems) => commentItems.Select(commentItem => new NetItem(commentItem.AvatarWwwValue.AvatarID, commentItem.AvatarName, commentItem.Date, commentItem.Stand, commentItem.Band, commentItem.Point, commentItem.Stand / (1000000.0 * NoteFiles.Length))
                    {
                        CommentItem = commentItem,
                        AvatarNetStatus = Event.Types.AvatarNetStatus.Clear,
                        QuitValue = Utility.GetQuitStatusValue(commentItem.Point, commentItem.Stand, 1.0, NoteFiles.Length),
                        HitPointsModeValue = commentItem.ModeComponentValue.HandlingHitPointsModeValue
                    });
                }
            }
        }

        public virtual ICollection<CommentItem> HandleTwilightNetItems(CommentItem[] commentItems) => commentItems;

        public virtual void OnFailed() => RGBSystem.Instance.IsFailed = true;

        public virtual void GetNetComments()
        {
            if (Configure.Instance.NetCommentFollow)
            {
                if (!_wasGetNetComments && _wasGetNetItems)
                {
                    _wasGetNetComments = true;
                    Utility.HandleLowlyParallelly(new ConcurrentBag<NetItem>(NetItems), QwilightComponent.CPUCount, async netItem =>
                    {
                        if (netItem.Comment == null)
                        {
                            try
                            {
                                var commentItem = netItem.CommentItem;
                                if (commentItem != null)
                                {
                                    var commentID = commentItem.CommentID;
                                    if (commentItem.IsTwilightComment)
                                    {
                                        netItem.Comment = Comment.Parser.ParseFrom(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/comment?noteID={NoteFile.GetNoteID512()}&commentID={commentID}").ConfigureAwait(false));
                                    }
                                    else
                                    {
                                        if (EventNoteEntryItem != null)
                                        {
                                            var commentFilePath = Path.Combine(QwilightComponent.CommentEntryPath, Path.ChangeExtension(commentID, ".zip"));
                                            if (File.Exists(commentFilePath))
                                            {
                                                using (var zipFile = ZipFile.Read(commentFilePath))
                                                {
                                                    var zipEntry = zipFile[LevyingComputingPosition.ToString()];
                                                    using var rms = PoolSystem.Instance.GetDataFlow((int)zipEntry.UncompressedSize);
                                                    zipFile[LevyingComputingPosition.ToString()].Extract(rms);
                                                    rms.Position = 0;
                                                    netItem.Comment = Comment.Parser.ParseFrom(rms);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var commentFilePath = Path.Combine(QwilightComponent.CommentEntryPath, commentID);
                                            if (File.Exists(commentFilePath))
                                            {
                                                using var fs = File.OpenRead(commentFilePath);
                                                netItem.Comment = Comment.Parser.ParseFrom(fs);
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    });
                }
            }
            else
            {
                if (_wasGetNetComments)
                {
                    _wasGetNetComments = false;
                    foreach (var netItem in NetItems)
                    {
                        netItem.Comment = null;
                    }
                }
            }
        }

        void HandleComment(ref int commentInputID, ref int commentMultiplierID, ref int commentAudioMultiplierID)
        {
            if (EventComment != null)
            {
                while (EventComment.Inputs.Count > commentInputID)
                {
                    var inputEvent = EventComment.Inputs[commentInputID];
                    if (Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, inputEvent.Wait) <= LoopingCounter)
                    {
                        HandleInput(inputEvent.Input);
                        commentInputID++;
                        continue;
                    }
                    break;
                }
                while (EventComment.Multipliers.Count > commentMultiplierID)
                {
                    var multiplierEvent = EventComment.Multipliers[commentMultiplierID];
                    if (Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, multiplierEvent.Wait) <= LoopingCounter)
                    {
                        MultiplierQueue.Enqueue(multiplierEvent.Multiplier);
                        ++commentMultiplierID;
                        if (!IsInEvents && !multiplierEvent.IsAutoEvent)
                        {
                            HandleUIAudio("Multiplier");
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                while (EventComment.AudioMultipliers.Count > commentAudioMultiplierID)
                {
                    var audioMultiplierEvent = EventComment.AudioMultipliers[commentAudioMultiplierID];
                    if (Utility.SetCommentWait(CommentWaitDate, AudioMultiplier, audioMultiplierEvent.Wait) <= LoopingCounter)
                    {
                        AudioMultiplierQueue.Enqueue(audioMultiplierEvent.AudioMultiplier);
                        ++commentAudioMultiplierID;
                        HandleUIAudio("Audio Multiplier");
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        void HandleUIAudio(string audioFileName, string defaultFileName = null)
        {
            if (!IsInEvents)
            {
                if (!UI.Instance.HandleAudio(audioFileName, defaultFileName))
                {
                    BaseUI.Instance.HandleAudio(audioFileName, defaultFileName);
                }
            }
        }

        public virtual void OnGetF()
        {
            IsF.SetValue(true);
            if (IsFailMode)
            {
                OnHandled();
            }
        }

        public void SetJudgmentMillis()
        {
            LowestJudgmentMillis = Component.GetJudgmentMillis(Component.Judged.Lowest, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, 0);
            HighestJudgmentMillis = Component.GetJudgmentMillis(Component.Judged.Lowest, ModeComponentValue, JudgmentStage, _judgmentModeDate, _judgmentMapDate, _longNoteAssistDate, 1);
        }

        public void SetStandMultiplier()
        {
            if (IsPostableItemMode)
            {
                _standMultiplier = 1.0;
            }
            else
            {
                _standMultiplier = Math.Min(AudioMultiplier, 1.0);
                switch (ModeComponentValue.JudgmentModeValue)
                {
                    case ModeComponent.JudgmentMode.Lower:
                        _standMultiplier *= 0.5;
                        break;
                    case ModeComponent.JudgmentMode.Lowest:
                        _standMultiplier *= 0.25;
                        break;
                }
                switch (ModeComponentValue.HandlingHitPointsModeValue)
                {
                    case ModeComponent.HitPointsMode.Lower:
                        _standMultiplier *= 0.5;
                        break;
                    case ModeComponent.HitPointsMode.Lowest:
                        _standMultiplier *= 0.25;
                        break;
                }
            }
        }

        public void SetCommentFile() => _comments.Add(Comment.Clone());
    }
}