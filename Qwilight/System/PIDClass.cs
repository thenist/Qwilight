using Qwilight.Utilities;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight
{
    public sealed class PIDClass : IDisposable
    {
        public static readonly PIDClass Instance = new();
        public static readonly string FilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Qwilight.#");

        Stream _fs;

        public void HaveIt(SplashScreen wpfLoadingAsset)
        {
            try
            {
                _fs = File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _fs.Write(Encoding.UTF8.GetBytes(Environment.ProcessId.ToString()));
                _fs.Flush();
            }
            catch
            {
                if (!QwilightComponent.IsVS)
                {
                    switch (PInvoke.MessageBox(HWND.Null, $"Qwilight is already running", "Qwilight", MESSAGEBOX_STYLE.MB_CANCELTRYCONTINUE | MESSAGEBOX_STYLE.MB_ICONQUESTION))
                    {
                        case MESSAGEBOX_RESULT.IDCANCEL:
                            wpfLoadingAsset.Show(true, true);
                            Environment.Exit(1);
                            break;
                        case MESSAGEBOX_RESULT.IDTRYAGAIN:
                            wpfLoadingAsset.Show(true, true);
                            try
                            {
                                using var sr = new StreamReader(File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Write));
                                Process.GetProcessById(Utility.ToInt32(sr.ReadToEnd())).Kill();
                            }
                            catch
                            {
                            }
                            Utility.WaitUntilCanOpen(FilePath, 1000.0);
                            HaveIt(wpfLoadingAsset);
                            break;
                        case MESSAGEBOX_RESULT.IDCONTINUE:
                            wpfLoadingAsset.Show(true, true);
                            HaveIt(wpfLoadingAsset);
                            break;
                    }
                }
            }
        }

        public void Dispose()
        {
            _fs?.Dispose();
            Utility.WipeFile(FilePath);
        }
    }
}
