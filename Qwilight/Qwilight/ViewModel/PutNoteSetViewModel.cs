using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class PutNoteSetViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => 0.25;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Bottom;

        public void OnMeterModified() => ViewModels.Instance.MainValue.ModeComponentValue.SetAutoPutNoteSetMillis();
    }
}