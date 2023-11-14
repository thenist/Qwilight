using System.Windows.Threading;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.UIComponent
{
    public sealed class NotifyXamlItem : Model
    {
        public Brush Paint { get; set; }

        public Color Color { get; set; }

        public NotifySystem.NotifyVariety Variety { get; set; }

        public DispatcherTimer Handler { get; set; }

        public string Contents { get; set; }

        public Action OnHandle { get; init; }

        public int ID { get; init; }

        public override bool Equals(object obj) => obj is NotifyXamlItem toNotifyXamlItem && ID == toNotifyXamlItem.ID;

        public override int GetHashCode() => ID.GetHashCode();

        public void Set(NotifyXamlItem toNotifyXamlItem)
        {
            Paint = toNotifyXamlItem.Paint;
            Color = toNotifyXamlItem.Color;
            Variety = toNotifyXamlItem.Variety;
            Contents = toNotifyXamlItem.Contents;
            OnPropertyChanged(nameof(Paint));
            OnPropertyChanged(nameof(Variety));
            OnPropertyChanged(nameof(Contents));
        }
    }
}