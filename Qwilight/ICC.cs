namespace Qwilight
{
    public sealed class ICC
    {
        public enum ID
        {
            MoveDefaultEntryView,
            MoveFrontEntryView,
            PointEventNoteView,
            PointZMaxView,
            GetSignInCipher,
            SetSignInCipher,
            GetSignUpCipher,
            InitSignUpCipher,
            GetPwWindowCipher,
            ClearPwWindowCipher,
            SetBaseDrawingUIElement,
            GetWPFView,
            GetWindowHandle,
            SetVoteWindowEdgeView
        }

        public ID IDValue { get; init; }

        public object Contents { get; init; }
    }
}