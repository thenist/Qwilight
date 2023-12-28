using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class SetNotePutViewModel : BaseViewModel
    {
        public override double TargetLength => double.NaN;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public void OnMeterModified() => ViewModels.Instance.MainValue.ModeComponentValue.SetAutoSetNotePutMillis();
    }
}