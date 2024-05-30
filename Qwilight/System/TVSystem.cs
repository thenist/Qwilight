using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.IO;

namespace Qwilight
{
    public sealed partial class TVSystem : Model
    {
        public static readonly TVSystem Instance = QwilightComponent.GetBuiltInData<TVSystem>(nameof(TVSystem));

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(TVSystem));

        readonly object _availableCSX = new();
        readonly bool[] _tvAssists = new bool[2];

        public bool TVAssist0
        {
            get => _tvAssists[0];

            set
            {
                if (SetProperty(ref _tvAssists[0], value, nameof(TVAssist0)))
                {
                    OnPropertyChanged(nameof(TVAssists));
                }
            }
        }

        public bool TVAssist1
        {
            get => _tvAssists[1];

            set
            {
                if (SetProperty(ref _tvAssists[1], value, nameof(TVAssist1)))
                {
                    OnPropertyChanged(nameof(TVAssists));
                }
            }
        }

        public bool TVAssists => TVAssist0 || TVAssist1;

        bool IsAvailable => !ViewModels.Instance.MainValue.IsPragmatic;

        public void HandleSystemIfAvailable()
        {
            lock (_availableCSX)
            {
                if (IsAvailable)
                {
                    Monitor.Pulse(_availableCSX);
                }
            }
        }

        public void HandleSystem()
        {
            var availableExes = new Dictionary<string, int>
            {
                { "obs64", 0 },
                { "TwitchStudio", 0 },
                { "PRISMLiveStudio", 0 },
                { "XGS64", 0 },
                { "Discord", 1 }
            };
            Span<bool> tvAssists = stackalloc bool[2];
            while (true)
            {
                try
                {
                    tvAssists.Clear();
                    foreach (var lppe in Process.GetProcesses())
                    {
                        foreach (var (fileName, tvAssist) in availableExes)
                        {
                            if (lppe.ProcessName == fileName)
                            {
                                tvAssists[tvAssist] = true;
                            }
                        }
                    }
                    if ((tvAssists[0] || tvAssists[1]) && !TVAssist0 && !TVAssist1 && Configure.Instance.TVAssistConfigure)
                    {
                        ViewModels.Instance.AssistValue.Open();
                    }
                    TVAssist0 = tvAssists[0];
                    TVAssist1 = tvAssists[1];

                    Thread.Sleep(1000);

                    lock (_availableCSX)
                    {
                        if (!IsAvailable)
                        {
                            Monitor.Wait(_availableCSX);
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.SaveFaultFile(FaultEntryPath, e);
                }
            }
        }
    }
}
