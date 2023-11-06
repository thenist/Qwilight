using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class AvatarItem : Model
    {
        public enum AvatarConfigure
        {
            Not = 0,
            Levying = 2,
            Compiling = 3,
            Compiled = 4,
        }

        bool _isSiteHand;
        bool _isMe;
        AvatarConfigure _avatarConfigureValue;
        AvatarGroup _avatarGroupValue;
        bool _isAudioInput;

        public bool IsSiteHand
        {
            get => _isSiteHand;

            set => SetProperty(ref _isSiteHand, value, nameof(IsSiteHand));
        }

        public bool IsMe
        {
            get => _isMe;

            set => SetProperty(ref _isMe, value, nameof(IsMe));
        }

        public AvatarConfigure AvatarConfigureValue
        {
            get => _avatarConfigureValue;

            set => SetProperty(ref _avatarConfigureValue, value, nameof(AvatarConfigureValue));
        }

        public AvatarGroup AvatarGroupValue
        {
            get => _avatarGroupValue;

            set => SetProperty(ref _avatarGroupValue, value, nameof(AvatarGroupValue));
        }

        public bool IsAudioInput
        {
            get => _isAudioInput;

            set => SetProperty(ref _isAudioInput, value, nameof(IsAudioInput));
        }

        public Brush AudioInputPaint => IsAudioInput ? Paints.Paint3 : Paints.Paint4;

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public AvatarWww AvatarWwwValue { get; }

        public bool IsValve { get; init; }

        public override bool Equals(object obj) => obj is AvatarItem avatarItem && AvatarID == avatarItem.AvatarID;

        public override int GetHashCode() => AvatarID.GetHashCode();

        public AvatarItem(string avatarID)
        {
            AvatarID = avatarID;
            AvatarWwwValue = new(avatarID);
        }
    }
}