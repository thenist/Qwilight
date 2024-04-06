using CommandLine;
using Qwilight.UIComponent;
using Qwilight.Utilities;
#if DEBUG
using System.Diagnostics;
#endif
using System.Globalization;
using System.IO;
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

        [GeneratedRegex("^(Salt)$")]
        public static partial Regex GetLoopingAudioComputer();

        [GeneratedRegex("[a-zA-Z\\d]+\\([\\d.+\\-, ]*\\)")]
        public static partial Regex GetCallComputer();

        public const int SendUnit = 1024 * 1024;

        public static readonly int CPUCount = Environment.ProcessorCount;
        public static readonly string[] BundleFileFormats = [".rar", ".zip", ".7z"];
        public static readonly string[] BMSNoteFileFormats = [".bms", ".bme", ".bml", ".pms"];
        public static readonly string[] BMSONNoteFileFormats = [".bmson"];
        public static readonly string[] NoteFileFormats = BMSNoteFileFormats.Concat(BMSONNoteFileFormats).ToArray();
        public static readonly string[] AudioFileFormats = [".aif", ".aiff", ".asf", ".flac", ".m4a", ".mid", ".midi", ".mp2", ".mp3", ".ogg", ".opus", ".raw", ".wav", ".wma"];
        public static readonly string[] DrawingFileFormats = [".bmp", ".gif", ".jpeg", ".jpg", ".png"];
        public static readonly string[] MediaFileFormats = [".avi", ".flv", ".m1v", ".mkv", ".mov", ".mp4", ".mpeg", ".mpg", ".wmv"];
        public static readonly string[] ModifiedMediaFileFormats = [".avi", ".flv", ".m1v", ".mpeg", ".mpg"];
        public static readonly string HashText = Utility.GetID512(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Qwilight.dll")));
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
        public static readonly string MSIXAssetsEntryPath;
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
        public static readonly string OSName = Environment.OSVersion.ToString();
        public static readonly ulong RAM;
        public static readonly string RAMName = string.Empty;
        public static readonly string GPUName = string.Empty;
        public static readonly string M2Name = string.Empty;
        public static readonly string AudioName = string.Empty;
        public static readonly string TVName = string.Empty;
        public static readonly string LANName = string.Empty;

        public static string TaehuiNetDDNS { get; set; }

        public static string TaehuiNetFE { get; set; }

        public static string TaehuiNetAPI { get; set; }

        public static string QwilightAPI { get; set; }

        public static bool IsValve { get; set; }

        public static string TestLanguage { get; set; }

        public static bool IsVS { get; set; }

        public static Func<string, object> OnGetBuiltInData { get; set; }

        public static T GetBuiltInData<T>(string data)
        {
            var value = OnGetBuiltInData(data);
            return value != null ? (T)value : default;
        }

        public static string GetBuiltInFloat64As(string data) => GetBuiltInData<double>(data).ToString(CultureInfo.InvariantCulture);

        public static void SetDDNS(string taehuiNetDDNS)
        {
            QwilightComponent.TaehuiNetDDNS = taehuiNetDDNS;
            switch (taehuiNetDDNS)
            {
                case "taehui.ddns.net":
                    QwilightComponent.TaehuiNetFE = "https://taehui.ddns.net";
                    QwilightComponent.TaehuiNetAPI = "https://taehui.ddns.net/www";
                    QwilightComponent.QwilightAPI = "https://taehui.ddns.net/qwilight/www";
                    break;
                case "taehui":
                    QwilightComponent.TaehuiNetFE = "http://taehui";
                    QwilightComponent.TaehuiNetAPI = "http://taehui:3000/www";
                    QwilightComponent.QwilightAPI = "http://taehui:7301/qwilight/www";
                    break;
                case "localhost":
                    QwilightComponent.TaehuiNetFE = "http://localhost";
                    QwilightComponent.TaehuiNetAPI = "http://localhost:3000/www";
                    QwilightComponent.QwilightAPI = "http://localhost:7301/qwilight/www";
                    break;
            }
        }

        static QwilightComponent()
        {
            AssetsClientJSON = Utility.GetJSON<JSON.AssetClient>(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Client.json")));
            Parser.Default.ParseArguments<Params.QwilightParams>(Environment.GetCommandLineArgs()).WithParsed(o =>
            {
                IsValve = o.IsValve;
                if (o.Language > 0)
                {
                    TestLanguage = Utility.GetLanguage(o.Language);
                }
#if DEBUG
                IsVS = true;
#else
                IsVS = o.IsVS;
#endif
            });

#if DEBUG
            SetDDNS(Process.GetProcessesByName("java").Length > 0 ? "localhost" : "taehui");
#else
            SetDDNS(IsVS ? "taehui" : "taehui.ddns.net");
#endif

            AssetsEntryPath = Path.Combine(AppContext.BaseDirectory, "Assets");
#if X64
            CPUAssetsEntryPath = Path.Combine(AssetsEntryPath, "x64");
#else
            CPUAssetsEntryPath = Path.Combine(AssetsEntryPath, "ARM64");
#endif
            MSIXAssetsEntryPath = Path.Combine(AssetsEntryPath, "MSIX");
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
            DefaultHwMode = new(rawHwMode.dmPelsWidth, rawHwMode.dmPelsHeight, rawHwMode.dmDisplayFrequency);

            try
            {
                AMD64Name = string.Join(", ", Utility.GetWMI("SELECT Name FROM Win32_Processor").Select(o => o["Name"]));
            }
            catch
            {
            }

            try
            {
                RAM = Utility.GetWMI("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem").Select(o => (ulong)o["TotalPhysicalMemory"]).Single();
            }
            catch
            {
            }
            RAMName = Utility.FormatLength((long)RAM);

            try
            {
                GPUName = string.Join(", ", Utility.GetWMI("SELECT Name FROM Win32_VideoController").Select(o => o["Name"]));
            }
            catch
            {
            }

            try
            {
                M2Name = string.Join(", ", Utility.GetWMI("SELECT Model FROM Win32_DiskDrive").Select(o => o["Model"]));
            }
            catch
            {
            }

            try
            {
                AudioName = string.Join(", ", Utility.GetWMI("SELECT ProductName FROM Win32_SoundDevice").Select(o => o["ProductName"]));
            }
            catch
            {
            }

            TVName = DefaultHwMode.ToString();

            try
            {
                LANName = string.Join(", ", Utility.GetWMI("SELECT ProductName FROM Win32_NetworkAdapter").Select(o => o["ProductName"]));
            }
            catch
            {
            }
        }
    }
}