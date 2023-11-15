using CommunityToolkit.Mvvm.Messaging.Messages;
using Windows.Graphics;

namespace Qwilight.MSG
{
    public sealed class GetWindowArea : RequestMessage<RectInt32>
    {
    }
}