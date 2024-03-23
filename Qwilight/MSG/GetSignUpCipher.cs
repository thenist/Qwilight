using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Qwilight.MSG
{
    public sealed class GetEnrollCipher : RequestMessage<(string, string)>
    {
    }
}