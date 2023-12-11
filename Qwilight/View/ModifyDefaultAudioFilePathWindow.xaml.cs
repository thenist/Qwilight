using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyDefaultAudioFilePathWindow
    {
        public ModifyDefaultAudioFilePathWindow()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<MoveDefaultAudioFilePathView>(this, (recipient, message) => DefaultAudioFilePathInput.ScrollIntoView(message.Target));
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as ModifyDefaultAudioFilePathViewModel).OnInputLower(e);
    }
}