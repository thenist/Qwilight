﻿using System.Collections.Concurrent;

namespace Qwilight.ViewModel
{
    public sealed class ViewModels
    {
        readonly ConcurrentDictionary<string, SiteViewModel> _siteViewModels = new();

        public static readonly ViewModels Instance = QwilightComponent.GetBuiltInData<ViewModels>(nameof(ViewModels));

        public BaseViewModel[] WindowViewModels { get; }

        public BaseViewModel[] SilentlyClosableViewModels { get; }

        public MainViewModel MainValue { get; } = new();

        public LogInViewModel LogInValue { get; } = new();

        public EnrollViewModel EnrollValue { get; } = new();

        public ConfigureViewModel ConfigureValue { get; } = new();

        public CommentViewModel CommentValue { get; } = new();

        public ModifyDefaultEntryViewModel ModifyDefaultEntryValue { get; } = new();

        public FavoriteEntryViewModel FavoriteEntryValue { get; } = new();

        public BundleViewModel BundleValue { get; } = new();

        public UbuntuViewModel UbuntuValue { get; } = new();

        public SiteContainerViewModel SiteContainerValue { get; } = new();

        public SiteWindowViewModel SiteWindowValue { get; } = new();

        public InputViewModel InputValue { get; } = new();

        public TwilightConfigureViewModel TwilightConfigure { get; } = new();

        public AssistViewModel AssistValue { get; } = new();

        public AssistFileViewModel AssistFileValue { get; } = new();

        public NoteFileViewModel NoteFileValue { get; } = new();

        public InputStandardControllerViewModel InputStandardControllerValue { get; } = new();

        public InputStandardViewModel InputStandardValue { get; } = new();

        public NotifyViewModel NotifyValue { get; } = new();

        public NetSiteCommentViewModel NetSiteCommentValue { get; } = new();

        public EqualizerViewModel EqualizerValue { get; } = new();

        public FavorJudgmentViewModel FavorJudgmentValue { get; } = new();

        public LongNoteModifyViewModel LongNoteModifyValue { get; } = new();

        public EventNoteViewModel EventNoteValue { get; } = new();

        public ModifyFrontEntryViewModel ModifyFrontEntryValue { get; } = new();

        public ModifyDefaultAudioFilePathViewModel ModifyDefaultAudioFilePathValue { get; } = new();

        public ModifyModeComponentViewModel ModifyModeComponentValue { get; } = new();

        public FontFamilyViewModel FontFamilyValue { get; } = new();

        public LevelViewModel LevelValue { get; } = new();

        public InputFavorLabelledViewModel InputFavorLabelledValue { get; } = new();

        public SetNotePutViewModel SetNotePutValue { get; } = new();

        public InputTextViewModel InputTextValue { get; } = new();

        public InputPwViewModel InputPwValue { get; } = new();

        public VoteViewModel VoteValue { get; } = new();

        public NotifyXamlViewModel NotifyXamlValue { get; } = new();

        public FavorHitPointsViewModel FavorHitPointsValue { get; } = new();

        public ColorViewModel ColorValue { get; } = new();

        public WwwLevelViewModel WwwLevelValue { get; } = new();

        public AvatarViewModel AvatarValue { get; } = new();

        public AvatarTitleViewModel AvatarTitleValue { get; } = new();

        public AvatarEdgeViewModel AvatarEdgeValue { get; } = new();

        public WantViewModel WantValue { get; } = new();

        public LevelVoteViewModel LevelVoteValue { get; } = new();

        public ViewModels()
        {
            WindowViewModels = new BaseViewModel[] {
                LogInValue,
                EnrollValue,
                ConfigureValue,
                CommentValue,
                BundleValue,
                UbuntuValue,
                SiteContainerValue,
                SiteWindowValue,
                ModifyDefaultEntryValue,
                InputValue,
                TwilightConfigure,
                AssistValue,
                AssistFileValue,
                NoteFileValue,
                InputStandardControllerValue,
                InputStandardValue,
                NotifyValue,
                NetSiteCommentValue,
                FavoriteEntryValue,
                EqualizerValue,
                FavorJudgmentValue,
                LongNoteModifyValue,
                EventNoteValue,
                ModifyFrontEntryValue,
                ModifyDefaultAudioFilePathValue,
                ModifyModeComponentValue,
                FontFamilyValue,
                LevelValue,
                InputFavorLabelledValue,
                SetNotePutValue,
                InputTextValue,
                InputPwValue,
                VoteValue,
                FavorHitPointsValue,
                ColorValue,
                WwwLevelValue,
                AvatarValue,
                AvatarTitleValue,
                AvatarEdgeValue,
                WantValue,
                LevelVoteValue
            };
            SilentlyClosableViewModels = WindowViewModels.Except([ConfigureValue]).ToArray();
        }

        public void HandleSiteViewModels(Action<SiteViewModel> onHandle)
        {
            foreach (var siteViewModel in _siteViewModels.Values)
            {
                onHandle(siteViewModel);
            }
        }

        public void HandleSilentlyClosableViewModels(Action<BaseViewModel> onHandle)
        {
            foreach (var silentlyClosableViewModel in SilentlyClosableViewModels)
            {
                onHandle(silentlyClosableViewModel);
            }
        }

        public void NotifyWindowViewModels()
        {
            foreach (var windowViewModel in WindowViewModels)
            {
                windowViewModel.NotifyArea();
                windowViewModel.NotifyIsOpened();
            }
        }

        public SiteViewModel GetSiteViewModel(string siteID) => _siteViewModels.GetValueOrDefault(siteID);

        public SiteViewModel NewSiteViewModel(string siteID)
        {
            var siteViewModel = new SiteViewModel();
            _siteViewModels[siteID] = siteViewModel;
            return siteViewModel;
        }

        public SiteViewModel WipeSiteViewModel(string siteID)
        {
            return _siteViewModels.TryRemove(siteID, out var siteViewModel) ? siteViewModel : null;
        }

        public bool HasSiteViewModel(Func<SiteViewModel, bool> onCondition = null) => onCondition != null ? _siteViewModels.Values.Any(onCondition) : _siteViewModels.Count > 0;

        public void WipeSiteViewModels()
        {
            _siteViewModels.Clear();
        }
    }
}