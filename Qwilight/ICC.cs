namespace Qwilight
{
    public sealed class ICC
    {
        public enum ID
        {
            ViewAllowWindow,
            ViewEntryWindow,
            ViewFileWindow,
            Quit,
            ViewPwWindow,
            ViewInputWindow,
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
            SetWindowedMode,
            SetNoteFileModeWindowInputs,
            GetWindowArea,
            SetWindowArea,
            GetWPFView,
            SetD2DView,
            SetD2DViewArea,
            SetD2DViewVisibility,
            GetWindowHandle,
            SetVoteWindowEdgeView,
            FadingLoadingView
        }

        public ID IDValue { get; init; }

        public object Contents { get; init; }
    }
}