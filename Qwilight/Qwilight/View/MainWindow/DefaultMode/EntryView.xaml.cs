using CommunityToolkit.Mvvm.Messaging;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class EntryView : IRecipient<ICC>
    {
        public EntryView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        void OnWant(object sender, SelectionChangedEventArgs e) => (DataContext as MainViewModel).Want();

        void OnWant(object sender, TextChangedEventArgs e) => (DataContext as MainViewModel).OnWant();

        void OnLevyNoteFile(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).OnLevyNoteFile(e);

        void OnFitMode(object sender, EventArgs e) => (DataContext as MainViewModel).OnFitMode();

        void OnEntryViewInputLower(object sender, KeyEventArgs e) => (DataContext as MainViewModel).OnEntryViewInputLower(e);

        void OnEntryViewPointLower(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).OnEntryViewPointLower(e);

        void OnInputWantPointed(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnInputWantPointed(true);

        void OnInputWantNotPointed(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnInputWantPointed(false);

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.MoveEntryView:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        if ((DataContext as MainViewModel).IsNoteFileMode)
                        {
                            EntryItemsView.ScrollIntoView(message.Contents);
                        }
                    });
                    break;
                case ICC.ID.PointEntryView:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        if ((DataContext as MainViewModel).IsNoteFileMode)
                        {
                            EntryItemsView.Focus();
                        }
                    });
                    break;
                case ICC.ID.SetNoteFileModeWindowInputs:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        var i = FitInput.SelectedIndex;
                        FitInput.Items.Refresh();
                        FitInput.SelectedIndex = -1;
                        FitInput.SelectedIndex = i;
                    });
                    break;
            }
        }
    }
}