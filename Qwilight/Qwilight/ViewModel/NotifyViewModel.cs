using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class NotifyViewModel : BaseViewModel
    {
        NotifyItem _toNotifyItemValue;

        public override double TargetLength => 0.8;

        public override double TargetHeight => 0.6;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Top;

        public ObservableCollection<NotifyItem> NotifyItemCollection { get; } = new();

        public NotifyItem NotifyItemValue
        {
            get => _toNotifyItemValue;

            set => SetProperty(ref _toNotifyItemValue, value, nameof(NotifyItemValue));
        }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var toNotifyItemValue = NotifyItemValue;
                if (toNotifyItemValue?.OnStop?.Invoke(false) != false)
                {
                    NotifyItemCollection.Remove(toNotifyItemValue);
                }
            }
        }

        public void OnPointLower(MouseButtonEventArgs e)
        {
            NotifyItemValue?.OnHandle?.Invoke();
        }

        [RelayCommand]
        void OnWipeTotalNotify()
        {
            for (var i = NotifyItemCollection.Count - 1; i >= 0; --i)
            {
                var toNotifyItemValue = NotifyItemCollection[i];
                if (toNotifyItemValue.OnStop?.Invoke(true) != false)
                {
                    NotifyItemCollection.Remove(toNotifyItemValue);
                }
            }
        }
    }
}