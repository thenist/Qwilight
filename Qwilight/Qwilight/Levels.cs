using System.Windows;

namespace Qwilight
{
    public static class Levels
    {
        public const double EdgeMargin = 1.25;
        public const double EdgeXY = (1.0 - EdgeMargin) / 2;

        public static readonly double FontLevel0 = QwilightComponent.GetBuiltInData<double>("FontLevel0");
        public static readonly double FontLevel1 = QwilightComponent.GetBuiltInData<double>("FontLevel1");
        public static readonly double StandardEllipse = QwilightComponent.GetBuiltInData<CornerRadius>("StandardEllipse").BottomLeft;
        public static readonly double StandardMargin = QwilightComponent.GetBuiltInData<Thickness>("StandardMargin").Left;
        public static readonly double WindowEllipse = QwilightComponent.GetBuiltInData<CornerRadius>("WindowEllipse").BottomLeft;
        public static readonly float FontLevel0Float32 = (float)FontLevel0;
        public static readonly float FontLevel1Float32 = (float)FontLevel1;
        public static readonly float StandardEllipseFloat32 = (float)StandardEllipse;
        public static readonly float StandardMarginFloat32 = (float)StandardMargin;
        public static readonly float StandardEdgeFloat32 = (float)QwilightComponent.GetBuiltInData<Thickness>("StandardEdge").Left;
    }
}
