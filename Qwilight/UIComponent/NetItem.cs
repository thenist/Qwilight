using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.UIComponent
{
    public sealed class NetItem : IComparable<NetItem>
    {
        int _targetPosition;
        Comment _comment;

        public double DrawingPosition { get; set; }

        public double TargetPositionStatus { get; set; }

        public int TargetPosition
        {
            get => _targetPosition;

            set
            {
                if (_targetPosition != value)
                {
                    _targetPosition = value;
                    TargetPositionStatus = 1.0;
                }
            }
        }

        public int SetTargetPosition { get; set; }

        public Event.Types.AvatarNetStatus AvatarNetStatus { get; set; }

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public DateTime Date { get; init; }

        public int StandValue { get; set; }

        public string Stand { get; set; }

        public double PointValue { get; set; }

        public string Point { get; set; }

        public int BandValue { get; set; }

        public string Band { get; set; }

        public double HitPoints { get; set; }

        public DefaultCompute.QuitStatus QuitValue { get; set; }

        public double IsFailedStatus { get; set; }

        public Component.Judged LastJudged { get; set; } = Component.Judged.Not;

        public float P2BuiltLength { get; set; }

        public float JudgmentMainPosition { get; set; }

        public ICollection<Event.Types.NetDrawing> Drawings { get; set; }

        public bool IsMyNetItem { get; init; }

        public bool IsFavorNetItem { get; init; }

        public CommentItem CommentItem { get; init; }

        public Comment Comment
        {
            get => _comment;

            set
            {
                _comment = value;
                if (value != null)
                {
                    PaintEventsDate = Utility.GetDate<Component.PaintEventsDate>(Version.Parse(value.Date), "1.14.91");
                }
                else
                {
                    PaintEventsDate = Component.PaintEventsDate._1_14_91;
                }
                LastPaintEventPosition = 0;
            }
        }

        public Component.PaintEventsDate PaintEventsDate { get; set; }

        public int LastPaintEventPosition { get; set; }

        public ModeComponent.HitPointsMode HitPointsModeValue { get; set; }

        public NetItem(string avatarID, string avatarName, DateTime date, int stand = default, int band = default, double point = default, double hitPoints = default)
        {
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            StandValue = stand;
            Stand = stand.ToString(LanguageSystem.Instance.StandContents);
            BandValue = band;
            Band = band.ToString(LanguageSystem.Instance.BandContents);
            PointValue = point;
            Point = (100 * point).ToString("0.##％");
            HitPoints = hitPoints;
        }

        public override bool Equals(object obj) => obj is NetItem netItem && AvatarID == netItem.AvatarID && IsMyNetItem == netItem.IsMyNetItem;

        public override int GetHashCode() => AvatarID.GetHashCode();

        public void SetValue(int stand, int band, double point, double hitPoints, int noteFileCount, ICollection<Event.Types.NetDrawing> drawings = null, Event.Types.DrawingComponent drawingComponent = null)
        {
            StandValue = stand;
            Stand = PoolSystem.Instance.GetValueText(stand, LanguageSystem.Instance.StandContents);
            BandValue = band;
            Band = PoolSystem.Instance.GetValueText(band, LanguageSystem.Instance.BandContents);
            PointValue = point;
            Point = PoolSystem.Instance.GetValueText(100.0 * point, "0.##％");
            HitPoints = hitPoints;
            QuitValue = Utility.GetQuitStatusValue(PointValue, StandValue, hitPoints, noteFileCount);
            Drawings = drawings;
            if (drawingComponent != null)
            {
                P2BuiltLength = drawingComponent.P2BuiltLength;
                JudgmentMainPosition = drawingComponent.JudgmentMainPosition;
            }
        }

        public int CompareTo(NetItem other)
        {
            var value = StandValue.CompareTo(other.StandValue);
            if (value != 0)
            {
                return value;
            }

            return other.TargetPosition.CompareTo(TargetPosition);
        }
    }
}