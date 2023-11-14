using CommandLine;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Globalization;
using System.IO;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace Qwilight
{
    public static partial class QwilightComponent
    {
        [GeneratedRegex("^(Lower Entry)|(Higher Entry)|(Lower Note File)|(Higher Note File)$")]
        public static partial Regex GetStopLastAudioComputer();

        [GeneratedRegex("^(Default)|(Salt)$")]
        public static partial Regex GetLoopingAudioComputer();

        [GeneratedRegex("[a-zA-Z\\d]+\\([\\d.+\\-, ]*\\)")]
        public static partial Regex GetCallComputer();

        public sealed class QwilightParams
        {
            [Option("valve")]
            public bool IsValve { get; set; }

            [Option("vs")]
            public bool IsVS { get; set; }

            [Option("language")]
            public int Language { get; set; }

            [Option("test")]
            public bool IsTest { get; set; }
        }

        public const int SendUnit = 1024 * 1024;

        public static readonly int CPUCount = Environment.ProcessorCount;
        public static readonly string[] BundleFileFormats = new[] { ".rar", ".zip", ".7z" };
        public static readonly string[] BMSNoteFileFormats = new[] { ".bms", ".bme", ".bml", ".pms" };
        public static readonly string[] BMSONNoteFileFormats = new[] { ".bmson" };
        public static readonly string[] NoteFileFormats = BMSNoteFileFormats.Concat(BMSONNoteFileFormats).ToArray();
        public static readonly string[] AudioFileFormatItems = new[] { ".aif", ".aiff", ".asf", ".flac", ".m4a", ".mid", ".midi", ".mp2", ".mp3", ".ogg", ".opus", ".raw", ".wav", ".wma" };
        public static readonly string[] DrawingFileFormats = new[] { ".bmp", ".gif", ".jpeg", ".jpg", ".png" };
        public static readonly string[] MediaFileFormats = new[] { ".avi", ".flv", ".m1v", ".mkv", ".mov", ".mp4", ".mpeg", ".mpg", ".wmv" };
        public static readonly string HashText = Utility.GetID512s(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Qwilight.dll")));
        public static readonly Version Date = Assembly.GetEntryAssembly().GetName().Version;
        public static readonly string DateText = $"{Date.Major}.{Date.Minor}.{Date.Build}";
        public static readonly JSON.AssetClient AssetsClientJSON;
        public static readonly double StandardUILoopMillis = 125.0;
        public static readonly double StandardLoopMillis = 1000.0 / 31;
        public static readonly TimeSpan StandardFrametime = TimeSpan.FromMilliseconds(1000.0 / 60);
        public static readonly double StandardWaitMillis = 500.0;
        public static readonly int HeapCount = GC.MaxGeneration + 1;
        public static readonly HwMode DefaultHwMode;
        public static readonly string AssetsEntryPath;
        public static readonly string CPUAssetsEntryPath;
        public static readonly string QwilightEntryPath;
        public static readonly string EdgeEntryPath;
        public static readonly string UIEntryPath;
        public static readonly string BundleEntryPath;
        public static readonly string CommentEntryPath;
        public static readonly string FaultEntryPath;
        public static readonly string MediaEntryPath;
        public static readonly string UtilityEntryPath;
        public static readonly string AMD64Name = string.Empty;
        public static readonly string OSName = string.Empty;
        public static readonly string RAMName = string.Empty;
        public static readonly string GPUName = string.Empty;
        public static readonly string M2Name = string.Empty;
        public static readonly string AudioName = string.Empty;
        public static readonly string TVName = string.Empty;
        public static readonly string LANName = string.Empty;

        public static string TaehuiNetHost { get; set; }

        public static string TaehuiNetFE { get; set; }

        public static string TaehuiNetAPI { get; set; }

        public static string QwilightAPI { get; set; }

        public static bool IsValve { get; set; }

        public static string TestLanguage { get; set; }

        public static bool IsTest { get; set; }

        public static bool IsVS { get; set; }

        public static int GetDigit(int value)
        {
            return value > 0 ? (int)(Math.Log10(value) + 1) : 1;
        }

        public static Func<string, object> OnGetBuiltInData { get; set; }

        public static T GetBuiltInData<T>(string data)
        {
            var value = OnGetBuiltInData(data);
            return value != null ? (T)value : default;
        }

        public static string GetBuiltInFloat64As(string data) => GetBuiltInData<double>(data).ToString(CultureInfo.InvariantCulture);

        static QwilightComponent()
        {
            AssetsClientJSON = Utility.GetJSON<JSON.AssetClient>(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Client.json")));
            Parser.Default.ParseArguments<QwilightParams>(Environment.GetCommandLineArgs()).WithParsed(o =>
            {
                IsValve = o.IsValve;
                if (o.Language > 0)
                {
                    TestLanguage = Utility.GetLanguage(o.Language);
                }
                IsTest = o.IsTest;
                IsVS = o.IsVS;
            });

#if DEBUG
            TaehuiNetHost = "localhost";
            TaehuiNetFE = "http://localhost";
            TaehuiNetAPI = "http://localhost:10100/www";
            QwilightAPI = "http://localhost:7301/qwilight/www";
#else
            TaehuiNetHost = IsVS ? "taehui" : "taehui.ddns.net";
            TaehuiNetFE = IsVS ? "http://taehui" : "https://taehui.ddns.net";
            TaehuiNetAPI = IsVS ? "http://taehui:10100/www" : "https://taehui.ddns.net/www";
            QwilightAPI = IsVS ? "http://taehui:7301/qwilight/www" : "https://taehui.ddns.net/qwilight/www";
#endif

            AssetsEntryPath = Path.Combine(AppContext.BaseDirectory, "Assets");
#if X64
            CPUAssetsEntryPath = Path.Combine(AssetsEntryPath, "x64");
#else
            CPUAssetsEntryPath = Path.Combine(AssetsEntryPath, "ARM64");
#endif
            QwilightEntryPath = Path.Combine(AppContext.BaseDirectory, IsValve ? "SavesDir" : Environment.UserName);
            EdgeEntryPath = Path.Combine(QwilightEntryPath, "Edge");
            FaultEntryPath = Path.Combine(QwilightEntryPath, "Fault");
            UIEntryPath = Path.Combine(QwilightEntryPath, "UI");
            BundleEntryPath = Path.Combine(QwilightEntryPath, "Bundle");
            CommentEntryPath = Path.Combine(QwilightEntryPath, "Comment");
            MediaEntryPath = Path.Combine(QwilightEntryPath, "Media");
            UtilityEntryPath = Path.Combine(QwilightEntryPath, "Utility");

            var rawHwMode = new DEVMODEW();
            PInvoke.EnumDisplaySettings(null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref rawHwMode);
            DefaultHwMode = new HwMode(rawHwMode.dmPelsWidth, rawHwMode.dmPelsHeight, rawHwMode.dmDisplayFrequency);

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                using var moc = mos.Get();
                AMD64Name = string.Join(", ", moc.Cast<ManagementBaseObject>().Select(o => o["Name"]));
            }
            catch
            {
            }

            try
            {
                OSName = Environment.OSVersion.ToString();
            }
            catch
            {
            }

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                using var moc = mos.Get();
                RAMName = Utility.FormatUnit((long)moc.Cast<ManagementBaseObject>().Select(o => (ulong)o["TotalPhysicalMemory"]).Single());
            }
            catch
            {
            }

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                using var moc = mos.Get();
                GPUName = string.Join(", ", moc.Cast<ManagementBaseObject>().Select(o => o["Name"]));
            }
            catch
            {
            }

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT Model FROM Win32_DiskDrive");
                using var moc = mos.Get();
                M2Name = string.Join(", ", moc.Cast<ManagementBaseObject>().Select(o => o["Model"]));
            }
            catch
            {
            }

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT ProductName FROM Win32_SoundDevice");
                using var moc = mos.Get();
                AudioName = string.Join(", ", moc.Cast<ManagementBaseObject>().Select(o => o["ProductName"]));
            }
            catch
            {
            }

            TVName = DefaultHwMode.ToString();

            try
            {
                using var mos = new ManagementObjectSearcher("SELECT ProductName FROM Win32_NetworkAdapter");
                using var moc = mos.Get();
                LANName = string.Join(", ", moc.Cast<ManagementBaseObject>().Select(o => o["ProductName"]));
            }
            catch
            {
            }
        }
    }
}