using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class CommentView
    {
        public CommentView() => InitializeComponent();

        void OnDefaultCommentViewModified(object sender, SelectionChangedEventArgs e) => (DataContext as MainViewModel).OnDefaultCommentViewModified();

        void OnDefaultComment(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                (DataContext as MainViewModel).OnDefaultComment();
            }
        }

        void OnTwilightCommentViewModified(object sender, SelectionChangedEventArgs e) => (DataContext as MainViewModel).OnTwilightCommentViewModified();

        void OnTwilightComment(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                (DataContext as MainViewModel).OnTwilightComment();
            }
        }

        void OnDefaultCommentViewInputLower(object sender, KeyEventArgs e) => (DataContext as MainViewModel).OnDefaultCommentViewInputLower(e);

        void OnTwilightCommentary(object sender, KeyEventArgs e) => (DataContext as MainViewModel).OnTwilightCommentary(e);

        void OnInputTwilightCommentaryPointed(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnInputTwilightCommentaryPointed(true);

        void OnInputTwilightCommentaryNotPointed(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnInputTwilightCommentaryPointed(false);
    }
}
