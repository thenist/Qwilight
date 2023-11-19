using Qwilight.UIComponent;
using System.Windows;

namespace Qwilight.View
{
    public partial class BundleView
    {
        public static readonly DependencyProperty BundleItemCollectionProperty = DependencyProperty.Register(nameof(BundleItemCollection), typeof(ICollection<BundleItem>), typeof(BundleView));

        public ICollection<BundleItem> BundleItemCollection
        {
            get => GetValue(BundleItemCollectionProperty) as ICollection<BundleItem>;

            set => SetValue(BundleItemCollectionProperty, value);
        }

        public BundleView() => InitializeComponent();
    }
}
