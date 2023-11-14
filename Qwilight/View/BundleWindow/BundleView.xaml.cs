using Qwilight.UIComponent;
using System.Windows;

namespace Qwilight.View
{
    public partial class BundleView
    {
        public static readonly DependencyProperty BundleCollectionProperty = DependencyProperty.Register(nameof(BundleCollection), typeof(ICollection<BundleItem>), typeof(BundleView));

        public ICollection<BundleItem> BundleCollection
        {
            get => GetValue(BundleCollectionProperty) as ICollection<BundleItem>;

            set => SetValue(BundleCollectionProperty, value);
        }

        public BundleView() => InitializeComponent();
    }
}
