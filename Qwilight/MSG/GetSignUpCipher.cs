using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Qwilight.MSG
{
    public sealed class GetSignUpCipher : RequestMessage<(string, string)>
    {
    }
}