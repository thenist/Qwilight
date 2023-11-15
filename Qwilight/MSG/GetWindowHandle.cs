using CommunityToolkit.Mvvm.Messaging.Messages;
using Windows.Win32.Foundation;

namespace Qwilight.MSG
{
    internal sealed class GetWindowHandle : RequestMessage<HWND>
    {
    }
}