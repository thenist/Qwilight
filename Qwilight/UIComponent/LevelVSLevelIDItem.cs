using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class LevelVSLevelIDItem
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string LevelID { get; init; }

        public int AvatarLevelVSCount { get; init; }

        public int TargetLevelVSCount { get; init; }

        public string LevelVSCountText => $"{AvatarLevelVSCount}{(AvatarLevelVSCount > TargetLevelVSCount ? " 👑" : string.Empty)}：{TargetLevelVSCount}{(TargetLevelVSCount > AvatarLevelVSCount ? " 👑" : string.Empty)}";
    }
}
