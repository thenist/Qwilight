namespace Qwilight
{
    public sealed class WantLevelConfigure
    {
        public string WantLevelName { get; set; }

        public string WantLevelNameText => !WantLevelSystem || string.IsNullOrEmpty(WantLevelName) ? "🔖" : WantLevelName;

        public string[] WantLevelIDs { get; set; } = [];

        public bool WantLevelSystem { get; set; }
    }
}
