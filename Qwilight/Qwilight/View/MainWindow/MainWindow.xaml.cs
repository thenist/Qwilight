using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Qwilight.Compute;
using Qwilight.ViewModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage.Pickers;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ResizeMode = System.Windows.ResizeMode;
using UIElement = Microsoft.UI.Xaml.UIElement;
using WindowState = System.Windows.WindowState;
using WindowStyle = System.Windows.WindowStyle;

namespace Qwilight.View
{
    public sealed partial class MainWindow : IRecipient<ICC>
    {
        [LibraryImport("NVIDIA")]
        private static partial void NotifyNVLL(uint statsWindowMessage);

        readonly HWND _handle;
        readonly WNDPROC _onWin32;
        readonly Dictionary<uint, Point> _pointIDPointPositionMap = new();
        readonly DesktopWindowXamlSource _windowXamlView = new();
        readonly DesktopChildSiteBridge _siteView;
        readonly CanvasSwapChainPanel _d2DView;

        public MainWindow()
        {
            InitializeComponent();

            _handle = (HWND)new WindowInteropHelper(this).EnsureHandle();
            _windowXamlView.Initialize(Win32Interop.GetWindowIdFromWindow(_handle));
            _d2DView = new CanvasSwapChainPanel();
            _d2DView.PointerPressed += OnD2DPointLower;
            _d2DView.PointerMoved += OnD2DPointMove;
            _d2DView.PointerReleased += OnD2DPointHigher;
            _d2DView.PointerEntered += OnD2DPointEnter;
            _d2DView.PointerExited += OnD2DPointExit;
            _d2DView.PointerWheelChanged += OnD2DPointSpin;
            _windowXamlView.Content = _d2DView;
            _windowXamlView.SystemBackdrop = new MicaBackdrop
            {
                Kind = MicaKind.BaseAlt
            };
            _siteView = _windowXamlView.SiteBridge;
            SetD2DViewVisibility(false);

            var pvAttribute = 1;
            unsafe
            {
                PInvoke.DwmSetWindowAttribute(_handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &pvAttribute, sizeof(int));
            }

            var windowInfo = new WINDOWINFO();
            PInvoke.GetWindowInfo(_handle, ref windowInfo);
            var windowDPI = PInvoke.GetDpiForWindow(_handle) / 96.0;
            var windowPosition0 = Configure.Instance.WindowPosition0V2;
            var windowPosition1 = Configure.Instance.WindowPosition1V2;
            var windowLength = (int)(windowDPI * Configure.Instance.WindowLengthV2 + (windowInfo.rcWindow.right - windowInfo.rcWindow.left) - (windowInfo.rcClient.right - windowInfo.rcClient.left));
            var windowHeight = (int)(windowDPI * Configure.Instance.WindowHeightV2 + (windowInfo.rcWindow.bottom - windowInfo.rcWindow.top) - (windowInfo.rcClient.bottom - windowInfo.rcClient.top));
            if (windowPosition0 <= -windowLength)
            {
                windowPosition0 = 0;
            }
            if (windowPosition1 <= -windowHeight)
            {
                windowPosition1 = 0;
            }
            PInvoke.SetWindowPos(_handle, HWND.Null, windowPosition0, windowPosition1, windowLength, windowHeight, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

#if DEBUG
            PInvoke.SetWindowText(_handle, "Qwilight α");
#else
            PInvoke.SetWindowText(_handle, QwilightComponent.IsVS ? "Qwilight β" : "Qwilight");
#endif

            var mainViewModel = ViewModels.Instance.MainValue;
            WNDPROC lpPrevWndFunc = null;
            unsafe
            {
                _onWin32 = (hWnd, Msg, wParam, lParam) =>
                {
                    switch (Msg)
                    {
                        case PInvoke.WM_CLOSE:
                            if (wParam != 0 && PInvoke.MessageBox(_handle, LanguageSystem.Instance.QuitContents, LanguageSystem.Instance.QuitText, MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON2) != MESSAGEBOX_RESULT.IDYES)
                            {
                                return (LRESULT)0;
                            }
                            break;
                        case PInvoke.WM_DESTROY:
                            mainViewModel.Close();
                            break;
                        case PInvoke.WM_SIZE when lParam != 0:
                            mainViewModel.OnModified();
                            break;
                        case PInvoke.WM_MOVE:
                            mainViewModel.OnMove();
                            break;
                        case PInvoke.WM_DPICHANGED:
                            mainViewModel.OnWindowDPIModified(PInvoke.GetDpiForWindow(_handle) / 96.0);
                            break;
                        case PInvoke.WM_ACTIVATE:
                            var wa = wParam & 65535;
                            mainViewModel.OnSetPoint(wa != 0);
                            break;
                        default:
                            if (Configure.Instance.IsNVLL)
                            {
                                NotifyNVLL(Msg);
                            }
                            break;
                    }
                    return PInvoke.CallWindowProc(lpPrevWndFunc, hWnd, Msg, wParam, lParam);
                };
            }
            lpPrevWndFunc = Marshal.GetDelegateForFunctionPointer<WNDPROC>(PInvoke.SetWindowLongPtr(_handle, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate<WNDPROC>(_onWin32)));

            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        void OnEssentialInputLower(object sender, KeyEventArgs e) => ViewModels.Instance.MainValue.OnEssentialInputLower(e);

        void OnInputLower(object sender, KeyEventArgs e) => ViewModels.Instance.MainValue.OnInputLower(e);

        void OnPointLower(object sender, MouseButtonEventArgs e) => ViewModels.Instance.MainValue.OnPointLower(e);

        void OnFileAs(object sender, DragEventArgs e) => ViewModels.Instance.MainValue.OnFileAs(e);

        void OnLoaded(object sender, EventArgs e)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            mainViewModel.OnWindowDPIModified(PInvoke.GetDpiForWindow(_handle) / 96.0);
            mainViewModel.OnLoaded(_handle);
        }

        public async void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.ViewAllowWindow:
                    var data = message.Contents as object[];
                    (data[2] as Action<MESSAGEBOX_RESULT>)(PInvoke.MessageBox(_handle, data[0] as string, "Qwilight", (MESSAGEBOX_STYLE)data[1]));
                    break;
                case ICC.ID.ViewEntryWindow:
                    var fp = new FolderPicker();
                    InitializeWithWindow.Initialize(fp, _handle);
                    fp.FileTypeFilter.Add("*");
                    var entry = await fp.PickSingleFolderAsync();
                    if (entry != null)
                    {
                        (message.Contents as Action<string>)(entry.Path);
                    }
                    break;
                case ICC.ID.ViewFileWindow:
                    data = message.Contents as object[];
                    var fop = new FileOpenPicker();
                    InitializeWithWindow.Initialize(fop, _handle);
                    foreach (var dataItem in data[0] as IEnumerable<string>)
                    {
                        fop.FileTypeFilter.Add(dataItem);
                    }
                    if (fop.FileTypeFilter.Count == 0)
                    {
                        fop.FileTypeFilter.Add("*");
                    }
                    var file = await fop.PickSingleFileAsync();
                    if (file != null)
                    {
                        (data[1] as Action<string>)(file.Path);
                    }
                    break;
                case ICC.ID.Quit:
                    PInvoke.PostMessage(_handle, PInvoke.WM_CLOSE, (bool)message.Contents ? (WPARAM)1 : (WPARAM)0, (LPARAM)0);
                    break;
                case ICC.ID.ViewPwWindow:
                    data = message.Contents as object[];
                    ViewModels.Instance.InputPwValue.Text = data[0] as string;
                    ViewModels.Instance.InputPwValue.Input = data[1] as string;
                    ViewModels.Instance.InputPwValue.IsInputEditable = (bool)data[2];
                    ViewModels.Instance.InputPwValue.Handler = data[3] as Action<string, string>;
                    ViewModels.Instance.InputPwValue.Open();
                    break;
                case ICC.ID.ViewInputWindow:
                    data = message.Contents as object[];
                    ViewModels.Instance.InputTextValue.Text = data[0] as string;
                    ViewModels.Instance.InputTextValue.Input = data[1] as string;
                    ViewModels.Instance.InputTextValue.Handler = data[2] as Action<string>;
                    ViewModels.Instance.InputTextValue.Open();
                    break;
                case ICC.ID.SetWindowedMode:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        if (Configure.Instance.WindowedMode)
                        {
                            ResizeMode = ResizeMode.CanResize;
                            WindowStyle = WindowStyle.SingleBorderWindow;
                            WindowState = WindowState.Normal;
                        }
                        else
                        {
                            ResizeMode = ResizeMode.NoResize;
                            WindowStyle = WindowStyle.None;
                            WindowState = WindowState.Maximized;
                        }
                    });
                    break;
                case ICC.ID.SetWindowArea:
                    var windowInfo = new WINDOWINFO
                    {
                        cbSize = (uint)Marshal.SizeOf<WINDOWINFO>()
                    };
                    PInvoke.GetWindowInfo(_handle, ref windowInfo);
                    var windowDPI = PInvoke.GetDpiForWindow(_handle) / 96.0;
                    PInvoke.SetWindowPos(_handle, HWND.Null, 0, 0,
                        (int)(windowDPI * Configure.Instance.WindowLengthV2 + (windowInfo.rcWindow.right - windowInfo.rcWindow.left) - (windowInfo.rcClient.right - windowInfo.rcClient.left)),
                        (int)(windowDPI * Configure.Instance.WindowHeightV2 + (windowInfo.rcWindow.bottom - windowInfo.rcWindow.top) - (windowInfo.rcClient.bottom - windowInfo.rcClient.top)),
                        SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                    );
                    break;
                case ICC.ID.GetWindowArea:
                    {
                        var windowArea = GetWindowArea();
                        (message.Contents as Action<int, int, int, int>)(windowArea.X, windowArea.Y, windowArea.Width, windowArea.Height);
                    }
                    break;
                case ICC.ID.GetWindowHandle:
                    (message.Contents as Action<HWND>)(_handle);
                    break;
                case ICC.ID.SetD2DView:
                    HandlingUISystem.Instance.HandleParallel(() => _d2DView.SwapChain = message.Contents as CanvasSwapChain);
                    break;
                case ICC.ID.SetD2DViewArea:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        var windowArea = GetWindowArea();
                        var windowAreaLength = windowArea.Width;
                        var windowAreaHeight = windowArea.Height;
                        var defaultLength = _d2DView.SwapChain.Size.Width;
                        var defaultHeight = _d2DView.SwapChain.Size.Height;
                        var windowDPI = PInvoke.GetDpiForWindow(_handle) / 96.0;
                        if (Configure.Instance.IsQwilightFill)
                        {
                            _d2DView.Width = windowAreaLength / windowDPI;
                            _d2DView.Height = windowAreaHeight / windowDPI;
                        }
                        else
                        {
                            if (defaultLength / windowAreaLength > defaultHeight / windowAreaHeight)
                            {
                                _d2DView.Width = windowAreaLength / windowDPI;
                                _d2DView.Height = (windowAreaLength * defaultHeight / defaultLength) / windowDPI;
                            }
                            else
                            {
                                _d2DView.Width = (windowAreaHeight * defaultLength / defaultHeight) / windowDPI;
                                _d2DView.Height = windowAreaHeight / windowDPI;
                            }
                        }
                        _siteView.MoveAndResize(new RectInt32(0, 0, windowAreaLength, windowAreaHeight));
                    });
                    break;
                case ICC.ID.SetD2DViewVisibility:
                    HandlingUISystem.Instance.HandleParallel(() => SetD2DViewVisibility((bool)message.Contents));
                    break;
            }
        }

        void SetD2DViewVisibility(bool isVisible)
        {
            if (isVisible)
            {
                _siteView.Enable();
                _siteView.Show();
            }
            else
            {
                _siteView.Hide();
                _siteView.Disable();
            }
        }

        void OnD2DPointLower(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                var point = e.GetCurrentPoint(sender as UIElement);
                DrawingSystem.Instance.LastPointedQueue.Enqueue((GetPointPosition(point.Position), point.Properties.IsRightButtonPressed));
            }
            catch (COMException)
            {
            }
        }

        void OnD2DPointMove(object sender, PointerRoutedEventArgs e)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            try
            {
                var point = e.GetCurrentPoint(sender as UIElement);
                var pointPosition = GetPointPosition(point.Position);
                if (point.PointerDeviceType == PointerDeviceType.Touch)
                {
                    var pointID = point.PointerId;
                    var lastPointPosition = _pointIDPointPositionMap[pointID];
                    _pointIDPointPositionMap[pointID] = pointPosition;
                    var defaultComputer = mainViewModel.Computer;
                    var lastInput = GetInput(lastPointPosition, defaultComputer);
                    var input = GetInput(pointPosition, defaultComputer);
                    if (lastInput != input)
                    {
                        if (lastInput > 0)
                        {
                            defaultComputer.Input(-lastInput, DefaultCompute.InputFlag.Pointer);
                        }
                        if (input > 0)
                        {
                            defaultComputer.Input(input, DefaultCompute.InputFlag.Pointer);
                        }
                    }
                }
                DrawingSystem.Instance.LastMovedQueue.Enqueue(pointPosition);
            }
            catch (COMException)
            {
            }
        }

        void OnD2DPointHigher(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                DrawingSystem.Instance.LastNotPointedQueue.Enqueue(GetPointPosition(e.GetCurrentPoint(sender as UIElement).Position));
            }
            catch (COMException)
            {
            }
        }

        void OnD2DPointEnter(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                var point = e.GetCurrentPoint(sender as UIElement);
                if (point.PointerDeviceType == PointerDeviceType.Touch)
                {
                    var pointPosition = GetPointPosition(point.Position);
                    _pointIDPointPositionMap[point.PointerId] = pointPosition;
                    var defaultComputer = ViewModels.Instance.MainValue.Computer;
                    var input = GetInput(pointPosition, defaultComputer);
                    if (input > 0)
                    {
                        defaultComputer.Input(input, DefaultCompute.InputFlag.Pointer);
                    }
                }
            }
            catch (COMException)
            {
            }
        }

        void OnD2DPointExit(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                var point = e.GetCurrentPoint(sender as UIElement);
                if (point.PointerDeviceType == PointerDeviceType.Touch)
                {
                    _pointIDPointPositionMap.Remove(point.PointerId);
                    var defaultComputer = ViewModels.Instance.MainValue.Computer;
                    var input = GetInput(GetPointPosition(point.Position), defaultComputer);
                    if (input > 0)
                    {
                        defaultComputer.Input(-input, DefaultCompute.InputFlag.Pointer);
                    }
                }
            }
            catch (COMException)
            {
            }
        }

        static int GetInput(Point pointPosition, DefaultCompute defaultComputer)
        {
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var p1BuiltLength = drawingComponentValue.p1BuiltLength;
            var positionX = pointPosition.X - drawingComponentValue.mainPosition;
            var inputMode = defaultComputer.InputMode;
            var input1PCount = defaultComputer.Input1PCount;
            var mainNoteLengthLevyingMap = drawingComponentValue.MainNoteLengthLevyingMap;
            var mainNoteLengthBuiltMap = drawingComponentValue.MainNoteLengthBuiltMap;
            if (0.0 <= positionX && positionX < p1BuiltLength)
            {
                for (var input = input1PCount; input > 0; --input)
                {
                    if (mainNoteLengthLevyingMap[input] <= positionX && positionX < mainNoteLengthBuiltMap[input])
                    {
                        return input;
                    }
                }
            }
            else if (defaultComputer.Has2P)
            {
                positionX -= drawingComponentValue.p2Position;
                var inputCount = Component.InputCounts[(int)inputMode];
                if (p1BuiltLength <= positionX && positionX < drawingComponentValue.p2BuiltLength)
                {
                    for (var input = inputCount; input > input1PCount; --input)
                    {
                        if (mainNoteLengthLevyingMap[input] <= positionX && positionX < mainNoteLengthBuiltMap[input])
                        {
                            return input;
                        }
                    }
                }
            }
            return 0;
        }

        Point GetPointPosition(Point point)
        {
            var windowDPI = PInvoke.GetDpiForWindow(_handle) / 96.0;
            var windowArea = GetWindowArea();
            var windowAreaLength = windowArea.Width;
            var windowAreaHeight = windowArea.Height;
            var d2DArea = _d2DView.SwapChain.Size;
            var d2DAreaLength = d2DArea.Width;
            var d2DAreaHeight = d2DArea.Height;
            if (Configure.Instance.IsQwilightFill)
            {
                return new Point(point.X * d2DAreaLength * windowDPI / windowAreaLength, point.Y * d2DAreaHeight * windowDPI / windowAreaHeight);
            }
            else
            {
                if (d2DAreaLength / windowAreaLength > d2DAreaHeight / windowAreaHeight)
                {
                    return new Point(point.X * d2DAreaLength * windowDPI / windowAreaLength, point.Y * d2DAreaLength * windowDPI / windowAreaLength);
                }
                else
                {
                    return new Point(point.X * d2DAreaHeight * windowDPI / windowAreaHeight, point.Y * d2DAreaHeight * windowDPI / windowAreaHeight);
                }
            }
        }

        void OnD2DPointSpin(object sender, PointerRoutedEventArgs e)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var pointProperties = e.GetCurrentPoint(sender as UIElement).Properties;
            var isHigher = pointProperties.MouseWheelDelta > 0;
            if (!pointProperties.IsHorizontalMouseWheel && mainViewModel.Computer.IsPausingWindowOpened)
            {
                if (isHigher)
                {
                    mainViewModel.HigherDefaultSpinningMode();
                }
                else
                {
                    mainViewModel.LowerDefaultSpinningMode();
                }
            }
        }

        RectInt32 GetWindowArea()
        {
            var windowInfo = new WINDOWINFO
            {
                cbSize = (uint)Marshal.SizeOf<WINDOWINFO>()
            };
            PInvoke.GetWindowInfo(_handle, ref windowInfo);
            return new RectInt32(windowInfo.rcClient.left, windowInfo.rcClient.top, windowInfo.rcClient.right - windowInfo.rcClient.left, windowInfo.rcClient.bottom - windowInfo.rcClient.top);
        }
    }
}