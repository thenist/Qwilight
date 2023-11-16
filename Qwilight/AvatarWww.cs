using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Windows.Media;

namespace Qwilight
{
    public sealed partial class AvatarWww : Model
    {
        bool _wantAvatarDrawing = true;
        ImageSource _avatarDrawing;
        bool _wantAvatarTitle;
        AvatarTitle? _avatarTitle;
        bool _wantAvatarEdge;
        ImageSource _avatarEdge;
        bool _allowNotAvatarTitle;

        public string AvatarID { get; }

        public ImageSource AvatarDrawing
        {
            get
            {
                if (_wantAvatarDrawing)
                {
                    _wantAvatarDrawing = false;

                    Task.Run(async () => SetProperty(ref _avatarDrawing, (await AvatarDrawingSystem.Instance.GetAvatarDrawing(AvatarID).ConfigureAwait(false)).DefaultDrawing, nameof(AvatarDrawing)));
                }
                return _avatarDrawing;
            }
        }

        public AvatarTitle? AvatarTitleValue
        {
            get
            {
                if (_wantAvatarTitle)
                {
                    _wantAvatarTitle = false;

                    Task.Run(async () => SetProperty(ref _avatarTitle, await AvatarTitleSystem.Instance.GetAvatarTitle(AvatarID, _allowNotAvatarTitle).ConfigureAwait(false), nameof(AvatarTitleValue)));
                }
                return _avatarTitle;
            }
        }

        public ImageSource AvatarEdge
        {
            get
            {
                if (_wantAvatarEdge)
                {
                    _wantAvatarEdge = false;

                    Task.Run(async () => SetProperty(ref _avatarEdge, (await AvatarEdgeSystem.Instance.GetAvatarEdge(AvatarID).ConfigureAwait(false)).DefaultDrawing, nameof(AvatarEdge)));
                }
                return _avatarEdge;
            }
        }

        [RelayCommand]
        void OnViewAvatar()
        {
            if (AvatarID.StartsWith('*'))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvatarViewFault);
            }
            else
            {
                var avatarViewModel = ViewModels.Instance.AvatarValue;
                avatarViewModel.Close();
                avatarViewModel.AvatarID = Utility.GetDefaultAvatarID(AvatarID);
                avatarViewModel.Open();
            }
        }

        [RelayCommand]
        void OnViewBundle()
        {
            if (AvatarID.StartsWith('*'))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvatarBundleFault);
            }
            else
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallBundle, AvatarID);
            }
        }

        [RelayCommand]
        void OnNewUbuntu()
        {
            if (AvatarID.StartsWith('*'))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvatarUbuntuFault);
            }
            else
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewUbuntu, AvatarID);
            }
        }

        public override bool Equals(object obj) => obj is AvatarWww avatarWww && AvatarID == avatarWww.AvatarID;

        public override int GetHashCode() => AvatarID.GetHashCode();

        public AvatarWww(string avatarID, AvatarTitle? avatarTitle = null, ImageSource avatarEdge = null, bool allowNotAvatarTitle = false)
        {
            _allowNotAvatarTitle = allowNotAvatarTitle;
            AvatarID = avatarID;
            _avatarDrawing = AvatarDrawingSystem.Instance.JustGetAvatarDrawing(avatarID)?.DefaultDrawing;
            _wantAvatarDrawing = _avatarDrawing == null;
            _avatarTitle = avatarTitle ?? AvatarTitleSystem.Instance.JustGetAvatarTitle(avatarID, _allowNotAvatarTitle);
            _wantAvatarTitle = _avatarTitle == null;
            _avatarEdge = avatarEdge ?? AvatarEdgeSystem.Instance.JustGetAvatarEdge(avatarID)?.DefaultDrawing;
            _wantAvatarEdge = _avatarEdge == null;
        }
    }
}
