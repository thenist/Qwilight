using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight
{
    public sealed class DefaultSystem : IDisposable
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(DefaultSystem));

        readonly IHandleInput _handleInput;
        readonly byte[] _rawData = new byte[40];
        readonly object _handlerCSX = new();
        HWND _handle;

        public DefaultSystem(IHandleInput handleInput) => _handleInput = handleInput;

        public void HandleSystem() => Utility.HandleParallelly(() =>
        {
            unsafe
            {
                fixed (char* lpszClassName = "Qwilight")
                {
                    var wcx = new WNDCLASSEXW
                    {
                        cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                        hInstance = (HINSTANCE)Process.GetCurrentProcess().Handle,
                        lpszClassName = lpszClassName,
                        lpfnWndProc = OnWMInput
                    };
                    PInvoke.RegisterClassEx(wcx);
                    _handle = PInvoke.CreateWindowEx(WINDOW_EX_STYLE.WS_EX_OVERLAPPEDWINDOW, lpszClassName, null, WINDOW_STYLE.WS_OVERLAPPEDWINDOW, PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, HWND.Null, HMENU.Null, wcx.hInstance);
                }
            }

            PInvoke.RegisterRawInputDevices(stackalloc RAWINPUTDEVICE[]
            {
                new()
                {
                    usUsagePage = 1,
                    usUsage = 6,
                    dwFlags = RAWINPUTDEVICE_FLAGS.RIDEV_INPUTSINK,
                    hwndTarget = _handle
                }
            }, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());

            while (PInvoke.GetMessage(out var lpMsg, _handle, PInvoke.WM_INPUT, PInvoke.WM_INPUT) != 0)
            {
                try
                {
                    PInvoke.TranslateMessage(lpMsg);
                    PInvoke.DispatchMessage(lpMsg);
                }
                catch (Exception e)
                {
                    Utility.SaveFaultFile(FaultEntryPath, e);
                }
            }
        });

        LRESULT OnWMInput(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
        {
            switch (Msg)
            {
                case PInvoke.WM_INPUT:
                    if (ViewModels.Instance.MainValue.HasPoint)
                    {
                        var dataLength = (uint)_rawData.Length;
                        unsafe
                        {
                            fixed (byte* pData = _rawData)
                            {
                                PInvoke.GetRawInputData((HRAWINPUT)lParam.Value, RAW_INPUT_DATA_COMMAND_FLAGS.RID_INPUT, pData, &dataLength, 24);
                            }
                        }
                        var rawInput = (VirtualKey)_rawData[30];
                        var isInput = (_rawData[26] & 1) != 1;
                        switch (rawInput)
                        {
                            case VirtualKey.Shift:
                                switch ((uint)_rawData[24])
                                {
                                    case PInvoke.SCANCODE_LSHIFT:
                                        _handleInput.HandleInput(VirtualKey.LeftShift, isInput);
                                        return (LRESULT)0;
                                    case PInvoke.SCANCODE_RSHIFT:
                                        _handleInput.HandleInput(VirtualKey.RightShift, isInput);
                                        return (LRESULT)0;
                                }
                                break;
                        }
                        _handleInput.HandleInput(rawInput, isInput);
                    }
                    return (LRESULT)0;
            }
            return PInvoke.DefWindowProc(hWnd, Msg, wParam, lParam);
        }

        public void Dispose()
        {
            if (_handle != nint.Zero)
            {
                PInvoke.RegisterRawInputDevices(stackalloc RAWINPUTDEVICE[]
                {
                    new()
                    {
                        usUsagePage = 1,
                        usUsage = 6,
                        dwFlags = RAWINPUTDEVICE_FLAGS.RIDEV_REMOVE,
                        hwndTarget = _handle
                    }
                }, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
                PInvoke.PostMessage(_handle, PInvoke.WM_CLOSE, (WPARAM)0, (LPARAM)0);
            }
        }
    }
}