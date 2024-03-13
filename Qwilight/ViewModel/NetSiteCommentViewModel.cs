
using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class NetSiteCommentViewModel : BaseViewModel
    {
        NetSiteCommentItems? _netSiteCommentItems;

        public ObservableCollection<NetSiteCommentItem> NetSiteCommentItemCollection { get; } = new();

        public ObservableCollection<NetSiteCommentItems> NetSiteCommentItemsCollection { get; } = new();

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        public NetSiteCommentItems? NetSiteCommentItemsValue
        {
            get => _netSiteCommentItems;

            set
            {
                if (SetProperty(ref _netSiteCommentItems, value, nameof(NetSiteCommentItemsValue)))
                {
                    var netSiteCommentItems = value?.Values;
                    if (netSiteCommentItems != null)
                    {
                        NetSiteCommentItemCollection.Clear();
                        foreach (var netSiteCommentItem in netSiteCommentItems)
                        {
                            NetSiteCommentItemCollection.Add(netSiteCommentItem);
                        }
                    }
                }
            }
        }
    }
}