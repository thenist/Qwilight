using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Qwilight.MSG
{
    public sealed class ViewOKWindow : AsyncRequestMessage<bool>
    {
        public string Text { get; init; }

        public bool IsDefaultOK { get; init; }
    }
}