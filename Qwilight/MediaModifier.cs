using Qwilight.Utilities;
using System.Diagnostics;
using System.IO;

namespace Qwilight
{
    public sealed class MediaModifier
    {
        readonly object _exeCSX = new();
        Process _exe;
        bool _isAvailable = true;

        public string Text { get; set; } = string.Empty;

        public void ModifyMedia(string mediaFilePath, string hashMediaFilePath, bool isWrongMedia, bool isCounterWave)
        {
            try
            {
                lock (_exeCSX)
                {
                    if (_isAvailable)
                    {
                        Text = LanguageSystem.Instance.MediaModifierContents;
                        _exe = new()
                        {
                            StartInfo = new(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "ffmpeg.exe"), $"""
                                -i "{mediaFilePath}" -y -an {(isWrongMedia || isCounterWave ? string.Empty : "-vcodec copy")} {(isCounterWave ? "-vf reverse" : string.Empty)} -preset ultrafast "{hashMediaFilePath}"
                            """)
                            {
                                CreateNoWindow = true
                            }
                        };
                        _exe.Start();
                        _exe.PriorityClass = ProcessPriorityClass.Idle;
                    }
                }
                if (_isAvailable)
                {
                    _exe.WaitForExit();
                    if (_exe.ExitCode != 0)
                    {
                        Utility.WipeFile(hashMediaFilePath);
                    }
                }
            }
            finally
            {
                Text = string.Empty;
            }
        }

        public void StopModifyMedia()
        {
            lock (_exeCSX)
            {
                _isAvailable = false;
                _exe?.Kill();
            }
        }
    }
}