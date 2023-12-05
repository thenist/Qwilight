using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class LongNoteModifyViewModel : BaseViewModel
    {
        public override double TargetLength => double.NaN;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public override bool ClosingCondition
        {
            get
            {
                var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                if (modeComponentValue.ConfigureLowestLongNoteModify > modeComponentValue.ConfigureHighestLongNoteModify)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.LongNoteModifyFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}