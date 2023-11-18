using DiscordRPC;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.IO;

namespace Qwilight
{
    public sealed class PlatformSystem
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(PlatformSystem));

        public static readonly PlatformSystem Instance = new();

        readonly Timestamps _time = Timestamps.Now;

        public void HandleSystem()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var time = new Timestamps();
            var data = new RichPresence
            {
                Assets = new()
                {
                    LargeImageKey = "qwilight",
                }
            };
            while (true)
            {
                try
                {
                    using var targetSystem = new DiscordRpcClient(QwilightComponent.AssetsClientJSON.platform);
                    if (targetSystem.Initialize())
                    {
                        while (true)
                        {
                            var defaultComputer = mainViewModel.Computer;
                            switch (mainViewModel.ModeValue)
                            {
                                case MainViewModel.Mode.NoteFile:
                                    var autoComputer = mainViewModel.AutoComputer;
                                    data.State = LanguageSystem.Instance.PlatformNoteFileMode;
                                    data.Details = autoComputer?.IsHandling == true ? autoComputer.PlatformText128 : "Idle";
                                    data.Timestamps = _time;
                                    break;
                                case MainViewModel.Mode.Computing:
                                    data.State = defaultComputer.PlatformVarietyContents;
                                    data.Details = defaultComputer.PlatformText128;
                                    time.StartUnixMilliseconds = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Math.Clamp(defaultComputer.LoopingCounter, 0, defaultComputer.Length));
                                    data.Timestamps = time;
                                    break;
                                case MainViewModel.Mode.Quit:
                                    data.State = LanguageSystem.Instance.PlatformQuitMode;
                                    data.Details = defaultComputer.PlatformText128;
                                    data.Timestamps = _time;
                                    break;
                            }
                            targetSystem.SetPresence(data);
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.SaveFaultFile(FaultEntryPath, e);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}