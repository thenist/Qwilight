namespace Qwilight
{
    public sealed class WantLevelConfigure
    {
        public string WantLevelName { get; set; }

        public string WantLevelNameText => string.IsNullOrEmpty(WantLevelName) ? "🔖" : WantLevelName;

        public string[] WantLevelIDs { get; set; } = [];
    }
}
