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
                var modeComponent = ViewModels.Instance.MainValue.ModeComponentValue;
                var lowestLongNoteModify = modeComponent.LowestLongNoteModify;
                var highestLongNoteModify = modeComponent.HighestLongNoteModify;
                if (lowestLongNoteModify == 0.0 || highestLongNoteModify == 0.0 || lowestLongNoteModify > highestLongNoteModify)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.LongNoteModifyFaultText);
                    return false;
                }

                return base.ClosingCondition;
            }
        }
    }
}