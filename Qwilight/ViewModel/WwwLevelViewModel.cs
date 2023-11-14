using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Net;

namespace Qwilight.ViewModel
{
    public sealed partial class WwwLevelViewModel : BaseViewModel
    {
        WwwLevelAvatar _wwwLevelAvatarValue;
        WwwLevelGroup _wwwLevelGroupValue;
        bool _isAvatarIDsLoading;
        bool _isLevelGroupsLoading;
        bool _isLevelGroupLoading;

        [RelayCommand]
        void OnWwwLevelTest() => WwwLevelAvatarValue = null;

        [RelayCommand]
        void OnViewBundle() => WwwLevelAvatarValue?.AvatarWwwValue?.ViewBundleCommand?.Execute(null);

        [RelayCommand]
        void OnNewUbuntu() => WwwLevelAvatarValue?.AvatarWwwValue?.NewUbuntuCommand?.Execute(null);

        [RelayCommand]
        void OnViewAvatar() => WwwLevelAvatarValue?.AvatarWwwValue?.ViewAvatarCommand?.Execute(null);

        public bool IsLevelGroupLoading
        {
            get => _isLevelGroupLoading;

            set => SetProperty(ref _isLevelGroupLoading, value, nameof(IsLevelGroupLoading));
        }

        public override double TargetLength => 0.9;

        public string[] HandledLevelIDs { get; set; }

        public WwwLevelAvatar WwwLevelAvatarValue
        {
            get => _wwwLevelAvatarValue;

            set
            {
                if (SetProperty(ref _wwwLevelAvatarValue, value, nameof(WwwLevelAvatar)))
                {
                    GetWwwLevelGroups();
                    async void GetWwwLevelGroups()
                    {
                        IsLevelGroupsLoading = true;

                        var avatarID = value?.AvatarWwwValue?.AvatarID ?? string.Empty;
                        var levelNames = await TwilightSystem.Instance.GetWwwParallel<string[]>($"{QwilightComponent.QwilightAPI}/level?avatarID={WebUtility.UrlEncode(avatarID)}");
                        if (levelNames != null && (WwwLevelAvatarValue?.AvatarWwwValue?.AvatarID ?? string.Empty) == avatarID)
                        {
                            Utility.SetUICollection(WwwLevelGroupCollection, levelNames.Select(levelName => new WwwLevelGroup
                            {
                                LevelName = levelName
                            }).ToArray());
                            WwwLevelGroupValue ??= WwwLevelGroupCollection.FirstOrDefault();
                        }

                        IsLevelGroupsLoading = false;
                    }
                }
            }
        }

        public ObservableCollection<WwwLevelGroup> WwwLevelGroupCollection { get; } = new();

        public ObservableCollection<WwwLevelAvatar> WwwLevelAvatarCollection { get; } = new();

        public bool IsAvatarIDsLoading
        {
            get => _isAvatarIDsLoading;

            set => SetProperty(ref _isAvatarIDsLoading, value, nameof(IsAvatarIDsLoading));
        }

        public bool IsLevelGroupsLoading
        {
            get => _isLevelGroupsLoading;

            set => SetProperty(ref _isLevelGroupsLoading, value, nameof(IsLevelGroupsLoading));
        }

        public WwwLevelGroup WwwLevelGroupValue
        {
            get => _wwwLevelGroupValue;

            set
            {
                if (SetProperty(ref _wwwLevelGroupValue, value, nameof(WwwLevelGroupValue)) && value != null)
                {
                    GetWwwLevelItems();
                    async void GetWwwLevelItems()
                    {
                        IsLevelGroupLoading = true;

                        var levelName = value.LevelName;
                        var twilightWwwLevels = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwLevels[]>($"{QwilightComponent.QwilightAPI}/level?levelName={WebUtility.UrlEncode(levelName)}");
                        if (twilightWwwLevels != null && WwwLevelGroupValue?.LevelName == levelName)
                        {
                            var wwwLevelItemCollection = value.WwwLevelItemCollection;
                            Utility.SetUICollection(wwwLevelItemCollection, twilightWwwLevels.Select(twilightWwwLevel =>
                            {
                                var levelID = twilightWwwLevel.levelID;
                                return new WwwLevelItem
                                {
                                    LevelID = levelID,
                                    Title = twilightWwwLevel.title,
                                    Comment = twilightWwwLevel.comment,
                                    LevelText = twilightWwwLevel.levelText,
                                    LevelValue = twilightWwwLevel.level,
                                    Handled = HandledLevelIDs.Contains(levelID),
                                    Avatars = twilightWwwLevel.avatars
                                };
                            }).ToArray());
                            value.WwwLevelItemValue ??= wwwLevelItemCollection.FirstOrDefault();
                        }

                        IsLevelGroupLoading = false;
                    }
                }
            }
        }

        public override bool OpeningCondition => ViewModels.Instance.MainValue.IsNoteFileMode;

        public override async void OnOpened()
        {
            base.OnOpened();
            IsAvatarIDsLoading = true;
            var twilightWwwLevelAvatars = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwLevelAvatars?>($"{QwilightComponent.QwilightAPI}/level?avatarIDMe={WebUtility.UrlEncode(TwilightSystem.Instance.AvatarID)}");
            if (twilightWwwLevelAvatars.HasValue)
            {
                var twilightWwwLevelAvatarsValue = twilightWwwLevelAvatars.Value;

                Utility.SetUICollection(WwwLevelAvatarCollection, twilightWwwLevelAvatarsValue.avatars.Select(avatar => new WwwLevelAvatar
                {
                    AvatarWwwValue = new AvatarWww(avatar.avatarID),
                    AvatarName = avatar.avatarName
                }).ToArray());

                HandledLevelIDs = twilightWwwLevelAvatarsValue.levelIDs;
                HandlingUISystem.Instance.HandleParallel(() => WwwLevelAvatarValue ??= WwwLevelAvatarCollection.FirstOrDefault());

                IsAvatarIDsLoading = false;
            }
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            ViewModels.Instance.MainValue.Want();
        }
    }
}