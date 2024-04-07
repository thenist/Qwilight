using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class InputTextViewModel : BaseViewModel
    {
        string _text;
        string _input = string.Empty;

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

        public override double TargetHeight => double.NaN;

        public Action<string> HandleOK { get; set; }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnOK();
            }
        }

        [RelayCommand]
        void OnOK()
        {
            Close();
            HandleOK(Input);
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            Input = string.Empty;
        }
    }
}