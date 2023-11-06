using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyModifyDefaultEntryWindow : IRecipient<ICC>
    {
        public ModifyModifyDefaultEntryWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.MoveDefaultEntryView:
                    DefaultEntryInput.ScrollIntoView(message.Contents);
                    break;
            }
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as ModifyDefaultEntryViewModel).OnInputLower(e);
    }
}