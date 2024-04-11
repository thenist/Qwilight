using Ionic.Zip;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using SplashScreen = System.Windows.SplashScreen;
using StartupEventArgs = System.Windows.StartupEventArgs;
using VirtualKey = Windows.System.VirtualKey;

namespace Qwilight.View
{
    public sealed partial class QwilightClass
    {
        static SplashScreen _wpfLoadingAsset = new("Assets/Drawing/Loading.png");

        [STAThread]
        static void Main(string[] args)
        {
            #region COMPATIBLE
            Compatible.Compatible.Qwilight(QwilightComponent.QwilightEntryPath);
            #endregion

            if (Utility.HasInput(VirtualKey.LeftShift))
            {
                PInvoke.AllocConsole();
            }

            GPUConfigure.Instance.Load();
            switch (GPUConfigure.Instance.GPUModeValue)
            {
                case GPUConfigure.GPUMode.NVIDIA:
                    NativeLibrary.TryLoad("nvapi64", out _);
                    break;
            }
            _wpfLoadingAsset.Show(true, true);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if DEBUG
            Environment.SetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_SOURCE_INFO", "1");
#endif
            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", QwilightComponent.EdgeEntryPath);
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            ProfileOptimization.SetProfileRoot(QwilightComponent.QwilightEntryPath);
            ProfileOptimization.StartProfile("Qwilight.$");
            ThreadPool.GetMaxThreads(out var tw, out var tIOCP);
            ThreadPool.SetMinThreads(tw, tIOCP);
            _ = PInvoke.timeBeginPeriod(1);

            try
            {
                using (var r = Registry.LocalMachine.OpenSubKey("SOFTWARE")?.OpenSubKey("WOW6432Node")?.OpenSubKey("Microsoft")?.OpenSubKey("EdgeUpdate")?.OpenSubKey("Clients")?.OpenSubKey("{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"))
                {
                    if (string.IsNullOrEmpty(r?.GetValue("pv") as string))
                    {
                        using var exe = Process.Start(Path.Combine(QwilightComponent.AssetsEntryPath, "MicrosoftEdgeWebview2Setup.exe"));
                        exe.WaitForExit();
                    }
                }
            }
            catch
            {
            }

            Application.Start(p =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                try
                {
                    new QwilightClass().Run();
                }
                finally
                {
                    Application.Current.Exit();
                }
            });
        }

        readonly ConcurrentDictionary<Exception, object> _handledFaultMap = new();

        void OnUnhandledFault(Exception e)
        {
            if (_handledFaultMap.TryAdd(e, null))
            {
                var (logFilePath, faultText) = Utility.SaveFaultFile(QwilightComponent.FaultEntryPath, e);
                if (!QwilightComponent.IsVS)
                {
                    _ = TwilightSystem.Instance.PostWwwParallel($"{QwilightComponent.QwilightAPI}/fault", faultText);
                }
                PInvoke.MessageBox(HWND.Null, e.Message, "Qwilight", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);
                Utility.OpenAs(logFilePath);
            }
        }

        QwilightClass() => InitializeComponent();

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var fault = e.ExceptionObject as Exception;
                if (!(fault is Win32Exception && (fault as Win32Exception).NativeErrorCode == 1400))
                {
                    OnUnhandledFault(fault);
                }
            };
            UIHandler.Instance.Init(OnUnhandledFault);

            QwilightComponent.OnGetBuiltInData = data => TryFindResource(data);

            try
            {
                Directory.CreateDirectory(QwilightComponent.BundleEntryPath);
                Directory.CreateDirectory(QwilightComponent.CommentEntryPath);
                Directory.CreateDirectory(QwilightComponent.EdgeEntryPath);
                Directory.CreateDirectory(QwilightComponent.MediaEntryPath);
                Directory.CreateDirectory(LevelSystem.EntryPath);
            }
            catch
            {
                PInvoke.MessageBox(HWND.Null, $"Cannot run Qwilight from {QwilightComponent.QwilightEntryPath}", "Qwilight", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);
                Environment.Exit(1);
            }

            PIDClass.Instance.HaveIt(_wpfLoadingAsset);

            Utility.CopyEntry(Path.Combine(QwilightComponent.AssetsEntryPath, "UI"), Path.Combine(QwilightComponent.UIEntryPath));

            var qwilightBundleFilePath = Path.Combine(QwilightComponent.QwilightEntryPath, "Qwilight.zip");
            if (File.Exists(qwilightBundleFilePath))
            {
                using (var zipFile = new ZipFile(qwilightBundleFilePath))
                {
                    zipFile.ExtractAll(QwilightComponent.QwilightEntryPath, ExtractExistingFileAction.OverwriteSilently);
                }
                Utility.WipeFile(qwilightBundleFilePath);
            }

            Configure.Instance.Load();
            if (Configure.Instance.WMPointer)
            {
                AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);
            }
            LanguageSystem.Instance.Init(Configure.Instance.Language);
            DB.Instance.Load();
            FastDB.Instance.Load();
            AudioSystem.Instance.Init();

            _wpfLoadingAsset = null;
        }
    }
}