using Qwilight.Utilities;
using System.Numerics;
using Windows.Foundation;

namespace Qwilight
{
    public struct Bound
    {
        public double Position0;
        public double Position1;
        public double Length;
        public double Height;

        public Bound(double position0, double position1, double length, double height)
        {
            Position0 = position0;
            Position1 = position1;
            Length = length;
            Height = height;
        }

        public void Set(double position0, double position1, double length, double height)
        {
            Position0 = position0;
            Position1 = position1;
            Length = Math.Max(0.0, length);
            Height = Math.Max(0.0, height);
        }

        public void SetPosition(double position0, double position1) => Set(position0, position1, 0.0, 0.0);

        public void SetArea(double length, double height) => Set(0.0, 0.0, length, height);

        public void Set(float[] point)
        {
            Position0 = point[0];
            Position1 = point[1];
            Length = Math.Max(0.0, point[2]);
            Height = Math.Max(0.0, point[3]);
        }

        public bool IsPoint(Point lastPointed) => Utility.IsPoint(Position0, Position1, Length, Height, lastPointed.X, lastPointed.Y);

        public bool CanPaint => Length > 0.0 && Height > 0.0;

        public static implicit operator Windows.Foundation.Rect(Bound r) => new Windows.Foundation.Rect(r.Position0, r.Position1, r.Length, r.Height);

        public static implicit operator Vector2(Bound r) => new Vector2((float)r.Position0, (float)r.Position1);

        public static implicit operator System.Windows.Rect(Bound r) => new System.Windows.Rect(r.Position0, r.Position1, r.Length, r.Height);

        public static implicit operator System.Windows.Point(Bound r) => new System.Windows.Point(r.Position0, r.Position1);

        public static implicit operator Bound(Windows.Foundation.Rect r) => new Bound(r.X, r.Y, r.Width, r.Height);

        public static implicit operator Bound(System.Windows.Rect r) => new Bound(r.X, r.Y, r.Width, r.Height);
    }
}
