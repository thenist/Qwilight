namespace Qwilight.UIComponent
{
    public sealed class LevelItem : Model
    {
        bool _isWanted;

        public string LevelID { get; init; }

        public bool IsWanted
        {
            get => _isWanted;

            set => SetProperty(ref _isWanted, value, nameof(IsWanted));
        }
    }
}
