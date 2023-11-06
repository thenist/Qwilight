
namespace Qwilight.ViewModel
{
    public sealed partial class AssistFileViewModel : BaseViewModel
    {
        public override double TargetLength => 0.8;

        public override double TargetHeight => 0.8;

        string _title;
        string _assist;

        public string Title
        {
            get => _title;

            set => SetProperty(ref _title, value, nameof(Title));
        }

        public string Assist
        {
            get => _assist;

            set => SetProperty(ref _assist, value, nameof(Assist));
        }
    }
}