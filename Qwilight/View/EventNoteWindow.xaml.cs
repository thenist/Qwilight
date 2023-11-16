using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class EventNoteWindow
    {
        public EventNoteWindow()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<PointEventNoteView>(this, (recipient, message) =>
            {
                if (ViewModels.Instance.MainValue.HasPoint)
                {
                    NoteFileInput.Focus();
                }
                NoteFileInput.ScrollIntoView(message.Target);
            });
        }

        void OnNoteFileView(object sender, KeyEventArgs e) => (DataContext as EventNoteViewModel).OnNoteFileView(e);

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as EventNoteViewModel).OnInputLower(e);
    }
}