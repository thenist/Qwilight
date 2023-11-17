using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class EntryView
    {
        public EntryView()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<MoveEntryView>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
            {
                if ((DataContext as MainViewModel).IsNoteFileMode)
                {
                    EntryItemsView.ScrollIntoView(message.Target);
                }
            }));
            StrongReferenceMessenger.Default.Register<PointEntryView>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
            {
                if ((DataContext as MainViewModel).IsNoteFileMode)
                {
                    EntryItemsView.Focus();
                }
            }));
            StrongReferenceMessenger.Default.Register<SetFitInputs>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
            {
                var i = FitInput.SelectedIndex;
                FitInput.Items.Refresh();
                FitInput.SelectedIndex = -1;
                FitInput.SelectedIndex = i;
            }));
        }

        void OnWant(object sender, SelectionChangedEventArgs e) => (DataContext as MainViewModel).Want();

        void OnWant(object sender, TextChangedEventArgs e) => (DataContext as MainViewModel).OnWant();

        void OnLevyNoteFile(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).OnLevyNoteFile(e);

        void OnFitMode(object sender, EventArgs e) => (DataContext as MainViewModel).OnFitMode();

        void OnEntryViewInputLower(object sender, KeyEventArgs e) => (DataContext as MainViewModel).OnEntryViewInputLower(e);

        void OnEntryViewPointLower(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).OnEntryViewPointLower(e);

        void OnSetInputWantPoint(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnSetInputWantPoint(true);

        void OnSetInputWantNotPoint(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnSetInputWantPoint(false);
    }
}