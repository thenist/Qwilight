using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class InputTextViewModel : BaseViewModel
    {
        string _text;
        string _input;

        public string Text
        {
            get => _text;

            set => SetProperty(ref _text, value, nameof(Text));
        }

        public string Input
        {
            get => _input;

            set => SetProperty(ref _input, value, nameof(Input));
        }

        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.15;

        public Action<string> OnOK { get; set; }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Close();
                OnOK(Input);
            }
        }
    }
}