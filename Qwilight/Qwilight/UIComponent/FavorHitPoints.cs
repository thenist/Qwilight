namespace Qwilight.UIComponent
{
    public struct FavorHitPoints
    {
        public string Name { get; init; }

        public double[][] Value { get; init; }

        public bool IsDefault { get; init; }

        public override string ToString() => Name;
    }
}
