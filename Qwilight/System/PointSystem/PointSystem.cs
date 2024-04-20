using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input;

namespace Qwilight
{
    public sealed class PointSystem : IDisposable
    {
        public static readonly PointSystem Instance = new();

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(PointSystem));

        HWND _handle;

        public void HandleSystem(nint handle)
        {
            _handle = (HWND)handle;

            PInvoke.RegisterRawInputDevices(stackalloc RAWINPUTDEVICE[]
            {
                new()
                {
                    usUsagePage = 1,
                    usUsage = 2,
                    dwFlags = 0,
                    hwndTarget = _handle
                }
            }, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
        }

        public void Dispose()
        {
            if (_handle != HWND.Null)
            {
                PInvoke.RegisterRawInputDevices(stackalloc RAWINPUTDEVICE[]
                {
                    new()
                    {
                        usUsagePage = 1,
                        usUsage = 2,
                        dwFlags = RAWINPUTDEVICE_FLAGS.RIDEV_REMOVE,
                        hwndTarget = _handle
                    }
                }, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
                PInvoke.PostMessage(_handle, PInvoke.WM_CLOSE, (WPARAM)0, (LPARAM)0);
            }
        }
    }
}