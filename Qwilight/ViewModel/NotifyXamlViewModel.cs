using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwilight.ViewModel
{
    public sealed class NotifyXamlViewModel : Model
    {
        public ObservableCollection<NotifyXamlItem> NotifyXamlItemUICollection { get; } = new();

        public List<NotifyXamlItem> NotifyXamlItemCollection { get; } = new();

        public void OnPointLower(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var toNotifyXamlItem = (sender as FrameworkElement).DataContext as NotifyXamlItem;
                if (toNotifyXamlItem != null)
                {
                    toNotifyXamlItem.OnHandle?.Invoke();
                    WipeNotify(toNotifyXamlItem);
                }
                e.Handled = true;
            }
        }

        public void NewNotify(NotifyXamlItem toNotifyXamlItem)
        {
            var targetNotifyXamlItem = NotifyXamlItemUICollection.SingleOrDefault(targetNotifyXamlItem => targetNotifyXamlItem.ID == toNotifyXamlItem.ID);
            if (targetNotifyXamlItem != null)
            {
                SetHandler(targetNotifyXamlItem);
                targetNotifyXamlItem.Set(toNotifyXamlItem);
            }
            else
            {
                SetHandler(toNotifyXamlItem);
                NotifyXamlItemUICollection.Add(toNotifyXamlItem);
                lock (NotifyXamlItemCollection)
                {
                    NotifyXamlItemCollection.Add(toNotifyXamlItem);
                }
            }

            void SetHandler(NotifyXamlItem toNotifyXamlItem)
            {
                toNotifyXamlItem.Handler?.Stop();
                toNotifyXamlItem.Handler = new(TimeSpan.FromSeconds(5), DispatcherPriority.Background, (sender, e) =>
                {
                    (sender as DispatcherTimer).Stop();
                    WipeNotify(toNotifyXamlItem);
                }, UIHandler.Instance.Handler);
            }
        }

        public void WipeNotify(NotifyXamlItem toNotifyXamlItem)
        {
            UIHandler.Instance.HandleParallel(() => NotifyXamlItemUICollection.Remove(toNotifyXamlItem));
            lock (NotifyXamlItemCollection)
            {
                NotifyXamlItemCollection.Remove(toNotifyXamlItem);
            }
        }
    }
}