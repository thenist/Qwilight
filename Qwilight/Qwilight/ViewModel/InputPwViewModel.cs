using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class InputPwViewModel : BaseViewModel
    {
        string _text;
        string _input;
        bool _isInputEditable;

        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.2;

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

        public Action<string, string> Handler { get; set; }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Close();
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.GetPwWindowCipher,
                    Contents = new Action<string>(inputCipher =>
                    {
                        Handler(Input, inputCipher);
                    })
                });
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.ClearPwWindowCipher
            });
        }
    }
}