using CommunityToolkit.Mvvm.Messaging.Messages;
using Qwilight.View;

namespace Qwilight.MSG
{
    public sealed class GetSiteView : RequestMessage<SiteView>
    {
        public string SiteID { get; init; }
    }
}