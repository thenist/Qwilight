using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class LongNoteModifyViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => 0.3;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Bottom;

        public override bool ClosingCondition
        {
            get
            {
                var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                if (modeComponentValue.LowestLongNoteModify > modeComponentValue.HighestLongNoteModify)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.LongNoteModifyFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}