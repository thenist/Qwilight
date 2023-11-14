namespace Qwilight.UIComponent
{
    public sealed class PostableUIItem : Model
    {
        bool _isWanted;

        public PostableItem PostableItemValue { get; init; }

        public bool IsWanted
        {
            get => _isWanted;

            set => SetProperty(ref _isWanted, value, nameof(IsWanted));
        }
    }
}
