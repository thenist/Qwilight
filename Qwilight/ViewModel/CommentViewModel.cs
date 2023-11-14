namespace Qwilight.ViewModel
{
    public sealed partial class CommentViewModel : BaseViewModel
    {
        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.9;

        public override bool OpeningCondition => BaseUI.Instance.HasCommentPoint;

        public override void OnOpened()
        {
            base.OnOpened();
            ViewModels.Instance.MainValue.LoadCommentCollection();
        }
    }
}