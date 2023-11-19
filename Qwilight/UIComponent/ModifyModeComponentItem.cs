using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class ModifyModeComponentItem : Model
    {
        public Brush PointedPaint => Paints.LowerStandPaints[PointedPaintID];

        public string LowerStandText => PointedPaintID > 0 ? "❕" : string.Empty;

        public int Value { get; init; }

        public int PointedPaintID { get; set; }

        public bool IsConfigureVisible => OnConfigure != null;

        public bool IsVConfigureVisible => OnVConfigure != null;

        public string Data { get; init; }

        public ImageSource Drawing { get; init; }

        public RelayCommand OnConfigure { get; init; }

        public Action OnVConfigure { get; init; }

        public Func<bool> GetIsVConfigure { get; init; }

        public bool IsVConfigure => GetIsVConfigure();

        public void NotifyIsVConfigure()
        {
            if (GetIsVConfigure != null)
            {
                OnPropertyChanged(nameof(IsVConfigure));
            }
        }

        public override bool Equals(object obj) => obj is ModifyModeComponentItem modeComponentItem && Value == modeComponentItem.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}