using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class LabelledInputFavorViewModel : BaseViewModel
    {
        public override double TargetLength => double.NaN;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public void OnMeterModified() => ViewModels.Instance.MainValue.ModeComponentValue.SetAutoLabelledInputFavorMillis();

        public override bool ClosingCondition
        {
            get
            {
                var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                var labelledInputFavorMillis = modeComponentValue.LabelledInputFavorMillis;
                if (labelledInputFavorMillis == 0.0)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.LabelledInputFavorFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}