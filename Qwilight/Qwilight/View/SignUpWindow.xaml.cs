﻿using CommunityToolkit.Mvvm.Messaging;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SignUpWindow : IRecipient<ICC>
    {
        public SignUpWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.GetSignUpCipher:
                    (message.Contents as Action<string, string>)(InputCipher.Password ?? string.Empty, InputCipherTest.Password ?? string.Empty);
                    break;
                case ICC.ID.InitSignUpCipher:
                    InputCipher.Password = string.Empty;
                    InputCipherTest.Password = string.Empty;
                    break;
            }
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as SignUpViewModel).OnInputLower(e);
    }
}