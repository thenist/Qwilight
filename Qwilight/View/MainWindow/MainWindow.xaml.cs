using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.Win32;
using Qwilight.Compute;
using Qwilight.MSG;
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
    public sealed partial class MainWindow
    {
        [LibraryImport("NVIDIA")]
        private static partial void NotifyNVLL(uint statsWindowMessage);

        readonly HWND _handle;
        readonly WNDPROC _onWin32;
        readonly Dictionary<uint, Point> _pointIDPointPositionMap = new();
        readonly DesktopWindowXamlSource _windowXamlView = new();
        readonly DesktopChildSiteBridge _siteView;
        readonly Viewbox _mainView;
        readonly CanvasSwapChainPanel _d2DView;

        public MainWindow()
        {
            InitializeComponent();

            _handle = (HWND)new WindowInteropHelper(this).EnsureHandle();
            _windowXamlView.Initialize(Win32Interop.GetWindowIdFromWindow(_handle));
            _d2DView = new();
            _mainView = new()
            {
                Child = _d2DView
            };
            _mainView.PointerPressed += OnD2DPointLower;
            _mainView.PointerMoved += OnD2DPointMove;
            _mainView.PointerReleased += OnD2DPointHigher;
            _mainView.PointerEntered += OnD2DPointEnter;
            _mainView.PointerExited += OnD2DPointExit;
            _mainView.PointerWheelChanged += OnD2DPointSpin;
            _windowXamlView.Content = _mainView;
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
#if X64
            PInvoke.SetWindowText(_handle, "Qwilight AMD64 α");
#endif
#if ARM64
            PInvoke.SetWindowText(_handle, "Qwilight ARM64 α");
#endif
#else
#if X64
            PInvoke.SetWindowText(_handle, QwilightComponent.IsVS ? "Qwilight AMD64 β" : "Qwilight AMD64");
#endif
#if ARM64
            PInvoke.SetWindowText(_handle, QwilightComponent.IsVS ? "Qwilight ARM64 β" : "Qwilight ARM64");
#endif
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

            StrongReferenceMessenger.Default.Register<SetWindowedMode>(this, (recipient, message) =>
            {
                UIHandler.Instance.HandleParallel(() =>
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
            });
            StrongReferenceMessenger.Default.Register<ViewAllowWindow>(this, (recipient, message) => message.Reply(PInvoke.MessageBox(_handle, message.Text, "Qwilight", (MESSAGEBOX_STYLE)message.Data)));
            StrongReferenceMessenger.Default.Register<ViewEntryWindow>(this, (recipient, message) =>
            {
                var entryWindow = new OpenFolderDialog();
                message.Reply(entryWindow.ShowDialog() == true ? entryWindow.FolderName : null);
            });
            StrongReferenceMessenger.Default.Register<ViewFileWindow>(this, (recipient, message) =>
            {
                var fileWindow = new FileOpenPicker();
                InitializeWithWindow.Initialize(fileWindow, _handle);
                foreach (var filter in message.Filters)
                {
                    fileWindow.FileTypeFilter.Add(filter);
                }
                message.Reply(fileWindow.PickSingleFileAsync().AsTask().ContinueWith(file => file.Result?.Path));
            });
            StrongReferenceMessenger.Default.Register<Quit>(this, (recipient, message) => PInvoke.PostMessage(_handle, PInvoke.WM_CLOSE, message.ViewAllowWindow ? (WPARAM)1 : (WPARAM)0, (LPARAM)0));
            StrongReferenceMessenger.Default.Register<SetWindowArea>(this, (recipient, message) =>
            {
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
            });
            StrongReferenceMessenger.Default.Register<GetWindowArea>(this, (recipient, message) => message.Reply(GetWindowArea()));
            StrongReferenceMessenger.Default.Register<GetWindowHandle>(this, (recipient, message) => message.Reply(_handle));
            StrongReferenceMessenger.Default.Register<SetD2DView>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() => _d2DView.SwapChain = message.D2DView));
            StrongReferenceMessenger.Default.Register<SetD2DViewArea>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
            {
                var d2DArea = _d2DView.SwapChain.Size;
                _d2DView.Width = d2DArea.Width;
                _d2DView.Height = d2DArea.Height;
                var windowArea = GetWindowArea();
                _siteView.MoveAndResize(new(0, 0, windowArea.Width, windowArea.Height));
            }));
            StrongReferenceMessenger.Default.Register<SetD2DViewVisibility>(this, (recipient, message) => SetD2DViewVisibility(message.IsVisible));
        }

        void OnEssentialInputLower(object sender, KeyEventArgs e) => ViewModels.Instance.MainValue.OnEssentialInputLower(e);

        void OnInputLower(object sender, KeyEventArgs e) => ViewModels.Instance.MainValue.OnInputLower(e);

        void OnPointLower(object sender, MouseButtonEventArgs e) => ViewModels.Instance.MainValue.OnPointLower(e);

        void OnFileAs(object sender, DragEventArgs e) => ViewModels.Instance.MainValue.OnFileAs(e);

        void OnLoaded(object sender, EventArgs e)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            mainViewModel.OnWindowDPIModified(PInvoke.GetDpiForWindow(_handle) / 96.0);
            _ = mainViewModel.OnLoaded(_handle);
        }

        void SetD2DViewVisibility(bool isVisible)
        {
            if (isVisible)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    _siteView.Show();
                    _mainView.Focus(FocusState.Pointer);
                });
            }
            else
            {
                UIHandler.Instance.HandleParallel(_siteView.Hide);
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
            var inputCount1P = defaultComputer.InputCount1P;
            var mainNoteLengthLevyingMap = drawingComponentValue.MainNoteLengthLevyingMap;
            var mainNoteLengthBuiltMap = drawingComponentValue.MainNoteLengthBuiltMap;
            if (0.0 <= positionX && positionX < p1BuiltLength)
            {
                for (var input = inputCount1P; input > 0; --input)
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
                    for (var input = inputCount; input > inputCount1P; --input)
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
            if (d2DAreaLength / windowAreaLength > d2DAreaHeight / windowAreaHeight)
            {
                return new Point(point.X * d2DAreaLength * windowDPI / windowAreaLength, point.Y * d2DAreaLength * windowDPI / windowAreaLength);
            }
            else
            {
                return new Point(point.X * d2DAreaHeight * windowDPI / windowAreaHeight, point.Y * d2DAreaHeight * windowDPI / windowAreaHeight);
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