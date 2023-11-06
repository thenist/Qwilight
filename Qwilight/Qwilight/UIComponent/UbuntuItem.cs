using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class UbuntuItem : Model
    {
        public enum UbuntuSituation
        {
            NotUbuntu, NotSignedIn, NoteFileMode, DefaultComputing, CommentComputing, AutoComputing, QuitMode, NetComputing, IOComputing
        }

        string _avatarName;
        UbuntuSituation _ubuntuSituationValue;
        string _situationText;

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string AvatarID { get; init; }

        public string AvatarName
        {
            get => _avatarName;

            set => SetProperty(ref _avatarName, value, nameof(AvatarName));
        }

        public UbuntuSituation UbuntuSituationValue
        {
            get => _ubuntuSituationValue;

            set => SetProperty(ref _ubuntuSituationValue, value, nameof(UbuntuSituationValue));
        }

        public string SituationText
        {
            get => _situationText;

            set => SetProperty(ref _situationText, value, nameof(SituationText));
        }

        public AvatarWww AvatarWwwValue { get; }

        public UbuntuItem(string avatarID)
        {
            AvatarID = avatarID;
            AvatarWwwValue = new AvatarWww(avatarID);
        }

        public override string ToString() => UbuntuSituationValue switch
        {
            UbuntuSituation.NotUbuntu => LanguageSystem.Instance.NotUbuntuSituationContents,
            UbuntuSituation.NotSignedIn => string.Format(LanguageSystem.Instance.NotSignedInSituationContents, DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(SituationText)).LocalDateTime.ToString("yyyy-MM-dd tt hh:mm:ss")),
            UbuntuSituation.NoteFileMode => string.Format(LanguageSystem.Instance.NoteFileModeSituationContents, SituationText),
            UbuntuSituation.DefaultComputing => string.Format(LanguageSystem.Instance.DefaultComputingSituationContents, SituationText),
            UbuntuSituation.CommentComputing => string.Format(LanguageSystem.Instance.CommentComputingSituationContents, SituationText),
            UbuntuSituation.AutoComputing => string.Format(LanguageSystem.Instance.AutoComputingSituationContents, SituationText),
            UbuntuSituation.QuitMode => string.Format(LanguageSystem.Instance.QuitComputingSituationContents, SituationText),
            UbuntuSituation.NetComputing => string.Format(LanguageSystem.Instance.NetComputingSituationContents, SituationText),
            UbuntuSituation.IOComputing => string.Format(LanguageSystem.Instance.IOComputingSituationContents, SituationText),
            _ => default,
        };

        public override bool Equals(object obj) => obj is UbuntuItem ubuntuItem && AvatarID == ubuntuItem.AvatarID;

        public override int GetHashCode() => AvatarID.GetHashCode();
    }
}