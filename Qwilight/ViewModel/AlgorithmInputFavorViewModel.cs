using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class AlgorithmInputFavorViewModel : BaseViewModel
    {
        public override double TargetLength => double.NaN;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public void OnMeterModified() => ViewModels.Instance.MainValue.ModeComponentValue.SetAutoAlgorithmInputFavorMillis();

        public override bool ClosingCondition
        {
            get
            {
                var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                var algorithmInputFavorMillis = modeComponentValue.AlgorithmInputFavorMillis;
                if (algorithmInputFavorMillis == 0.0)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.AlgorithmInputFavorFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}