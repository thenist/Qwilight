namespace Qwilight
{
    public sealed class ModeComponentBundle : Model
    {
        string _valueName;

        public string Name
        {
            get => _valueName;

            set => SetProperty(ref _valueName, value, nameof(Name));
        }

        public ModeComponent Value { get; set; } = new ModeComponent();
    }
}
