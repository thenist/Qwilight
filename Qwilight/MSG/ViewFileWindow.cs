using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Qwilight.MSG
{
    public sealed class ViewFileWindow : AsyncRequestMessage<string>
    {
        public IEnumerable<string> Filters { get; init; }
    }
}