using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyFrontEntryWindow
    {
        public ModifyFrontEntryWindow()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<MoveFrontEntryView>(this, (recipient, message) => FrontEntryInput.ScrollIntoView(message.Target));
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as ModifyFrontEntryViewModel).OnInputLower(e);
    }
}