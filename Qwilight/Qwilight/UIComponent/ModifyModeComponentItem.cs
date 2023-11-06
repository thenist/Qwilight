using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class ModifyModeComponentItem : Model
    {
        public Brush PointedPaint => Paints.LowerStandPaints[PointedPaintID];

        public string LowerStandText => PointedPaintID > 0 ? "❕" : string.Empty;

        public int Value { get; init; }

        public int PointedPaintID { get; set; }

        public Visibility IsConfigureVisible => OnConfigure != null ? Visibility.Visible : Visibility.Collapsed;

        public string Data { get; init; }

        public ImageSource Drawing { get; init; }

        public RelayCommand OnConfigure { get; init; }

        public override bool Equals(object obj) => obj is ModifyModeComponentItem modeComponentItem && Value == modeComponentItem.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}