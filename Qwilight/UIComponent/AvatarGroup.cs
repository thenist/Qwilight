using System.Windows.Media;
using Colors = Microsoft.UI.Colors;

namespace Qwilight.UIComponent
{
    public struct AvatarGroup : IEquatable<AvatarGroup>
    {
        static readonly Brush[] _avatarGroupPaints = new[]
        {
            DrawingSystem.Instance.GetDefaultPaint(Colors.Gray),
            DrawingSystem.Instance.GetDefaultPaint(Colors.White),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightYellow),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightBlue),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightPink)
        };

        public int Data { get; init; }

        public Brush Paint => _avatarGroupPaints.ElementAtOrDefault(Data);

        public override string ToString() => Data switch
        {
            0 => LanguageSystem.Instance.NotAvatarGroup,
            1 => "TEAM SOBREM",
            2 => "TEAM SCTL",
            3 => "TEAM CHILLDIVE",
            4 => "TEAM SPEHS",
            _ => string.Empty
        };

        public string Title => Data switch
        {
            0 => string.Empty,
            _ => ToString()
        };

        public override bool Equals(object obj) => obj is AvatarGroup avatarGroup && Equals(avatarGroup);

        public bool Equals(AvatarGroup other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public static bool operator ==(AvatarGroup left, AvatarGroup right) => left.Equals(right);

        public static bool operator !=(AvatarGroup left, AvatarGroup right) => !(left == right);
    }
}