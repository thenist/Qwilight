using System.Diagnostics;
using System.IO;

namespace Qwilight
{
    public sealed class XwindowSystem
    {
        public static readonly XwindowSystem Instance = new();

        Process _exe;

        public void HandleSystem()
        {
            try
            {
                _exe = Process.Start(Path.Combine(QwilightComponent.SoftwareEntryPath, "Xwindow.exe"), $"\"{PIDClass.FilePath}\"");
            }
            catch
            {
            }
        }

        public void Stop()
        {
            if (_exe != null)
            {
                using (_exe)
                {
                    _exe.Kill();
                }
                _exe = null;
            }
        }
    }
}
