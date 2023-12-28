using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.UIComponent;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyModifyDefaultEntryWindow
    {
        public ModifyModifyDefaultEntryWindow()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<MoveDefaultEntryView>(this, (recipient, message) => DefaultEntryInput.ScrollIntoView(message.Target));
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as ModifyDefaultEntryViewModel).OnInputLower(e);

        void OnLevyNoteFile(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                ((sender as FrameworkElement).DataContext as EntryItem).NoteFile.OnLevyNoteFile();
            }
        }
    }
}