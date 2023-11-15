using CommunityToolkit.Mvvm.Messaging.Messages;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight.MSG
{
    internal sealed class ViewAllowWindow : RequestMessage<MESSAGEBOX_RESULT>
    {
        public string Text { get; init; }

        internal MESSAGEBOX_STYLE Data { get; init; }
    }
}