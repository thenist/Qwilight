using System.Windows.Media;
using Colors = Microsoft.UI.Colors;

namespace Qwilight.UIComponent
{
    public struct ValidNetMode : IEquatable<ValidNetMode>
    {
        static readonly Brush[] _validNetModePaints = new[]
        {
            DrawingSystem.Instance.GetDefaultPaint(Colors.White),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightGreen),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightGreen),
            DrawingSystem.Instance.GetDefaultPaint(Colors.LightGreen),
            DrawingSystem.Instance.GetDefaultPaint(Colors.Red)
        };

        public int Data { get; init; }

        public Brush Paint => _validNetModePaints[Data];

        public override string ToString() => Data switch
        {
            0 => LanguageSystem.Instance.ValidNetMode0Text,
            1 => LanguageSystem.Instance.ValidNetMode1Text,
            2 => LanguageSystem.Instance.ValidNetMode2Text,
            3 => LanguageSystem.Instance.ValidNetMode3Text,
            4 => LanguageSystem.Instance.ValidNetMode4Text,
            _ => string.Empty
        };

        public override bool Equals(object obj) => obj is ValidNetMode validNetMode && Equals(validNetMode);

        public bool Equals(ValidNetMode other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public static bool operator ==(ValidNetMode left, ValidNetMode right) => left.Equals(right);

        public static bool operator !=(ValidNetMode left, ValidNetMode right) => !(left == right);
    }
}