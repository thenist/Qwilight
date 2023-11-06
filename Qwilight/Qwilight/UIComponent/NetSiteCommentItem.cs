using Qwilight.Utilities;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct NetSiteCommentItem : IEquatable<NetSiteCommentItem>
    {
        public Event.Types.AvatarNetStatus AvatarNetStatus { get; init; }

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public AvatarWww AvatarWwwValue { get; }

        public int StandValue { get; init; }

        public string Stand { get; init; }

        public string Band { get; init; }

        public double PointValue { get; init; }

        public string Point { get; init; }

        public string HighestJudgment { get; init; }

        public string HigherJudgment { get; init; }

        public string HighJudgment { get; init; }

        public string LowJudgment { get; init; }

        public string LowerJudgment { get; init; }

        public int LowestJudgmentValue { get; init; }

        public string LowestJudgment => LowestJudgmentValue.ToString(LanguageSystem.Instance.CountContents);

        public Brush PointedPaint => Paints.NetSiteCommentPaints[(int)AvatarNetStatus];

        public ImageSource QuitDrawing => BaseUI.Instance.QuitDrawings[(int)Utility.GetQuitStatusValue(PointValue, StandValue, AvatarNetStatus == Event.Types.AvatarNetStatus.Failed ? 0.0 : 1.0, 1)][LowestJudgmentValue > 0 ? 0 : 1]?.DefaultDrawing;

        public NetSiteCommentItem(JSON.TwilightCallNetSiteComments.CallNetSIteCommentItem toCallNetSiteCommentItem)
        {
            AvatarID = toCallNetSiteCommentItem.avatarID;
            AvatarWwwValue = new AvatarWww(AvatarID);
            AvatarNetStatus = (Event.Types.AvatarNetStatus)toCallNetSiteCommentItem.avatarNetStatus;
            AvatarName = toCallNetSiteCommentItem.avatarName;
            StandValue = toCallNetSiteCommentItem.stand;
            Stand = StandValue.ToString(LanguageSystem.Instance.StandContents);
            Band = toCallNetSiteCommentItem.band.ToString(LanguageSystem.Instance.BandContents);
            PointValue = toCallNetSiteCommentItem.point;
            Point = Math.Round(100.0 * PointValue, 2).ToString("0.##％");
            HighestJudgment = toCallNetSiteCommentItem.highestJudgment.ToString(LanguageSystem.Instance.CountContents);
            HigherJudgment = toCallNetSiteCommentItem.higherJudgment.ToString(LanguageSystem.Instance.CountContents);
            HighJudgment = toCallNetSiteCommentItem.highJudgment.ToString(LanguageSystem.Instance.CountContents);
            LowJudgment = toCallNetSiteCommentItem.lowJudgment.ToString(LanguageSystem.Instance.CountContents);
            LowerJudgment = toCallNetSiteCommentItem.lowerJudgment.ToString(LanguageSystem.Instance.CountContents);
            LowestJudgmentValue = toCallNetSiteCommentItem.lowestJudgment;
        }

        public override bool Equals(object obj) => obj is NetSiteCommentItem netSiteCommentItem && Equals(netSiteCommentItem);

        public bool Equals(NetSiteCommentItem other) => AvatarID == other.AvatarID;

        public override int GetHashCode() => AvatarID.GetHashCode();

        public static bool operator ==(NetSiteCommentItem left, NetSiteCommentItem right) => left.Equals(right);

        public static bool operator !=(NetSiteCommentItem left, NetSiteCommentItem right) => !(left == right);
    }
}