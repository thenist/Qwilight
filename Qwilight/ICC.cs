namespace Qwilight
{
    public sealed class ICC
    {
        public enum ID
        {
            MoveEntryView,
            PointEntryView,
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
            SetNoteFileModeWindowInputs,
            GetWPFView,
            GetWindowHandle,
            SetVoteWindowEdgeView,
            FadingLoadingView
        }

        public ID IDValue { get; init; }

        public object Contents { get; init; }
    }
}