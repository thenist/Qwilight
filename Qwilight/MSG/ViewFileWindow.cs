using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Qwilight.MSG
{
    internal sealed class ViewFileWindow : AsyncRequestMessage<string>
    {
        public IEnumerable<string> Filters { get; init; }
    }
}