﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Qwilight.Compute;
using Qwilight.Note;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.Win32;
using Windows.Win32.System.WinRT.Direct3D11;
using WinRT;
using Brush = System.Windows.Media.Brush;
using GradientStop = System.Windows.Media.GradientStop;
using ImageSource = System.Windows.Media.ImageSource;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using Pen = System.Windows.Media.Pen;
using PixelFormats = System.Windows.Media.PixelFormats;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace Qwilight
{
    public partial class DrawingSystem : Model, IDrawingContainer
    {
        public enum MediaInputAreaStatus
        {
            Not, Position, Area
        }

        enum ReflexMarker
        {
            eSimulationStart,
            eSimulationEnd,
            eRenderSubmitStart,
            eRenderSubmitEnd,
            ePresentStart,
            ePresentEnd
        }

        [LibraryImport("NVIDIA")]
        private static partial void InitNVLL(nint d3dDevice);

        [LibraryImport("NVIDIA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsNVLLAvailable();

        [LibraryImport("NVIDIA")]
        private static partial void GetNVLLFrame();

        [LibraryImport("NVIDIA")]
        private static partial void SetNVLLFlag(ReflexMarker marker);

        [LibraryImport("NVIDIA")]
        private static partial void WaitNVLL();

        public static readonly DrawingSystem Instance = QwilightComponent.GetBuiltInData<DrawingSystem>(nameof(DrawingSystem));

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(DrawingSystem));

        readonly ConcurrentDictionary<IDrawingContainer, ConcurrentDictionary<string, DrawingItem>> _drawingMap = new();
        readonly ConcurrentDictionary<IDrawingContainer, ConcurrentDictionary<string, ImageSource>> _defaultDrawingMap = new();
        readonly ConcurrentDictionary<IDrawingContainer, ConcurrentBag<IDisposable>> _toCloseValues = new();
        readonly ICanvasBrush[] _lowHitPointsPaints = new ICanvasBrush[101];
        readonly ICanvasBrush[][] _avatarNetStatusPaints = new ICanvasBrush[][]
        {
            new ICanvasBrush[101],
            new ICanvasBrush[101],
            new ICanvasBrush[101],
        };
        Vector2 _drawingQuality;
        CanvasSwapChain _rawTargetSystem;
        CanvasRenderTarget _targetSystem;
        byte[] _rawTargetSystemData;
        IBuffer _targetSystemData;
        DrawingItem _faintNoteDrawing;
        Action _eventHandler;
        NetItem _netItemParam;
        Action<NetItem> _netItemHandler;
        NotifyXamlItem _toNotifyXamlItemParam;
        Action<NotifyXamlItem> _toNotifyXamlItemHandler;
        MediaInputAreaStatus _mediaInputAreaStatus;
        double _mediaInputPosition0;
        double _mediaInputPosition1;

        public bool CanNVLL { get; set; }

        public WriteableBitmap D3D9Drawing { get; set; }

        public HandledDrawingItem ClearedDrawing { get; set; }

        /// <summary>
        /// 다이렉트 2D 싱글 스레드 제어용 락
        /// </summary>
        public object D2D1CSX { get; } = new();

        public ICanvasBrush[] FaintFilledPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] FaintClearedPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] FaintPositiveItemPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] FaintNegativeItemPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] FaintNeutralItemPaints { get; } = new ICanvasBrush[101];

        public Dictionary<int, ICanvasBrush[]> FaintItemPaints { get; set; } = new();

        public DrawingSystem()
        {
            MeterFont = GetFont();
            UtilityFont = GetFont();
            NotifyXamlFont = GetFont();
            NoteItemFont = GetFont();
            InputAssistFont = GetFont();
            PauseNotifyFont = GetFont();
            NetFont = GetFont();
            JudgmentMeterViewFont = GetFont();
            StatusViewFont = GetFont();

            SetFaintPaints(null, FaintFilledPaints, Colors.Black);
            SetFaintPaints(null, FaintClearedPaints, Colors.White);
            SetFaintPaints(null, FaintPositiveItemPaints, Colors.DeepSkyBlue);
            SetFaintPaints(null, FaintNegativeItemPaints, Colors.DeepPink);
            SetFaintPaints(null, FaintNeutralItemPaints, Colors.Gray);
            FaintItemPaints[1] = FaintPositiveItemPaints;
            FaintItemPaints[0] = FaintNegativeItemPaints;
            FaintItemPaints[-1] = FaintNeutralItemPaints;
            SetFaintPaints(null, _lowHitPointsPaints, Colors.Red);
            SetFaintPaints(null, _avatarNetStatusPaints[(int)Event.Types.AvatarNetStatus.Default], Colors.White);
            SetFaintPaints(null, _avatarNetStatusPaints[(int)Event.Types.AvatarNetStatus.Clear], Colors.Green);
            SetFaintPaints(null, _avatarNetStatusPaints[(int)Event.Types.AvatarNetStatus.Failed], Colors.Red);

            Init();

            DefaultDrawing = ClearedDrawing;
            VeilDrawing = ClearedDrawing;
        }

        public virtual void Init()
        {
            CanvasDevice.GetSharedDevice().As<IDirect3DDxgiInterfaceAccess>().GetInterface(new Guid("6007896C-3244-4AFD-BF18-A6D3BEDA5023"), out var rawSystem);
            InitNVLL(Marshal.GetIUnknownForObject(rawSystem));
            if (!(CanNVLL = IsNVLLAvailable()))
            {
                Configure.Instance.NVLLModeValue = Configure.NVLLMode.Not;
                Configure.Instance.NVLLFramerate = 0;
                Configure.Instance.AutoNVLLFramerate = false;
            }
            Configure.Instance.SetNVLLConfigureImpl();

            var faintNoteDrawing = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), 1F, 200F, 96F, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Premultiplied);
            using (var targetSession = faintNoteDrawing.CreateDrawingSession())
            {
                targetSession.Clear(Colors.Transparent);
                for (var i = (int)faintNoteDrawing.SizeInPixels.Height - 1; i >= 0; --i)
                {
                    targetSession.FillRectangle(0F, i, 1F, 1F, FaintFilledPaints[Math.Min(i, 100)]);
                }
            }
            _faintNoteDrawing = new(false)
            {
                Drawing = faintNoteDrawing,
                DrawingBound = faintNoteDrawing.Bounds
            };

            var d2DClearedDrawing = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), 1F, 1F, 96F, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Premultiplied);
            using (var targetSession = d2DClearedDrawing.CreateDrawingSession())
            {
                targetSession.Clear(Colors.Transparent);
            }
            var defaultClearedDrawing = BitmapSource.Create(1, 1, 96.0, 96.0, PixelFormats.Pbgra32, null, new byte[] { 0, 0, 0, 0 }, 4);
            defaultClearedDrawing.Freeze();
            ClearedDrawing = new()
            {
                Drawing = new(false)
                {
                    Drawing = d2DClearedDrawing,
                    DrawingBound = d2DClearedDrawing.Bounds
                },
                DefaultDrawing = defaultClearedDrawing
            };
        }

        public ConcurrentQueue<(Point, bool)> LastPointedQueue { get; } = new();

        public ConcurrentQueue<Point> LastMovedQueue { get; } = new();

        public ConcurrentQueue<Point> LastNotPointedQueue { get; } = new();

        public ICanvasBrush[] MeterNotePaints { get; } = new ICanvasBrush[101];

        public uint MeterNoteAverageColor { get; set; }

        public ICanvasBrush[] AudioVisualizerMainPaints { get; } = new ICanvasBrush[101];

        public ICanvasBrush[] AudioVisualizerInputPaints { get; } = new ICanvasBrush[101];

        public HandledDrawingItem DefaultDrawing { get; set; }

        public HandledDrawingItem VeilDrawing { get; set; }

        public CanvasTextFormat MeterFont { get; }

        public CanvasTextFormat UtilityFont { get; }

        public CanvasTextFormat NotifyXamlFont { get; }

        public CanvasTextFormat NoteItemFont { get; }

        public CanvasTextFormat InputAssistFont { get; }

        public CanvasTextFormat PauseNotifyFont { get; }

        public CanvasTextFormat NetFont { get; }

        public CanvasTextFormat JudgmentMeterViewFont { get; }

        public CanvasTextFormat StatusViewFont { get; }

        public int DrawingItemCount
        {
            get
            {
                var drawingItemCount = 0;
                foreach (var drawingItems in _drawingMap.Values)
                {
                    drawingItemCount += drawingItems.Count;
                }
                foreach (var defaultDrawing in _defaultDrawingMap.Values)
                {
                    drawingItemCount += defaultDrawing.Count;
                }
                return drawingItemCount;
            }
        }

        public void InitMediaInputArea()
        {
            _mediaInputPosition0 = 0.0;
            _mediaInputPosition1 = 0.0;
            _mediaInputAreaStatus = MediaInputAreaStatus.Position;
        }

        public void LoadDefaultDrawing()
        {
            DefaultDrawing.Dispose();
            try
            {
                var filePath = Configure.Instance.DefaultDrawingFilePath;
                if (File.Exists(filePath))
                {
                    DefaultDrawing = new()
                    {
                        Drawing = Load(filePath, null),
                        DefaultDrawing = LoadDefault(filePath, null)
                    };
                }
                else
                {
                    DefaultDrawing = BaseUI.Instance.DefaultDrawing ?? ClearedDrawing;
                }
            }
            catch
            {
                DefaultDrawing = ClearedDrawing;
            }
            OnPropertyChanged(nameof(DefaultDrawing));
        }

        public void LoadVeilDrawing()
        {
            VeilDrawing.Dispose();
            try
            {
                var filePath = Configure.Instance.VeilDrawingFilePath;
                if (File.Exists(filePath))
                {
                    VeilDrawing = new()
                    {
                        Drawing = Load(filePath, null),
                        DefaultDrawing = LoadDefault(filePath, null)
                    };
                }
                else
                {
                    VeilDrawing = UI.Instance.VeilDrawing ?? ClearedDrawing;
                }
            }
            catch
            {
                VeilDrawing = ClearedDrawing;
            }
            OnPropertyChanged(nameof(VeilDrawing));
        }

        public void HandleSystem()
        {
            Span<int> judgments = stackalloc int[6];
            var mainViewModel = ViewModels.Instance.MainValue;
            var r = new Bound();
            var s = new Bound();
            var v0 = new Vector2();
            var v1 = new Vector2();
            var pauseNotify0Position0 = 0F;
            var pauseNotify0Position1 = 0F;
            var pauseNotify1Position0 = 0F;
            var pauseNotify1Position1 = 0F;
            DefaultCompute defaultComputer = null;
            CanvasDrawingSession targetSession = null;

            var handleNotifyXamlItemImpl = new Action<NotifyXamlItem>(toNotifyXamlItem =>
            {
                toNotifyXamlItem.OnHandle?.Invoke();
                ViewModels.Instance.NotifyXamlValue.WipeNotify(toNotifyXamlItem);
            });

            var handleCommentNetItemImpl = new Action<NetItem>(mainViewModel.HandleViewComment);

            var handleIONetItemImpl = new Action<NetItem>(netItem => TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallIo, new
            {
                avatarID = netItem.AvatarID,
                ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }));

            var setDefaultSpinningModeUnpause = new Action(() =>
            {
                Configure.Instance.DefaultSpinningModeValue = Configure.DefaultSpinningMode.Unpause;
                defaultComputer.Unpause();
            });
            var setDefaultSpinningModeConfigure = new Action(() =>
            {
                Configure.Instance.DefaultSpinningModeValue = Configure.DefaultSpinningMode.Configure;
                ViewModels.Instance.ConfigureValue.Open();
            });
            var setDefaultSpinningModeUndo = new Action(() =>
            {
                Configure.Instance.DefaultSpinningModeValue = Configure.DefaultSpinningMode.Undo;
                if (defaultComputer.CanUndo)
                {
                    defaultComputer.SetUndo = true;
                }
            });
            var setDefaultSpinningModeStop = new Action(() =>
            {
                Configure.Instance.DefaultSpinningModeValue = Configure.DefaultSpinningMode.Stop;
                defaultComputer.Unpause();
                defaultComputer.SetNoteFileMode();
            });
            var handleViewCommentImpl = new Action(mainViewModel.HandleViewComment);
            var handleUndoImpl = new Action(mainViewModel.HandleInitComment);
            var handleQuitMove0Impl = new Action(() => defaultComputer.NotifyCompute(-1));
            var handleQuitMove1Impl = new Action(() => defaultComputer.NotifyCompute(1));
            var getAvatarDrawing = new Action<object>(async avatarID => await AvatarDrawingSystem.Instance.GetAvatarDrawing(avatarID as string));
            var getAvatarTitle = new Action<object>(async avatarID => await AvatarTitleSystem.Instance.GetAvatarTitle(avatarID as string));
            var getAvatarEdge = new Action<object>(async avatarID => await AvatarEdgeSystem.Instance.GetAvatarEdge(avatarID as string));

            var distanceMillisMax = double.MinValue;
            var frametime = 0.0;
            var frameCount = 0;
            var framerate = string.Empty;
            var framerateLowest = string.Empty;
            var textGCs = new string[QwilightComponent.HeapCount];
            Array.Fill(textGCs, string.Empty);
            var textHeap = string.Empty;
            var lastMillis = 0.0;
            var lastHeap = 0L;
            var distanceMillis = 0.0;
            var loopingHandler = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    var wasLastPointed = LastPointedQueue.TryDequeue(out var lastPointed);
                    var wasLastMoved = LastMovedQueue.TryDequeue(out var lastMoved);
                    var wasLastNotPointed = LastNotPointedQueue.TryDequeue(out var lastNotPointed);

                    var isWPFViewVisible = mainViewModel.IsWPFViewVisible;
                    var isNVLL = Configure.Instance.IsNVLL;
                    var allowFramerate = TelnetSystem.Instance.IsAvailable;
                    var mode = mainViewModel.ModeValue;
                    var defaultLength = (float)_rawTargetSystem.Size.Width;
                    var defaultHeight = (float)_rawTargetSystem.Size.Height;
                    var fadingValue = mainViewModel.FadingValue;
                    var fadingStatus = fadingValue.Status;
                    pauseNotify0Position0 = Levels.StandardMarginFloat32;
                    pauseNotify0Position1 = Levels.StandardMarginFloat32;
                    pauseNotify1Position0 = defaultLength - Levels.StandardMarginFloat32;
                    pauseNotify1Position1 = Levels.StandardMarginFloat32;
                    if (fadingStatus < 1.0)
                    {
                        ModeComponent modeComponentValue;
                        switch (mode)
                        {
                            case MainViewModel.Mode.NoteFile:
                                lock (D2D1CSX)
                                {
                                    using (targetSession = _rawTargetSystem.CreateDrawingSession(Colors.Black))
                                    {
                                        lock (UI.Instance.ContentsCSX)
                                        {
                                            PaintFading();
                                            PaintNotifyXamlItems();
                                        }
                                        PaintFramerate();
                                    }
                                }

                                _rawTargetSystem.Present();
                                break;
                            case MainViewModel.Mode.Computing:
                                defaultComputer = mainViewModel.Computer;
                                modeComponentValue = defaultComputer.ModeComponentValue;
                                lock (D2D1CSX)
                                {
                                    using (targetSession = _targetSystem.CreateDrawingSession())
                                    {
                                        targetSession.Clear(Colors.Black);

                                        lock (UI.Instance.ContentsCSX)
                                        {
                                            var faultText = UI.Instance.FaultText;
                                            if (string.IsNullOrEmpty(faultText))
                                            {
                                                var isItemMode = defaultComputer.IsPostableItemMode;
                                                var drawingComponentValue = defaultComputer.DrawingComponentValue;
                                                var loopingCounter = defaultComputer.LoopingCounter;
                                                var faintNoteModeValue = modeComponentValue.FaintNoteModeValue;
                                                var inputMode = defaultComputer.InputMode;
                                                var isIn2P = defaultComputer.IsIn2P;
                                                var has2P = defaultComputer.Has2P;
                                                var inputCount = Component.InputCounts[(int)inputMode];
                                                var input1PCount = defaultComputer.Input1PCount;
                                                var mainPosition = drawingComponentValue.mainPosition;
                                                var p2Position = drawingComponentValue.p2Position;
                                                var p1Length = drawingComponentValue.p1BuiltLength;
                                                var distance2P = p1Length + p2Position;
                                                var isHandling = defaultComputer.IsHandling;
                                                var hitPoints = defaultComputer.HitPoints.Value;
                                                var status = defaultComputer.Status;
                                                var comment = defaultComputer.Comment;
                                                var setStop = defaultComputer.SetStop;
                                                var isAutoMode = defaultComputer.IsAutoMode;
                                                var d2dJudgmentPaints = BaseUI.Instance.D2DJudgmentPaints;
                                                var isValidNetDrawings = defaultComputer.IsValidNetDrawings;
                                                judgments[(int)Component.Judged.Highest] = defaultComputer.InheritedHighestJudgment + (setStop ? 0 : comment.HighestJudgment);
                                                judgments[(int)Component.Judged.Higher] = defaultComputer.InheritedHigherJudgment + (setStop ? 0 : comment.HigherJudgment);
                                                judgments[(int)Component.Judged.High] = defaultComputer.InheritedHighJudgment + (setStop ? 0 : comment.HighJudgment);
                                                judgments[(int)Component.Judged.Low] = defaultComputer.InheritedLowJudgment + (setStop ? 0 : comment.LowJudgment);
                                                judgments[(int)Component.Judged.Lower] = defaultComputer.InheritedLowerJudgment + (setStop ? 0 : comment.LowerJudgment);
                                                judgments[(int)Component.Judged.Lowest] = defaultComputer.InheritedLowestJudgment + (setStop ? 0 : comment.LowestJudgment);
                                                var drawingNoteLengthMap = drawingComponentValue.DrawingNoteLengthMap;
                                                var mainPosition1 = drawingComponentValue.mainPosition1;
                                                var mainHeight = drawingComponentValue.mainHeight;
                                                var binJudgmentValueMap = UI.Instance.BinJudgmentValueMap;
                                                var drawingInputModeMap = UI.Instance.DrawingInputModeMap[(int)inputMode];
                                                var drawingPipelines = UI.Instance.DrawingPipeline;

                                                var paintPipelines = UI.Instance.PaintPipelineValues;
                                                foreach (var paintPipeline in paintPipelines)
                                                {
                                                    if (CanPaint(paintPipeline))
                                                    {
                                                        if (paintPipeline >= PaintPipelineID.PaintProperty0 && paintPipeline <= PaintPipelineID.PaintProperty255)
                                                        {
                                                            var paintPropertyID = (int)paintPipeline - (int)PaintPipelineID.PaintProperty0;
                                                            lock (drawingComponentValue.PaintPropertyCSX)
                                                            {
                                                                if (drawingComponentValue.PaintPropertyIDs.Contains(paintPropertyID))
                                                                {
                                                                    var paintPropertyIntMap = drawingComponentValue.PaintPropertyIntMap[paintPropertyID];
                                                                    var paintPropertyPipeline = (PaintPipelineID)paintPropertyIntMap[PaintProperty.ID.Pipeline];
                                                                    if ((int)paintPropertyPipeline == -1 || paintPipelines.Contains(paintPropertyPipeline) && CanPaint(paintPropertyPipeline))
                                                                    {
                                                                        if (paintPropertyIntMap[PaintProperty.ID.Frame] > 0)
                                                                        {
                                                                            var paintPropertyAlt = paintPropertyIntMap[PaintProperty.ID.Alt];
                                                                            var paintPropertyValueMap = drawingComponentValue.PaintPropertyValueMap[paintPropertyID];
                                                                            var paintPropertyPosition0 = paintPropertyValueMap[PaintProperty.ID.Position0];
                                                                            var paintPropertyPosition1 = paintPropertyValueMap[PaintProperty.ID.Position1];
                                                                            var paintPropertyLength = paintPropertyValueMap[PaintProperty.ID.Length];
                                                                            var paintPropertyHeight = paintPropertyValueMap[PaintProperty.ID.Height];
                                                                            var paintPropertyDrawing = UI.Instance.PaintPropertyValues[paintPropertyID].Drawings[defaultComputer.PaintPropertyFrames[paintPropertyID]];
                                                                            for (var i = paintPropertyAlt >> 1; i >= paintPropertyAlt % 2; --i)
                                                                            {
                                                                                var distancePaint = i == 1 && has2P ? distance2P : 0F;
                                                                                r.Set(paintPropertyPosition0 + distancePaint, paintPropertyPosition1, paintPropertyLength, paintPropertyHeight);
                                                                                targetSession.PaintDrawing(ref r, paintPropertyDrawing, 1F, (CanvasComposite)paintPropertyIntMap[PaintProperty.ID.Composition]);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            switch (paintPipeline)
                                                            {
                                                                case PaintPipelineID.Media:
                                                                    var mediaFaint = (float)Configure.Instance.UIConfigureValue.MediaFaintV2;
                                                                    var mediaPosition0 = drawingComponentValue.mediaPosition0;
                                                                    var mediaPosition1 = drawingComponentValue.mediaPosition1;
                                                                    var mediaLength = drawingComponentValue.mediaLength;
                                                                    var mediaHeight = drawingComponentValue.mediaHeight;
                                                                    var altMedia = drawingComponentValue.altMedia;
                                                                    for (var i = altMedia >> 1; i >= altMedia % 2; --i)
                                                                    {
                                                                        var distanceMedia = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(mediaPosition0 + distanceMedia, mediaPosition1, mediaLength, mediaHeight);
                                                                        defaultComputer.PaintMedia(targetSession, ref r, mediaFaint);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.AudioVisualizer:
                                                                    Utility.PaintAudioVisualizer(targetSession, ref r, (int)(100 * Configure.Instance.UIConfigureValue.AudioVisualizerFaintV2), drawingComponentValue.audioVisualizerPosition0, drawingComponentValue.audioVisualizerPosition1, drawingComponentValue.audioVisualizerLength, drawingComponentValue.audioVisualizerHeight);
                                                                    break;
                                                                case PaintPipelineID.MainWall:
                                                                    var mainWalls = UI.Instance.MainWalls;
                                                                    var mainWall0Position1 = drawingComponentValue.mainWall0Position1;
                                                                    var mainWall0Length = drawingComponentValue.mainWall0Length;
                                                                    var mainWall0Height = drawingComponentValue.mainWall0Height;
                                                                    var altWall0 = drawingComponentValue.altWall0;
                                                                    var mainWallDrawing0 = mainWalls[0];
                                                                    for (var i = altWall0 >> 1; i >= altWall0 % 2; --i)
                                                                    {
                                                                        var distanceMainWall0 = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(mainPosition + distanceMainWall0 - mainWall0Length, mainWall0Position1, mainWall0Length, mainWall0Height);
                                                                        targetSession.PaintDrawing(ref r, mainWallDrawing0);
                                                                    }
                                                                    var mainWall1Position1 = drawingComponentValue.mainWall1Position1;
                                                                    var mainWall1Length = drawingComponentValue.mainWall1Length;
                                                                    var mainWall1Height = drawingComponentValue.mainWall1Height;
                                                                    var altWall1 = drawingComponentValue.altWall1;
                                                                    var mainWallDrawing1 = mainWalls[1];
                                                                    for (var i = altWall1 >> 1; i >= altWall1 % 2; --i)
                                                                    {
                                                                        var distanceMainWall1 = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(mainPosition + p1Length + distanceMainWall1, mainWall1Position1, mainWall1Length, mainWall1Height);
                                                                        targetSession.PaintDrawing(ref r, mainWallDrawing1);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Main:
                                                                    var mainDrawings = UI.Instance.MainDrawings[(int)inputMode];
                                                                    var mainFaint = (float)Configure.Instance.UIConfigureValue.MainFaintV2;
                                                                    foreach (var drawingPipeline in drawingPipelines)
                                                                    {
                                                                        for (var i = inputCount; i > 0; --i)
                                                                        {
                                                                            if (drawingPipeline == drawingInputModeMap[i])
                                                                            {
                                                                                var mainDrawing = mainDrawings?[i]?[Configure.Instance.UIPipelineMainDrawingPaint ? defaultComputer.MainFrames[i] : 0];
                                                                                if (mainDrawing.HasValue)
                                                                                {
                                                                                    r.Set(defaultComputer.GetPosition(i), mainPosition1, drawingNoteLengthMap[i], mainHeight);
                                                                                    targetSession.PaintDrawing(ref r, mainDrawing, mainFaint);
                                                                                    defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Main, mainDrawing.Value.AverageColor, r.Position0 - drawingComponentValue.mainPosition, r.Position1, r.Length, r.Height);
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (isItemMode)
                                                                    {
                                                                        for (var i = 1; i >= -1; --i)
                                                                        {
                                                                            var postedItemFaint = defaultComputer.PostedItemFaints[i] / 5;
                                                                            var faintItemPaint = FaintItemPaints[i][(int)(100 * postedItemFaint)];
                                                                            r.Set(mainPosition, 0.0, p1Length, mainHeight);
                                                                            targetSession.FillRectangle(r, faintItemPaint);
                                                                            if (has2P)
                                                                            {
                                                                                r.Position0 += distance2P;
                                                                                targetSession.FillRectangle(r, faintItemPaint);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.AutoInput:
                                                                    if (!isAutoMode || UI.Instance.MaintainAutoInput)
                                                                    {
                                                                        var autoInputDrawings = UI.Instance.AutoInputDrawings[(int)inputMode];
                                                                        var autoInputPosition1s = drawingComponentValue.autoInputPosition1s;
                                                                        var autoInputHeights = drawingComponentValue.autoInputHeights;
                                                                        foreach (var drawingPipeline in drawingPipelines)
                                                                        {
                                                                            for (var i = inputCount; i > 0; --i)
                                                                            {
                                                                                if (drawingPipeline == drawingInputModeMap[i])
                                                                                {
                                                                                    if (defaultComputer.IsSuitableAsAutoJudge(i))
                                                                                    {
                                                                                        r.Set(defaultComputer.GetPosition(i), autoInputPosition1s[i], drawingNoteLengthMap[i], autoInputHeights[i]);
                                                                                        targetSession.PaintDrawing(ref r, autoInputDrawings[i]);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.HitPoints:
                                                                    var hitPointsPosition0 = drawingComponentValue.hitPointsPosition0;
                                                                    var hitPointsPosition1 = drawingComponentValue.hitPointsPosition1;
                                                                    var hitPointsLength = drawingComponentValue.hitPointsLength;
                                                                    var hitPointsHeight = drawingComponentValue.hitPointsHeight;
                                                                    var hitPointsDrawing = UI.Instance.HitPointsDrawings[(int)modeComponentValue.HandlingHitPointsModeValue];
                                                                    var altHitPoints = drawingComponentValue.altHitPoints;
                                                                    var hitPointsSystem = drawingComponentValue.hitPointsSystem;
                                                                    for (var i = altHitPoints >> 1; i >= altHitPoints % 2; --i)
                                                                    {
                                                                        var distanceHitPoints = i == 1 && has2P ? distance2P : 0F;
                                                                        if (hitPointsDrawing.HasValue)
                                                                        {
                                                                            var hitPointsDrawingValue = hitPointsDrawing.Value;
                                                                            var hitPointsDrawingBound = hitPointsDrawingValue.DrawingBound;
                                                                            var hitPointsDrawingLength = hitPointsDrawingBound.Length;
                                                                            var hitPointsDrawingHeight = hitPointsDrawingBound.Height;
                                                                            switch (hitPointsSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(hitPointsPosition0 + distanceHitPoints, hitPointsPosition1 + hitPointsHeight * (1 - hitPoints), hitPointsLength, hitPointsHeight * hitPoints);
                                                                                    s.Set(0.0, (1 - hitPoints) * hitPointsDrawingHeight, hitPointsDrawingLength, hitPointsDrawingHeight * hitPoints);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(hitPointsPosition0 + distanceHitPoints, hitPointsPosition1, hitPointsLength, hitPointsHeight * hitPoints);
                                                                                    s.SetArea(hitPointsDrawingLength, hitPointsDrawingHeight * hitPoints);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(hitPointsPosition0 + distanceHitPoints + hitPointsLength * (1 - hitPoints), hitPointsPosition1, hitPointsLength * hitPoints, hitPointsHeight);
                                                                                    s.Set((1 - hitPoints) * hitPointsDrawingLength, 0.0, hitPointsDrawingLength * hitPoints, hitPointsDrawingHeight);
                                                                                    break;
                                                                                case 3:
                                                                                    r.Set(hitPointsPosition0 + distanceHitPoints, hitPointsPosition1, hitPointsLength * hitPoints, hitPointsHeight);
                                                                                    s.SetArea(hitPointsDrawingLength * hitPoints, hitPointsDrawingHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, ref s, hitPointsDrawingValue);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Note:
                                                                    for (var i = BaseNote.LowestLayer; i <= BaseNote.HighestLayer; ++i)
                                                                    {
                                                                        foreach (var drawingPipeline in drawingPipelines)
                                                                        {
                                                                            var paintedNotes = defaultComputer.PaintedNotes;
                                                                            lock (paintedNotes)
                                                                            {
                                                                                foreach (var paintedNote in paintedNotes)
                                                                                {
                                                                                    if (paintedNote.Layer == i)
                                                                                    {
                                                                                        if (drawingPipeline == drawingInputModeMap[paintedNote.TargetInput])
                                                                                        {
                                                                                            paintedNote.Paint(targetSession, isValidNetDrawings, defaultComputer, ref r);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    if (faintNoteModeValue == ModeComponent.FaintNoteMode.Faint)
                                                                    {
                                                                        r.Set(mainPosition, 0.0, p1Length, mainHeight);
                                                                        targetSession.PaintDrawing(ref r, _faintNoteDrawing);
                                                                        if (has2P)
                                                                        {
                                                                            r.Position0 += distance2P;
                                                                            targetSession.PaintDrawing(ref r, _faintNoteDrawing);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Input:
                                                                    var inputDrawings = UI.Instance.InputDrawings[(int)inputMode];
                                                                    var inputPosition0s = drawingComponentValue.inputPosition0s;
                                                                    var inputPosition1s = drawingComponentValue.inputPosition1s;
                                                                    var inputLengths = drawingComponentValue.inputLengths;
                                                                    var inputHeights = drawingComponentValue.inputHeights;
                                                                    var inputFrames = defaultComputer.InputFrames;
                                                                    foreach (var drawingPipeline in drawingPipelines)
                                                                    {
                                                                        for (var i = inputCount; i > 0; --i)
                                                                        {
                                                                            if (drawingPipeline == drawingInputModeMap[i])
                                                                            {
                                                                                var inputLength = inputLengths[i];
                                                                                r.Set(defaultComputer.GetPosition(i) + inputPosition0s[i] - inputLength / 2, inputPosition1s[i], drawingNoteLengthMap[i] + inputLength, inputHeights[i]);
                                                                                targetSession.PaintDrawing(ref r, inputDrawings?[i]?[defaultComputer.InputFrames[i]]);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Band:
                                                                    var binBandMap = UI.Instance.BinBandMap;
                                                                    var band = defaultComputer.Band.Value;
                                                                    if (band > 0)
                                                                    {
                                                                        var bandPosition0 = drawingComponentValue.bandPosition0;
                                                                        var bandPosition1 = drawingComponentValue.bandPosition1;
                                                                        var binBandLength = drawingComponentValue.binBandLength;
                                                                        var binBandHeight = drawingComponentValue.binBandHeight;
                                                                        var bandSystem = drawingComponentValue.bandSystem;
                                                                        var altBand = drawingComponentValue.altBand;
                                                                        var bandDrawingMap = defaultComputer.BandEnlargedMap;
                                                                        var bandDrawingFrames = defaultComputer.BandDrawingFrames;
                                                                        var bandDigit = QwilightComponent.GetDigit(band);
                                                                        for (var i = altBand >> 1; i >= altBand % 2; --i)
                                                                        {
                                                                            var distanceBand = i == 1 && has2P ? distance2P : 0F;
                                                                            for (var j = bandDigit - 1; j >= 0; --j)
                                                                            {
                                                                                var m = (int)(band / Math.Pow(10, j) % 10);
                                                                                var enlarge = bandDrawingMap.TryGetValue(j, out var unlarge) ? 2 * unlarge : 0.0;
                                                                                var binBandDrawing = binBandMap[m, bandDrawingFrames.TryGetValue(j, out var frame) ? frame : 0];
                                                                                switch (bandSystem)
                                                                                {
                                                                                    case 0:
                                                                                        r.Set(bandPosition0 + binBandLength * (bandDigit - j - 1) + distanceBand - unlarge, bandPosition1 - unlarge, binBandLength + enlarge, binBandHeight + enlarge);
                                                                                        break;
                                                                                    case 1:
                                                                                        r.Set(bandPosition0 + binBandLength * (0.5 * bandDigit - j - 1) + distanceBand - unlarge, bandPosition1 - unlarge, binBandLength + enlarge, binBandHeight + enlarge);
                                                                                        break;
                                                                                    case 2:
                                                                                        r.Set(bandPosition0 - binBandLength * (j + 1) + distanceBand - unlarge, bandPosition1 - unlarge, binBandLength + enlarge, binBandHeight + enlarge);
                                                                                        break;
                                                                                }
                                                                                targetSession.PaintDrawing(ref r, binBandDrawing);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentMeter:
                                                                    var judgmentMeterPosition0 = drawingComponentValue.judgmentMeterPosition0;
                                                                    var judgmentMeterPosition1 = drawingComponentValue.judgmentMeterPosition1;
                                                                    var binJudgmentMeterLength = drawingComponentValue.binJudgmentMeterLength;
                                                                    var binJudgmentMeterHeight = drawingComponentValue.binJudgmentMeterHeight;
                                                                    var judgmentMeterFrontDrawingLength = drawingComponentValue.judgmentMeterFrontDrawingLength;
                                                                    var judgmentMeterUnitDrawingLength = drawingComponentValue.judgmentMeterUnitDrawingLength;
                                                                    var judgmentMeters = defaultComputer.JudgmentMeters;
                                                                    var judgmentMeterSystem = drawingComponentValue.judgmentMeterSystem;
                                                                    var altJudgmentMeter = drawingComponentValue.altJudgmentMeter;
                                                                    var binJudgmentMeterMap = UI.Instance.BinJudgmentMeterMap;
                                                                    var drawingJudgmentMeterHigher = UI.Instance.JudgmentMererHigherDrawing;
                                                                    var drawingJudgmentMeterLower = UI.Instance.JudgmentMeterLowerDrawing;
                                                                    var drawingJudgmentMeterUnit = UI.Instance.JudgmentMeterUnitDrawing;
                                                                    for (var i = altJudgmentMeter >> 1; i >= altJudgmentMeter % 2; --i)
                                                                    {
                                                                        var judgmentMeter = judgmentMeters[i];
                                                                        var judgmentMeterValue = (int)Math.Round(judgmentMeter);
                                                                        var absJudgmentMeter = Math.Abs(judgmentMeterValue);
                                                                        if (absJudgmentMeter >= Configure.Instance.JudgmentMeterMillis)
                                                                        {
                                                                            var distanceJudgmentMeter = i == 1 && has2P ? distance2P : 0F;
                                                                            var judgmentMeterDigit = QwilightComponent.GetDigit(absJudgmentMeter);
                                                                            switch (judgmentMeterSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(judgmentMeterPosition0 - binJudgmentMeterLength + distanceJudgmentMeter, judgmentMeterPosition1, judgmentMeterFrontDrawingLength, binJudgmentMeterHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(judgmentMeterPosition0 - 0.5 * (binJudgmentMeterLength * judgmentMeterDigit + judgmentMeterFrontDrawingLength) + distanceJudgmentMeter, judgmentMeterPosition1, judgmentMeterFrontDrawingLength, binJudgmentMeterHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(judgmentMeterPosition0 - binJudgmentMeterLength * judgmentMeterDigit - judgmentMeterFrontDrawingLength + distanceJudgmentMeter, judgmentMeterPosition1, judgmentMeterFrontDrawingLength, binJudgmentMeterHeight);
                                                                                    break;
                                                                            }
                                                                            if (absJudgmentMeter > 0)
                                                                            {
                                                                                targetSession.PaintDrawing(ref r, judgmentMeter > 0 ? drawingJudgmentMeterHigher : drawingJudgmentMeterLower);
                                                                            }
                                                                            r.Position0 += judgmentMeterFrontDrawingLength;
                                                                            r.Length = binJudgmentMeterLength;
                                                                            for (var j = judgmentMeterDigit - 1; j >= 0; --j)
                                                                            {
                                                                                targetSession.PaintDrawing(ref r, binJudgmentMeterMap[(int)(absJudgmentMeter / Math.Pow(10, j) % 10)]);
                                                                                r.Position0 += binJudgmentMeterLength;
                                                                            }
                                                                            r.Length = judgmentMeterUnitDrawingLength;
                                                                            targetSession.PaintDrawing(ref r, drawingJudgmentMeterUnit);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Stand:
                                                                    var stand = defaultComputer.Stand.Value;
                                                                    var standDigit = QwilightComponent.GetDigit(stand);
                                                                    var standCommaDrawing = UI.Instance.CommaDrawing;
                                                                    var standCommaDrawingLength = drawingComponentValue.standCommaDrawingLength;
                                                                    var standCommaCount = (standDigit - 1) / 3;
                                                                    var standPosition0 = drawingComponentValue.standPosition0;
                                                                    var standPosition1 = drawingComponentValue.standPosition1;
                                                                    var binStandLength = drawingComponentValue.binStandLength;
                                                                    var binStandHeight = drawingComponentValue.binStandHeight;
                                                                    var standSystem = drawingComponentValue.standSystem;
                                                                    var binStandMap = UI.Instance.BinStandMap;
                                                                    var altStand = drawingComponentValue.altStand;
                                                                    for (var i = altStand >> 1; i >= altStand % 2; --i)
                                                                    {
                                                                        var distanceStand = i == 1 && has2P ? distance2P : 0F;
                                                                        switch (standSystem)
                                                                        {
                                                                            case 0:
                                                                                r.Set(standPosition0 + (standDigit - 1) * binStandLength + standCommaCount * standCommaDrawingLength + distanceStand, standPosition1, binStandLength, binStandHeight);
                                                                                break;
                                                                            case 1:
                                                                                r.Set(standPosition0 + (standDigit / 2 - 1) * binStandLength + (standCommaCount * standCommaDrawingLength) / 2 + distanceStand, standPosition1, binStandLength, binStandHeight);
                                                                                break;
                                                                            case 2:
                                                                                r.Set(standPosition0 - binStandLength + distanceStand, standPosition1, binStandLength, binStandHeight);
                                                                                break;
                                                                        }
                                                                        targetSession.PaintDrawing(ref r, binStandMap[stand % 10]);
                                                                        if (standDigit >= 2)
                                                                        {
                                                                            r.Position0 -= binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 10 % 10]);
                                                                        }
                                                                        if (standDigit >= 3)
                                                                        {
                                                                            r.Position0 -= binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 100 % 10]);
                                                                        }
                                                                        if (standDigit >= 4)
                                                                        {
                                                                            r.Position0 -= standCommaDrawingLength;
                                                                            r.Length = standCommaDrawingLength;
                                                                            targetSession.PaintDrawing(ref r, standCommaDrawing);
                                                                            r.Position0 -= binStandLength;
                                                                            r.Length = binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 1000 % 10]);
                                                                        }
                                                                        if (standDigit >= 5)
                                                                        {
                                                                            r.Position0 -= binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 10000 % 10]);
                                                                        }
                                                                        if (standDigit >= 6)
                                                                        {
                                                                            r.Position0 -= binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 100000 % 10]);
                                                                        }
                                                                        if (standDigit >= 7)
                                                                        {
                                                                            r.Position0 -= standCommaDrawingLength;
                                                                            r.Length = standCommaDrawingLength;
                                                                            targetSession.PaintDrawing(ref r, standCommaDrawing);
                                                                            r.Position0 -= binStandLength;
                                                                            r.Length = binStandLength;
                                                                            targetSession.PaintDrawing(ref r, binStandMap[stand / 1000000 % 10]);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Point:
                                                                    var pointInt = (int)(10000 * defaultComputer.Point.Value);
                                                                    var pointStopPointDrawingLength = drawingComponentValue.pointStopPointDrawingLength;
                                                                    var pointPosition0 = drawingComponentValue.pointPosition0;
                                                                    var pointPosition1 = drawingComponentValue.pointPosition1;
                                                                    var binPointLength = drawingComponentValue.binPointLength;
                                                                    var binPointHeight = drawingComponentValue.binPointHeight;
                                                                    var pointUnitDrawingLength = drawingComponentValue.pointUnitDrawingLength;
                                                                    var pointSystem = drawingComponentValue.pointSystem;
                                                                    var altPoint = drawingComponentValue.altPoint;
                                                                    var binPointMap = UI.Instance.BinPointMap;
                                                                    var pointUnitDrawing = UI.Instance.PointUnitDrawing;
                                                                    var pointStopPointDrawing = UI.Instance.PointStopPointDrawing;
                                                                    var pointDigit = pointInt < 100 ? 3 : QwilightComponent.GetDigit(pointInt);
                                                                    for (var i = altPoint >> 1; i >= altPoint % 2; --i)
                                                                    {
                                                                        var distancePoint = i == 1 && has2P ? distance2P : 0F;
                                                                        for (var j = pointDigit - 1; j >= 2; --j)
                                                                        {
                                                                            switch (pointSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(pointPosition0 + binPointLength * (pointDigit - j - 1) + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(pointPosition0 + binPointLength * (0.5 * pointDigit - j - 1) - pointStopPointDrawingLength * 0.5 - pointUnitDrawingLength * 0.5 + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(pointPosition0 - binPointLength * (j + 1) - pointStopPointDrawingLength - pointUnitDrawingLength + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, binPointMap[(int)(pointInt / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                        r.Position0 += binPointLength;
                                                                        r.Length = pointStopPointDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, pointStopPointDrawing);
                                                                        for (var j = 1; j >= 0; --j)
                                                                        {
                                                                            switch (pointSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(pointPosition0 + binPointLength * (pointDigit - j - 1) + pointStopPointDrawingLength + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(pointPosition0 + binPointLength * (0.5 * pointDigit - j - 1) + 0.5 * pointStopPointDrawingLength - 0.5 * pointUnitDrawingLength + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(pointPosition0 - binPointLength * (j + 1) - pointUnitDrawingLength + distancePoint, pointPosition1, binPointLength, binPointHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, binPointMap[(int)(pointInt / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                        r.Position0 += binPointLength;
                                                                        r.Length = pointUnitDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, pointUnitDrawing);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.BPM:
                                                                    var bpmInt = (int)(Math.Min(Math.Round(Math.Abs(Configure.Instance.BPMVarietyValue.Data switch
                                                                    {
                                                                        BPMVariety.BPM => defaultComputer.HandlingBPM,
                                                                        BPMVariety.AudioMultiplier => defaultComputer.HandlingBPM * defaultComputer.AudioMultiplier,
                                                                        BPMVariety.Multiplier => defaultComputer.HandlingBPM * defaultComputer.AudioMultiplier * defaultComputer.ModeComponentValue.Multiplier,
                                                                        _ => default
                                                                    })), int.MaxValue));
                                                                    var bpmPosition0 = drawingComponentValue.bpmPosition0;
                                                                    var bpmPosition1 = drawingComponentValue.bpmPosition1;
                                                                    var binBPMLength = drawingComponentValue.binBPMLength;
                                                                    var binBPMHeight = drawingComponentValue.binBPMHeight;
                                                                    var bpmUnitDrawingLength = drawingComponentValue.bpmUnitDrawingLength;
                                                                    var bpmSystem = drawingComponentValue.bpmSystem;
                                                                    var bpmDigit = QwilightComponent.GetDigit(bpmInt);
                                                                    var altBPM = drawingComponentValue.altBPM;
                                                                    var binBPMMap = UI.Instance.BinBPMMap;
                                                                    var bpmUnitDrawing = UI.Instance.BPMUnitDrawing;
                                                                    for (var i = altBPM >> 1; i >= altBPM % 2; --i)
                                                                    {
                                                                        var distanceBPM = i == 1 && has2P ? distance2P : 0F;
                                                                        for (var j = bpmDigit - 1; j >= 0; --j)
                                                                        {
                                                                            switch (bpmSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(bpmPosition0 + binBPMLength * (bpmDigit - j - 1) + distanceBPM, bpmPosition1, binBPMLength, binBPMHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(bpmPosition0 + binBPMLength * (0.5 * bpmDigit - j - 1) + distanceBPM, bpmPosition1, binBPMLength, binBPMHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(bpmPosition0 - binBPMLength * (j + 1) + distanceBPM, bpmPosition1, binBPMLength, binBPMHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, binBPMMap[(int)(bpmInt / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                        r.Position0 += binBPMLength;
                                                                        r.Length = bpmUnitDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, bpmUnitDrawing);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Multiplier:
                                                                    var multiplierInt = (int)Math.Round(10 * defaultComputer.ModeComponentValue.Multiplier);
                                                                    var multiplierPosition0 = drawingComponentValue.multiplierPosition0;
                                                                    var multiplierPosition1 = drawingComponentValue.multiplierPosition1;
                                                                    var binMultiplierLength = drawingComponentValue.binMultiplierLength;
                                                                    var binMultiplierHeight = drawingComponentValue.binMultiplierHeight;
                                                                    var multiplierStopPointDrawingLength = drawingComponentValue.multiplierStopPointDrawingLength;
                                                                    var multiplierUnitDrawingLength = drawingComponentValue.multiplierUnitDrawingLength;
                                                                    var multiplierDigit = multiplierInt < 10 ? 2 : QwilightComponent.GetDigit(multiplierInt);
                                                                    var multiplierSystem = drawingComponentValue.multiplierSystem;
                                                                    var altMultiplier = drawingComponentValue.altMultiplier;
                                                                    var binMultiplierMap = UI.Instance.BinMultiplierMap;
                                                                    var multiplierUnitDrawing = UI.Instance.MultiplierUnitDrawing;
                                                                    var multiplierStopPointDrawing = UI.Instance.MultiplierStopPointDrawing;
                                                                    for (var i = altMultiplier >> 1; i >= altMultiplier % 2; --i)
                                                                    {
                                                                        var distanceMultiplier = i == 1 && has2P ? distance2P : 0F;
                                                                        switch (multiplierSystem)
                                                                        {
                                                                            case 0:
                                                                                r.Set(multiplierPosition0 + multiplierStopPointDrawingLength + (multiplierDigit - 1) * binMultiplierLength + distanceMultiplier, multiplierPosition1, binMultiplierLength, binMultiplierHeight);
                                                                                break;
                                                                            case 1:
                                                                                r.Set(multiplierPosition0 + ((multiplierDigit / 2.0) - 1) * binMultiplierLength + multiplierStopPointDrawingLength / 2 + distanceMultiplier, multiplierPosition1, binMultiplierLength, binMultiplierHeight);
                                                                                break;
                                                                            case 2:
                                                                                r.Set(multiplierPosition0 - binMultiplierLength + distanceMultiplier, multiplierPosition1, binMultiplierLength, binMultiplierHeight);
                                                                                break;
                                                                        }
                                                                        targetSession.PaintDrawing(ref r, binMultiplierMap[multiplierInt % 10]);
                                                                        r.Position0 -= multiplierStopPointDrawingLength;
                                                                        r.Length = multiplierStopPointDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, multiplierStopPointDrawing);
                                                                        r.Position0 -= binMultiplierLength;
                                                                        r.Length = binMultiplierLength;
                                                                        targetSession.PaintDrawing(ref r, binMultiplierMap[multiplierInt / 10 % 10]);
                                                                        for (var j = 2; j < multiplierDigit; ++j)
                                                                        {
                                                                            r.Position0 -= binMultiplierLength;
                                                                            targetSession.PaintDrawing(ref r, binMultiplierMap[multiplierInt / (int)Math.Pow(10, j) % 10]);
                                                                        }
                                                                        r.Position0 -= multiplierUnitDrawingLength;
                                                                        r.Length = multiplierUnitDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, multiplierUnitDrawing);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentPaint:
                                                                    var judgmentPaintComposition = (CanvasComposite)drawingComponentValue.judgmentPaintComposition;
                                                                    var judgmentPaints = defaultComputer.JudgmentPaints;
                                                                    lock (judgmentPaints)
                                                                    {
                                                                        foreach (var judgmentPaint in judgmentPaints.Values)
                                                                        {
                                                                            judgmentPaint.Paint(ref r, targetSession, 1F, judgmentPaintComposition);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.HitNotePaint:
                                                                    var hitNotePaintComposition = (CanvasComposite)drawingComponentValue.hitNotePaintComposition;
                                                                    var hitNotePaints = defaultComputer.HitNotePaints;
                                                                    lock (hitNotePaints)
                                                                    {
                                                                        foreach (var hitNotePaint in hitNotePaints.Values)
                                                                        {
                                                                            hitNotePaint.Paint(ref r, targetSession, 1F, hitNotePaintComposition);
                                                                        }
                                                                    }
                                                                    var hitLongNotePaints = defaultComputer.HitLongNotePaints;
                                                                    lock (hitLongNotePaints)
                                                                    {
                                                                        foreach (var hitLongNotePaint in hitLongNotePaints.Values)
                                                                        {
                                                                            hitLongNotePaint.Paint(ref r, targetSession, 1F, hitNotePaintComposition);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Net:
                                                                    var netItemFaint = (float)Configure.Instance.UIConfigureValue.NetItemFaintV2;
                                                                    if (netItemFaint > 0F)
                                                                    {
                                                                        var netItems = defaultComputer.NetItems;
                                                                        if (netItems.Count > 0)
                                                                        {
                                                                            var netItemFaintInt = (int)(100 * netItemFaint);
                                                                            var levyingNetPosition = defaultComputer.LevyingNetPosition;
                                                                            var quitNetPosition = defaultComputer.QuitNetPosition;
                                                                            var netSystem = drawingComponentValue.netSystem;
                                                                            var netPosition0 = drawingComponentValue.netPosition0;
                                                                            var netPosition1 = drawingComponentValue.netPosition1;
                                                                            var altNet = drawingComponentValue.altNet;
                                                                            var netPaint = UI.Instance.NetPaints[netItemFaintInt];
                                                                            var netInPaint = UI.Instance.NetInPaints[netItemFaintInt];
                                                                            for (var i = quitNetPosition; i >= levyingNetPosition; --i)
                                                                            {
                                                                                foreach (var netItem in netItems)
                                                                                {
                                                                                    if (netItem.TargetPosition == i)
                                                                                    {
                                                                                        var drawings = netItem.Drawings;
                                                                                        var drawingPosition = netItem.DrawingPosition - levyingNetPosition;
                                                                                        var d2dHitPointsPaint = BaseUI.Instance.D2DHitPointsPaints[(int)netItem.HitPointsModeValue][netItem.IsMyNetItem ? netItemFaintInt : netItemFaintInt / 2];
                                                                                        var netItemValue = Math.Min(netItem.HitPoints, 1.0);
                                                                                        var avatarNetStatusPaint = _avatarNetStatusPaints[(int)netItem.AvatarNetStatus][netItemFaintInt];
                                                                                        var avatarID = netItem.AvatarID;
                                                                                        var avatarDrawing = AvatarDrawingSystem.Instance.JustGetAvatarDrawing(avatarID);
                                                                                        if (!avatarDrawing.HasValue && AvatarDrawingSystem.Instance.CanCallAPI(avatarID))
                                                                                        {
                                                                                            Task.Factory.StartNew(getAvatarDrawing, avatarID);
                                                                                        }
                                                                                        var lowHitPointsPaint = _lowHitPointsPaints[(int)(netItem.IsFailedStatus * netItemFaintInt)];
                                                                                        var avatarTitle = AvatarTitleSystem.Instance.JustGetAvatarTitle(avatarID);
                                                                                        if (!avatarTitle.HasValue && AvatarTitleSystem.Instance.CanCallAPI(avatarID))
                                                                                        {
                                                                                            Task.Factory.StartNew(getAvatarTitle, avatarID);
                                                                                        }
                                                                                        var avatarEdge = AvatarEdgeSystem.Instance.JustGetAvatarEdge(avatarID);
                                                                                        if (!avatarEdge.HasValue && AvatarEdgeSystem.Instance.CanCallAPI(avatarID))
                                                                                        {
                                                                                            Task.Factory.StartNew(getAvatarEdge, avatarID);
                                                                                        }
                                                                                        var hasAvatarTitle = !string.IsNullOrEmpty(avatarTitle?.Title);
                                                                                        var textItem0 = hasAvatarTitle ? PoolSystem.Instance.GetTextItem(avatarTitle.Value.Title, NetFont) : null;
                                                                                        var textBound0 = textItem0?.LayoutBounds ?? default;
                                                                                        var textBound0Length = textBound0.Width;
                                                                                        var textBound0Height = textBound0.Height;
                                                                                        var textItem1 = PoolSystem.Instance.GetTextItem(netItem.AvatarName, NetFont);
                                                                                        var textBound1 = textItem1.LayoutBounds;
                                                                                        var textBound1Length = textBound1.Width;
                                                                                        var textBound1Height = textBound1.Height;
                                                                                        var textItem2 = PoolSystem.Instance.GetTextItem(netItem.Stand, NetFont);
                                                                                        var textBound2 = textItem2.LayoutBounds;
                                                                                        var textBound2Length = textBound2.Width;
                                                                                        var textBound2Height = textBound2.Height;
                                                                                        var textItem3 = PoolSystem.Instance.GetTextItem(netItem.Point, NetFont);
                                                                                        var textBound3 = textItem3.LayoutBounds;
                                                                                        var textBound3Length = textBound3.Width;
                                                                                        var textBound3Height = textBound3.Height;
                                                                                        var textItem4 = PoolSystem.Instance.GetTextItem(netItem.Band, NetFont);
                                                                                        var textBound4 = textItem4.LayoutBounds;
                                                                                        var textBound4Length = textBound4.Width;
                                                                                        var textBound4Height = textBound4.Height;

                                                                                        defaultComputer.HighestNetHeight = (float)Math.Max(defaultComputer.HighestNetHeight, Levels.StandardMargin + Math.Max(textBound0Height, textBound1Height) + Levels.StandardMargin + Utility.Max(textBound2Height, textBound3Height, textBound4Height) + Levels.StandardMarginFloat32);
                                                                                        var highestNetHeight = defaultComputer.HighestNetHeight;

                                                                                        defaultComputer.HighestNetLength = (float)Utility.Max(defaultComputer.HighestNetLength, highestNetHeight + Levels.StandardMargin + textBound0Length + Levels.StandardMargin + textBound1Length + Levels.StandardMargin + highestNetHeight, highestNetHeight + Levels.StandardMargin + textBound2Length + Levels.StandardMargin + textBound3Length + Levels.StandardMargin + textBound4Length + Levels.StandardMargin + highestNetHeight);
                                                                                        var highestNetLength = defaultComputer.HighestNetLength;

                                                                                        var lastJudged = netItem.LastJudged;
                                                                                        var hasLastJudged = lastJudged != Component.Judged.Not;
                                                                                        var drawingPosition1 = netPosition1 + (Levels.StandardEdgeFloat32 + highestNetHeight + Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32) * drawingPosition;

                                                                                        var targetPosition0 = netPosition0;
                                                                                        if (netSystem == 2)
                                                                                        {
                                                                                            targetPosition0 -= Levels.StandardEdgeFloat32 + highestNetLength + Levels.StandardEdgeFloat32;
                                                                                            if (drawings != null)
                                                                                            {
                                                                                                targetPosition0 -= Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32 + highestNetHeight + Levels.StandardEdgeFloat32;
                                                                                            }
                                                                                        }

                                                                                        for (var j = altNet >> 1; j >= altNet % 2; --j)
                                                                                        {
                                                                                            var distanceNet = j == 1 && has2P ? distance2P : 0F;
                                                                                            r.Set(targetPosition0 + distanceNet, drawingPosition1, Levels.StandardEdgeFloat32 + highestNetLength + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + highestNetHeight + Levels.StandardEdgeFloat32);
                                                                                            targetSession.DrawRectangle(r, avatarNetStatusPaint);
                                                                                            if (!defaultComputer.IsPausingWindowOpened && defaultComputer.CanSetPosition)
                                                                                            {
                                                                                                SetNetItemHandler(ref r, handleCommentNetItemImpl, netItem);
                                                                                            }

                                                                                            r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, r.Length - 2 * Levels.StandardEdgeFloat32, r.Height - 2 * Levels.StandardEdgeFloat32);
                                                                                            targetSession.FillRectangle(r, netInPaint);

                                                                                            r.Position0 += highestNetHeight;
                                                                                            r.Length = (highestNetLength - 2 * highestNetHeight) * netItemValue;
                                                                                            r.Height = highestNetHeight;
                                                                                            targetSession.FillRectangle(r, d2dHitPointsPaint);

                                                                                            r.Position0 += highestNetLength - 2 * highestNetHeight;
                                                                                            r.Length = highestNetHeight;
                                                                                            targetSession.PaintDrawing(ref r, BaseUI.Instance.QuitDrawings[(int)netItem.QuitValue][0]?.Drawing, netItemFaint);

                                                                                            r.SetPosition(targetPosition0 + highestNetHeight + Levels.StandardMargin + distanceNet, drawingPosition1 + Levels.StandardMarginFloat32);
                                                                                            if (hasAvatarTitle)
                                                                                            {
                                                                                                var position0 = Levels.StandardMargin + textBound0Length;
                                                                                                targetSession.PaintText(textItem0, ref r, avatarTitle.Value.TitleColor);
                                                                                                r.Position0 += position0;
                                                                                                targetSession.PaintText(textItem1, ref r, netPaint);
                                                                                                r.Position0 -= position0;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                targetSession.PaintText(textItem1, ref r, netPaint);
                                                                                            }
                                                                                            r.Position1 += Levels.StandardMargin + textBound1Height;
                                                                                            targetSession.PaintText(textItem2, ref r, netPaint);
                                                                                            r.Position0 += Levels.StandardMargin + textBound2Length;
                                                                                            targetSession.PaintText(textItem3, ref r, netPaint);
                                                                                            r.Position0 += Levels.StandardMargin + textBound3Length;
                                                                                            targetSession.PaintText(textItem4, ref r, netPaint);

                                                                                            r.Set(targetPosition0 + distanceNet + Levels.StandardEdgeFloat32, drawingPosition1 + Levels.StandardEdgeFloat32, highestNetHeight, highestNetHeight);
                                                                                            targetSession.PaintDrawing(ref r, avatarDrawing?.Drawing, netItemFaint);

                                                                                            s.Set(r.Position0 + r.Length * Levels.EdgeXY, r.Position1 + r.Height * Levels.EdgeXY, r.Length * Levels.EdgeMargin, r.Height * Levels.EdgeMargin);
                                                                                            targetSession.PaintDrawing(ref s, avatarEdge?.Drawing, netItemFaint);

                                                                                            targetSession.FillRectangle(r, lowHitPointsPaint);

                                                                                            if (drawings != null && netItem.AvatarNetStatus == Event.Types.AvatarNetStatus.Default)
                                                                                            {
                                                                                                var valueLength = highestNetHeight / netItem.P2BuiltLength;
                                                                                                var valueHeight = highestNetHeight / netItem.JudgmentMainPosition;
                                                                                                var target = PoolSystem.Instance.GetTargetItem(highestNetHeight, highestNetHeight);
                                                                                                using (var session = target.CreateDrawingSession())
                                                                                                {
                                                                                                    session.Clear(Colors.Black);
                                                                                                    foreach (var drawing in drawings)
                                                                                                    {
                                                                                                        var averageColor = drawing.Param;
                                                                                                        var valueColor = Color.FromArgb((byte)(averageColor & 255), (byte)((averageColor & 65280) >> 8), (byte)((averageColor & 16711680) >> 16), (byte)((averageColor & 4278190080) >> 24));
                                                                                                        switch (drawing.DrawingVariety)
                                                                                                        {
                                                                                                            case Event.Types.NetDrawing.Types.Variety.Note:
                                                                                                                s.Set(drawing.Position0 * valueLength, drawing.Position1 * valueHeight, drawing.Length * valueLength, drawing.Height * valueHeight);
                                                                                                                var ellipse = (float)Math.Min(s.Length / 2, s.Height / 2);
                                                                                                                session.FillRoundedRectangle(s, ellipse, ellipse, valueColor);
                                                                                                                break;
                                                                                                            case Event.Types.NetDrawing.Types.Variety.Main:
                                                                                                                s.Set(drawing.Position0 * valueLength, drawing.Position1 * valueHeight, drawing.Length * valueLength, drawing.Height * valueHeight);
                                                                                                                session.FillRectangle(s, valueColor);
                                                                                                                break;
                                                                                                            case Event.Types.NetDrawing.Types.Variety.Meter:
                                                                                                                s.Set(0F, drawing.Position1 * valueHeight, highestNetHeight, valueHeight);
                                                                                                                session.FillRectangle(s, valueColor);
                                                                                                                break;
                                                                                                        }
                                                                                                    }
                                                                                                }

                                                                                                r.Set(targetPosition0 + distanceNet + Levels.StandardEdgeFloat32 + highestNetLength + Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32, drawingPosition1, Levels.StandardEdgeFloat32 + highestNetHeight + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + highestNetHeight + Levels.StandardEdgeFloat32);
                                                                                                targetSession.DrawRectangle(r, (hasLastJudged ? d2dJudgmentPaints[(int)lastJudged] : FaintClearedPaints)[netItemFaintInt]);
                                                                                                if (!isItemMode && !defaultComputer.IsPausingWindowOpened && 0.0 <= loopingCounter)
                                                                                                {
                                                                                                    SetNetItemHandler(ref r, handleIONetItemImpl, netItem);
                                                                                                }

                                                                                                r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, r.Length - 2 * Levels.StandardEdgeFloat32, r.Height - 2 * Levels.StandardEdgeFloat32);
                                                                                                targetSession.PaintDrawing(ref r, target, netItemFaint);
                                                                                            }
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Unpause:
                                                                    var pauseCount = defaultComputer.PauseCount;
                                                                    if (pauseCount > 0)
                                                                    {
                                                                        var pausePosition0 = drawingComponentValue.pausePosition0;
                                                                        var pausePosition1 = drawingComponentValue.pausePosition1;
                                                                        var pauseLength = drawingComponentValue.pauseLength;
                                                                        var pauseHeight = drawingComponentValue.pauseHeight;
                                                                        var pauseSystem = drawingComponentValue.pauseSystem;
                                                                        var altPause = drawingComponentValue.altPause;
                                                                        var pauseDrawings = UI.Instance.PauseDrawings;
                                                                        for (var i = altPause >> 1; i >= altPause % 2; --i)
                                                                        {
                                                                            var distancePause = i == 1 && has2P ? distance2P : 0F;
                                                                            switch (pauseSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(pausePosition0 + distancePause, pausePosition1, pauseLength, pauseHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(pausePosition0 - pauseLength / 2 + distancePause, pausePosition1, pauseLength, pauseHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(pausePosition0 - pauseLength + distancePause, pausePosition1, pauseLength, pauseHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, pauseDrawings[pauseCount - 1][defaultComputer.PauseFrames[pauseCount - 1]]);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Status:
                                                                    var statusPosition0 = drawingComponentValue.statusPosition0;
                                                                    var statusPosition1 = drawingComponentValue.statusPosition1;
                                                                    var statusLength = drawingComponentValue.statusLength;
                                                                    var statusHeight = drawingComponentValue.statusHeight;
                                                                    var statusSystem = drawingComponentValue.statusSystem;
                                                                    var altStatus = drawingComponentValue.altStatus;
                                                                    var statusDrawing = UI.Instance.StatusDrawing;
                                                                    if (statusDrawing.HasValue)
                                                                    {
                                                                        var statusDrawingValue = statusDrawing.Value;
                                                                        var statusDrawingBound = statusDrawingValue.DrawingBound;
                                                                        var statusDrawingLength = statusDrawingBound.Length;
                                                                        var statusDrawingHeight = statusDrawingBound.Height;
                                                                        for (var i = altStatus >> 1; i >= altStatus % 2; --i)
                                                                        {
                                                                            var distanceStatus = i == 1 && has2P ? distance2P : 0F;
                                                                            switch (statusSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(statusPosition0 + distanceStatus, statusPosition1 + statusHeight * (1 - status), statusLength, statusHeight * status);
                                                                                    s.Set(0.0, (1 - status) * statusDrawingHeight, statusDrawingLength, statusDrawingHeight * status);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(statusPosition0 + distanceStatus, statusPosition1, statusLength, statusHeight * status);
                                                                                    s.SetArea(statusDrawingLength, statusDrawingHeight * status);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(statusPosition0 + distanceStatus + statusLength * (1 - status), statusPosition1, statusLength * status, statusHeight);
                                                                                    s.Set((1 - status) * statusDrawingLength, 0.0, statusDrawingLength * status, statusDrawingHeight);
                                                                                    break;
                                                                                case 3:
                                                                                    r.Set(statusPosition0 + distanceStatus, statusPosition1, statusLength * status, statusHeight);
                                                                                    s.SetArea(statusDrawingLength * status, statusDrawingHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, ref s, statusDrawingValue);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Hms:
                                                                    var length = (int)(defaultComputer.Length / 1000.0);
                                                                    var mLength = length / 60;
                                                                    var sLength = length % 60;
                                                                    var wait = Math.Clamp((int)(loopingCounter / 1000.0), 0, length);
                                                                    var mWait = wait / 60;
                                                                    var sWait = wait % 60;
                                                                    var hmsColonDrawingLength = drawingComponentValue.hmsColonDrawingLength;
                                                                    var hmsSlashDrawingLength = drawingComponentValue.hmsSlashDrawingLength;
                                                                    var hmsPosition0 = drawingComponentValue.hmsPosition0;
                                                                    var hmsPosition1 = drawingComponentValue.hmsPosition1;
                                                                    var binHmsLength = drawingComponentValue.binHmsLength;
                                                                    var binHmsHeight = drawingComponentValue.binHmsHeight;
                                                                    var mWaitDigit = QwilightComponent.GetDigit(mWait);
                                                                    var mLengthDigit = QwilightComponent.GetDigit(mLength);
                                                                    var hmsSystem = drawingComponentValue.hmsSystem;
                                                                    var binHmsMap = UI.Instance.BinHmsMap;
                                                                    var altHms = drawingComponentValue.altHms;
                                                                    var hmsColonDrawimg = UI.Instance.ColonDrawing;
                                                                    var hmsSlashDrawing = UI.Instance.SlashDrawing;
                                                                    for (var i = altHms >> 1; i >= altHms % 2; --i)
                                                                    {
                                                                        var distanceHms = i == 1 && has2P ? distance2P : 0F;
                                                                        switch (hmsSystem)
                                                                        {
                                                                            case 0:
                                                                                r.Set(hmsPosition0 + 2 * hmsColonDrawingLength + hmsSlashDrawingLength + (mWaitDigit + mLengthDigit + 3) * binHmsLength + distanceHms, hmsPosition1, binHmsLength, binHmsHeight);
                                                                                break;
                                                                            case 1:
                                                                                r.Set(hmsPosition0 + (mWaitDigit / 2.0 + mLengthDigit / 2.0 + 1) * binHmsLength + hmsColonDrawingLength / 2 + hmsSlashDrawingLength / 2 + distanceHms, hmsPosition1, binHmsLength, binHmsHeight);
                                                                                break;
                                                                            case 2:
                                                                                r.Set(hmsPosition0 - binHmsLength + distanceHms, hmsPosition1, binHmsLength, binHmsHeight);
                                                                                break;
                                                                        }
                                                                        targetSession.PaintDrawing(ref r, binHmsMap[sLength / 1 % 10]);
                                                                        r.Position0 -= binHmsLength;
                                                                        r.Length = binHmsLength;
                                                                        targetSession.PaintDrawing(ref r, binHmsMap[sLength / 10 % 10]);
                                                                        r.Position0 -= hmsColonDrawingLength;
                                                                        r.Length = hmsColonDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, hmsColonDrawimg);
                                                                        for (var j = 0; j < mLengthDigit; ++j)
                                                                        {
                                                                            r.Position0 -= binHmsLength;
                                                                            r.Length = binHmsLength;
                                                                            targetSession.PaintDrawing(ref r, binHmsMap[(int)(mLength / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                        r.Position0 -= hmsSlashDrawingLength;
                                                                        r.Length = hmsSlashDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, hmsSlashDrawing);
                                                                        r.Length = binHmsLength;
                                                                        for (var j = 1; j <= 10; j *= 10)
                                                                        {
                                                                            r.Position0 -= binHmsLength;
                                                                            targetSession.PaintDrawing(ref r, binHmsMap[sWait / j % 10]);
                                                                        }
                                                                        r.Position0 -= hmsColonDrawingLength;
                                                                        r.Length = hmsColonDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, hmsColonDrawimg);
                                                                        for (var j = 0; j < mWaitDigit; ++j)
                                                                        {
                                                                            r.Position0 -= binHmsLength;
                                                                            r.Length = binHmsLength;
                                                                            targetSession.PaintDrawing(ref r, binHmsMap[(int)(mWait / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentPoints:
                                                                    var totalJudgmentPoints = 0.0;
                                                                    foreach (var judgment in judgments)
                                                                    {
                                                                        totalJudgmentPoints += judgment;
                                                                    }
                                                                    var judgmentPointsPosition0 = drawingComponentValue.judgmentPointsPosition0;
                                                                    var judgmentPointsPosition1 = drawingComponentValue.judgmentPointsPosition1;
                                                                    var judgmentPointsLength = drawingComponentValue.judgmentPointsLength;
                                                                    var judgmentPointsHeight = drawingComponentValue.judgmentPointsHeight;
                                                                    var judgmentPointsDrawings = UI.Instance.JudgmentPointsDrawings;
                                                                    var altJudgmentPoints = drawingComponentValue.altJudgmentPoints;
                                                                    var judgmentPointsSystem = drawingComponentValue.judgmentPointsSystem;
                                                                    var lastPosition = 0.0;
                                                                    for (var i = (int)Component.Judged.Highest; i <= (int)Component.Judged.Lowest; ++i)
                                                                    {
                                                                        var judgmentPoint = judgments[i] / totalJudgmentPoints;
                                                                        var judgmentPointDrawing = judgmentPointsDrawings[i];
                                                                        if (judgmentPointDrawing.HasValue)
                                                                        {
                                                                            var judgmentPointDrawingValue = judgmentPointDrawing.Value;
                                                                            var judgmentPointDrawingBound = judgmentPointDrawingValue.DrawingBound;
                                                                            var judgmentPointDrawingLength = judgmentPointDrawingBound.Length;
                                                                            var judgmentPointDrawingHeight = judgmentPointDrawingBound.Height;
                                                                            for (var j = altJudgmentPoints >> 1; j >= altJudgmentPoints % 2; --j)
                                                                            {
                                                                                var distanceJudgmentPoints = j == 1 && has2P ? distance2P : 0F;
                                                                                switch (judgmentPointsSystem)
                                                                                {
                                                                                    case 0:
                                                                                        r.Set(judgmentPointsPosition0 + distanceJudgmentPoints, judgmentPointsPosition1 + judgmentPointsHeight * lastPosition, judgmentPointsLength, judgmentPointsHeight * judgmentPoint);
                                                                                        s.Set(0.0, lastPosition * judgmentPointDrawingHeight, judgmentPointDrawingLength, judgmentPointDrawingHeight * judgmentPoint);
                                                                                        break;
                                                                                    case 1:
                                                                                        r.Set(judgmentPointsPosition0 + distanceJudgmentPoints, judgmentPointsPosition1 + judgmentPointsHeight * (1 - lastPosition - judgmentPoint), judgmentPointsLength, judgmentPointsHeight * judgmentPoint);
                                                                                        s.Set(0.0, judgmentPointDrawingHeight * (1 - lastPosition - judgmentPoint), judgmentPointDrawingLength, judgmentPointDrawingHeight * judgmentPoint);
                                                                                        break;
                                                                                    case 2:
                                                                                        r.Set(judgmentPointsPosition0 + distanceJudgmentPoints + judgmentPointsLength * lastPosition, judgmentPointsPosition1, judgmentPointsLength * judgmentPoint, judgmentPointsHeight);
                                                                                        s.Set(judgmentPointDrawingLength * lastPosition, 0.0, judgmentPointDrawingLength * judgmentPoint, judgmentPointDrawingHeight);
                                                                                        break;
                                                                                    case 3:
                                                                                        r.Set(judgmentPointsPosition0 + distanceJudgmentPoints + judgmentPointsLength * (1 - lastPosition - judgmentPoint), judgmentPointsPosition1, judgmentPointsLength * judgmentPoint, judgmentPointsHeight);
                                                                                        s.Set(judgmentPointDrawingLength * (1 - lastPosition - judgmentPoint), 0.0, judgmentPointDrawingLength * judgmentPoint, judgmentPointDrawingHeight);
                                                                                        break;
                                                                                }
                                                                                targetSession.PaintDrawing(ref r, ref s, judgmentPointDrawingValue);
                                                                            }
                                                                            lastPosition += judgmentPoint;
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentMain:
                                                                    var judgmentMainDrawings = UI.Instance.JudgmentMainDrawings[(int)inputMode];
                                                                    var judgmentMainFrames = defaultComputer.JudgmentMainFrames;
                                                                    var judgmentMainPosition1s = drawingComponentValue.judgmentMainPosition1s;
                                                                    var judgmentMainHeights = drawingComponentValue.judgmentMainHeights;
                                                                    foreach (var drawingPipeline in drawingPipelines)
                                                                    {
                                                                        for (var i = inputCount; i > 0; --i)
                                                                        {
                                                                            if (drawingPipeline == drawingInputModeMap[i])
                                                                            {
                                                                                r.Set(defaultComputer.GetPosition(i), judgmentMainPosition1s[i], drawingNoteLengthMap[i], judgmentMainHeights[i]);
                                                                                targetSession.PaintDrawing(ref r, judgmentMainDrawings[i][judgmentMainFrames[i]]);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.MainAreaFaint:
                                                                    var mainAreaFaint = Configure.Instance.MainAreaFaint;
                                                                    if (mainAreaFaint > 0.0)
                                                                    {
                                                                        var mainAreaFaintPaint = FaintFilledPaints[(int)(100 * mainAreaFaint)];
                                                                        r.Set(mainPosition, mainPosition1, p1Length, mainHeight);
                                                                        targetSession.FillRectangle(r, mainAreaFaintPaint);
                                                                        if (has2P)
                                                                        {
                                                                            r.Position0 += distance2P;
                                                                            targetSession.FillRectangle(r, mainAreaFaintPaint);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.AudioMultiplier:
                                                                    var audioMultiplierInt = (int)Math.Round(100 * defaultComputer.AudioMultiplier);
                                                                    var audioMultiplierSystem = drawingComponentValue.audioMultiplierSystem;
                                                                    var audioMultiplierPosition0 = drawingComponentValue.audioMultiplierPosition0;
                                                                    var audioMultiplierPosition1 = drawingComponentValue.audioMultiplierPosition1;
                                                                    var binAudioMultiplierLength = drawingComponentValue.binAudioMultiplierLength;
                                                                    var binAudioMultiplierHeight = drawingComponentValue.binAudioMultiplierHeight;
                                                                    var altAudioMultiplier = drawingComponentValue.altAudioMultiplier;
                                                                    var audioMultiplierStopPointDrawingLength = drawingComponentValue.audioMultiplierStopPointDrawingLength;
                                                                    var audioMultiplierUnitDrawingLength = drawingComponentValue.audioMultiplierUnitDrawingLength;
                                                                    var binAudioMultiplierMap = UI.Instance.BinAudioMultiplierMap;
                                                                    var audioMultiplierUnitDrawing = UI.Instance.AudioMultiplierUnitDrawing;
                                                                    var audioMultiplierStopPointDrawing = UI.Instance.AudioMultiplierStopPointDrawing;
                                                                    for (var i = altAudioMultiplier >> 1; i >= altAudioMultiplier % 2; --i)
                                                                    {
                                                                        var distanceAudioMultiplier = i == 1 && has2P ? distance2P : 0F;
                                                                        switch (audioMultiplierSystem)
                                                                        {
                                                                            case 0:
                                                                                r.Set(audioMultiplierPosition0 + audioMultiplierStopPointDrawingLength + 2 * binAudioMultiplierLength + distanceAudioMultiplier, audioMultiplierPosition1, binAudioMultiplierLength, binAudioMultiplierHeight);
                                                                                break;
                                                                            case 1:
                                                                                r.Set(audioMultiplierPosition0 + 0.5 * binAudioMultiplierLength + audioMultiplierStopPointDrawingLength / 2 + distanceAudioMultiplier, audioMultiplierPosition1, binAudioMultiplierLength, binAudioMultiplierHeight);
                                                                                break;
                                                                            case 2:
                                                                                r.Set(audioMultiplierPosition0 - binAudioMultiplierLength + distanceAudioMultiplier, audioMultiplierPosition1, binAudioMultiplierLength, binAudioMultiplierHeight);
                                                                                break;
                                                                        }
                                                                        targetSession.PaintDrawing(ref r, binAudioMultiplierMap[audioMultiplierInt / 1 % 10]);
                                                                        r.Position0 -= binAudioMultiplierLength;
                                                                        r.Length = binAudioMultiplierLength;
                                                                        targetSession.PaintDrawing(ref r, binAudioMultiplierMap[audioMultiplierInt / 10 % 10]);
                                                                        r.Position0 -= audioMultiplierStopPointDrawingLength;
                                                                        r.Length = audioMultiplierStopPointDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, audioMultiplierStopPointDrawing);
                                                                        r.Position0 -= binAudioMultiplierLength;
                                                                        r.Length = binAudioMultiplierLength;
                                                                        targetSession.PaintDrawing(ref r, binAudioMultiplierMap[audioMultiplierInt / 100 % 10]);
                                                                        r.Position0 -= audioMultiplierUnitDrawingLength;
                                                                        r.Length = audioMultiplierUnitDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, audioMultiplierUnitDrawing);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.HitPointsVisualizer:
                                                                    var hitPointsInt = (int)(100 * hitPoints);
                                                                    var hitPointsVisualizerPosition0 = drawingComponentValue.hitPointsVisualizerPosition0;
                                                                    var hitPointsVisualizerPosition1 = drawingComponentValue.hitPointsVisualizerPosition1;
                                                                    var binHitPointsVisualizerLength = drawingComponentValue.binHitPointsVisualizerLength;
                                                                    var binHitPointsVisualizerHeight = drawingComponentValue.binHitPointsVisualizerHeight;
                                                                    var hitPointsVisualizerUnitDrawingLength = drawingComponentValue.hitPointsVisualizerUnitDrawingLength;
                                                                    var hitPointsVisualizerSystem = drawingComponentValue.hitPointsVisualizerSystem;
                                                                    var altHitPointsVisualizer = drawingComponentValue.altHitPointsVisualizer;
                                                                    var binHitPointsVisualizerMap = UI.Instance.BinHitPointsVisualizerMap;
                                                                    var hitPointsVisualizerUnitDrawing = UI.Instance.HitPointsVisualizerUnitDrawing;
                                                                    var hitPointsDigit = QwilightComponent.GetDigit(hitPointsInt);
                                                                    for (var i = altHitPointsVisualizer >> 1; i >= altHitPointsVisualizer % 2; --i)
                                                                    {
                                                                        var distanceHitPointsVisualizer = i == 1 && has2P ? distance2P : 0F;
                                                                        for (var j = hitPointsDigit - 1; j >= 0; --j)
                                                                        {
                                                                            switch (hitPointsVisualizerSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(hitPointsVisualizerPosition0 + binHitPointsVisualizerLength * (hitPointsDigit - j - 1) + distanceHitPointsVisualizer, hitPointsVisualizerPosition1, binHitPointsVisualizerLength, binHitPointsVisualizerHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(hitPointsVisualizerPosition0 + binHitPointsVisualizerLength * (0.5 * hitPointsDigit - j - 1) + distanceHitPointsVisualizer, hitPointsVisualizerPosition1, binHitPointsVisualizerLength, binHitPointsVisualizerHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(hitPointsVisualizerPosition0 - binHitPointsVisualizerLength * (j + 1) + distanceHitPointsVisualizer, hitPointsVisualizerPosition1, binHitPointsVisualizerLength, binHitPointsVisualizerHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, binHitPointsVisualizerMap[(int)(hitPointsInt / Math.Pow(10, j) % 10)]);
                                                                        }
                                                                        r.Position0 += binHitPointsVisualizerLength;
                                                                        r.Length = hitPointsVisualizerUnitDrawingLength;
                                                                        targetSession.PaintDrawing(ref r, hitPointsVisualizerUnitDrawing);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.HighestJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.Highest], drawingComponentValue.highestJudgmentValueSystem, drawingComponentValue.highestJudgmentValuePosition0, drawingComponentValue.highestJudgmentValuePosition1, drawingComponentValue.binHighestJudgmentValueLength, drawingComponentValue.binHighestJudgmentValueHeight, drawingComponentValue.altHighestJudgmentValue, binJudgmentValueMap[(int)Component.Judged.Highest]);
                                                                    break;
                                                                case PaintPipelineID.HigherJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.Higher], drawingComponentValue.higherJudgmentValueSystem, drawingComponentValue.higherJudgmentValuePosition0, drawingComponentValue.higherJudgmentValuePosition1, drawingComponentValue.binHigherJudgmentValueLength, drawingComponentValue.binHigherJudgmentValueHeight, drawingComponentValue.altHigherJudgmentValue, binJudgmentValueMap[(int)Component.Judged.Higher]);
                                                                    break;
                                                                case PaintPipelineID.HighJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.High], drawingComponentValue.highJudgmentValueSystem, drawingComponentValue.highJudgmentValuePosition0, drawingComponentValue.highJudgmentValuePosition1, drawingComponentValue.binHighJudgmentValueLength, drawingComponentValue.binHighJudgmentValueHeight, drawingComponentValue.altHighJudgmentValue, binJudgmentValueMap[(int)Component.Judged.High]);
                                                                    break;
                                                                case PaintPipelineID.LowJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.Low], drawingComponentValue.lowJudgmentValueSystem, drawingComponentValue.lowerJudgmentValuePosition0, drawingComponentValue.lowJudgmentValuePosition1, drawingComponentValue.binLowJudgmentValueLength, drawingComponentValue.binLowJudgmentValueHeight, drawingComponentValue.altLowJudgmentValue, binJudgmentValueMap[(int)Component.Judged.Low]);
                                                                    break;
                                                                case PaintPipelineID.LowerJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.Lower], drawingComponentValue.lowerJudgmentValueSystem, drawingComponentValue.lowerJudgmentValuePosition0, drawingComponentValue.lowerJudgmentValuePosition1, drawingComponentValue.binLowerJudgmentValueLength, drawingComponentValue.binLowerJudgmentValueHeight, drawingComponentValue.altLowerJudgmentValue, binJudgmentValueMap[(int)Component.Judged.Lower]);
                                                                    break;
                                                                case PaintPipelineID.LowestJudgment:
                                                                    PaintInt(judgments[(int)Component.Judged.Lowest], drawingComponentValue.lowestJudgmentValueSystem, drawingComponentValue.lowestJudgmentValuePosition0, drawingComponentValue.lowestJudgmentValuePosition1, drawingComponentValue.binLowestJudgmentValueLength, drawingComponentValue.binLowestJudgmentValueHeight, drawingComponentValue.altLowestJudgmentValue, binJudgmentValueMap[(int)Component.Judged.Lowest]);
                                                                    break;
                                                                case PaintPipelineID.HighestBand:
                                                                    PaintInt(defaultComputer.HighestBand, drawingComponentValue.highestBandSystem, drawingComponentValue.highestBandPosition0, drawingComponentValue.highestBandPosition1, drawingComponentValue.binHighestBandLength, drawingComponentValue.binHighestBandHeight, drawingComponentValue.altHighestBand, UI.Instance.BinHighestBandMap);
                                                                    break;
                                                                case PaintPipelineID.Limiter:
                                                                    var limiterPosition1 = drawingComponentValue.limiterPosition1;
                                                                    var limiterLength = drawingComponentValue.limiterLength;
                                                                    var limiterHeight = drawingComponentValue.limiterHeight;
                                                                    var limiterColor = Configure.Instance.LimiterColor;
                                                                    var inputMappingValue = (int)defaultComputer.InputMappingValue;
                                                                    if (Configure.Instance.TotalLimiterVariety)
                                                                    {
                                                                        var defaultPaintValues = Component.BasePaintMap[inputMappingValue, (int)inputMode];
                                                                        for (var i = inputCount; i > 1; --i)
                                                                        {
                                                                            var limiterPosition0 = defaultComputer.GetPosition(defaultPaintValues[i]);
                                                                            targetSession.DrawLine(limiterPosition0, limiterPosition1, limiterPosition0, limiterHeight, limiterColor, limiterLength);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Configure.Instance.AutoableLimiterVariety)
                                                                        {
                                                                            var autoableInputs = Component.AutoableInputs[(int)inputMode];
                                                                            for (var i = inputCount; i > 0; --i)
                                                                            {
                                                                                if (Array.IndexOf(autoableInputs, i) != -1)
                                                                                {
                                                                                    var limiterPosition0 = defaultComputer.GetPosition(i);
                                                                                    targetSession.DrawLine(limiterPosition0, limiterPosition1, limiterPosition0, limiterHeight, limiterColor, limiterLength);
                                                                                    limiterPosition0 += drawingNoteLengthMap[i];
                                                                                    targetSession.DrawLine(limiterPosition0, limiterPosition1, limiterPosition0, limiterHeight, limiterColor, limiterLength);
                                                                                }
                                                                            }
                                                                        }
                                                                        if (Configure.Instance.CenterLimiterVariety)
                                                                        {
                                                                            var limiterCenterValues = Component.LimiterCenterMap[inputMappingValue, (int)inputMode, has2P ? 1 : 0];
                                                                            for (var i = inputCount; i > 0; --i)
                                                                            {
                                                                                for (var j = limiterCenterValues[i] - 1; j >= 0; --j)
                                                                                {
                                                                                    var limiterPosition0 = defaultComputer.GetPosition(i) + j * drawingNoteLengthMap[i];
                                                                                    targetSession.DrawLine(limiterPosition0, limiterPosition1, limiterPosition0, limiterHeight, limiterColor, limiterLength);
                                                                                }
                                                                            }
                                                                        }
                                                                        if (Configure.Instance.Limiter57Variety && (inputMode == Component.InputMode.InputMode242 || inputMode == Component.InputMode.InputMode484))
                                                                        {
                                                                            var limiter57Values = Component.Limiter57Map[(int)inputMode];
                                                                            for (var i = inputCount; i > 0; --i)
                                                                            {
                                                                                if (limiter57Values[i])
                                                                                {
                                                                                    var limiterPosition0 = defaultComputer.GetPosition(i);
                                                                                    targetSession.DrawLine(limiterPosition0, limiterPosition1, limiterPosition0, limiterHeight, limiterColor, limiterLength);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentVisualizer:
                                                                    var judgmentVisualizerPosition0 = drawingComponentValue.judgmentVisualizerPosition0;
                                                                    var judgmentVisualizerPosition1 = drawingComponentValue.judgmentVisualizerPosition1;
                                                                    var judgmentVisualizerLength = drawingComponentValue.judgmentVisualizerLength;
                                                                    var judgmentVisualizerHeight = drawingComponentValue.judgmentVisualizerHeight;
                                                                    var judgmentVisualizerContentsLength = drawingComponentValue.judgmentVisualizerContentsLength;
                                                                    var judgmentVisualizerContentsHeight = drawingComponentValue.judgmentVisualizerContentsHeight;
                                                                    var judgmentVisualizerSystem = drawingComponentValue.judgmentVisualizerSystem;
                                                                    var altJudgmentVisualizer = drawingComponentValue.altJudgmentVisualizer;
                                                                    var targetJudgmentVisualizerValues = defaultComputer.JudgmentVisualizerValues;
                                                                    for (var i = altJudgmentVisualizer >> 1; i >= altJudgmentVisualizer % 2; --i)
                                                                    {
                                                                        var distanceJudgmentVisualizer = i == 1 && has2P ? distance2P : 0F;
                                                                        var judgmentVisualizerPosition0Float = judgmentVisualizerPosition0 + distanceJudgmentVisualizer;
                                                                        var judgmentVisualizerValues = targetJudgmentVisualizerValues[i];
                                                                        lock (judgmentVisualizerValues)
                                                                        {
                                                                            foreach (var judgmentVisualizerValue in judgmentVisualizerValues)
                                                                            {
                                                                                var judgmentColorPaint = d2dJudgmentPaints[(int)judgmentVisualizerValue.Judged][(int)(100 * judgmentVisualizerValue.Status)];
                                                                                switch (judgmentVisualizerSystem)
                                                                                {
                                                                                    case 0:
                                                                                        var judgmentVisualizerPosition1Value = (float)(judgmentVisualizerPosition1 + judgmentVisualizerHeight * (1 - judgmentVisualizerValue.Judgment));
                                                                                        targetSession.DrawLine(judgmentVisualizerPosition0Float, judgmentVisualizerPosition1Value, judgmentVisualizerPosition0Float + judgmentVisualizerContentsLength, judgmentVisualizerPosition1Value, judgmentColorPaint, judgmentVisualizerContentsHeight);
                                                                                        break;
                                                                                    case 1:
                                                                                        judgmentVisualizerPosition1Value = (float)(judgmentVisualizerPosition1 + judgmentVisualizerHeight * judgmentVisualizerValue.Judgment);
                                                                                        targetSession.DrawLine(judgmentVisualizerPosition0Float, judgmentVisualizerPosition1Value, judgmentVisualizerPosition0Float + judgmentVisualizerContentsLength, judgmentVisualizerPosition1Value, judgmentColorPaint, judgmentVisualizerContentsHeight);
                                                                                        break;
                                                                                    case 2:
                                                                                        var judgmentVisualizerPosition0Value = (float)(judgmentVisualizerPosition0Float + judgmentVisualizerLength * (1 - judgmentVisualizerValue.Judgment));
                                                                                        targetSession.DrawLine(judgmentVisualizerPosition0Value, judgmentVisualizerPosition1, judgmentVisualizerPosition0Value, judgmentVisualizerPosition1 + judgmentVisualizerContentsHeight, judgmentColorPaint, judgmentVisualizerContentsLength);
                                                                                        break;
                                                                                    case 3:
                                                                                        judgmentVisualizerPosition0Value = (float)(judgmentVisualizerPosition0Float + judgmentVisualizerLength * judgmentVisualizerValue.Judgment);
                                                                                        targetSession.DrawLine(judgmentVisualizerPosition0Value, judgmentVisualizerPosition1, judgmentVisualizerPosition0Value, judgmentVisualizerPosition1 + judgmentVisualizerContentsHeight, judgmentColorPaint, judgmentVisualizerContentsLength);
                                                                                        break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.InputVisualizer:
                                                                    PaintInt(defaultComputer.InputCountQueue.Count, drawingComponentValue.inputVisualizerSystem, drawingComponentValue.inputVisualizerPosition0, drawingComponentValue.inputVisualizerPosition1, drawingComponentValue.binInputVisualizerLength, drawingComponentValue.binInputVisualizerHeight, drawingComponentValue.altInputVisualizer, UI.Instance.BinInputVisualizerMap);
                                                                    break;
                                                                case PaintPipelineID.Hunter:
                                                                    var hunter = defaultComputer.Hunter.Value;
                                                                    if (hunter.HasValue)
                                                                    {
                                                                        var hunterPosition0 = drawingComponentValue.hunterPosition0;
                                                                        var hunterPosition1 = drawingComponentValue.hunterPosition1;
                                                                        var binHunterLength = drawingComponentValue.binHunterLength;
                                                                        var binHunterHeight = drawingComponentValue.binHunterHeight;
                                                                        var hunterFrontDrawingLength = drawingComponentValue.hunterFrontDrawingLength;
                                                                        var hunterSystem = drawingComponentValue.hunterSystem;
                                                                        var altHunter = drawingComponentValue.altHunter;
                                                                        var binHunterMap = UI.Instance.BinHunterMap;
                                                                        var drawingHunterHigher = UI.Instance.HunterHigherDrawing;
                                                                        var drawingHunterLower = UI.Instance.HunterLowerDrawing;
                                                                        var hunterValue = hunter.Value;
                                                                        var absHunter = Math.Abs(hunterValue);
                                                                        var hunterDigit = QwilightComponent.GetDigit(absHunter);
                                                                        for (var i = altHunter >> 1; i >= altHunter % 2; --i)
                                                                        {
                                                                            var distanceHunter = i == 1 && has2P ? distance2P : 0F;
                                                                            switch (hunterSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(hunterPosition0 - binHunterLength + distanceHunter, hunterPosition1, hunterFrontDrawingLength, binHunterHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(hunterPosition0 - 0.5 * (binHunterLength * hunterDigit + hunterFrontDrawingLength) + distanceHunter, hunterPosition1, hunterFrontDrawingLength, binHunterHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(hunterPosition0 - binHunterLength * hunterDigit - hunterFrontDrawingLength + distanceHunter, hunterPosition1, hunterFrontDrawingLength, binHunterHeight);
                                                                                    break;
                                                                            }
                                                                            if (absHunter > 0)
                                                                            {
                                                                                targetSession.PaintDrawing(ref r, hunterValue > 0 ? drawingHunterHigher : drawingHunterLower);
                                                                            }
                                                                            r.Position0 += hunterFrontDrawingLength;
                                                                            r.Length = binHunterLength;
                                                                            for (var j = hunterDigit - 1; j >= 0; --j)
                                                                            {
                                                                                targetSession.PaintDrawing(ref r, binHunterMap[(int)(absHunter / Math.Pow(10, j) % 10)]);
                                                                                r.Position0 += binHunterLength;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Title:
                                                                    var title = defaultComputer.Title;
                                                                    var titleFont = UI.Instance.TitleFont;
                                                                    var titleColor = UI.Instance.TitleColor;
                                                                    var titlePosition0 = drawingComponentValue.titlePosition0;
                                                                    var titlePosition1 = drawingComponentValue.titlePosition1;
                                                                    var titleLength = drawingComponentValue.titleLength;
                                                                    var titleHeight = drawingComponentValue.titleHeight;
                                                                    var altTitle = drawingComponentValue.altTitle;
                                                                    for (var i = altTitle >> 1; i >= altTitle % 2; --i)
                                                                    {
                                                                        var distanceTitle = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(titlePosition0 + distanceTitle, titlePosition1, titleLength, titleHeight);
                                                                        targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(title, titleFont, (float)r.Length, (float)r.Height), ref r, titleColor);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Artist:
                                                                    var artist = defaultComputer.Artist;
                                                                    var artistFont = UI.Instance.ArtistFont;
                                                                    var artistColor = UI.Instance.ArtistColor;
                                                                    var artistPosition0 = drawingComponentValue.artistPosition0;
                                                                    var artistPosition1 = drawingComponentValue.artistPosition1;
                                                                    var artistLength = drawingComponentValue.artistLength;
                                                                    var artistHeight = drawingComponentValue.artistHeight;
                                                                    var altArtist = drawingComponentValue.altArtist;
                                                                    for (var i = altArtist >> 1; i >= altArtist % 2; --i)
                                                                    {
                                                                        var distanceArtist = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(artistPosition0 + distanceArtist, artistPosition1, artistLength, artistHeight);
                                                                        targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(artist, artistFont, (float)r.Length, (float)r.Height), ref r, artistColor);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.MediaInput:
                                                                    var mediaInputFaint = Configure.Instance.MediaInputFaint;
                                                                    if (mediaInputFaint > 0.0)
                                                                    {
                                                                        r.Set(Configure.Instance.MediaInputPosition0, Configure.Instance.MediaInputPosition1, Configure.Instance.MediaInputLength, Configure.Instance.MediaInputHeight);
                                                                        MediaInputSystem.Instance.PaintMediaInput(targetSession, ref r, (float)mediaInputFaint);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.VeilDrawing:
                                                                    var veilDrawing = VeilDrawing.Drawing;
                                                                    if (veilDrawing != null)
                                                                    {
                                                                        var veilDrawingValue = veilDrawing.Value;
                                                                        var veilDrawingBound = veilDrawingValue.DrawingBound;
                                                                        var veilDrawingHeight = veilDrawingBound.Height * p1Length / veilDrawingBound.Length;
                                                                        r.Set(mainPosition, Configure.Instance.VeilDrawingHeight - veilDrawingHeight, p1Length, veilDrawingHeight);
                                                                        targetSession.PaintDrawing(ref r, veilDrawing);
                                                                        if (has2P)
                                                                        {
                                                                            r.Position0 += distance2P;
                                                                            targetSession.PaintDrawing(ref r, veilDrawing);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Genre:
                                                                    var genre = defaultComputer.GenreText;
                                                                    var genreFont = UI.Instance.GenreFont;
                                                                    var genreColor = UI.Instance.GenreColor;
                                                                    var genrePosition0 = drawingComponentValue.genrePosition0;
                                                                    var genrePosition1 = drawingComponentValue.genrePosition1;
                                                                    var genreLength = drawingComponentValue.genreLength;
                                                                    var genreHeight = drawingComponentValue.genreHeight;
                                                                    var altGenre = drawingComponentValue.altGenre;
                                                                    for (var i = altGenre >> 1; i >= altGenre % 2; --i)
                                                                    {
                                                                        var distanceGenre = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(genrePosition0 + distanceGenre, genrePosition1, genreLength, genreHeight);
                                                                        targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(genre, genreFont, (float)r.Length, (float)r.Height), ref r, genreColor);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.LevelText:
                                                                    var levelText = defaultComputer.LevelText;
                                                                    var levelTextFont = UI.Instance.LevelTextFont;
                                                                    var levelTextColor = BaseUI.Instance.D2DLevelColors[(int)defaultComputer.LevelValue];
                                                                    var levelTextPosition0 = drawingComponentValue.levelTextPosition0;
                                                                    var levelTextPosition1 = drawingComponentValue.levelTextPosition1;
                                                                    var levelTextLength = drawingComponentValue.levelTextLength;
                                                                    var levelTextHeight = drawingComponentValue.levelTextHeight;
                                                                    var altLevelText = drawingComponentValue.altLevelText;
                                                                    for (var i = altLevelText >> 1; i >= altLevelText % 2; --i)
                                                                    {
                                                                        var distanceLevelText = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(levelTextPosition0 + distanceLevelText, levelTextPosition1, levelTextLength, levelTextHeight);
                                                                        targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(levelText, levelTextFont, (float)r.Length, (float)r.Height), ref r, levelTextColor);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.WantLevel:
                                                                    var wantLevelID = defaultComputer.NoteFile.WantLevelID;
                                                                    var wantLevelFont = UI.Instance.WantLevelFont;
                                                                    var wantLevelIDColor = UI.Instance.WantLevelIDColor;
                                                                    var wantLevelPosition0 = drawingComponentValue.wantLevelPosition0;
                                                                    var wantLevelPosition1 = drawingComponentValue.wantLevelPosition1;
                                                                    var wantLevelLength = drawingComponentValue.wantLevelLength;
                                                                    var wantLevelHeight = drawingComponentValue.wantLevelHeight;
                                                                    var altWantLevel = drawingComponentValue.altWantLevel;
                                                                    for (var i = altWantLevel >> 1; i >= altWantLevel % 2; --i)
                                                                    {
                                                                        var distanceWantLevel = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(wantLevelPosition0 + distanceWantLevel, wantLevelPosition1, wantLevelLength, wantLevelHeight);
                                                                        targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(wantLevelID, wantLevelFont, (float)r.Length, (float)r.Height), ref r, wantLevelIDColor);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.Level:
                                                                    var levelDrawings = UI.Instance.LevelDrawings[(int)defaultComputer.LevelValue];
                                                                    var levelPosition0 = drawingComponentValue.levelPosition0;
                                                                    var levelPosition1 = drawingComponentValue.levelPosition1;
                                                                    var levelLength = drawingComponentValue.levelLength;
                                                                    var levelHeight = drawingComponentValue.levelHeight;
                                                                    var altLevel = drawingComponentValue.altLevel;
                                                                    var levelFrame = defaultComputer.LevelFrame;
                                                                    for (var i = altLevel >> 1; i >= altLevel % 2; --i)
                                                                    {
                                                                        var distanceLevel = i == 1 && has2P ? distance2P : 0F;
                                                                        r.Set(levelPosition0 + distanceLevel, levelPosition1, levelLength, levelHeight);
                                                                        targetSession.PaintDrawing(ref r, levelDrawings[levelFrame]);
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.AutoMain:
                                                                    if (isAutoMode)
                                                                    {
                                                                        var autoMainDrawings = UI.Instance.AutoMainDrawings;
                                                                        var autoMainPosition0 = drawingComponentValue.autoMainPosition0;
                                                                        var autoMainPosition1 = drawingComponentValue.autoMainPosition1;
                                                                        var autoMainLength = drawingComponentValue.autoMainLength;
                                                                        var autoMainHeight = drawingComponentValue.autoMainHeight;
                                                                        var autoMainSystem = drawingComponentValue.autoMainSystem;
                                                                        var altAutoMain = drawingComponentValue.altAutoMain;
                                                                        var autoMainFrame = defaultComputer.AutoMainFrame;
                                                                        for (var i = altAutoMain >> 1; i >= altAutoMain % 2; --i)
                                                                        {
                                                                            var distanceAutoMain = i == 1 && has2P ? distance2P : 0F;
                                                                            switch (autoMainSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(autoMainPosition0 + distanceAutoMain, autoMainPosition1, autoMainLength, autoMainHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(autoMainPosition0 - autoMainLength / 2 + distanceAutoMain, autoMainPosition1, autoMainLength, autoMainHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(autoMainPosition0 - autoMainLength + distanceAutoMain, autoMainPosition1, autoMainLength, autoMainHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, autoMainDrawings[autoMainFrame]);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.EarlyValue:
                                                                    PaintInt(defaultComputer.EarlyValue, drawingComponentValue.earlyValueSystem, drawingComponentValue.earlyValuePosition0, drawingComponentValue.earlyValuePosition1, drawingComponentValue.binEarlyValueLength, drawingComponentValue.binEarlyValueHeight, drawingComponentValue.altEarlyValue, UI.Instance.BinEarlyValueMap);
                                                                    break;
                                                                case PaintPipelineID.LateValue:
                                                                    PaintInt(defaultComputer.LateValue, drawingComponentValue.lateValueSystem, drawingComponentValue.lateValuePosition0, drawingComponentValue.lateValuePosition1, drawingComponentValue.binLateValueLength, drawingComponentValue.binLateValueHeight, drawingComponentValue.altLateValue, UI.Instance.BinLateValueMap);
                                                                    break;
                                                                case PaintPipelineID.JudgmentVSVisualizer:
                                                                    var vsJudgment = judgments[(int)Component.Judged.Higher] + judgments[(int)Component.Judged.High] + judgments[(int)Component.Judged.Low] + judgments[(int)Component.Judged.Lower] + judgments[(int)Component.Judged.Lowest];
                                                                    if (vsJudgment > 0)
                                                                    {
                                                                        var judgmentVSVisualizerInt = (int)(100 * judgments[(int)Component.Judged.Highest] / vsJudgment);
                                                                        var judgmentVSVisualizerStopPointDrawingLength = drawingComponentValue.judgmentVSVisualizerStopPointDrawingLength;
                                                                        var judgmentVSVisualizerPosition0 = drawingComponentValue.judgmentVSVisualizerPosition0;
                                                                        var judgmentVSVisualizerPosition1 = drawingComponentValue.judgmentVSVisualizerPosition1;
                                                                        var binJudgmentVSVisualizerLength = drawingComponentValue.binJudgmentVSVisualizerLength;
                                                                        var binJudgmentVSVisualizerHeight = drawingComponentValue.binJudgmentVSVisualizerHeight;
                                                                        var judgmentVSVisualizerSystem = drawingComponentValue.judgmentVSVisualizerSystem;
                                                                        var altJudgmentVSVisualizer = drawingComponentValue.altJudgmentVSVisualizer;
                                                                        var binJudgmentVSVisualizerMap = UI.Instance.BinJudgmentVSVisualizerMap;
                                                                        var judgmentVSVisualizerStopPointDrawing = UI.Instance.JudgmentVSVisualizerStopPointDrawing;
                                                                        var judgmentVSVisualizerDigit = judgmentVSVisualizerInt < 100 ? 3 : QwilightComponent.GetDigit(judgmentVSVisualizerInt);
                                                                        for (var i = altJudgmentVSVisualizer >> 1; i >= altJudgmentVSVisualizer % 2; --i)
                                                                        {
                                                                            var distancePoint = i == 1 && has2P ? distance2P : 0F;
                                                                            for (var j = judgmentVSVisualizerDigit - 1; j >= 2; --j)
                                                                            {
                                                                                switch (judgmentVSVisualizerSystem)
                                                                                {
                                                                                    case 0:
                                                                                        r.Set(judgmentVSVisualizerPosition0 + binJudgmentVSVisualizerLength * (judgmentVSVisualizerDigit - j - 1) + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                    case 1:
                                                                                        r.Set(judgmentVSVisualizerPosition0 + binJudgmentVSVisualizerLength * (0.5 * judgmentVSVisualizerDigit - j - 1) - judgmentVSVisualizerStopPointDrawingLength * 0.5 + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                    case 2:
                                                                                        r.Set(judgmentVSVisualizerPosition0 - binJudgmentVSVisualizerLength * (j + 1) - judgmentVSVisualizerStopPointDrawingLength + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                }
                                                                                targetSession.PaintDrawing(ref r, binJudgmentVSVisualizerMap[(int)(judgmentVSVisualizerInt / Math.Pow(10, j) % 10)]);
                                                                            }
                                                                            r.Position0 += binJudgmentVSVisualizerLength;
                                                                            r.Length = judgmentVSVisualizerStopPointDrawingLength;
                                                                            targetSession.PaintDrawing(ref r, judgmentVSVisualizerStopPointDrawing);
                                                                            for (var j = 1; j >= 0; --j)
                                                                            {
                                                                                switch (judgmentVSVisualizerSystem)
                                                                                {
                                                                                    case 0:
                                                                                        r.Set(judgmentVSVisualizerPosition0 + binJudgmentVSVisualizerLength * (judgmentVSVisualizerDigit - j - 1) + judgmentVSVisualizerStopPointDrawingLength + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                    case 1:
                                                                                        r.Set(judgmentVSVisualizerPosition0 + binJudgmentVSVisualizerLength * (0.5 * judgmentVSVisualizerDigit - j - 1) + 0.5 * judgmentVSVisualizerStopPointDrawingLength + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                    case 2:
                                                                                        r.Set(judgmentVSVisualizerPosition0 - binJudgmentVSVisualizerLength * (j + 1) + distancePoint, judgmentVSVisualizerPosition1, binJudgmentVSVisualizerLength, binJudgmentVSVisualizerHeight);
                                                                                        break;
                                                                                }
                                                                                targetSession.PaintDrawing(ref r, binJudgmentVSVisualizerMap[(int)(judgmentVSVisualizerInt / Math.Pow(10, j) % 10)]);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.MainJudgmentMeter:
                                                                    var mainJudgmentMeterDrawings = UI.Instance.MainJudgmentMeterDrawings[(int)inputMode];
                                                                    var mainJudgmentMeterFrames = defaultComputer.MainJudgmentMeterFrames;
                                                                    var mainJudgmentMeterPosition1s = drawingComponentValue.mainJudgmentMeterPosition1s;
                                                                    var mainJudgmentMeterHeights = drawingComponentValue.mainJudgmentMeterHeights;
                                                                    foreach (var drawingPipeline in drawingPipelines)
                                                                    {
                                                                        for (var i = inputCount; i > 0; --i)
                                                                        {
                                                                            if (drawingPipeline == drawingInputModeMap[i])
                                                                            {
                                                                                r.Set(defaultComputer.GetPosition(i), mainJudgmentMeterPosition1s[i], drawingNoteLengthMap[i], mainJudgmentMeterHeights[i]);
                                                                                targetSession.PaintDrawing(ref r, mainJudgmentMeterDrawings[i].GetValueOrDefault(mainJudgmentMeterFrames[i]));
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.StatusSlider:
                                                                    var statusSliderPosition0 = drawingComponentValue.statusSliderPosition0;
                                                                    var statusSliderPosition1 = drawingComponentValue.statusSliderPosition1;
                                                                    var statusSliderLength = drawingComponentValue.statusSliderLength;
                                                                    var statusSliderHeight = drawingComponentValue.statusSliderHeight;
                                                                    var statusSliderContentsLength = drawingComponentValue.statusSliderContentsLength;
                                                                    var statusSliderContentsHeight = drawingComponentValue.statusSliderContentsHeight;
                                                                    var statusSliderSystem = drawingComponentValue.statusSystem;
                                                                    var altStatusSlider = drawingComponentValue.altStatusSlider;
                                                                    var statusSliderDrawing = UI.Instance.StatusSliderDrawing;
                                                                    if (statusSliderDrawing.HasValue)
                                                                    {
                                                                        var statusSliderDrawingValue = statusSliderDrawing.Value;
                                                                        var statusSliderDrawingBound = statusSliderDrawingValue.DrawingBound;
                                                                        var statusSliderDrawingLength = statusSliderDrawingBound.Length;
                                                                        var statusSliderDrawingHeight = statusSliderDrawingBound.Height;
                                                                        for (var i = altStatusSlider >> 1; i >= altStatusSlider % 2; --i)
                                                                        {
                                                                            var distanceStatusSlider = i == 1 && has2P ? distance2P : 0F;
                                                                            switch (statusSliderSystem)
                                                                            {
                                                                                case 0:
                                                                                    r.Set(statusSliderPosition0 + distanceStatusSlider, statusSliderPosition1 + statusSliderHeight * (1 - status) - statusSliderContentsHeight / 2, statusSliderContentsLength, statusSliderContentsHeight);
                                                                                    break;
                                                                                case 1:
                                                                                    r.Set(statusSliderPosition0 + distanceStatusSlider, statusSliderPosition1 + statusSliderHeight * status - statusSliderContentsHeight / 2, statusSliderContentsLength, statusSliderContentsHeight);
                                                                                    break;
                                                                                case 2:
                                                                                    r.Set(statusSliderPosition0 + distanceStatusSlider + statusSliderLength * (1 - status) - statusSliderContentsLength / 2, statusSliderPosition1, statusSliderContentsLength, statusSliderContentsHeight);
                                                                                    break;
                                                                                case 3:
                                                                                    r.Set(statusSliderPosition0 + distanceStatusSlider + statusSliderLength * status - statusSliderContentsLength / 2, statusSliderPosition1, statusSliderContentsLength, statusSliderContentsHeight);
                                                                                    break;
                                                                            }
                                                                            targetSession.PaintDrawing(ref r, statusSliderDrawingValue);
                                                                        }
                                                                    }
                                                                    break;
                                                                case PaintPipelineID.JudgmentInputVisualizer:
                                                                    var judgmentInputVisualizerPosition0 = drawingComponentValue.judgmentInputVisualizerPosition0;
                                                                    var judgmentInputVisualizerPosition1 = drawingComponentValue.judgmentInputVisualizerPosition1;
                                                                    var judgmentInputVisualizerLength = drawingComponentValue.judgmentInputVisualizerLength;
                                                                    var judgmentInputVisualizerHeight = drawingComponentValue.judgmentInputVisualizerHeight;
                                                                    var altJudgmentInputVisualizer = drawingComponentValue.altJudgmentInputVisualizer;
                                                                    var judgmentInputValues = defaultComputer.JudgmentInputValues;
                                                                    var targetJudgmentInputVisualizerLength = judgmentInputVisualizerLength / 100;
                                                                    var judgmentInputDrawings = UI.Instance.JudgmentInputDrawings;
                                                                    for (var i = altJudgmentInputVisualizer >> 1; i >= altJudgmentInputVisualizer % 2; --i)
                                                                    {
                                                                        var distanceJudgmentInputVisualizer = i == 1 && has2P ? distance2P : 0F;
                                                                        var judgmentInputVisualizerPosition0Float = judgmentInputVisualizerPosition0 + distanceJudgmentInputVisualizer;
                                                                        var judgmentInputValuesLength = judgmentInputValues.Length;
                                                                        for (var j = 0; j < judgmentInputValuesLength; ++j)
                                                                        {
                                                                            var lastJudgmentInputValue = 1.0;
                                                                            var judgmentInputValue = judgmentInputValues[j];
                                                                            r.Set(judgmentInputVisualizerPosition0 + j * targetJudgmentInputVisualizerLength, judgmentInputVisualizerPosition1 + judgmentInputVisualizerHeight, targetJudgmentInputVisualizerLength, 0.0);
                                                                            for (var m = judgmentInputValue.Length - 1; m >= 0; --m)
                                                                            {
                                                                                var value = judgmentInputValue[m];
                                                                                lastJudgmentInputValue -= value;
                                                                                r.Height = judgmentInputVisualizerHeight * value;
                                                                                if (r.Height > 0.0)
                                                                                {
                                                                                    var judgmentInputDrawing = judgmentInputDrawings[m];
                                                                                    if (judgmentInputDrawing.HasValue)
                                                                                    {
                                                                                        var judgmentInputDrawingBound = judgmentInputDrawing.Value.DrawingBound;
                                                                                        var judgmentInputDrawingHeight = judgmentInputDrawingBound.Height;
                                                                                        r.Position1 -= r.Height;
                                                                                        s.Set(0.0, lastJudgmentInputValue * judgmentInputDrawingHeight, judgmentInputDrawingBound.Length, judgmentInputDrawingHeight * value);
                                                                                        targetSession.PaintDrawing(ref r, ref s, judgmentInputDrawing);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                            }

                                                            void PaintInt(int value, int system, double position0, double position1, double length, double height, int alt, DrawingItem?[] drawingItems)
                                                            {
                                                                var digit = QwilightComponent.GetDigit(value);
                                                                for (var i = alt >> 1; i >= alt % 2; --i)
                                                                {
                                                                    var distance = i == 1 && has2P ? distance2P : 0F;
                                                                    for (var j = digit - 1; j >= 0; --j)
                                                                    {
                                                                        var drawingItem = drawingItems[(int)(value / Math.Pow(10, j) % 10)];
                                                                        switch (system)
                                                                        {
                                                                            case 0:
                                                                                r.Set(position0 + length * (digit - j - 1) + distance, position1, length, height);
                                                                                break;
                                                                            case 1:
                                                                                r.Set(position0 + length * (0.5 * digit - j - 1) + distance, position1, length, height);
                                                                                break;
                                                                            case 2:
                                                                                r.Set(position0 - length * (j + 1) + distance, position1, length, height);
                                                                                break;
                                                                        }
                                                                        targetSession.PaintDrawing(ref r, drawingItem);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (!isValidNetDrawings)
                                                {
                                                    defaultComputer.IsValidNetDrawings = true;
                                                }

                                                var assistTextPosition1 = drawingComponentValue.assistTextPosition1;
                                                if (defaultComputer.CanUndo && defaultComputer.IsPausingWindowOpened)
                                                {
                                                    assistTextPosition1 += PaintAssistText(assistTextPosition1, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[3].ToString(), InputAssistFont), 100, FaintClearedPaints, FaintFilledPaints, null, 0, null, null, PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.UndoContents, InputAssistFont), 100);
                                                }
                                                else if (defaultComputer.IsPassable)
                                                {
                                                    assistTextPosition1 += PaintAssistText(assistTextPosition1, PoolSystem.Instance.GetTextItem("ENTER", InputAssistFont), 100, FaintClearedPaints, FaintFilledPaints, null, 0, null, null, PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.PassContents, InputAssistFont), 100);
                                                }

                                                var assistTextFaint = loopingCounter < 0.0 ? (int)(100 * Math.Sqrt(-loopingCounter / Component.LevyingWait)) : 0;
                                                if (assistTextFaint > 0)
                                                {
                                                    assistTextPosition1 += PaintAssistText(assistTextPosition1, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.LowerMultiplier].ToString(), InputAssistFont), assistTextFaint, FaintClearedPaints, FaintFilledPaints, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HigherMultiplier].ToString(), InputAssistFont), assistTextFaint, FaintClearedPaints, FaintFilledPaints, PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.MultiplierContents, InputAssistFont), assistTextFaint);

                                                    if (defaultComputer.LoadedMedia)
                                                    {
                                                        assistTextPosition1 += PaintAssistText(assistTextPosition1, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.Media].ToString(), InputAssistFont), assistTextFaint, FaintClearedPaints, FaintFilledPaints, null, assistTextFaint, FaintClearedPaints, FaintFilledPaints, PoolSystem.Instance.GetTextItem(Configure.Instance.Media ? "BGA ON" : "BGA OFF", InputAssistFont), assistTextFaint);
                                                    }

                                                    if (isItemMode)
                                                    {
                                                        assistTextPosition1 += PaintAssistText(assistTextPosition1, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem0].ToString(), InputAssistFont), assistTextFaint, FaintClearedPaints, FaintFilledPaints, PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem1].ToString(), InputAssistFont), assistTextFaint, FaintClearedPaints, FaintFilledPaints, PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.PostItemContents, InputAssistFont), assistTextFaint);
                                                    }
                                                }

                                                if (defaultComputer.IsPostableItemMode)
                                                {
                                                    var lastPostableItems = defaultComputer.LastPostableItems;
                                                    if (lastPostableItems != null)
                                                    {
                                                        var assistFaint0 = (int)(100 * defaultComputer.PostableItemFaints[0].Value);
                                                        var assistFaint1 = (int)(100 * defaultComputer.PostableItemFaints[1].Value);
                                                        var assistTextItem0 = assistFaint0 > 0.0 ? PoolSystem.Instance.GetTextItem(lastPostableItems[0]?.ToString(), InputAssistFont) : null;
                                                        var assistTextItem1 = assistFaint1 > 0.0 ? PoolSystem.Instance.GetTextItem(lastPostableItems[1]?.ToString(), InputAssistFont) : null;
                                                        assistTextPosition1 += PaintAssistText(assistTextPosition1, assistTextItem0, assistFaint0, lastPostableItems[0]?.ItemPaints, FaintClearedPaints, assistTextItem1, assistFaint1, lastPostableItems[1]?.ItemPaints, FaintClearedPaints, null, 0);
                                                    }

                                                    var postedItemVariety = defaultComputer.PostedItemVariety;
                                                    var postedItemFaint = (int)(100 * defaultComputer.PostedItemFaints[postedItemVariety]);
                                                    if (postedItemFaint > 0.0)
                                                    {
                                                        var postedItemTextItem = PoolSystem.Instance.GetTextItem(defaultComputer.PostedItemText, InputAssistFont);
                                                        var postedItemTextBound = postedItemTextItem.LayoutBounds;
                                                        var postedItemTextBoundLength = postedItemTextBound.Width;
                                                        var postedItemTextBoundHeight = postedItemTextBound.Height;
                                                        r.Set(mainPosition + (p1Length - postedItemTextBoundLength) / 2, assistTextPosition1 - postedItemTextBoundHeight / 2, postedItemTextBoundLength, postedItemTextBoundHeight);
                                                        targetSession.PaintVisibleText(postedItemTextItem, ref r, FaintItemPaints[postedItemVariety][postedItemFaint], FaintFilledPaints[postedItemFaint]);
                                                        assistTextPosition1 += (float)(Levels.StandardMargin + Utility.Max(postedItemTextBoundHeight, postedItemTextBoundLength, postedItemTextBoundHeight));
                                                    }

                                                    var valueItemPosition0 = 0F;
                                                    foreach (var (valueItem, valueItemStatus) in defaultComputer.PostableItemStatusMap)
                                                    {
                                                        if (valueItemStatus.IsHandling)
                                                        {
                                                            var valueItemTextBoundHeight = 24F;
                                                            var valueItemTextItem = PoolSystem.Instance.GetTextItem(PoolSystem.Instance.GetFormattedText("{0} ({1})", valueItem.ToString(), valueItemStatus.AvatarName), NotifyXamlFont, 0F, valueItemTextBoundHeight);
                                                            var valueItemTextBound = valueItemTextItem.LayoutBounds;
                                                            var valueItemHeight = Levels.StandardMarginFloat32 + valueItemTextBoundHeight + Levels.StandardMarginFloat32;

                                                            valueItemPosition0 += Levels.StandardMarginFloat32;
                                                            var valueItemLength = Levels.StandardMarginFloat32 + valueItemTextBound.Width + Levels.StandardMarginFloat32;
                                                            r.Set(valueItemPosition0, Levels.StandardMarginFloat32, Levels.StandardEdgeFloat32 + valueItemLength + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + valueItemHeight + Levels.StandardEdgeFloat32);
                                                            targetSession.DrawRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, Colors.White);

                                                            r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, valueItemLength, r.Height - 2 * Levels.StandardEdgeFloat32);
                                                            targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, FaintFilledPaints[50]);

                                                            r.Length *= valueItemStatus.Wait / valueItemStatus.TotalWait;
                                                            targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, valueItem.ItemColor);

                                                            r.SetPosition(r.Position0 + Levels.StandardMarginFloat32, r.Position1 + Levels.StandardMarginFloat32);
                                                            targetSession.PaintText(valueItemTextItem, ref r, Colors.White);
                                                            valueItemPosition0 += (float)(Levels.StandardEdgeFloat32 + valueItemLength + Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32);
                                                        }
                                                    }
                                                }

                                                float PaintAssistText(float assistTextPosition1, CanvasTextLayout assistTextItem0, int assistFaint0, ICanvasBrush[] assistTextItem0Paints, ICanvasBrush[] assistTextItem0TextPaints, CanvasTextLayout assistTextItem1, int assistFaint1, ICanvasBrush[] assistTextItem1Paints, ICanvasBrush[] assistTextItem1TextPaints, CanvasTextLayout assistTextItem2, int assistFaint2)
                                                {
                                                    var assistTextBound0 = assistTextItem0?.LayoutBounds;
                                                    var assistTextBound0Length = assistTextBound0.HasValue ? Levels.StandardMargin + assistTextBound0.Value.Width + Levels.StandardMargin : 0F;
                                                    var assistTextBound0Height = assistTextBound0.HasValue ? Levels.StandardMargin + assistTextBound0.Value.Height + Levels.StandardMargin : 0F;
                                                    var assistTextBound1 = assistTextItem1?.LayoutBounds;
                                                    var assistTextBound1Length = assistTextBound1.HasValue ? Levels.StandardMargin + assistTextBound1.Value.Width + Levels.StandardMargin : 0F;
                                                    var assistTextBound1Height = assistTextBound1.HasValue ? Levels.StandardMargin + assistTextBound1.Value.Height + Levels.StandardMargin : 0F;
                                                    var assistTextBound2 = assistTextItem2?.LayoutBounds;
                                                    var assistTextBound2Length = assistTextBound2.HasValue ? assistTextBound2.Value.Width : 0.0;
                                                    var assistTextBound2Height = assistTextBound2.HasValue ? assistTextBound2.Value.Height : 0.0;

                                                    var assistTextPosition0 = mainPosition + (p1Length - assistTextBound0Length - Levels.StandardMarginFloat32 - assistTextBound1Length - Levels.StandardMarginFloat32 - assistTextBound2Length) / 2;
                                                    if (assistTextItem0 != null)
                                                    {
                                                        r.Set(assistTextPosition0, assistTextPosition1 - assistTextBound0Height / 2, assistTextBound0Length, assistTextBound0Height);
                                                        targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, assistTextItem0Paints[assistFaint0]);
                                                        r.Position0 += Levels.StandardMargin;
                                                        r.Position1 += Levels.StandardMargin;
                                                        targetSession.PaintText(assistTextItem0, ref r, assistTextItem0TextPaints[assistFaint0]);

                                                        assistTextPosition0 += Levels.StandardMarginFloat32 + assistTextBound0Length;
                                                    }

                                                    if (assistTextItem1 != null)
                                                    {
                                                        r.Set(assistTextPosition0, assistTextPosition1 - assistTextBound1Height / 2, assistTextBound1Length, assistTextBound1Height);
                                                        targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, assistTextItem1Paints[assistFaint1]);
                                                        r.Position0 += Levels.StandardMargin;
                                                        r.Position1 += Levels.StandardMargin;
                                                        targetSession.PaintText(assistTextItem1, ref r, assistTextItem1TextPaints[assistFaint1]);

                                                        assistTextPosition0 += Levels.StandardMarginFloat32 + assistTextBound1Length;
                                                    }

                                                    if (assistTextItem2 != null)
                                                    {
                                                        r.Set(assistTextPosition0, assistTextPosition1 - assistTextBound2Height / 2, assistTextBound2Length, assistTextBound2Height);
                                                        targetSession.PaintVisibleText(assistTextItem2, ref r, FaintClearedPaints[assistFaint2], FaintFilledPaints[assistFaint2]);
                                                    }

                                                    return (float)(Levels.StandardMargin + Utility.Max(assistTextBound0Height, assistTextBound1Height, assistTextBound2Height));
                                                }

                                                var inputAssistTextFaint = assistTextFaint;
                                                if (inputAssistTextFaint > 0)
                                                {
                                                    var inputAssistTextPosition1 = drawingComponentValue.inputAssistTextPosition1;
                                                    var mainNoteLengthLevyingMap = drawingComponentValue.MainNoteLengthLevyingMap;
                                                    for (var i = inputCount; i > 0; --i)
                                                    {
                                                        var inputAssistTextItem = PoolSystem.Instance.GetTextItem(Configure.Instance.DefaultInputBundlesV6.Inputs[(int)inputMode][i][0].ToString(), InputAssistFont);
                                                        var inputAssistTextBound = inputAssistTextItem.LayoutBounds;
                                                        var inputAssistTextLength = inputAssistTextBound.Width;
                                                        var inputAssistTextHeight = inputAssistTextBound.Height;
                                                        r.Set(mainPosition + (isIn2P[i] ? p2Position : 0.0) + mainNoteLengthLevyingMap[i] + (drawingNoteLengthMap[i] - inputAssistTextLength) / 2, inputAssistTextPosition1 - inputAssistTextHeight / 2, inputAssistTextLength, inputAssistTextHeight);
                                                        targetSession.PaintVisibleText(inputAssistTextItem, ref r, FaintClearedPaints[inputAssistTextFaint], FaintFilledPaints[inputAssistTextFaint]);
                                                    }
                                                }

                                                var defaultHitPoints = defaultComputer.DefaultHitPoints;
                                                if (Configure.Instance.LowHitPointsFaintUI && defaultHitPoints < 0.5 && isHandling && defaultComputer.IsFailMode)
                                                {
                                                    targetSession.FillRectangle(0F, 0F, defaultLength, defaultHeight, _lowHitPointsPaints[(int)(50 * (0.5 - defaultHitPoints))]);
                                                }

                                                if (faintNoteModeValue == ModeComponent.FaintNoteMode.TotalFading)
                                                {
                                                    targetSession.FillRectangle(0F, 0F, defaultLength, defaultHeight, FaintFilledPaints[(int)(100 * defaultComputer.FaintCosine)]);
                                                }

                                                if (defaultComputer.IsPausingWindowOpened)
                                                {
                                                    targetSession.FillRectangle(0F, 0F, defaultLength, defaultHeight, FaintFilledPaints[50]);
                                                }

                                                var mediaModifierContents = defaultComputer.MediaModifierValue.Text;
                                                if (!string.IsNullOrEmpty(mediaModifierContents))
                                                {
                                                    pauseNotify0Position1 += PaintNotify0Contents(mediaModifierContents, pauseNotify0Position0, pauseNotify0Position1, PauseNotifyFont, Colors.White) + Levels.StandardMarginFloat32;
                                                }

                                                var wwwLevelDataValue = defaultComputer.WwwLevelDataValue;
                                                if (wwwLevelDataValue != null)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(wwwLevelDataValue.StandContents))
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(wwwLevelDataValue.StandContents, pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, wwwLevelDataValue.IsStandSatisify ? Colors.Green : Colors.Red) + Levels.StandardMarginFloat32;
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(wwwLevelDataValue.PointContents))
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(wwwLevelDataValue.PointContents, pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, wwwLevelDataValue.IsPointSatisify ? Colors.Green : Colors.Red) + Levels.StandardMarginFloat32;
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(wwwLevelDataValue.BandContents))
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(wwwLevelDataValue.BandContents, pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, wwwLevelDataValue.IsBandSatisify ? Colors.Green : Colors.Red) + Levels.StandardMarginFloat32;
                                                    }
                                                    if (!defaultComputer.CanPause)
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(LanguageSystem.Instance.NotAllowPauseText, pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, Colors.Red) + Levels.StandardMarginFloat32;
                                                    }
                                                    for (var i = wwwLevelDataValue.JudgmentContents.Length - 1; i >= 0; --i)
                                                    {
                                                        var judgmentContents = wwwLevelDataValue.JudgmentContents[i];
                                                        if (!string.IsNullOrWhiteSpace(judgmentContents))
                                                        {
                                                            pauseNotify1Position1 += PaintNotify1Contents(judgmentContents, pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, wwwLevelDataValue.IsJudgmentsSatisify[i] ? Colors.Green : Colors.Red) + Levels.StandardMarginFloat32;
                                                        }
                                                    }
                                                }

                                                var ioAvatarNames = defaultComputer.IOAvatarNames;
                                                var ioAvatarNamesCount = ioAvatarNames.Count;
                                                if (ioAvatarNamesCount > 0)
                                                {
                                                    if (ioAvatarNamesCount > 3)
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(PoolSystem.Instance.GetValueText(ioAvatarNamesCount, LanguageSystem.Instance.IOHigherContents), pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, Colors.White) + Levels.StandardMarginFloat32;
                                                    }
                                                    else
                                                    {
                                                        pauseNotify1Position1 += PaintNotify1Contents(PoolSystem.Instance.GetFormattedText(LanguageSystem.Instance.IOLowerContents, string.Join(", ", ioAvatarNames)), pauseNotify1Position0, pauseNotify1Position1, PauseNotifyFont, Colors.White) + Levels.StandardMarginFloat32;
                                                    }
                                                }

                                                var waitingTwilightLevel = defaultComputer.WaitingTwilightLevel;
                                                if (waitingTwilightLevel != DefaultCompute.WaitingTwilight.Default)
                                                {
                                                    pauseNotify0Position1 += PaintNotify0Contents(waitingTwilightLevel switch
                                                    {
                                                        DefaultCompute.WaitingTwilight.Net => LanguageSystem.Instance.WaitingNetContents,
                                                        DefaultCompute.WaitingTwilight.WaitIO => LanguageSystem.Instance.WaitingIOContents,
                                                        DefaultCompute.WaitingTwilight.CallIO => LanguageSystem.Instance.CallingIOContents,
                                                        _ => default
                                                    }, pauseNotify0Position0, pauseNotify0Position1, PauseNotifyFont, Colors.White) + Levels.StandardMarginFloat32;
                                                }

                                                if (!isItemMode && defaultComputer.IsTwilightNetItems && Configure.Instance.UIPipelineNet && defaultComputer.IsF)
                                                {
                                                    pauseNotify0Position1 += PaintNotify0Contents(LanguageSystem.Instance.TwilightCommentIOContents, pauseNotify0Position0, pauseNotify0Position1, PauseNotifyFont, Colors.White) + Levels.StandardMarginFloat32;
                                                }

                                                if (defaultComputer.IsPausingWindowOpened)
                                                {
                                                    var defaultSpinningModeValue = Configure.Instance.DefaultSpinningModeValue;
                                                    r.Set(drawingComponentValue.pausedUnpausePosition0, drawingComponentValue.pausedUnpausePosition1, drawingComponentValue.pausedUnpauseLength, drawingComponentValue.pausedUnpauseHeight);
                                                    SetEventHandler(ref r, setDefaultSpinningModeUnpause);
                                                    targetSession.PaintDrawing(ref r, UI.Instance.PausedUnpauseDrawings[defaultSpinningModeValue == Configure.DefaultSpinningMode.Unpause ? 1 : 0]);

                                                    r.Set(drawingComponentValue.pausedConfigurePosition0, drawingComponentValue.pausedConfigurePosition1, drawingComponentValue.pausedConfigureLength, drawingComponentValue.pausedConfigureHeight);
                                                    SetEventHandler(ref r, setDefaultSpinningModeConfigure);
                                                    targetSession.PaintDrawing(ref r, UI.Instance.PausedConfigureDrawings[defaultSpinningModeValue == Configure.DefaultSpinningMode.Configure ? 1 : 0]);

                                                    r.Set(drawingComponentValue.pausedUndoPosition0, drawingComponentValue.pausedUndoPosition1, drawingComponentValue.pausedUndoLength, drawingComponentValue.pausedUndoHeight);
                                                    SetEventHandler(ref r, setDefaultSpinningModeUndo);
                                                    targetSession.PaintDrawing(ref r, UI.Instance.PausedUndoDrawings[defaultSpinningModeValue == Configure.DefaultSpinningMode.Undo ? 1 : 0]);

                                                    r.Set(drawingComponentValue.pausedStopPosition0, drawingComponentValue.pausedStopPosition1, drawingComponentValue.pausedStopLength, drawingComponentValue.pausedStopHeight);
                                                    SetEventHandler(ref r, setDefaultSpinningModeStop);
                                                    targetSession.PaintDrawing(ref r, UI.Instance.PausedStopDrawings[defaultSpinningModeValue == Configure.DefaultSpinningMode.Stop ? 1 : 0]);
                                                }

                                                PaintFading();
                                                PaintNotifyXamlItems();
                                            }
                                            else
                                            {
                                                pauseNotify0Position1 += PaintNotify0Contents(faultText, pauseNotify0Position0, pauseNotify0Position1, PauseNotifyFont, Colors.Red) + Levels.StandardMarginFloat32;
                                            }
                                        }
                                        PaintFramerate();
                                    }

                                    CopyD3D9Drawing();

                                    using (targetSession = _rawTargetSystem.CreateDrawingSession(Colors.Black))
                                    {
                                        targetSession.DrawImage(_targetSystem);

                                        SetNVLLFlagIf(ReflexMarker.eSimulationEnd);
                                        SetNVLLFlagIf(ReflexMarker.eRenderSubmitStart);
                                    }
                                    SetNVLLFlagIf(ReflexMarker.eRenderSubmitEnd);
                                }

                                if (isNVLL)
                                {
                                    WaitNVLL();
                                }

                                SetNVLLFlagIf(ReflexMarker.ePresentStart);
                                var vesa = Configure.Instance.VESAV2;
                                _rawTargetSystem.Present(vesa ? 1 : 0);
                                SetNVLLFlagIf(ReflexMarker.ePresentEnd);

                                if (isNVLL)
                                {
                                    GetNVLLFrame();
                                }
                                SetNVLLFlagIf(ReflexMarker.eSimulationStart);

                                void SetNVLLFlagIf(ReflexMarker setFlag)
                                {
                                    if (isNVLL)
                                    {
                                        SetNVLLFlag(setFlag);
                                    }
                                }
                                break;
                            case MainViewModel.Mode.Quit:
                                defaultComputer = mainViewModel.Computer;
                                modeComponentValue = defaultComputer.ModeComponentValue;
                                var handlingComputer = mainViewModel.GetHandlingComputer();
                                lock (D2D1CSX)
                                {
                                    using (targetSession = _targetSystem.CreateDrawingSession())
                                    {
                                        targetSession.Clear(Colors.Black);

                                        lock (UI.Instance.ContentsCSX)
                                        {
                                            var faultText = BaseUI.Instance.FaultText;
                                            if (string.IsNullOrEmpty(faultText))
                                            {
                                                var judgmentQuitColors = BaseUI.Instance.JudgmentQuitColors;
                                                var judgmentColors = BaseUI.Instance.JudgmentColors;

                                                PaintBaseProperty(0);

                                                r.Set(BaseUI.Instance.TitleQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.Title, BaseUI.Instance.TitleQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.TitleQuitColor);
                                                r.Set(BaseUI.Instance.ArtistQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.Artist, BaseUI.Instance.ArtistQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.ArtistQuitColor);
                                                r.Set(BaseUI.Instance.GenreQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.GenreText, BaseUI.Instance.GenreQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.GenreQuitColor);
                                                r.Set(BaseUI.Instance.LevelQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.LevelText, BaseUI.Instance.LevelQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.D2DLevelColors[(int)defaultComputer.LevelValue]);
                                                r.Set(BaseUI.Instance.WantLevelIDQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.NoteFile.WantLevelID, BaseUI.Instance.WantLevelIDQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.WantLevelIDQuitColor);

                                                var judgmentDrawings = BaseUI.Instance.JudgmentDrawings;
                                                r.Set(BaseUI.Instance.TotalNotesJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.TotalNotesJudgmentDrawing);
                                                r.Set(BaseUI.Instance.HighestJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.Highest]?.Drawing);
                                                r.Set(BaseUI.Instance.HigherJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.Higher]?.Drawing);
                                                r.Set(BaseUI.Instance.HighJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.High]?.Drawing);
                                                r.Set(BaseUI.Instance.LowJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.Low]?.Drawing);
                                                r.Set(BaseUI.Instance.LowerJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.Lower]?.Drawing);
                                                r.Set(BaseUI.Instance.LowestJudgmentQuitPoint);
                                                targetSession.PaintDrawing(ref r, judgmentDrawings[(int)Component.Judged.Lowest]?.Drawing);

                                                r.Set(BaseUI.Instance.TotalNotesJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.TotalNotesInQuit, BaseUI.Instance.TotalNotesQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.TotalNotesJudgmentQuitColor);
                                                r.Set(BaseUI.Instance.HighestJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.HighestJudgmentInQuit, BaseUI.Instance.HighestJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.Highest]);
                                                r.Set(BaseUI.Instance.HigherJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.HigherJudgmentInQuit, BaseUI.Instance.HigherJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.Higher]);
                                                r.Set(BaseUI.Instance.HighJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.HighJudgmentInQuit, BaseUI.Instance.HighJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.High]);
                                                r.Set(BaseUI.Instance.LowJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.LowJudgmentInQuit, BaseUI.Instance.LowJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.Low]);
                                                r.Set(BaseUI.Instance.LowerJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.LowerJudgmentInQuit, BaseUI.Instance.LowerJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.Lower]);
                                                r.Set(BaseUI.Instance.LowestJudgmentContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.LowestJudgmentInQuit, BaseUI.Instance.LowestJudgmentQuitFont, (float)r.Length, (float)r.Height), ref r, judgmentQuitColors[(int)Component.Judged.Lowest]);

                                                r.Set(BaseUI.Instance.QuitDrawingPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.QuitDrawings[(int)defaultComputer.QuitStatusValue][defaultComputer.IsP ? 1 : 0]?.Drawing);

                                                r.Set(BaseUI.Instance.StandQuitPoint);
                                                if (defaultComputer.NetPosition != -1)
                                                {
                                                    targetSession.PaintDrawing(ref r, BaseUI.Instance.NetPositionDrawings.ElementAtOrDefault(defaultComputer.NetPosition) ?? BaseUI.Instance.StandDrawing);
                                                }
                                                else
                                                {
                                                    targetSession.PaintDrawing(ref r, defaultComputer.IsNewStand || TelnetSystem.Instance.IsAlwaysNewStand ? BaseUI.Instance.NewStandDrawing : BaseUI.Instance.StandDrawing);
                                                }
                                                r.Set(BaseUI.Instance.PointQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.PointDrawing);
                                                r.Set(BaseUI.Instance.BandQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.HighestBandDrawing);

                                                r.Set(BaseUI.Instance.StandContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(PoolSystem.Instance.GetValueText(defaultComputer.Stand.TargetValue, "#,##0"), BaseUI.Instance.StandQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.StandQuitColor);
                                                r.Set(BaseUI.Instance.PointContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(PoolSystem.Instance.GetValueText((100.0 * defaultComputer.Point.TargetValue), "0.00％"), BaseUI.Instance.PointQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.PointQuitColor);
                                                r.Set(BaseUI.Instance.BandContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(PoolSystem.Instance.GetValueText(defaultComputer.HighestBand, string.Empty), BaseUI.Instance.BandQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.BandQuitColor);

                                                r.Set(BaseUI.Instance.CommentPlace0Point);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.CommentPlace0Text, BaseUI.Instance.CommentPlace0Font, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.CommentPlaceColor);
                                                r.Set(BaseUI.Instance.CommentPlace1Point);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.CommentPlace1Text, BaseUI.Instance.CommentPlace1Font, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.CommentPlaceColor);

                                                var judgmentMeterViewPosition0 = BaseUI.Instance.JudgmentMeterViewPoint[0];
                                                var judgmentMeterViewPosition1 = BaseUI.Instance.JudgmentMeterViewPoint[1];
                                                var judgmentMeterViewLength = BaseUI.Instance.JudgmentMeterViewPoint[2];
                                                var judgmentMeterViewHeight = BaseUI.Instance.JudgmentMeterViewPoint[3];
                                                r.Set(judgmentMeterViewPosition0, judgmentMeterViewPosition1, judgmentMeterViewLength, judgmentMeterViewHeight);
                                                targetSession.DrawRectangle(r, Colors.White);
                                                v0.Set(judgmentMeterViewPosition0, judgmentMeterViewPosition1 + judgmentMeterViewHeight / 2);
                                                v1.Set(judgmentMeterViewPosition0 + judgmentMeterViewLength, judgmentMeterViewPosition1 + judgmentMeterViewHeight / 2);
                                                targetSession.DrawLine(v0, v1, Colors.White);
                                                var lowestJudgmentMillis = defaultComputer.LowestJudgmentMillis;
                                                var highestJudgmentMillis = defaultComputer.HighestJudgmentMillis;
                                                var noteFileLength = defaultComputer.NoteFile.Length;
                                                var judgmentMeterEventValues = defaultComputer.JudgmentMeterEventValues;
                                                lock (judgmentMeterEventValues)
                                                {
                                                    for (var i = (int)Component.Judged.Lowest; i >= (int)Component.Judged.Highest; --i)
                                                    {
                                                        foreach (var (wait, judgmentMeter) in judgmentMeterEventValues[i])
                                                        {
                                                            var judgmentMeterPosition0 = (float)(judgmentMeterViewPosition0 + judgmentMeterViewLength * wait / noteFileLength);
                                                            var judgmentMeterPosition1 = (float)(judgmentMeterViewPosition1 + judgmentMeterViewHeight * Math.Clamp(0.5 + (judgmentMeter < 0.0 ? -judgmentMeter / lowestJudgmentMillis : judgmentMeter / highestJudgmentMillis) / 2, 0.0, 1.0));
                                                            if (judgmentMeterViewPosition0 <= judgmentMeterPosition0 &&
                                                                judgmentMeterPosition0 <= judgmentMeterViewPosition0 + judgmentMeterViewLength &&
                                                                judgmentMeterViewPosition1 <= judgmentMeterPosition1 &&
                                                                judgmentMeterPosition1 <= judgmentMeterViewPosition1 + judgmentMeterViewHeight)
                                                            {
                                                                r.SetPosition(judgmentMeterPosition0, judgmentMeterPosition1);
                                                                targetSession.FillCircle(r, 1F, judgmentColors[i]);
                                                            }
                                                        }
                                                    }
                                                }

                                                r.SetPosition(judgmentMeterViewPosition0 + 5.0, judgmentMeterViewPosition1 + 5.0);
                                                var textItem = PoolSystem.Instance.GetTextItem(defaultComputer.JudgmentMeterText, JudgmentMeterViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, Colors.White);
                                                r.Position1 += textItem.LayoutBounds.Height + Levels.StandardMarginFloat32;
                                                textItem = PoolSystem.Instance.GetTextItem(defaultComputer.EarlyLateText, JudgmentMeterViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, Colors.White);

                                                var statusViewPosition0 = BaseUI.Instance.StatusViewPoint[0];
                                                var statusViewPosition1 = BaseUI.Instance.StatusViewPoint[1];
                                                var statusViewLength = BaseUI.Instance.StatusViewPoint[2];
                                                var statusViewHeight = BaseUI.Instance.StatusViewPoint[3];
                                                r.Set(statusViewPosition0, statusViewPosition1, statusViewLength, statusViewHeight);
                                                targetSession.DrawRectangle(r, Colors.White);

                                                var hitPointsStatusViewColor = BaseUI.Instance.HitPointsStatusViewColor;
                                                var hitPointsEventValues = defaultComputer.HitPointsEventValues;
                                                var standStatusViewColor = BaseUI.Instance.StandStatusViewColor;
                                                var standEventValues = defaultComputer.StandEventValues;
                                                var pointStatusViewColor = BaseUI.Instance.PointStatusViewColor;
                                                var pointEventValues = defaultComputer.PointEventValues;
                                                var bandStatusViewColor = BaseUI.Instance.BandStatusViewColor;
                                                var bandEventValues = defaultComputer.BandEventValues;
                                                lock (defaultComputer.EventValuesCSX)
                                                {
                                                    for (var i = hitPointsEventValues.Count - 2; i >= 0; --i)
                                                    {
                                                        var hitPointsEventValue = hitPointsEventValues[i];
                                                        var hitPointsEventValueAs = hitPointsEventValues[i + 1];

                                                        v0.Set((float)(statusViewPosition0 + hitPointsEventValue.Key * statusViewLength), (float)(statusViewPosition1 + hitPointsEventValue.Value * statusViewHeight));
                                                        v1.Set((float)(statusViewPosition0 + hitPointsEventValueAs.Key * statusViewLength), (float)(statusViewPosition1 + hitPointsEventValueAs.Value * statusViewHeight));
                                                        targetSession.DrawLine(v0, v1, hitPointsStatusViewColor);
                                                    }

                                                    for (var i = standEventValues.Count - 2; i >= 0; --i)
                                                    {
                                                        var standEventValue = standEventValues[i];
                                                        var standEventValueAs = standEventValues[i + 1];

                                                        v0.Set((float)(statusViewPosition0 + standEventValue.Key * statusViewLength), (float)(statusViewPosition1 + standEventValue.Value * statusViewHeight));
                                                        v1.Set((float)(statusViewPosition0 + standEventValueAs.Key * statusViewLength), (float)(statusViewPosition1 + standEventValueAs.Value * statusViewHeight));
                                                        targetSession.DrawLine(v0, v1, standStatusViewColor);
                                                    }

                                                    for (var i = pointEventValues.Count - 2; i >= 0; --i)
                                                    {
                                                        var pointEventValue = pointEventValues[i];
                                                        var pointEventValueAs = pointEventValues[i + 1];

                                                        v0.Set((float)(statusViewPosition0 + pointEventValue.Key * statusViewLength), (float)(statusViewPosition1 + pointEventValue.Value * statusViewHeight));
                                                        v1.Set((float)(statusViewPosition0 + pointEventValueAs.Key * statusViewLength), (float)(statusViewPosition1 + pointEventValueAs.Value * statusViewHeight));
                                                        targetSession.DrawLine(v0, v1, pointStatusViewColor);
                                                    }

                                                    for (var i = bandEventValues.Count - 2; i >= 0; --i)
                                                    {
                                                        var bandEventValue = bandEventValues[i];
                                                        var bandEventValueAs = bandEventValues[i + 1];

                                                        v0.Set((float)(statusViewPosition0 + bandEventValue.Key * statusViewLength), (float)(statusViewPosition1 + bandEventValue.Value * statusViewHeight));
                                                        v1.Set((float)(statusViewPosition0 + bandEventValueAs.Key * statusViewLength), (float)(statusViewPosition1 + bandEventValueAs.Value * statusViewHeight));
                                                        targetSession.DrawLine(v0, v1, bandStatusViewColor);
                                                    }
                                                }

                                                r.SetPosition(statusViewPosition0 + 5.0, statusViewPosition1 + 5.0);
                                                textItem = PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.HitPointsLegend, StatusViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, BaseUI.Instance.HitPointsStatusViewColor);
                                                r.Position1 += textItem.LayoutBounds.Height + Levels.StandardMarginFloat32;
                                                textItem = PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.StandLegend, StatusViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, BaseUI.Instance.StandStatusViewColor);
                                                r.Position1 += textItem.LayoutBounds.Height + Levels.StandardMarginFloat32;
                                                textItem = PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.PointLegend, StatusViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, BaseUI.Instance.PointStatusViewColor);
                                                r.Position1 += textItem.LayoutBounds.Height + Levels.StandardMarginFloat32;
                                                textItem = PoolSystem.Instance.GetTextItem(LanguageSystem.Instance.BandLegend, StatusViewFont, (float)r.Length, (float)r.Height);
                                                targetSession.PaintVisibleText(textItem, ref r, BaseUI.Instance.BandStatusViewColor);

                                                r.Set(BaseUI.Instance.AutoModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.AutoModeVariety][(int)modeComponentValue.AutoModeValue]?.Drawing, modeComponentValue.AutoModeValue == ModeComponent.AutoMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.NoteSaltModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.NoteSaltModeVariety][(int)modeComponentValue.NoteSaltModeValue]?.Drawing, modeComponentValue.NoteSaltModeValue == ModeComponent.NoteSaltMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.FaintNoteModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.FaintNoteModeVariety][(int)modeComponentValue.FaintNoteModeValue]?.Drawing, modeComponentValue.FaintNoteModeValue == ModeComponent.FaintNoteMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.JudgmentModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.JudgmentModeVariety][(int)modeComponentValue.JudgmentModeValue]?.Drawing, modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.HitPointsModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.HitPointsModeVariety][(int)modeComponentValue.HandlingHitPointsModeValue]?.Drawing, modeComponentValue.HandlingHitPointsModeValue == ModeComponent.HitPointsMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.NoteMobilityModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.NoteMobilityModeVariety][(int)modeComponentValue.NoteMobilityModeValue]?.Drawing, modeComponentValue.NoteMobilityModeValue == ModeComponent.NoteMobilityMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.InputFavorModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.InputFavorModeVariety][(int)modeComponentValue.InputFavorModeValue]?.Drawing, modeComponentValue.InputFavorModeValue == ModeComponent.InputFavorMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.LongNoteModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.LongNoteModeVariety][(int)modeComponentValue.LongNoteModeValue]?.Drawing, modeComponentValue.LongNoteModeValue == ModeComponent.LongNoteMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.NoteModifyModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.NoteModifyModeVariety][(int)modeComponentValue.NoteModifyModeValue]?.Drawing, modeComponentValue.NoteModifyModeValue == ModeComponent.NoteModifyMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.BPMModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.BPMModeVariety][(int)modeComponentValue.BPMModeValue]?.Drawing, modeComponentValue.BPMModeValue == ModeComponent.BPMMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.WaveModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.WaveModeVariety][(int)modeComponentValue.WaveModeValue]?.Drawing, modeComponentValue.WaveModeValue == ModeComponent.WaveMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.SetNoteModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.SetNoteModeVariety][(int)modeComponentValue.SetNoteModeValue]?.Drawing, modeComponentValue.SetNoteModeValue == ModeComponent.SetNoteMode.Default ? 0.125F : 1F);
                                                r.Set(BaseUI.Instance.LowestJudgmentConditionModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ModeComponentDrawings[ModifyModeComponentViewModel.LowestJudgmentConditionModeVariety][(int)modeComponentValue.LowestJudgmentConditionModeValue]?.Drawing, modeComponentValue.LowestJudgmentConditionModeValue == ModeComponent.LowestJudgmentConditionMode.Default ? 0.125F : 1F);

                                                r.Set(BaseUI.Instance.JudgmentStageQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.JudgmentStageQuitDrawing);
                                                r.Set(BaseUI.Instance.HighestInputCountQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.HighestInputCountQuitDrawing);
                                                r.Set(BaseUI.Instance.LengthQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.LengthQuitDrawing);
                                                r.Set(BaseUI.Instance.BPMQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.BPMQuitDrawing);
                                                r.Set(BaseUI.Instance.InputModeQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.InputModeQuitDrawing);

                                                r.Set(BaseUI.Instance.JudgmentStageContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.JudgmentStageContents, BaseUI.Instance.JudgmentStageQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.JudgmentStageQuitColor);
                                                r.Set(BaseUI.Instance.HighestInputCountContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(Utility.GetHighestInputCountText(defaultComputer.AverageInputCount, defaultComputer.HighestInputCount, defaultComputer.AudioMultiplier), BaseUI.Instance.HighestInputCountQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.HighestInputCountQuitColor);
                                                r.Set(BaseUI.Instance.LengthContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.LengthText, BaseUI.Instance.LengthQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.LengthQuitColor);
                                                r.Set(BaseUI.Instance.BPMContentsQuitPoint);
                                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.BPMText, BaseUI.Instance.BPMQuitFont, (float)r.Length, (float)r.Height), ref r, BaseUI.Instance.BPMQuitColor);
                                                r.Set(BaseUI.Instance.InputModeContentsQuitPoint);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.InputModeDrawings[(int)defaultComputer.NoteFile.InputMode]?.Drawing);

                                                r.Set(BaseUI.Instance.ViewCommentPoint);
                                                SetEventHandler(ref r, handleViewCommentImpl);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.ViewCommentDrawing);

                                                r.Set(BaseUI.Instance.HandleUndoPoint);
                                                SetEventHandler(ref r, handleUndoImpl);
                                                targetSession.PaintDrawing(ref r, BaseUI.Instance.HandleUndoDrawing);

                                                if (defaultComputer.LevyingComputingPosition > 0 && BaseUI.Instance.QuitMove0Point != null)
                                                {
                                                    r.Set(BaseUI.Instance.QuitMove0Point);
                                                    SetEventHandler(ref r, handleQuitMove0Impl);
                                                    targetSession.PaintDrawing(ref r, BaseUI.Instance.QuitMove0Drawing);
                                                }

                                                if (defaultComputer.LevyingComputingPosition < defaultComputer.HighestComputingPosition && BaseUI.Instance.QuitMove1Point != null)
                                                {
                                                    r.Set(BaseUI.Instance.QuitMove1Point);
                                                    SetEventHandler(ref r, handleQuitMove1Impl);
                                                    targetSession.PaintDrawing(ref r, BaseUI.Instance.QuitMove1Drawing);
                                                }

                                                PaintBaseProperty(1);

                                                PaintFading();
                                                PaintNotifyXamlItems();

                                                void PaintBaseProperty(int layer)
                                                {
                                                    foreach (var paintPropertyValue in BaseUI.Instance.PaintPropertyValues)
                                                    {
                                                        if (paintPropertyValue?.Layer == layer)
                                                        {
                                                            paintPropertyValue.Paint(targetSession, distanceMillis, defaultComputer, handlingComputer);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                PaintNotify0Contents(faultText, pauseNotify0Position0, pauseNotify0Position1, PauseNotifyFont, Colors.Red);
                                            }
                                        }
                                        PaintFramerate();
                                    }

                                    CopyD3D9Drawing();

                                    using (targetSession = _rawTargetSystem.CreateDrawingSession(Colors.Black))
                                    {
                                        targetSession.DrawImage(_targetSystem);
                                    }
                                }

                                _rawTargetSystem.Present();
                                break;
                        }
                    }

                    var millis = loopingHandler.GetMillis();
                    distanceMillis = millis - lastMillis;
                    lastMillis = millis;

                    if (wasLastPointed && !HandleLastPointed())
                    {
                        if (mainViewModel.IsComputingMode)
                        {
                            var lastPointedPosition = lastPointed.Item1;
                            var lastPointedPositionX = lastPointedPosition.X;
                            var lastPointedPositionY = lastPointedPosition.Y;
                            switch (_mediaInputAreaStatus)
                            {
                                case MediaInputAreaStatus.Not:
                                    if (!defaultComputer.IsPausingWindowOpened && defaultComputer.CanSetPosition)
                                    {
                                        var drawingComponentValue = defaultComputer.DrawingComponentValue;
                                        var has2P = defaultComputer.Has2P;
                                        var distance2P = drawingComponentValue.p1BuiltLength + drawingComponentValue.p2Position;
                                        if (!MoveStatus(drawingComponentValue.statusPosition0, drawingComponentValue.statusPosition1, drawingComponentValue.statusLength, drawingComponentValue.statusHeight, drawingComponentValue.statusSystem, drawingComponentValue.altStatus))
                                        {
                                            MoveStatus(drawingComponentValue.statusSliderPosition0, drawingComponentValue.statusSliderPosition1, drawingComponentValue.statusSliderLength, drawingComponentValue.statusSliderHeight, drawingComponentValue.statusSliderSystem, drawingComponentValue.altStatusSlider);
                                        }

                                        bool MoveStatus(float position0, float position1, float length, float height, int system, int alt)
                                        {
                                            for (var i = alt >> 1; i >= alt % 2; --i)
                                            {
                                                var distance = i == 1 && has2P ? distance2P : 0F;
                                                if (lastPointedPositionX >= position0 + distance && lastPointedPositionX < position0 + distance + length && lastPointedPositionY >= position1 && lastPointedPositionY < position1 + height)
                                                {
                                                    defaultComputer.LevyingWait = lastPointed.Item2 ? 0.0 : system switch
                                                    {
                                                        0 => defaultComputer.Length * (1 - (lastPointedPositionY - position1) / height),
                                                        1 => defaultComputer.Length * (lastPointedPositionY - position1) / height,
                                                        2 => defaultComputer.Length * (1 - (lastPointedPositionX - position0 - distance) / length),
                                                        3 => defaultComputer.Length * (lastPointedPositionX - position0 - distance) / length,
                                                        _ => default,
                                                    };
                                                    defaultComputer.SetUndo = true;
                                                    return true;
                                                }
                                            }
                                            return false;
                                        }
                                    }
                                    break;
                                case MediaInputAreaStatus.Position:
                                    _mediaInputPosition0 = lastPointedPositionX;
                                    _mediaInputPosition1 = lastPointedPositionY;
                                    _mediaInputAreaStatus = MediaInputAreaStatus.Area;
                                    Configure.Instance.MediaInputPosition0 = lastPointedPositionX;
                                    Configure.Instance.MediaInputPosition1 = lastPointedPositionY;
                                    break;
                            }
                        }
                    }

                    if (wasLastMoved)
                    {
                        if (mainViewModel.IsComputingMode)
                        {
                            switch (_mediaInputAreaStatus)
                            {
                                case MediaInputAreaStatus.Area:
                                    var lastMovedPositionX = lastMoved.X;
                                    var lastMovedPositionY = lastMoved.Y;
                                    if (_mediaInputPosition0 < lastMovedPositionX)
                                    {
                                        Configure.Instance.MediaInputLength = lastMovedPositionX - _mediaInputPosition0;
                                    }
                                    else
                                    {
                                        Configure.Instance.MediaInputPosition0 = lastMovedPositionX;
                                        Configure.Instance.MediaInputLength = _mediaInputPosition0 - lastMovedPositionX;
                                    }
                                    if (_mediaInputPosition1 < lastMovedPositionY)
                                    {
                                        Configure.Instance.MediaInputHeight = lastMovedPositionY - _mediaInputPosition1;
                                    }
                                    else
                                    {
                                        Configure.Instance.MediaInputPosition1 = lastMovedPositionY;
                                        Configure.Instance.MediaInputHeight = _mediaInputPosition1 - lastMovedPositionY;
                                    }
                                    break;
                            }
                        }
                    }

                    if (wasLastNotPointed)
                    {
                        if (mainViewModel.IsComputingMode)
                        {
                            switch (_mediaInputAreaStatus)
                            {
                                case MediaInputAreaStatus.Area:
                                    _mediaInputAreaStatus = MediaInputAreaStatus.Not;
                                    break;
                            }
                        }
                    }

                    _eventHandler = null;
                    _netItemHandler = null;
                    _toNotifyXamlItemHandler = null;

                    if (allowFramerate)
                    {
                        distanceMillisMax = Math.Max(distanceMillisMax, distanceMillis);
                        ++frameCount;
                        frametime += distanceMillis;
                        if (frametime >= 1000.0)
                        {
                            framerate = PoolSystem.Instance.GetValueText(Math.Round(1000.0 * frameCount / frametime), "0 frame/s");
                            framerateLowest = PoolSystem.Instance.GetValueText(Math.Round(1000.0 / distanceMillisMax), "0 frame/s (\\0％ Low)");
                            frameCount = 0;
                            frametime = 0.0;
                            distanceMillisMax = 0.0;
                            for (var i = 0; i < QwilightComponent.HeapCount; ++i)
                            {
                                textGCs[i] = PoolSystem.Instance.GetFormattedText("GC{0} {1}", PoolSystem.Instance.GetValueText(i, string.Empty), PoolSystem.Instance.GetValueText(GC.CollectionCount(i), string.Empty));
                            }
                            var valueHeap = GC.GetTotalMemory(false);
                            var distanceHeap = valueHeap - lastHeap;
                            textHeap = PoolSystem.Instance.GetFormattedText("Heap {0} ({1}{2}/s)", PoolSystem.Instance.GetFormattedUnitText(valueHeap), distanceHeap >= 0 ? "＋" : "－", PoolSystem.Instance.GetFormattedUnitText(Math.Abs(distanceHeap)));
                            lastHeap = valueHeap;
                        }
                    }

                    void PaintFading()
                    {
                        if (fadingStatus > 0.0)
                        {
                            var fadingComputer = mainViewModel.FadingViewComputer;
                            var fadingPropertyValue = BaseUI.Instance.FadingPropertyValues[(int)mode]?[fadingValue.Layer];
                            var fadingPropertyFrame = fadingPropertyValue?.Frame ?? 0;
                            if (fadingPropertyFrame > 0)
                            {
                                if (fadingPropertyValue.DrawingStatus <= fadingStatus)
                                {
                                    var hasContents = false;
                                    if (fadingComputer != null)
                                    {
                                        lock (fadingComputer.ContentsCSX)
                                        {
                                            var fadingViewDrawing = fadingComputer.NoteHandledDrawingItem;
                                            hasContents = fadingComputer.HasContents && fadingViewDrawing != null;
                                            if (hasContents)
                                            {
                                                HandleDrawing(fadingViewDrawing);
                                            }
                                        }
                                    }
                                    if (!hasContents)
                                    {
                                        HandleDrawing(DrawingSystem.Instance.DefaultDrawing);
                                    }

                                    void HandleDrawing(HandledDrawingItem? fadingViewDrawing)
                                    {
                                        var drawing = fadingViewDrawing?.Drawing;
                                        if (drawing.HasValue)
                                        {
                                            r.SetArea(defaultLength, defaultHeight);
                                            targetSession.FillRectangle(r, Colors.Black);
                                            var drawingBound = drawing.Value.DrawingBound;
                                            Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, drawingBound.Length, drawingBound.Height, 0.0, 0.0, defaultLength, defaultHeight);
                                            targetSession.PaintDrawing(ref r, drawing);
                                        }
                                    }
                                }
                                r.SetArea(defaultLength, defaultHeight);
                                targetSession.PaintDrawing(ref r, fadingPropertyValue.HandledDrawingItems[(int)Math.Floor(fadingStatus * (fadingPropertyFrame - 1))]?.Drawing);
                            }
                        }
                    }

                    void PaintNotifyXamlItems()
                    {
                        var toNotifyXamlItemCollection = ViewModels.Instance.NotifyXamlValue.NotifyXamlItemCollection;
                        var toNotifyPosition1 = 0F;
                        lock (toNotifyXamlItemCollection)
                        {
                            foreach (var toNotifyXamlItem in toNotifyXamlItemCollection)
                            {
                                var toNotifyTextBoundHeight = 24F;
                                var toNotifyTextItem = PoolSystem.Instance.GetTextItem(toNotifyXamlItem.Contents, NotifyXamlFont, 0F, toNotifyTextBoundHeight);
                                var toNotifyTextBound = toNotifyTextItem.LayoutBounds;
                                var toNotifyHeight = Levels.StandardMarginFloat32 + toNotifyTextBoundHeight + Levels.StandardMarginFloat32;
                                var toNotifyDrawing = BaseUI.Instance.NotifyDrawings[(int)toNotifyXamlItem.Variety];
                                var toNotifyDrawingLength = toNotifyDrawing?.Drawing != null ? (float)(toNotifyTextBoundHeight * toNotifyDrawing.Value.Drawing.Value.DrawingBound.Length / toNotifyDrawing.Value.Drawing.Value.DrawingBound.Height) : 0F;

                                toNotifyPosition1 += Levels.StandardMarginFloat32;
                                r.Set(Levels.StandardMarginFloat32, toNotifyPosition1, Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32 + toNotifyDrawingLength + 2 * Levels.StandardMarginFloat32 + toNotifyTextBound.Width + Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + toNotifyHeight + Levels.StandardEdgeFloat32);
                                SetNotifyXamlItemHandler(ref r, handleNotifyXamlItemImpl, toNotifyXamlItem);
                                targetSession.DrawRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, Colors.White);

                                r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, r.Length - 2 * Levels.StandardEdgeFloat32, r.Height - 2 * Levels.StandardEdgeFloat32);
                                targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, toNotifyXamlItem.Color);

                                r.Set(r.Position0 + Levels.StandardMarginFloat32, r.Position1 + Levels.StandardMarginFloat32, toNotifyDrawingLength, toNotifyTextBoundHeight);
                                targetSession.PaintDrawing(ref r, toNotifyDrawing?.Drawing);

                                r.Position0 += r.Length + 2 * Levels.StandardMarginFloat32;
                                targetSession.PaintText(toNotifyTextItem, ref r, Colors.White);
                                toNotifyPosition1 += (float)(Levels.StandardEdgeFloat32 + toNotifyHeight + Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32);
                            }
                        }
                    }

                    void PaintFramerate()
                    {
                        if (allowFramerate)
                        {
                            r.SetPosition(Levels.StandardMarginFloat32, Levels.StandardMarginFloat32);

                            var textItem = PoolSystem.Instance.GetTextItem(framerate, UtilityFont);
                            targetSession.PaintText(textItem, ref r, Colors.Red);
                            r.Position1 += (float)(textItem.LayoutBounds.Height + Levels.StandardMarginFloat32);

                            textItem = PoolSystem.Instance.GetTextItem(framerateLowest, UtilityFont);
                            targetSession.PaintText(textItem, ref r, Colors.Red);
                            r.Position1 += (float)(textItem.LayoutBounds.Height + Levels.StandardMarginFloat32);

                            for (var i = 0; i < QwilightComponent.HeapCount; ++i)
                            {
                                textItem = PoolSystem.Instance.GetTextItem(textGCs[i], UtilityFont);
                                targetSession.PaintText(textItem, ref r, Colors.Red);
                                r.Position1 += (float)(textItem.LayoutBounds.Height + Levels.StandardMarginFloat32);
                            }

                            textItem = PoolSystem.Instance.GetTextItem(textHeap, UtilityFont);
                            targetSession.PaintText(textItem, ref r, Colors.Red);
                        }
                    }

                    void CopyD3D9Drawing()
                    {
                        if (isWPFViewVisible)
                        {
                            _targetSystem.GetPixelBytes(_targetSystemData);
                            var w = new WriteableBitmap((int)_targetSystem.SizeInPixels.Width, (int)_targetSystem.SizeInPixels.Height, _targetSystem.Dpi, _targetSystem.Dpi, PixelFormats.Bgra32, null);
                            w.WritePixels(new(0, 0, w.PixelWidth, w.PixelHeight), _rawTargetSystemData, w.Format.BitsPerPixel / 8 * w.PixelWidth, 0);
                            w.Freeze();
                            D3D9Drawing = w;
                        }
                    }

                    float PaintNotify0Contents(string toNotifyContents, float toNotifyPosition0, float toNotifyPosition1, CanvasTextFormat toNotifyFont, Color toNotifyColor, Action onHandled = null)
                    {
                        var toNotifyTextItem = PoolSystem.Instance.GetTextItem(toNotifyContents, toNotifyFont);
                        var textBound = toNotifyTextItem.LayoutBounds;
                        var textBoundLength = textBound.Width;
                        var textBoundHeight = textBound.Height;

                        r.Set(toNotifyPosition0, toNotifyPosition1, Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32 + textBoundLength + Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32 + textBoundHeight + Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32);
                        if (onHandled != null)
                        {
                            SetEventHandler(ref r, onHandled);
                        }
                        targetSession.DrawRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, Colors.White);

                        r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, r.Length - 2 * Levels.StandardEdgeFloat32, r.Height - 2 * Levels.StandardEdgeFloat32);
                        targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, FaintFilledPaints[50]);

                        r.Position0 += Levels.StandardMarginFloat32;
                        r.Position1 += Levels.StandardMarginFloat32;
                        targetSession.PaintText(toNotifyTextItem, ref r, toNotifyColor);
                        r.Height += 2 * Levels.StandardEdgeFloat32;
                        return (float)r.Height;
                    }

                    float PaintNotify1Contents(string toNotifyContents, float toNotifyPosition0, float toNotifyPosition1, CanvasTextFormat toNotifyFont, Color toNotifyColor, Action onHandled = null)
                    {
                        var toNotifyTextItem = PoolSystem.Instance.GetTextItem(toNotifyContents, toNotifyFont);
                        var textBound = toNotifyTextItem.LayoutBounds;
                        var textBoundLength = textBound.Width;
                        var textBoundHeight = textBound.Height;

                        r.Set(toNotifyPosition0, toNotifyPosition1, Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32 + textBoundLength + Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32, Levels.StandardEdgeFloat32 + Levels.StandardMarginFloat32 + textBoundHeight + Levels.StandardMarginFloat32 + Levels.StandardEdgeFloat32);
                        r.Position0 -= r.Length; if (onHandled != null)
                        {
                            SetEventHandler(ref r, onHandled);
                        }
                        targetSession.DrawRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, Colors.White);

                        r.Set(r.Position0 + Levels.StandardEdgeFloat32, r.Position1 + Levels.StandardEdgeFloat32, r.Length - 2 * Levels.StandardEdgeFloat32, r.Height - 2 * Levels.StandardEdgeFloat32);
                        targetSession.FillRoundedRectangle(r, Levels.StandardEllipseFloat32, Levels.StandardEllipseFloat32, FaintFilledPaints[50]);

                        r.Position0 += Levels.StandardMarginFloat32;
                        r.Position1 += Levels.StandardMarginFloat32;
                        targetSession.PaintText(toNotifyTextItem, ref r, toNotifyColor);
                        r.Height += 2 * Levels.StandardEdgeFloat32;
                        return (float)r.Height;
                    }

                    bool CanPaint(PaintPipelineID paintPipeline) => paintPipeline switch
                    {
                        PaintPipelineID.AudioVisualizer => Configure.Instance.AudioVisualizer,
                        PaintPipelineID.JudgmentMeter => Configure.Instance.UIPipelineJudgmentMeter,
                        PaintPipelineID.BPM => Configure.Instance.UIPipelineBPM,
                        PaintPipelineID.JudgmentPaint => Configure.Instance.UIPipelineJudgmentPaint,
                        PaintPipelineID.Net => Configure.Instance.UIPipelineNet,
                        PaintPipelineID.JudgmentInputVisualizer => Configure.Instance.UIPipelineJudgmentInputVisualizer,
                        PaintPipelineID.JudgmentMain => Configure.Instance.UIPipelineJudgmentMain,
                        PaintPipelineID.HighestJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.HigherJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.HighJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.LowJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.LowerJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.LowestJudgment => Configure.Instance.UIPipelineJudgmentCount,
                        PaintPipelineID.Limiter => Configure.Instance.UIPipelineLimiter,
                        PaintPipelineID.JudgmentVisualizer => Configure.Instance.UIPipelineJudgmentVisualizer,
                        PaintPipelineID.HitNotePaint => Configure.Instance.UIPipelineHitNotePaint,
                        PaintPipelineID.Hunter => Configure.Instance.UIPipelineHunter,
                        PaintPipelineID.MediaInput => Configure.Instance.MediaInput,
                        PaintPipelineID.VeilDrawing => Configure.Instance.VeilDrawingHeight > 0.0,
                        PaintPipelineID.MainJudgmentMeter => Configure.Instance.UIPipelineMainJudgmentMeter,
                        _ => true
                    };

                    bool HandleLastPointed()
                    {
                        if (_eventHandler != null)
                        {
                            _eventHandler();
                            return true;
                        }
                        else if (_netItemHandler != null)
                        {
                            _netItemHandler(_netItemParam);
                            return true;
                        }
                        else if (_toNotifyXamlItemHandler != null)
                        {
                            _toNotifyXamlItemHandler(_toNotifyXamlItemParam);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    void SetEventHandler(ref Bound r, Action eventHandler)
                    {
                        if (wasLastPointed && r.IsPoint(lastPointed.Item1))
                        {
                            _eventHandler = eventHandler;
                        }
                    }

                    void SetNetItemHandler(ref Bound r, Action<NetItem> netItemHandler, NetItem netItemParam)
                    {
                        if (wasLastPointed && r.IsPoint(lastPointed.Item1))
                        {
                            _eventHandler = null;
                            _netItemHandler = netItemHandler;
                            _netItemParam = netItemParam;
                        }
                    }

                    void SetNotifyXamlItemHandler(ref Bound r, Action<NotifyXamlItem> toNotifyXamlItemHandler, NotifyXamlItem toNotifyXamlItemParam)
                    {
                        if (wasLastPointed && r.IsPoint(lastPointed.Item1))
                        {
                            _eventHandler = null;
                            _netItemHandler = null;
                            _toNotifyXamlItemHandler = toNotifyXamlItemHandler;
                            _toNotifyXamlItemParam = toNotifyXamlItemParam;
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                }
                catch (Exception e)
                {
                    Utility.SetFault(FaultEntryPath, e);
                }
                finally
                {
                    AvatarDrawingSystem.Instance.ClosePendingAvatarDrawings();
                    AvatarEdgeSystem.Instance.ClosePendingAvatarEdges();
                }
            }
        }

        public virtual void SetFaintPaints(IDrawingContainer drawingContainer, ICanvasBrush[] faintPaints, Color faintColor)
        {
            for (var i = faintPaints.Length - 1; i >= 0; --i)
            {
                var faintPaint = new CanvasSolidColorBrush(CanvasDevice.GetSharedDevice(), faintColor)
                {
                    Opacity = i / 100F
                };
                faintPaints[i] = faintPaint;
                if (drawingContainer != null)
                {
                    _toCloseValues.AddOrUpdate(drawingContainer, (drawingContainer, faintPaint) => new()
                    {
                        faintPaint
                    }, (drawingContainer, drawingItems, faintPaint) =>
                    {
                        drawingItems.Add(faintPaint);
                        return drawingItems;
                    }, faintPaint);
                }
            }
        }

        public void OnModified() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.GetWindowArea,
            Contents = new Action<int, int, int, int>((windowAreaPosition0, windowAreaPosition1, windowAreaLength, windowAreaHeight) =>
            {
                var mainViewModel = ViewModels.Instance.MainValue;
                var defaultLength = (float)mainViewModel.DefaultLength;
                var defaultHeight = (float)mainViewModel.DefaultHeight;
                var drawingQuality = new Vector2((float)(96.0 * windowAreaLength / defaultLength), (float)(96.0 * windowAreaHeight / defaultHeight));
                var targetWindowDPI = Math.Max(drawingQuality.X, drawingQuality.Y);
                var dataCount = Configure.Instance.DataCount;
                if (_rawTargetSystem == null)
                {
                    Task.Run(() =>
                    {
                        lock (D2D1CSX)
                        {
                            _targetSystem?.Dispose();
                            _targetSystem = new(CanvasDevice.GetSharedDevice(), defaultLength, defaultHeight, targetWindowDPI, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Ignore);
                            _rawTargetSystemData = new byte[_targetSystem.SizeInPixels.Width * _targetSystem.SizeInPixels.Height * 4];
                            _targetSystemData = _rawTargetSystemData.AsBuffer();
                            _rawTargetSystem?.Dispose();
                            _rawTargetSystem = new(CanvasDevice.GetSharedDevice(), defaultLength, defaultHeight, targetWindowDPI, DirectXPixelFormat.B8G8R8A8UIntNormalized, dataCount, CanvasAlphaMode.Ignore);
                        }
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.SetD2DView,
                            Contents = _rawTargetSystem
                        });
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.SetD2DViewArea
                        });
                    });
                }
                else if (_rawTargetSystem.Size.Width != defaultLength || _rawTargetSystem.Size.Height != defaultHeight || _drawingQuality != drawingQuality || _rawTargetSystem.BufferCount != dataCount)
                {
                    Task.Run(() =>
                    {
                        lock (D2D1CSX)
                        {
                            _targetSystem?.Dispose();
                            _targetSystem = new(CanvasDevice.GetSharedDevice(), defaultLength, defaultHeight, targetWindowDPI, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Ignore);
                            _rawTargetSystemData = new byte[_targetSystem.SizeInPixels.Width * _targetSystem.SizeInPixels.Height * 4];
                            _targetSystemData = _rawTargetSystemData.AsBuffer();
                            _rawTargetSystem.ResizeBuffers(defaultLength, defaultHeight, targetWindowDPI, DirectXPixelFormat.B8G8R8A8UIntNormalized, dataCount);
                        }
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.SetD2DViewArea
                        });
                    });
                }
                _drawingQuality = drawingQuality;
            })
        });

        public DrawingItem Load(string drawingFilePath, IDrawingContainer drawingContainer)
        {
            using var fs = File.OpenRead(drawingFilePath);
            return Load(fs, drawingContainer);
        }

        public DrawingItem Load(Stream s, IDrawingContainer drawingContainer, bool setAverage = false)
        {
            s.Position = 0;
            var hash = Utility.GetID128s(s);
            if (drawingContainer != null && _drawingMap.TryGetValue(drawingContainer, out var drawingItems) && drawingItems.TryGetValue(hash, out var drawingItem))
            {
                return drawingItem;
            }
            drawingItem = Load(s, setAverage);
            if (drawingContainer != null)
            {
                _drawingMap.AddOrUpdate(drawingContainer, (drawingContainer, drawingItem) => new(new[] { KeyValuePair.Create(hash, drawingItem) }), (drawingContainer, drawingItems, drawingItem) =>
                {
                    drawingItems[hash] = drawingItem;
                    return drawingItems;
                }, drawingItem);
            }
            return drawingItem;
        }

        public DrawingItem LoadBMS(string drawingFilePath, IDrawingContainer drawingContainer)
        {
            using var fs = File.OpenRead(drawingFilePath);
            var hash = $"@{Utility.GetID128s(fs)}";
            if (_drawingMap.TryGetValue(drawingContainer, out var drawingItems) && drawingItems.TryGetValue(hash, out var drawingItem))
            {
                return drawingItem;
            }

            drawingItem = Load(fs);
            var defaultBound = drawingItem.DrawingBound;
            drawingItem = new()
            {
                Drawing = new ChromaKeyEffect
                {
                    Source = drawingItem.Drawing,
                    Color = Colors.Black,
                    Tolerance = 1 / 512F
                },
                DrawingBound = defaultBound
            };

            var defaultLength = defaultBound.Length;
            var defaultHeight = defaultBound.Height;
            if (defaultLength != defaultHeight && defaultLength <= 256 && defaultHeight <= 256)
            {
                var areaLength = (float)Math.Max(defaultLength, defaultHeight);
                var target = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), areaLength, areaLength, 96F);
                using (var session = target.CreateDrawingSession())
                {
                    session.Clear(Colors.Transparent);
                    var r = new Bound((areaLength - defaultLength) / 2, 0.0, target.Size.Width, target.Size.Height);
                    session.PaintDrawing(ref r, drawingItem);
                }
                drawingItem.Dispose();
                drawingItem = new()
                {
                    Drawing = target,
                    DrawingBound = target.Bounds
                };
            }

            _drawingMap.AddOrUpdate(drawingContainer, (drawingContainer, drawingItem) => new(new[] { KeyValuePair.Create(hash, drawingItem) }), (drawingContainer, drawingItems, drawingItem) =>
            {
                drawingItems[hash] = drawingItem;
                return drawingItems;
            }, drawingItem);
            return drawingItem;
        }

        static DrawingItem Load(Stream s, bool setAverage = false)
        {
            s.Position = 0;
            var drawing = Utility.Await(CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), s.AsRandomAccessStream()));
            var averageColor = 0U;
            var averageHeight = 0.0;
            if (setAverage)
            {
                var averageCount = 0;
                var averageColor0 = 0L;
                var averageColor1 = 0L;
                var averageColor2 = 0L;
                var averageColor3 = 0L;
                var drawingColors = drawing.GetPixelColors();
                var drawingLength = drawing.SizeInPixels.Width;
                var drawingHeight = drawing.SizeInPixels.Height;
                var hasColors = ArrayPool<bool>.Shared.Rent((int)drawingHeight);
                Array.Clear(hasColors, 0, (int)drawingHeight);
                for (var i = (int)(drawingHeight - 1); i >= 0; --i)
                {
                    for (var j = (int)(drawingLength - 1); j >= 0; --j)
                    {
                        var drawingColor = drawingColors[drawingLength * i + j];
                        if (drawingColor.R > 0 || drawingColor.G > 0 || drawingColor.B > 0)
                        {
                            if (drawingColor.A > 0)
                            {
                                if (drawingColor.A > 127)
                                {
                                    averageColor0 += drawingColor.B;
                                    averageColor1 += drawingColor.G;
                                    averageColor2 += drawingColor.R;
                                    averageColor3 += drawingColor.A;
                                    ++averageCount;
                                }
                                hasColors[i] = true;
                            }
                        }
                    }
                }
                if (averageCount > 0)
                {
                    averageColor0 /= averageCount;
                    averageColor1 /= averageCount;
                    averageColor2 /= averageCount;
                    averageColor3 /= averageCount;
                    averageColor = (uint)(16777216 * averageColor0 + 65536 * averageColor1 + 256 * averageColor2 + averageColor3);
                }
                var lowestPosition1 = 0;
                var highestPosition1 = 0;
                for (var i = 0; i < drawingHeight; ++i)
                {
                    if (hasColors[i])
                    {
                        lowestPosition1 = i;
                        break;
                    }
                }
                for (var i = (int)(drawingHeight - 1); i >= 0; --i)
                {
                    if (hasColors[i])
                    {
                        highestPosition1 = i + 1;
                        break;
                    }
                }
                ArrayPool<bool>.Shared.Return(hasColors);
                averageHeight = (double)(highestPosition1 - lowestPosition1) / drawingHeight;
            }
            return new()
            {
                Drawing = drawing,
                DrawingBound = drawing.Bounds,
                AverageColor = averageColor,
                StandardHeight = averageHeight
            };
        }

        public ImageSource LoadDefault(string drawingFilePath, IDrawingContainer drawingContainer)
        {
            using var fs = File.OpenRead(drawingFilePath);
            return LoadDefault(fs, drawingContainer);
        }

        public ImageSource LoadDefault(Stream s, IDrawingContainer drawingContainer)
        {
            if (drawingContainer != null)
            {
                var hash = Utility.GetID128s(s);
                if (_defaultDrawingMap.TryGetValue(drawingContainer, out var defaultDrawings) && defaultDrawings.TryGetValue(hash, out var defaultDrawing))
                {
                    return defaultDrawing;
                }
                defaultDrawing = LoadDefault(s);
                _defaultDrawingMap.AddOrUpdate(drawingContainer, (drawingContainer, defaultDrawing) => new(new[] { KeyValuePair.Create(hash, defaultDrawing) }), (drawingContainer, defaultDrawings, defaultDrawing) =>
                {
                    defaultDrawings[hash] = defaultDrawing;
                    return defaultDrawings;
                }, defaultDrawing);
                return defaultDrawing;
            }
            else
            {
                return LoadDefault(s);
            }
        }

        public ImageSource LoadDefaultBMS(string drawingFilePath, IDrawingContainer drawingContainer)
        {
            using var fs = File.OpenRead(drawingFilePath);
            if (drawingContainer != null)
            {
                var hash = $"@{Utility.GetID128s(fs)}";
                if (_defaultDrawingMap.TryGetValue(drawingContainer, out var defaultDrawings) && defaultDrawings.TryGetValue(hash, out var defaultDrawing))
                {
                    return defaultDrawing;
                }
                defaultDrawing = LoadImpl();
                _defaultDrawingMap.AddOrUpdate(drawingContainer, (drawingContainer, defaultDrawing) => new(new[] { KeyValuePair.Create(hash, defaultDrawing) }), (drawingContainer, defaultDrawings, defaultDrawing) =>
                {
                    defaultDrawings[hash] = defaultDrawing;
                    return defaultDrawings;
                }, defaultDrawing);
                return defaultDrawing;
            }
            else
            {
                return LoadImpl();
            }

            BitmapSource LoadImpl()
            {
                BitmapSource defaultDrawing;

                fs.Position = 0;
                var drawing = new BitmapImage();
                drawing.BeginInit();
                drawing.CacheOption = BitmapCacheOption.OnLoad;
                drawing.StreamSource = fs;
                drawing.EndInit();
                drawing.Freeze();
                defaultDrawing = drawing;

                if (defaultDrawing.Format != PixelFormats.Bgra32)
                {
                    var formatDrawing = new FormatConvertedBitmap();
                    formatDrawing.BeginInit();
                    formatDrawing.Source = defaultDrawing;
                    formatDrawing.DestinationFormat = PixelFormats.Bgra32;
                    formatDrawing.EndInit();
                    formatDrawing.Freeze();
                    defaultDrawing = formatDrawing;
                }

                var defaultLength = defaultDrawing.PixelWidth;
                var defaultHeight = defaultDrawing.PixelHeight;
                var row = 4 * defaultLength;
                var data = ArrayPool<byte>.Shared.Rent(row * defaultHeight);
                try
                {
                    defaultDrawing.CopyPixels(data, row, 0);

                    for (var i = defaultHeight - 1; i >= 0; --i)
                    {
                        for (var j = defaultLength - 1; j >= 0; --j)
                        {
                            var m = row * i + 4 * j;
                            if (data[m] == 0 && data[m + 1] == 0 && data[m + 2] == 0)
                            {
                                data[m + 3] = 0;
                            }
                        }
                    }

                    if (defaultLength != defaultHeight && defaultLength <= 256 && defaultHeight <= 256)
                    {
                        var targetValue = Math.Max(defaultLength, defaultHeight);
                        var rowTarget = 4 * targetValue;
                        var target = ArrayPool<byte>.Shared.Rent(rowTarget * targetValue);
                        try
                        {
                            var distanceLength = (targetValue - defaultLength) / 2;

                            for (var i = targetValue - 1; i >= 0; --i)
                            {
                                for (var j = targetValue - 1; j >= 0; --j)
                                {
                                    if (i < defaultHeight && distanceLength <= j && j < targetValue - distanceLength)
                                    {
                                        var m = row * i + 4 * (j - distanceLength);
                                        if (data[m] == 0 && data[m + 1] == 0 && data[m + 2] == 0)
                                        {
                                            data[m + 3] = 0;
                                        }
                                        var mTarget = rowTarget * i + 4 * j;
                                        target[mTarget] = data[m];
                                        target[mTarget + 1] = data[m + 1];
                                        target[mTarget + 2] = data[m + 2];
                                        target[mTarget + 3] = data[m + 3];
                                    }
                                    else
                                    {
                                        var m = rowTarget * i + 4 * j;
                                        target[m] = 0;
                                        target[m + 1] = 0;
                                        target[m + 2] = 0;
                                        target[m + 3] = 0;
                                    }
                                }
                            }

                            defaultDrawing = BitmapSource.Create(targetValue, targetValue, defaultDrawing.DpiX, defaultDrawing.DpiY, defaultDrawing.Format, null, target, rowTarget);
                            defaultDrawing.Freeze();
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(target);
                        }
                    }
                    else
                    {
                        defaultDrawing = BitmapSource.Create(defaultLength, defaultHeight, defaultDrawing.DpiX, defaultDrawing.DpiY, defaultDrawing.Format, null, data, row);
                        defaultDrawing.Freeze();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }

                return defaultDrawing;
            }
        }

        static ImageSource LoadDefault(Stream s)
        {
            s.Position = 0;
            var defaultDrawing = new BitmapImage();
            defaultDrawing.BeginInit();
            defaultDrawing.CacheOption = BitmapCacheOption.OnLoad;
            defaultDrawing.StreamSource = s;
            defaultDrawing.EndInit();
            defaultDrawing.Freeze();
            return defaultDrawing;
        }

        public Brush GetDefaultPaint(string titleColor)
        {
            switch (titleColor)
            {
                case "titleLV2000":
                    var titleLV2000 = new LinearGradientBrush(new(new GradientStop[]
                    {
                        new (System.Windows.Media.Colors.Red, 0.0),
                        new (System.Windows.Media.Colors.Orange, 1.0 / 6),
                        new (System.Windows.Media.Colors.Yellow, 2.0 / 6),
                        new (System.Windows.Media.Colors.Green, 3.0 / 6),
                        new (System.Windows.Media.Colors.Blue, 4.0 / 6),
                        new (System.Windows.Media.Colors.Indigo, 5.0 / 6),
                        new (System.Windows.Media.Colors.Purple, 1.0)
                    }));
                    titleLV2000.Freeze();
                    return titleLV2000;
                default:
                    return null;
            }
        }

        public Brush GetDefaultPaint(Color valueColor, int faint = 100)
        {
            var defaultPaint = new SolidColorBrush(Utility.ModifyColor(valueColor))
            {
                Opacity = faint / 100.0
            };
            defaultPaint.Freeze();
            return defaultPaint;
        }

        public Pen GetPen(Brush defaultPaint)
        {
            var defaultPen = new Pen(defaultPaint, 1.0);
            defaultPen.Freeze();
            return defaultPen;
        }

        public void Close(IDrawingContainer drawingContainer)
        {
            if (_drawingMap.TryRemove(drawingContainer, out var drawingItems))
            {
                foreach (var drawingItem in drawingItems.Values)
                {
                    drawingItem.Dispose();
                }
            }
            _defaultDrawingMap.TryRemove(drawingContainer, out _);
            if (_toCloseValues.TryRemove(drawingContainer, out var toCloseValues))
            {
                foreach (var toCloseValue in toCloseValues)
                {
                    toCloseValue.Dispose();
                }
            }
        }

        public void Migrate(IDrawingContainer src, IDrawingContainer target)
        {
            if (_drawingMap.TryRemove(src, out var drawingItems))
            {
                _drawingMap[target] = drawingItems;
            }
            if (_defaultDrawingMap.TryRemove(src, out var defaultDrawings))
            {
                _defaultDrawingMap[target] = defaultDrawings;
            }
            if (_toCloseValues.TryRemove(src, out var toCloseValues))
            {
                _toCloseValues[target] = toCloseValues;
            }
        }

        public void SetFontFamily()
        {
            SetFontFamily(MeterFont);
            SetFontLevel(MeterFont, Levels.FontLevel1Float32);
            SetFontFamily(UtilityFont);
            SetFontLevel(UtilityFont, Levels.FontLevel0Float32);
            SetFontSystem(UtilityFont, 1, 0);
            SetFontFamily(NotifyXamlFont);
            SetFontLevel(NotifyXamlFont, Levels.FontLevel0Float32);
            SetFontSystem(NotifyXamlFont, 1, 2);
            SetFontFamily(NoteItemFont);
            SetFontLevel(NoteItemFont, Levels.FontLevel1Float32);
            SetFontFamily(InputAssistFont);
            SetFontLevel(InputAssistFont, Levels.FontLevel1Float32);
            SetFontFamily(PauseNotifyFont);
            SetFontLevel(PauseNotifyFont, Levels.FontLevel0Float32);
            SetFontFamily(NetFont);
            SetFontLevel(NetFont, Levels.FontLevel0Float32);
            SetFontFamily(JudgmentMeterViewFont);
            SetFontLevel(JudgmentMeterViewFont, Levels.FontLevel0Float32);
            SetFontFamily(StatusViewFont);
            SetFontLevel(StatusViewFont, Levels.FontLevel0Float32);
        }

        public virtual CanvasTextFormat GetFont() => new()
        {
            WordWrapping = CanvasWordWrapping.NoWrap
        };

        public virtual void SetFontLevel(CanvasTextFormat font, float fontLevel)
        {
            font.FontSize = fontLevel;
        }

        public virtual void SetFontSystem(CanvasTextFormat font, int fontSystem0, int fontSystem1)
        {
            font.HorizontalAlignment = fontSystem0 switch
            {
                0 => CanvasHorizontalAlignment.Right,
                1 => CanvasHorizontalAlignment.Left,
                2 => CanvasHorizontalAlignment.Center,
                _ => default,
            };
            font.VerticalAlignment = fontSystem1 switch
            {
                0 => CanvasVerticalAlignment.Top,
                1 => CanvasVerticalAlignment.Bottom,
                2 => CanvasVerticalAlignment.Center,
                _ => default,
            };
        }

        public virtual void SetFontFamily(CanvasTextFormat font)
        {
            font.FontFamily = Configure.Instance.GetAFontFamily();
        }
    }
}