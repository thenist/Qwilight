using CommunityToolkit.Mvvm.Messaging;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class EventNoteWindow : IRecipient<ICC>
    {
        public EventNoteWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        void OnNoteFileView(object sender, KeyEventArgs e) => (DataContext as EventNoteViewModel).OnNoteFileView(e);

        void OnInputLower(object sender, KeyEventArgs e) => _ = (DataContext as EventNoteViewModel).OnInputLower(e);

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.PointEventNoteView:
                    if (message.IDValue == ICC.ID.PointEventNoteView)
                    {
                        if (ViewModels.Instance.MainValue.HasPoint)
                        {
                            NoteFileInput.Focus();
                        }
                        NoteFileInput.ScrollIntoView(message.Contents);
                    }
                    break;
            }
        }
    }
}