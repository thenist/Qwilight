using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public sealed partial class AvatarViewModel : BaseViewModel
    {
        public sealed class AvatarComputing : Computing
        {
            public override BaseNoteFile.NoteVariety NoteVarietyValue => AvatarNoteVarietyValue;

            public BaseNoteFile.NoteVariety AvatarNoteVarietyValue { get; init; }

            public override void OnCompiled()
            {
            }

            public override void OnFault(Exception e)
            {
            }

            public string AvatarValue { get; set; }
        }

        public ObservableCollection<AvatarComputing> FavoriteAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> LastAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability5KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability7KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Ability9KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarLevelItem> WwwLevelCollection { get; } = new();

        public override double TargetHeight => 0.8;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        readonly int[] _avatarLevels = new int[3];
        string _avatarAbility5KPlaceText0 = string.Empty;
        string _avatarAbility5KPlaceText1 = string.Empty;
        ImageSource _abilityClass5KDrawing;
        double _avatarAbility5K;
        string _avatarAbility7KPlaceText0 = string.Empty;
        string _avatarAbility7KPlaceText1 = string.Empty;
        ImageSource _abilityClass7KDrawing;
        double _avatarAbility7K;
        string _avatarAbility9KPlaceText0 = string.Empty;
        string _avatarAbility9KPlaceText1 = string.Empty;
        ImageSource _abilityClass9KDrawing;
        double _avatarAbility9K;
        bool _isAvatarLoading;
        string _avatarID;
        string _avatarName;
        int _totalCount;
        double _totalLength;
        int _highestCount;
        DateTime _date;
        int _avatarTabPosition;
        int _abilityTabPosition;
        bool _isAvatarFavoritesLoading;
        bool _isAvatarLastsLoading;
        bool _isAvatarAbility5KLoading;
        bool _isAvatarAbility7KLoading;
        bool _isAvatarAbility9KLoading;
        bool _isAvatarWwwLevelLoading;

        public bool IsAvatarFavoritesLoading
        {
            get => _isAvatarFavoritesLoading;

            set => SetProperty(ref _isAvatarFavoritesLoading, value, nameof(IsAvatarFavoritesLoading));
        }

        public bool IsAvatarLastsLoading
        {
            get => _isAvatarLastsLoading;

            set => SetProperty(ref _isAvatarLastsLoading, value, nameof(IsAvatarLastsLoading));
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

        async Task LoadAvatarCollection()
        {
            switch (AvatarTabPosition)
            {
                case 0:
                    IsAvatarFavoritesLoading = true;
                    var twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites?avatarID={AvatarID}");
                    if (twilightWwwAvatarFavorites != null)
                    {
                        FavoriteAvatarComputingCollection.Clear();
                        foreach (var data in twilightWwwAvatarFavorites)
                        {
                            FavoriteAvatarComputingCollection.Add(new()
                            {
                                AvatarNoteVarietyValue = data.noteVariety,
                                Title = data.title,
                                Artist = data.artist,
                                Genre = data.genre,
                                LevelValue = data.level,
                                LevelText = data.levelText,
                                AvatarValue = data.totalCount.ToString(LanguageSystem.Instance.HandledContents)
                            });
                        }
                    }
                    IsAvatarFavoritesLoading = false;
                    break;
                case 1:
                    IsAvatarLastsLoading = true;
                    var twilightWwwAvatarLast = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts?avatarID={AvatarID}");
                    if (twilightWwwAvatarLast != null)
                    {
                        LastAvatarComputingCollection.Clear();
                        foreach (var data in twilightWwwAvatarLast)
                        {
                            LastAvatarComputingCollection.Add(new()
                            {
                                AvatarNoteVarietyValue = data.noteVariety,
                                Title = data.title,
                                Artist = data.artist,
                                Genre = data.genre,
                                LevelValue = data.level,
                                LevelText = data.levelText,
                                AvatarValue = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                            });
                        }
                    }
                    IsAvatarLastsLoading = false;
                    break;
                case 2:
                    switch (AbilityTabPosition)
                    {
                        case 0:
                            IsAvatarAbility5KLoading = true;
                            var twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability5K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability5KAvatarComputingCollection.Add(new AvatarComputing
                                    {
                                        AvatarNoteVarietyValue = data.noteVariety,
                                        Title = data.title,
                                        Artist = data.artist,
                                        Genre = data.genre,
                                        LevelValue = data.level,
                                        LevelText = data.levelText,
                                        AvatarValue = string.Format(LanguageSystem.Instance.AbilityStandContents, data.stand.ToString("#,##0"), Math.Round(data.ability, 3))
                                    });
                                }
                            }
                            IsAvatarAbility5KLoading = false;
                            break;
                        case 1:
                            IsAvatarAbility7KLoading = true;
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability7K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability7KAvatarComputingCollection.Add(new AvatarComputing
                                    {
                                        AvatarNoteVarietyValue = data.noteVariety,
                                        Title = data.title,
                                        Artist = data.artist,
                                        Genre = data.genre,
                                        LevelValue = data.level,
                                        LevelText = data.levelText,
                                        AvatarValue = string.Format(LanguageSystem.Instance.AbilityStandContents, data.stand.ToString("#,##0"), Math.Round(data.ability, 3))
                                    });
                                }
                            }
                            IsAvatarAbility7KLoading = false;
                            break;
                        case 2:
                            IsAvatarAbility9KLoading = true;
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability9K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability9KAvatarComputingCollection.Add(new AvatarComputing
                                    {
                                        AvatarNoteVarietyValue = data.noteVariety,
                                        Title = data.title,
                                        Artist = data.artist,
                                        Genre = data.genre,
                                        LevelValue = data.level,
                                        LevelText = data.levelText,
                                        AvatarValue = string.Format(LanguageSystem.Instance.AbilityStandContents, data.stand.ToString("#,##0"), Math.Round(data.ability, 3))
                                    });
                                }
                            }
                            IsAvatarAbility9KLoading = false;
                            break;
                    }
                    break;
                case 3:
                    IsAvatarWwwLevelLoading = true;
                    var twilightWwwWwwLevel = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarWwwLevel[]>($"{QwilightComponent.QwilightAPI}/avatar/wwwLevels?avatarID={AvatarID}");
                    if (twilightWwwWwwLevel != null)
                    {
                        WwwLevelCollection.Clear();
                        foreach (var data in twilightWwwWwwLevel)
                        {
                            WwwLevelCollection.Add(new()
                            {
                                Title = data.title,
                                LevelValue = data.level,
                                LevelText = data.levelText,
                                Date = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(data.date).ToString()
                            });
                        }
                    }
                    IsAvatarWwwLevelLoading = false;
                    OnPropertyChanged(nameof(AvatarViewWwwLevelText));
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
                    _ = LoadAvatarCollection();
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
                    _ = LoadAvatarCollection();
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

        public string AvatarID { get; set; }

        public bool IsMe => _avatarID == TwilightSystem.Instance.AvatarID;

        public string[] QuitStatusTexts { get; } = new string[7];

        public int[] DateValues { get; } = new int[91];

        public bool IsAvatarLoading
        {
            get => _isAvatarLoading;

            set => SetProperty(ref _isAvatarLoading, value, nameof(IsAvatarLoading));
        }

        public AvatarWww AvatarWwwValue { get; set; }

        public string AvatarViewText => string.Format(LanguageSystem.Instance.AvatarViewText, _avatarName, _avatarID);

        public string AvatarViewTotalCountText => string.Format(LanguageSystem.Instance.AvatarViewTotalCountText, _totalCount.ToString("#,##0"));

        public string AvatarViewTotalLengthText => string.Format(LanguageSystem.Instance.AvatarViewTotalLengthText, ((int)_totalLength / 1000 / 60 / 60).ToString("#,##0"), ((int)_totalLength / 1000 / 60 % 60).ToString(), ((int)_totalLength / 1000 % 60).ToString());

        public string AvatarViewHighestCountText => string.Format(LanguageSystem.Instance.AvatarViewHighestCountText, _highestCount.ToString("#,##0"));

        public string AvatarViewDateText => string.Format(LanguageSystem.Instance.AvatarViewDateText, _date);

        public string AvatarViewLevelText0 => $"LV. {_avatarLevels[0]}";

        public string AvatarViewLevelText1 => $"XP: {_avatarLevels[1]}／{_avatarLevels[2]}";

        public double AvatarViewLevelValue => _avatarLevels[2] > 0 ? 100.0 * _avatarLevels[1] / _avatarLevels[2] : 0.0;

        public string AvatarViewWwwLevelText => string.Format(LanguageSystem.Instance.AvatarViewWwwLevelText, WwwLevelCollection.Count);

        public string AvatarIntro { get; set; }

        public override void OnOpened()
        {
            base.OnOpened();
            UIHandler.Instance.HandleParallel(async () =>
            {
                InitAvatarWwwValue(string.Empty);
                IsAvatarLoading = true;
                var twilightWwwAvatar = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatar?>($"{QwilightComponent.QwilightAPI}/avatar?avatarID={AvatarID}");
                if (twilightWwwAvatar.HasValue)
                {
                    var twilightWwwAvatarValue = twilightWwwAvatar.Value;

                    _avatarID = twilightWwwAvatarValue.avatarID;
                    NotifyIsMe();

                    _avatarName = twilightWwwAvatarValue.avatarName;
                    OnPropertyChanged(nameof(AvatarViewText));

                    AvatarIntro = twilightWwwAvatarValue.avatarIntro;
                    OnPropertyChanged(nameof(AvatarIntro));

                    InitAvatarWwwValue(twilightWwwAvatarValue.avatarID);

                    _totalCount = twilightWwwAvatarValue.totalCount;
                    OnPropertyChanged(nameof(AvatarViewTotalCountText));

                    _totalLength = twilightWwwAvatarValue.totalLength;
                    OnPropertyChanged(nameof(AvatarViewTotalLengthText));

                    _highestCount = twilightWwwAvatarValue.highestCount;
                    OnPropertyChanged(nameof(AvatarViewHighestCountText));

                    _date = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(twilightWwwAvatarValue.date);
                    OnPropertyChanged(nameof(AvatarViewDateText));

                    Array.Copy(twilightWwwAvatarValue.avatarLevels, _avatarLevels, _avatarLevels.Length);
                    OnPropertyChanged(nameof(AvatarViewLevelText0));
                    OnPropertyChanged(nameof(AvatarViewLevelText1));
                    OnPropertyChanged(nameof(AvatarViewLevelValue));

                    var avatarAbility5KClass = twilightWwwAvatarValue.avatarAbility5KClass;
                    AbilityClass5KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass5K={(avatarAbility5KClass < 0 ? avatarAbility5KClass : 100 * avatarAbility5KClass)}"), null);
                    if (twilightWwwAvatarValue.avatarAbility5KPlace > 0)
                    {
                        AvatarAbility5KPlaceText0 = twilightWwwAvatarValue.avatarAbility5KPlace.ToString("＃#,##0");
                        AvatarAbility5KPlaceText1 = twilightWwwAvatarValue.avatarAbility5KCount.ToString("／#,##0");
                    }
                    else
                    {
                        AvatarAbility5KPlaceText0 = string.Empty;
                        AvatarAbility5KPlaceText1 = string.Empty;
                    }
                    _avatarAbility5K = twilightWwwAvatarValue.avatarAbility5K;
                    OnPropertyChanged(nameof(AvatarViewAbility5KText));

                    var avatarAbility7KClass = twilightWwwAvatarValue.avatarAbility7KClass;
                    AbilityClass7KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass7K={(avatarAbility7KClass < 0 ? avatarAbility7KClass : 100 * avatarAbility7KClass)}"), null);
                    if (twilightWwwAvatarValue.avatarAbility7KPlace > 0)
                    {
                        AvatarAbility7KPlaceText0 = twilightWwwAvatarValue.avatarAbility7KPlace.ToString("＃#,##0");
                        AvatarAbility7KPlaceText1 = twilightWwwAvatarValue.avatarAbility7KCount.ToString("／#,##0");
                    }
                    else
                    {
                        AvatarAbility7KPlaceText0 = string.Empty;
                        AvatarAbility7KPlaceText1 = string.Empty;
                    }
                    _avatarAbility7K = twilightWwwAvatarValue.avatarAbility7K;
                    OnPropertyChanged(nameof(AvatarViewAbility7KText));

                    var avatarAbility9KClass = twilightWwwAvatarValue.avatarAbility9KClass;
                    AbilityClass9KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass9K={(avatarAbility9KClass < 0 ? avatarAbility9KClass : 100 * avatarAbility9KClass)}"), null);
                    if (twilightWwwAvatarValue.avatarAbility9KPlace > 0)
                    {
                        AvatarAbility9KPlaceText0 = twilightWwwAvatarValue.avatarAbility9KPlace.ToString("＃#,##0");
                        AvatarAbility9KPlaceText1 = twilightWwwAvatarValue.avatarAbility9KCount.ToString("／#,##0");
                    }
                    else
                    {
                        AvatarAbility9KPlaceText0 = string.Empty;
                        AvatarAbility9KPlaceText1 = string.Empty;
                    }
                    _avatarAbility9K = twilightWwwAvatarValue.avatarAbility9K;
                    OnPropertyChanged(nameof(AvatarViewAbility9KText));

                    for (var i = twilightWwwAvatarValue.quitStatusValues.Length - 1; i >= 0; --i)
                    {
                        QuitStatusTexts[i] = twilightWwwAvatarValue.quitStatusValues[i].ToString(LanguageSystem.Instance.CountContents);
                    }
                    OnPropertyChanged(nameof(QuitStatusTexts));

                    Array.Copy(twilightWwwAvatarValue.dateValues, DateValues, DateValues.Length);

                    _ = LoadAvatarCollection();
                }
                else
                {
                    Close();
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvatarViewFault);
                }
                IsAvatarLoading = false;
            });
        }

        [RelayCommand]
        static void OnAvatarTitle() => ViewModels.Instance.AvatarTitleValue.Open();

        [RelayCommand]
        static void OnAvatarEdge() => ViewModels.Instance.AvatarEdgeValue.Open();

        public async Task OnAvatarDrawing()
        {
            if (IsMe)
            {
                var fileName = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
                {
                    Filters = new[] { ".png" }
                });
                if (!string.IsNullOrEmpty(fileName) && await TwilightSystem.Instance.PostAvatarDrawingParallel($"{QwilightComponent.TaehuiNetAPI}/avatar/drawing", fileName).ConfigureAwait(false))
                {
                    InitAvatarWwwValue(TwilightSystem.Instance.AvatarID);
                    TwilightSystem.Instance.NotifyAvatarDrawing();
                }
            }
        }

        public void NotifyIsMe() => OnPropertyChanged(nameof(IsMe));

        public void InitAvatarWwwValue(string avatarID, AvatarTitle? avatarTitle = null, ImageSource avatarEdge = null)
        {
            AvatarWwwValue = new(avatarID, avatarTitle, avatarEdge, true);
            OnPropertyChanged(nameof(AvatarWwwValue));
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            if (IsMe)
            {
                _ = TwilightSystem.Instance.PutAvatarParallel($"{QwilightComponent.TaehuiNetAPI}/avatar/avatarIntro", AvatarIntro);
            }
        }
    }
}