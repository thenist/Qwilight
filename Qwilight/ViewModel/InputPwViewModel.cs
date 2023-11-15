using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class InputPwViewModel : BaseViewModel
    {
        string _text;
        string _input;
        bool _isInputEditable;

        public override double TargetLength => 0.5;

        public override double TargetHeight => double.NaN;

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

        public bool IsInputEditable
        {
            get => _isInputEditable;

            set => SetProperty(ref _isInputEditable, value, nameof(IsInputEditable));
        }

        public Action<string, string> HandleOK { get; set; }

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
            HandleOK(Input, StrongReferenceMessenger.Default.Send<GetPwWindowCipher>().Response);
        }

        public override void OnOpened()
        {
            base.OnOpened();
            StrongReferenceMessenger.Default.Send<InitPwWindowCipher>();
        }
    }
}