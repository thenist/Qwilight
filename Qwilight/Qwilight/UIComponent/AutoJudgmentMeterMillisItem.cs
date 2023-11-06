using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct AutoJudgmentMeterMillisItem : IEquatable<AutoJudgmentMeterMillisItem>
    {
        public Component.Judged Judged { get; init; }

        [JsonIgnore]
        public ImageSource Drawing => BaseUI.Instance.JudgmentDrawings[(int)Judged]?.DefaultDrawing;

        public override bool Equals(object obj) => obj is AutoJudgmentMeterMillisItem autoJudgmentMeterMillisItem && Equals(autoJudgmentMeterMillisItem);

        public bool Equals(AutoJudgmentMeterMillisItem other) => Judged == other.Judged;

        public override int GetHashCode() => Judged.GetHashCode();

        public static bool operator ==(AutoJudgmentMeterMillisItem left, AutoJudgmentMeterMillisItem right) => left.Equals(right);

        public static bool operator !=(AutoJudgmentMeterMillisItem left, AutoJudgmentMeterMillisItem right) => !(left == right);
    }
}