using CommunityToolkit.Mvvm.ComponentModel;

namespace Qwilight
{
    public sealed class WantLevelConfigure : ObservableObject
    {
        string _wantLevelName;

        public string WantLevelName
        {
            get => _wantLevelName;

            set
            {
                if (SetProperty(ref _wantLevelName, value, nameof(WantLevelName)))
                {
                    OnPropertyChanged(nameof(WantLevelNameText));
                }
            }
        }

        public string WantLevelNameText => string.IsNullOrEmpty(WantLevelName) ? "🔖" : WantLevelName;

        public string[] WantLevelIDs { get; set; }

        public WantLevelConfigure(string wantLevelName, string[] wantLevelIDs)
        {
            WantLevelName = wantLevelName;
            WantLevelIDs = wantLevelIDs;
        }
    }
}
