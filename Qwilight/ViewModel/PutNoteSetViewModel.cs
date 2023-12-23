using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class PutNoteSetViewModel : BaseViewModel
    {
        public override double TargetLength => double.NaN;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public void OnMeterModified() => ViewModels.Instance.MainValue.ModeComponentValue.SetAutoPutNoteSetMillis();

        public override bool ClosingCondition
        {
            get
            {
                var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                var putNoteSetMillis = modeComponentValue.PutNoteSetMillis;
                if (putNoteSetMillis == 0.0)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.PutNoteSetFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}