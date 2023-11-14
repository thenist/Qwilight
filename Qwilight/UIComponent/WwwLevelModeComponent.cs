using CommunityToolkit.Mvvm.Input;

namespace Qwilight.UIComponent
{
    public struct WwwLevelModeComponent<T> : IEquatable<WwwLevelModeComponent<T>>
    {
        public T Value { get; init; }

        public RelayCommand OnInput { get; init; }

        public override bool Equals(object obj) => obj is WwwLevelModeComponent<T> wwwLevelModeComponent && Equals(wwwLevelModeComponent);

        public bool Equals(WwwLevelModeComponent<T> other) => Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(WwwLevelModeComponent<T> left, WwwLevelModeComponent<T> right) => left.Equals(right);

        public static bool operator !=(WwwLevelModeComponent<T> left, WwwLevelModeComponent<T> right) => !(left == right);
    }
}
