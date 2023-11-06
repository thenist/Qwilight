using CommunityToolkit.Mvvm.Messaging;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyFrontEntryWindow : IRecipient<ICC>
    {
        public ModifyFrontEntryWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.MoveFrontEntryView:
                    FrontEntryInput.ScrollIntoView(message.Contents);
                    break;
            }
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as ModifyFrontEntryViewModel).OnInputLower(e);
    }
}