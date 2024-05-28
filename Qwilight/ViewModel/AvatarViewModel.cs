using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public sealed partial class AvatarViewModel : BaseViewModel
    {
        public sealed class AvatarComputing : BaseNoteFile
        {
            string _avatarID;

            public override BaseNoteFile.NoteVariety NoteVarietyValue { get; }

            public override void OnCompiled()
            {
            }

            public override void OnFault(Exception e)
            {
            }

            public new void OnLevyNoteFile(MouseButtonEventArgs e)
            {
                var mainViewModel = ViewModels.Instance.MainValue;
                if (mainViewModel.NoteID512s.TryGetValue(GetNoteID512(), out var noteFile))
                {
                    mainViewModel.EntryItemValue = noteFile.EntryItem;
                    if (e.ClickCount >= 2)
                    {
                        mainViewModel.HandleLevyNoteFile(noteFile, noteFile.EntryItem, _avatarID);
                    }
                }
            }

            public bool HaveIt { get; }

            public AvatarComputing(string avatarID, JSON.Computing data) : base(ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.NoteFilePath, ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.DefaultEntryItem, ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.EntryItem)
            {
                _avatarID = avatarID;
                SetNoteIDs(null, null, data.noteID);
                HaveIt = ViewModels.Instance.MainValue.NoteID512s.TryGetValue(GetNoteID512(), out var noteFile);
                if (HaveIt)
                {
                    NoteVarietyValue = noteFile.NoteVarietyValue;
                    Title = noteFile.Title;
                    Artist = noteFile.Artist;
                    Genre = noteFile.Genre;
                    LevelValue = noteFile.LevelValue;
                    LevelText = noteFile.LevelText;
                    NoteDrawingPath = noteFile.NoteDrawingPath;
                    BannerDrawingPath = noteFile.BannerDrawingPath;
                }
                else
                {
                    NoteVarietyValue = data.noteVariety;
                    Title = data.title;
                    Artist = data.artist;
                    Genre = data.genre;
                    LevelValue = data.level;
                    LevelText = data.levelText;
                }
                HandledValue = new()
                {
                    IDValue = data.handled
                };
            }
        }

        readonly int[] _avatarLevels = new int[3];
        Dictionary<string, JSON.TwilightWwwAvatarLevelVS> _twilightWwwAvatarLevelVSMap;
        Dictionary<string, JSON.TwilightWwwAvatarHandled> _twilightWwwAvatarHandledMap;
        string _avatarAbility5KPlaceText0 = string.Empty;
        string _avatarAbility5KPlaceText1 = string.Empty;
        ImageSource _abilityClass5KDrawing;
        double _avatarAbility5K;
        double _avatarAbility5KStatus;
        string _avatarAbility7KPlaceText0 = string.Empty;
        string _avatarAbility7KPlaceText1 = string.Empty;
        ImageSource _abilityClass7KDrawing;
        double _avatarAbility7K;
        double _avatarAbility7KStatus;
        string _avatarAbility9KPlaceText0 = string.Empty;
        string _avatarAbility9KPlaceText1 = string.Empty;
        ImageSource _abilityClass9KDrawing;
        double _avatarAbility9K;
        double _avatarAbility9KStatus;
        bool _isAvatarLoading;
        string _avatarID;
        string _avatarName;
        int _totalCount;
        double _totalLength;
        int _topCount;
        DateTime _date;
        int _wwwLevelIDCount;
        int _avatarTabPosition;
        int _abilityTabPosition;
        int _favoritesTabPosition;
        int _lastsTabPosition;
        bool _isAvatarFavorites6KLoading;
        bool _isAvatarFavorites5KLoading;
        bool _isAvatarFavorites7KLoading;
        bool _isAvatarFavorites9KLoading;
        bool _isAvatarFavorites10KLoading;
        bool _isAvatarFavorites14KLoading;
        bool _isAvatarFavorites24KLoading;
        bool _isAvatarFavorites48KLoading;
        bool _isAvatarLasts6KLoading;
        bool _isAvatarLasts5KLoading;
        bool _isAvatarLasts7KLoading;
        bool _isAvatarLasts9KLoading;
        bool _isAvatarLasts10KLoading;
        bool _isAvatarLasts14KLoading;
        bool _isAvatarLasts24KLoading;
        bool _isAvatarLasts48KLoading;
        bool _isAvatarAbility5KLoading;
        bool _isAvatarAbility7KLoading;
        bool _isAvatarAbility9KLoading;
        bool _isAvatarWwwLevelLoading;
        string _handledLevelName;
        HandledLevelIDItem _handledLevelIDItem;
        string _handledYell1Text;
        string _handledBand1Text;
        string _handledHighestClearText;
        string _handledHigherClearText;
        string _handledClearText;
        string _handledAssistClearText;
        bool _isHandledLoading;
        string _levelVSLevelName;
        AvatarWww _levelVSMyAvatarWww;
        string _levelVSMyAvatarName;
        AvatarWww _levelVSTargetAvatarWww;
        string _levelVSTargetAvatarName;
        LevelVSLevelIDItem _levelVSLevelIDItem;
        bool _isLevelVSLoading;

        public ObservableCollection<AvatarComputing> Favorites6KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites5KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites7KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites9KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites10KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites14KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites24KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites48KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts6KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts5KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts7KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts9KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts10KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts14KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts24KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Lasts48KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability5KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability7KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability9KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarLevelItem> WwwLevelIDCollection { get; } = new();

        public ObservableCollection<string> LevelVSLevelNameCollection { get; } = new();

        public ObservableCollection<HandledLevelIDItem> HandledLevelIDItemCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledYell1AvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledBand1AvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledHighestClearAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledHigherClearAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledAssistClearAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> HandledClearAvatarComputingCollection { get; } = new();

        public ObservableCollection<string> HandledLevelNameCollection { get; } = new();

        public string HandledYell1Text
        {
            get => _handledYell1Text;

            set => SetProperty(ref _handledYell1Text, value, nameof(HandledYell1Text));
        }

        public string HandledBand1Text
        {
            get => _handledBand1Text;

            set => SetProperty(ref _handledBand1Text, value, nameof(HandledBand1Text));
        }

        public string HandledHighestClearText
        {
            get => _handledHighestClearText;

            set => SetProperty(ref _handledHighestClearText, value, nameof(HandledHighestClearText));
        }

        public string HandledHigherClearText
        {
            get => _handledHigherClearText;

            set => SetProperty(ref _handledHigherClearText, value, nameof(HandledHigherClearText));
        }

        public string HandledClearText
        {
            get => _handledClearText;

            set => SetProperty(ref _handledClearText, value, nameof(HandledClearText));
        }

        public string HandledAssistClearText
        {
            get => _handledAssistClearText;

            set => SetProperty(ref _handledAssistClearText, value, nameof(HandledAssistClearText));
        }

        public ObservableCollection<LevelVSLevelIDItem> LevelVSLevelIDItemCollection { get; } = new();

        public ObservableCollection<AvatarComputing> LevelVSMyAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> LevelVSTargetAvatarComputingCollection { get; } = new();

        public bool IsHandledLoading
        {
            get => _isHandledLoading;

            set => SetProperty(ref _isHandledLoading, value, nameof(IsHandledLoading));
        }

        public string HandledLevelName
        {
            get => _handledLevelName;

            set
            {
                if (SetProperty(ref _handledLevelName, value, nameof(HandledLevelName)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public HandledLevelIDItem HandledLevelIDItemValue
        {
            get => _handledLevelIDItem;

            set
            {
                if (SetProperty(ref _handledLevelIDItem, value, nameof(HandledLevelIDItemValue)))
                {
                    HandledYell1AvatarComputingCollection.Clear();
                    HandledBand1AvatarComputingCollection.Clear();
                    HandledHighestClearAvatarComputingCollection.Clear();
                    HandledHigherClearAvatarComputingCollection.Clear();
                    HandledAssistClearAvatarComputingCollection.Clear();
                    HandledClearAvatarComputingCollection.Clear();
                    if (value != null)
                    {
                        var avatarHandledMap = _twilightWwwAvatarHandledMap[value.LevelID];
                        var noteIDCount = avatarHandledMap.noteIDCount;
                        var avatarHandledItems = avatarHandledMap.avatarHandledItems;

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.Yell1, out var handledYell1Items))
                        {
                            foreach (var data in handledYell1Items)
                            {
                                HandledYell1AvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledYell1Text = string.Format(LanguageSystem.Instance.AvatarHandledYell1Text, (100.0 * avatarHandledMap.handledYell1Count / noteIDCount).ToString("0.##％"));

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.Band1, out var handledBand1Items))
                        {
                            foreach (var data in handledBand1Items)
                            {
                                HandledBand1AvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledBand1Text = string.Format(LanguageSystem.Instance.AvatarHandledBand1Text, (100.0 * avatarHandledMap.handledBand1Count / noteIDCount).ToString("0.##％"));

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.HighestClear, out var handledHighestClearItems))
                        {
                            foreach (var data in handledHighestClearItems)
                            {
                                HandledHighestClearAvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledHighestClearText = string.Format(LanguageSystem.Instance.AvatarHandledHighestClearText, (100.0 * avatarHandledMap.handledHighestClearCount / noteIDCount).ToString("0.##％"));

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.HigherClear, out var handledHigherClearItems))
                        {
                            foreach (var data in handledHigherClearItems)
                            {
                                HandledHigherClearAvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledHigherClearText = string.Format(LanguageSystem.Instance.AvatarHandledHigherClearText, (100.0 * avatarHandledMap.handledHigherClearCount / noteIDCount).ToString("0.##％"));

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.Clear, out var handledClearItems))
                        {
                            foreach (var data in handledClearItems)
                            {
                                HandledClearAvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledClearText = string.Format(LanguageSystem.Instance.AvatarHandledClearText, (100.0 * avatarHandledMap.handledClearCount / noteIDCount).ToString("0.##％"));

                        if (avatarHandledItems.TryGetValue(BaseNoteFile.Handled.ID.AssistClear, out var handledAssistClearItems))
                        {
                            foreach (var data in handledAssistClearItems)
                            {
                                HandledAssistClearAvatarComputingCollection.Add(new(_avatarID, data)
                                {
                                    FittedText = data.stand.ToString(LanguageSystem.Instance.StandContents)
                                });
                            }
                        }
                        HandledAssistClearText = string.Format(LanguageSystem.Instance.AvatarHandledAssistClearText, (100.0 * avatarHandledMap.handledAssistClearCount / noteIDCount).ToString("0.##％"));
                    }
                }
            }
        }

        public string LevelVSLevelName
        {
            get => _levelVSLevelName;

            set
            {
                if (SetProperty(ref _levelVSLevelName, value, nameof(LevelVSLevelName)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public bool IsLevelVSVisible => TwilightSystem.Instance.IsLoggedIn && !IsMe;

        public void NotifyIsLevelVSVisible() => OnPropertyChanged(nameof(IsLevelVSVisible));

        public LevelVSLevelIDItem LevelVSLevelIDItemValue
        {
            get => _levelVSLevelIDItem;

            set
            {
                if (SetProperty(ref _levelVSLevelIDItem, value, nameof(LevelVSLevelIDItemValue)))
                {
                    LevelVSMyAvatarComputingCollection.Clear();
                    LevelVSTargetAvatarComputingCollection.Clear();
                    if (value != null)
                    {
                        var twilightWwwAvatarLevelVS = _twilightWwwAvatarLevelVSMap[value.LevelID];
                        foreach (var data in twilightWwwAvatarLevelVS.avatarLevelVSItems)
                        {
                            LevelVSMyAvatarComputingCollection.Add(new(_avatarID, data)
                            {
                                FittedText = string.Format(LanguageSystem.Instance.LevelVSStandContents, data.stand.ToString("#,##0"), data.levelVSStand.ToString("+#,##0;-#,##0"))
                            });
                        }
                        foreach (var data in twilightWwwAvatarLevelVS.targetLevelVSItems)
                        {
                            LevelVSTargetAvatarComputingCollection.Add(new(_avatarID, data)
                            {
                                FittedText = string.Format(LanguageSystem.Instance.LevelVSStandContents, data.stand.ToString("#,##0"), data.levelVSStand.ToString("+#,##0;-#,##0"))
                            });
                        }
                    }
                }
            }
        }

        public AvatarWww LevelVSMyAvatarWwwValue
        {
            get => _levelVSMyAvatarWww;

            set => SetProperty(ref _levelVSMyAvatarWww, value, nameof(LevelVSMyAvatarWwwValue));
        }

        public string LevelVSMyAvatarName
        {
            get => _levelVSMyAvatarName;

            set => SetProperty(ref _levelVSMyAvatarName, value, nameof(LevelVSMyAvatarName));
        }

        public AvatarWww LevelVSTargetAvatarWwwValue
        {
            get => _levelVSTargetAvatarWww;

            set => SetProperty(ref _levelVSTargetAvatarWww, value, nameof(LevelVSTargetAvatarWwwValue));
        }

        public string LevelVSTargetAvatarName
        {
            get => _levelVSTargetAvatarName;

            set => SetProperty(ref _levelVSTargetAvatarName, value, nameof(LevelVSTargetAvatarName));
        }

        public bool IsLevelVSLoading
        {
            get => _isLevelVSLoading;

            set => SetProperty(ref _isLevelVSLoading, value, nameof(IsLevelVSLoading));
        }

        public override double TargetHeight => 0.9;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        public AvatarWww AvatarWwwValue { get; set; }

        public bool IsAvatarFavorites6KLoading
        {
            get => _isAvatarFavorites6KLoading;

            set => SetProperty(ref _isAvatarFavorites6KLoading, value, nameof(IsAvatarFavorites6KLoading));
        }

        public bool IsAvatarFavorites5KLoading
        {
            get => _isAvatarFavorites5KLoading;

            set => SetProperty(ref _isAvatarFavorites5KLoading, value, nameof(IsAvatarFavorites5KLoading));
        }

        public bool IsAvatarFavorites7KLoading
        {
            get => _isAvatarFavorites7KLoading;

            set => SetProperty(ref _isAvatarFavorites7KLoading, value, nameof(IsAvatarFavorites7KLoading));
        }

        public bool IsAvatarFavorites9KLoading
        {
            get => _isAvatarFavorites9KLoading;

            set => SetProperty(ref _isAvatarFavorites9KLoading, value, nameof(IsAvatarFavorites9KLoading));
        }

        public bool IsAvatarFavorites10KLoading
        {
            get => _isAvatarFavorites10KLoading;

            set => SetProperty(ref _isAvatarFavorites10KLoading, value, nameof(IsAvatarFavorites10KLoading));
        }

        public bool IsAvatarFavorites14KLoading
        {
            get => _isAvatarFavorites14KLoading;

            set => SetProperty(ref _isAvatarFavorites14KLoading, value, nameof(IsAvatarFavorites14KLoading));
        }

        public bool IsAvatarFavorites24KLoading
        {
            get => _isAvatarFavorites24KLoading;

            set => SetProperty(ref _isAvatarFavorites24KLoading, value, nameof(IsAvatarFavorites24KLoading));
        }

        public bool IsAvatarFavorites48KLoading
        {
            get => _isAvatarFavorites48KLoading;

            set => SetProperty(ref _isAvatarFavorites48KLoading, value, nameof(IsAvatarFavorites48KLoading));
        }

        public bool IsAvatarLasts6KLoading
        {
            get => _isAvatarLasts6KLoading;

            set => SetProperty(ref _isAvatarLasts6KLoading, value, nameof(IsAvatarLasts6KLoading));
        }

        public bool IsAvatarLasts5KLoading
        {
            get => _isAvatarLasts5KLoading;

            set => SetProperty(ref _isAvatarLasts5KLoading, value, nameof(IsAvatarLasts5KLoading));
        }

        public bool IsAvatarLasts7KLoading
        {
            get => _isAvatarLasts7KLoading;

            set => SetProperty(ref _isAvatarLasts7KLoading, value, nameof(IsAvatarLasts7KLoading));
        }

        public bool IsAvatarLasts9KLoading
        {
            get => _isAvatarLasts9KLoading;

            set => SetProperty(ref _isAvatarLasts9KLoading, value, nameof(IsAvatarLasts9KLoading));
        }

        public bool IsAvatarLasts10KLoading
        {
            get => _isAvatarLasts10KLoading;

            set => SetProperty(ref _isAvatarLasts10KLoading, value, nameof(IsAvatarLasts10KLoading));
        }

        public bool IsAvatarLasts14KLoading
        {
            get => _isAvatarLasts14KLoading;

            set => SetProperty(ref _isAvatarLasts14KLoading, value, nameof(IsAvatarLasts14KLoading));
        }

        public bool IsAvatarLasts24KLoading
        {
            get => _isAvatarLasts24KLoading;

            set => SetProperty(ref _isAvatarLasts24KLoading, value, nameof(IsAvatarLasts24KLoading));
        }

        public bool IsAvatarLasts48KLoading
        {
            get => _isAvatarLasts48KLoading;

            set => SetProperty(ref _isAvatarLasts48KLoading, value, nameof(IsAvatarLasts48KLoading));
        }

        public bool IsAvatarAbility5KLoading
        {
            get => _isAvatarAbility5KLoading;

            set => SetProperty(ref _isAvatarAbility5KLoading, value, nameof(IsAvatarAbility5KLoading));
        }

        public bool IsAvatarAbility7KLoading
        {
            get => _isAvatarAbility7KLoading;

            set => SetProperty(ref _isAvatarAbility7KLoading, value, nameof(IsAvatarAbility7KLoading));
        }

        public bool IsAvatarAbility9KLoading
        {
            get => _isAvatarAbility9KLoading;

            set => SetProperty(ref _isAvatarAbility9KLoading, value, nameof(IsAvatarAbility9KLoading));
        }

        public bool IsAvatarWwwLevelLoading
        {
            get => _isAvatarWwwLevelLoading;

            set => SetProperty(ref _isAvatarWwwLevelLoading, value, nameof(IsAvatarWwwLevelLoading));
        }

        async Task CallAvatarAPI()
        {
            switch (AvatarTabPosition)
            {
                case 0:
                    switch (FavoritesTabPosition)
                    {
                        case 0:
                            IsAvatarFavorites6KLoading = true;
                            var twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/6K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites6KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites6KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        Title = data.title,
                                        Artist = data.artist,
                                        Genre = data.genre,
                                        LevelValue = data.level,
                                        LevelText = data.levelText,
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites6KLoading = false;
                            break;
                        case 1:
                            IsAvatarFavorites5KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/5K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites5KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites5KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        Title = data.title,
                                        Artist = data.artist,
                                        Genre = data.genre,
                                        LevelValue = data.level,
                                        LevelText = data.levelText,
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites5KLoading = false;
                            break;
                        case 2:
                            IsAvatarFavorites7KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/7K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites7KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites7KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites7KLoading = false;
                            break;
                        case 3:
                            IsAvatarFavorites9KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/9K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites9KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites9KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites9KLoading = false;
                            break;
                        case 4:
                            IsAvatarFavorites10KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/10K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites10KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites10KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites10KLoading = false;
                            break;
                        case 5:
                            IsAvatarFavorites14KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/14K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites14KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites14KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites14KLoading = false;
                            break;
                        case 6:
                            IsAvatarFavorites24KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/24K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites24KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites24KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites24KLoading = false;
                            break;
                        case 7:
                            IsAvatarFavorites48KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/48K?avatarID={_avatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites48KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites48KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                                    });
                                }
                            }
                            IsAvatarFavorites48KLoading = false;
                            break;
                    }
                    break;
                case 1:
                    switch (LastsTabPosition)
                    {
                        case 0:
                            IsAvatarLasts6KLoading = true;
                            var twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/6K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts6KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts6KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts6KLoading = false;
                            break;
                        case 1:
                            IsAvatarLasts5KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/5K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts5KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts5KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts5KLoading = false;
                            break;
                        case 2:
                            IsAvatarLasts7KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/7K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts7KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts7KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts7KLoading = false;
                            break;
                        case 3:
                            IsAvatarLasts9KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/9K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts9KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts9KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts9KLoading = false;
                            break;
                        case 4:
                            IsAvatarLasts10KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/10K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts10KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts10KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts10KLoading = false;
                            break;
                        case 5:
                            IsAvatarLasts14KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/14K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts14KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts14KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts14KLoading = false;
                            break;
                        case 6:
                            IsAvatarLasts24KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/24K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts24KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts24KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts24KLoading = false;
                            break;
                        case 7:
                            IsAvatarLasts48KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/48K?avatarID={_avatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts48KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts48KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                                    });
                                }
                            }
                            IsAvatarLasts48KLoading = false;
                            break;
                    }
                    break;
                case 2:
                    switch (AbilityTabPosition)
                    {
                        case 0:
                            IsAvatarAbility5KLoading = true;
                            var twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/5K?avatarID={_avatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                Ability5KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability5KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = string.Format(LanguageSystem.Instance.AbilityFittedText, data.stand.ToString("#,##0"), Math.Round(data.ability, 2))
                                    });
                                }
                            }
                            IsAvatarAbility5KLoading = false;
                            break;
                        case 1:
                            IsAvatarAbility7KLoading = true;
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/7K?avatarID={_avatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                Ability7KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability7KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = string.Format(LanguageSystem.Instance.AbilityFittedText, data.stand.ToString("#,##0"), Math.Round(data.ability, 2))
                                    });
                                }
                            }
                            IsAvatarAbility7KLoading = false;
                            break;
                        case 2:
                            IsAvatarAbility9KLoading = true;
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/9K?avatarID={_avatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                Ability9KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability9KAvatarComputingCollection.Add(new(_avatarID, data)
                                    {
                                        FittedText = string.Format(LanguageSystem.Instance.AbilityFittedText, data.stand.ToString("#,##0"), Math.Round(data.ability, 2))
                                    });
                                }
                            }
                            IsAvatarAbility9KLoading = false;
                            break;
                    }
                    break;
                case 3:
                    IsAvatarWwwLevelLoading = true;
                    var twilightWwwWwwLevel = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarWwwLevel[]>($"{QwilightComponent.QwilightAPI}/avatar/wwwLevels?avatarID={_avatarID}");
                    if (twilightWwwWwwLevel != null)
                    {
                        WwwLevelIDCollection.Clear();
                        foreach (var data in twilightWwwWwwLevel)
                        {
                            WwwLevelIDCollection.Add(new()
                            {
                                Title = data.title,
                                LevelValue = data.level,
                                LevelText = data.levelText,
                                Date = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                            });
                        }
                    }
                    IsAvatarWwwLevelLoading = false;
                    break;
                case 4:
                    HandledLevelName ??= HandledLevelNameCollection.FirstOrDefault();
                    IsHandledLoading = true;
                    var handledLevelName = HandledLevelName;
                    var twilightWwwAvatarHandledMap = await TwilightSystem.Instance.GetWwwParallel<Dictionary<string, JSON.TwilightWwwAvatarHandled>>($"{QwilightComponent.QwilightAPI}/avatar/handled?avatarID={_avatarID}&levelName={handledLevelName}");
                    if (handledLevelName == HandledLevelName)
                    {
                        if (twilightWwwAvatarHandledMap != null)
                        {
                            _twilightWwwAvatarHandledMap = twilightWwwAvatarHandledMap;
                            HandledLevelIDItemCollection.Clear();
                            foreach (var data in twilightWwwAvatarHandledMap)
                            {
                                HandledLevelIDItemCollection.Add(new()
                                {
                                    LevelID = data.Key
                                });
                            }
                            HandledLevelIDItemValue = HandledLevelIDItemCollection.FirstOrDefault();
                        }
                        IsHandledLoading = false;
                    }
                    break;
                case 5 when IsLevelVSVisible:
                    LevelVSMyAvatarWwwValue = new(TwilightSystem.Instance.AvatarID);
                    LevelVSMyAvatarName = TwilightSystem.Instance.AvatarName;
                    LevelVSTargetAvatarWwwValue = new(_avatarID);
                    LevelVSTargetAvatarName = _avatarName;
                    LevelVSLevelName ??= LevelVSLevelNameCollection.FirstOrDefault();
                    IsLevelVSLoading = true;
                    var levelVSLevelName = LevelVSLevelName;
                    var twilightWwwAvatarLevelVSMap = await TwilightSystem.Instance.GetWwwParallel<Dictionary<string, JSON.TwilightWwwAvatarLevelVS>>($"{QwilightComponent.QwilightAPI}/avatar/levelVS?avatarID={TwilightSystem.Instance.AvatarID}&targetID={_avatarID}&levelName={levelVSLevelName}");
                    if (levelVSLevelName == LevelVSLevelName)
                    {
                        if (twilightWwwAvatarLevelVSMap != null)
                        {
                            _twilightWwwAvatarLevelVSMap = twilightWwwAvatarLevelVSMap;
                            LevelVSLevelIDItemCollection.Clear();
                            foreach (var data in twilightWwwAvatarLevelVSMap)
                            {
                                LevelVSLevelIDItemCollection.Add(new()
                                {
                                    LevelID = data.Key,
                                    AvatarLevelVSCount = data.Value.avatarLevelVSCount,
                                    TargetLevelVSCount = data.Value.targetLevelVSCount
                                });
                            }
                            LevelVSLevelIDItemValue = LevelVSLevelIDItemCollection.FirstOrDefault();
                        }
                        IsLevelVSLoading = false;
                    }
                    break;
            }
        }

        public int AvatarTabPosition
        {
            get => _avatarTabPosition;

            set
            {
                if (SetProperty(ref _avatarTabPosition, value, nameof(AvatarTabPosition)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public int FavoritesTabPosition
        {
            get => _favoritesTabPosition;

            set
            {
                if (SetProperty(ref _favoritesTabPosition, value, nameof(FavoritesTabPosition)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public int LastsTabPosition
        {
            get => _lastsTabPosition;

            set
            {
                if (SetProperty(ref _lastsTabPosition, value, nameof(LastsTabPosition)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public int AbilityTabPosition
        {
            get => _abilityTabPosition;

            set
            {
                if (SetProperty(ref _abilityTabPosition, value, nameof(AbilityTabPosition)))
                {
                    _ = CallAvatarAPI();
                }
            }
        }

        public string AvatarAbility5KPlaceText0
        {
            get => _avatarAbility5KPlaceText0;

            set => SetProperty(ref _avatarAbility5KPlaceText0, value, nameof(AvatarAbility5KPlaceText0));
        }

        public string AvatarAbility5KPlaceText1
        {
            get => _avatarAbility5KPlaceText1;

            set => SetProperty(ref _avatarAbility5KPlaceText1, value, nameof(AvatarAbility5KPlaceText1));
        }

        public ImageSource AbilityClass5KDrawing
        {
            get => _abilityClass5KDrawing;

            set => SetProperty(ref _abilityClass5KDrawing, value, nameof(AbilityClass5KDrawing));
        }

        public string AvatarViewAbility5KText => _avatarAbility5K.ToString("#,##0.## Point");

        public double AvatarAbility5KStatus => 100.0 * _avatarAbility5KStatus;

        public string AvatarAbility7KPlaceText0
        {
            get => _avatarAbility7KPlaceText0;

            set => SetProperty(ref _avatarAbility7KPlaceText0, value, nameof(AvatarAbility7KPlaceText0));
        }

        public string AvatarAbility7KPlaceText1
        {
            get => _avatarAbility7KPlaceText1;

            set => SetProperty(ref _avatarAbility7KPlaceText1, value, nameof(AvatarAbility7KPlaceText1));
        }

        public ImageSource AbilityClass7KDrawing
        {
            get => _abilityClass7KDrawing;

            set => SetProperty(ref _abilityClass7KDrawing, value, nameof(AbilityClass7KDrawing));
        }

        public string AvatarViewAbility7KText => _avatarAbility7K.ToString("#,##0.## Point");

        public double AvatarAbility7KStatus => 100.0 * _avatarAbility7KStatus;

        public string AvatarAbility9KPlaceText0
        {
            get => _avatarAbility9KPlaceText0;

            set => SetProperty(ref _avatarAbility9KPlaceText0, value, nameof(AvatarAbility9KPlaceText0));
        }

        public string AvatarAbility9KPlaceText1
        {
            get => _avatarAbility9KPlaceText1;

            set => SetProperty(ref _avatarAbility9KPlaceText1, value, nameof(AvatarAbility9KPlaceText1));
        }

        public ImageSource AbilityClass9KDrawing
        {
            get => _abilityClass9KDrawing;

            set => SetProperty(ref _abilityClass9KDrawing, value, nameof(AbilityClass9KDrawing));
        }

        public string AvatarViewAbility9KText => _avatarAbility9K.ToString("#,##0.## Point");

        public double AvatarAbility9KStatus => 100.0 * _avatarAbility9KStatus;

        public string CallingAvatarID { get; set; }

        public bool IsMe => _avatarID == TwilightSystem.Instance.AvatarID;

        public string[] QuitStatusTexts { get; } = new string[7];

        public int[] DateValues { get; } = new int[91];

        public bool IsAvatarLoading
        {
            get => _isAvatarLoading;

            set => SetProperty(ref _isAvatarLoading, value, nameof(IsAvatarLoading));
        }

        public string AvatarViewText => string.Format(LanguageSystem.Instance.AvatarViewText, _avatarName, _avatarID);

        public string AvatarViewTotalCountText => string.Format(LanguageSystem.Instance.AvatarViewTotalCountText, _totalCount.ToString("#,##0"));

        public string AvatarViewTotalLengthText => string.Format(LanguageSystem.Instance.AvatarViewTotalLengthText, (_totalLength / 1000.0 / 60 / 60).ToString("#,##0"), (_totalLength / 1000.0 / 60 % 60).ToString("#,##0"), (_totalLength / 1000 % 60).ToString("#,##0"));

        public string AvatarViewTopCountText => string.Format(LanguageSystem.Instance.AvatarViewTopCountText, _topCount.ToString("#,##0"));

        public string AvatarViewDateText => string.Format(LanguageSystem.Instance.AvatarViewDateText, _date);

        public string AvatarViewLevelText => $"LV. {_avatarLevels[0]} (XP: {_avatarLevels[1]}／{_avatarLevels[2]})";

        public double AvatarViewLevelValue => _avatarLevels[2] > 0 ? 100.0 * _avatarLevels[1] / _avatarLevels[2] : 0.0;

        public string AvatarViewWwwLevelText => string.Format(LanguageSystem.Instance.AvatarViewWwwLevelText, _wwwLevelIDCount);

        public string AvatarIntro { get; set; }

        [RelayCommand]
        static void OnAvatarTitle() => ViewModels.Instance.AvatarTitleValue.Open();

        [RelayCommand]
        static void OnAvatarEdge() => ViewModels.Instance.AvatarEdgeValue.Open();

        [RelayCommand]
        async Task OnAvatarDrawing()
        {
            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
            {
                Filters = [".png"]
            });
            if (!string.IsNullOrEmpty(filePath) && await TwilightSystem.Instance.PostAvatarDrawingParallel($"{QwilightComponent.TaehuiNetAPI}/avatar/drawing", filePath).ConfigureAwait(false))
            {
                AvatarWwwValue = new(TwilightSystem.Instance.AvatarID, AvatarWwwValue.AvatarTitleValue, AvatarWwwValue.AvatarEdge, true);
                NotifyAvatarWwwValue();
                TwilightSystem.Instance.NotifyAvatarWwwValue();
            }
        }

        public void NotifyIsMe() => OnPropertyChanged(nameof(IsMe));

        public void NotifyAvatarWwwValue() => OnPropertyChanged(nameof(AvatarWwwValue));

        public override void OnOpened()
        {
            base.OnOpened();
            UIHandler.Instance.HandleParallel(async () =>
            {
                IsAvatarLoading = true;

                var avatarID = CallingAvatarID;
                var twilightWwwAvatar = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatar?>($"{QwilightComponent.QwilightAPI}/avatar?want=!{avatarID}");
                if (avatarID == CallingAvatarID)
                {
                    if (twilightWwwAvatar.HasValue)
                    {
                        var twilightWwwAvatarValue = twilightWwwAvatar.Value;

                        _avatarID = twilightWwwAvatarValue.avatarID;
                        NotifyIsMe();
                        NotifyIsLevelVSVisible();

                        _avatarName = twilightWwwAvatarValue.avatarName;
                        OnPropertyChanged(nameof(AvatarViewText));

                        AvatarIntro = twilightWwwAvatarValue.avatarIntro;
                        OnPropertyChanged(nameof(AvatarIntro));

                        AvatarWwwValue = new(twilightWwwAvatarValue.avatarID, null, null, true);
                        NotifyAvatarWwwValue();

                        _totalCount = twilightWwwAvatarValue.totalCount;
                        OnPropertyChanged(nameof(AvatarViewTotalCountText));

                        _totalLength = twilightWwwAvatarValue.totalLength;
                        OnPropertyChanged(nameof(AvatarViewTotalLengthText));

                        _topCount = twilightWwwAvatarValue.topCount;
                        OnPropertyChanged(nameof(AvatarViewTopCountText));

                        _date = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(twilightWwwAvatarValue.date);
                        OnPropertyChanged(nameof(AvatarViewDateText));

                        Array.Copy(twilightWwwAvatarValue.avatarLevels, _avatarLevels, _avatarLevels.Length);
                        OnPropertyChanged(nameof(AvatarViewLevelText));
                        OnPropertyChanged(nameof(AvatarViewLevelValue));

                        try
                        {
                            AbilityClass5KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass5K={twilightWwwAvatarValue.avatarAbility5K}"), null);
                        }
                        catch
                        {
                        }
                        if (twilightWwwAvatarValue.avatarAbility5KPlace > 0)
                        {
                            AvatarAbility5KPlaceText0 = twilightWwwAvatarValue.avatarAbility5KPlace.ToString("＃#,##0");
                        }
                        else
                        {
                            AvatarAbility5KPlaceText0 = "＃？";
                        }
                        AvatarAbility5KPlaceText1 = twilightWwwAvatarValue.avatarAbility5KCount.ToString("／#,##0");
                        _avatarAbility5K = twilightWwwAvatarValue.avatarAbility5K;
                        OnPropertyChanged(nameof(AvatarViewAbility5KText));
                        _avatarAbility5KStatus = twilightWwwAvatarValue.avatarAbility5KStatus;
                        OnPropertyChanged(nameof(AvatarAbility5KStatus));

                        try
                        {
                            AbilityClass7KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass7K={twilightWwwAvatarValue.avatarAbility7K}"), null);
                        }
                        catch
                        {
                        }
                        if (twilightWwwAvatarValue.avatarAbility7KPlace > 0)
                        {
                            AvatarAbility7KPlaceText0 = twilightWwwAvatarValue.avatarAbility7KPlace.ToString("＃#,##0");
                        }
                        else
                        {
                            AvatarAbility7KPlaceText0 = "＃？";
                        }
                        AvatarAbility7KPlaceText1 = twilightWwwAvatarValue.avatarAbility7KCount.ToString("／#,##0");
                        _avatarAbility7K = twilightWwwAvatarValue.avatarAbility7K;
                        OnPropertyChanged(nameof(AvatarViewAbility7KText));
                        _avatarAbility7KStatus = twilightWwwAvatarValue.avatarAbility7KStatus;
                        OnPropertyChanged(nameof(AvatarAbility7KStatus));

                        try
                        {
                            AbilityClass9KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass9K={twilightWwwAvatarValue.avatarAbility9K}"), null);
                        }
                        catch
                        {
                        }
                        if (twilightWwwAvatarValue.avatarAbility9KPlace > 0)
                        {
                            AvatarAbility9KPlaceText0 = twilightWwwAvatarValue.avatarAbility9KPlace.ToString("＃#,##0");
                        }
                        else
                        {
                            AvatarAbility9KPlaceText0 = "＃？";
                        }
                        AvatarAbility9KPlaceText1 = twilightWwwAvatarValue.avatarAbility9KCount.ToString("／#,##0");
                        _avatarAbility9K = twilightWwwAvatarValue.avatarAbility9K;
                        OnPropertyChanged(nameof(AvatarViewAbility9KText));
                        _avatarAbility9KStatus = twilightWwwAvatarValue.avatarAbility9KStatus;
                        OnPropertyChanged(nameof(AvatarAbility9KStatus));

                        for (var i = twilightWwwAvatarValue.quitStatusValues.Length - 1; i >= 0; --i)
                        {
                            QuitStatusTexts[i] = twilightWwwAvatarValue.quitStatusValues[i].ToString(LanguageSystem.Instance.CountContents);
                        }
                        OnPropertyChanged(nameof(QuitStatusTexts));

                        Array.Copy(twilightWwwAvatarValue.dateValues, DateValues, DateValues.Length);

                        _wwwLevelIDCount = twilightWwwAvatarValue.wwwLevelIDCount;
                        OnPropertyChanged(nameof(AvatarViewWwwLevelText));

                        Utility.SetUICollection(HandledLevelNameCollection, twilightWwwAvatarValue.levelNames.Except(["*"]).ToArray());
                        Utility.SetUICollection(LevelVSLevelNameCollection, twilightWwwAvatarValue.levelNames);

                        _ = CallAvatarAPI();
                    }
                    else
                    {
                        Close();
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvatarViewFault);
                    }

                    IsAvatarLoading = false;
                }
            });
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            if (IsMe)
            {
                _ = TwilightSystem.Instance.PutAvatarParallel($"{QwilightComponent.TaehuiNetAPI}/avatar/avatarIntro", AvatarIntro);
            }
        }
    }
}