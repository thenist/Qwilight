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

        public ObservableCollection<AvatarComputing> Favorites5KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites7KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites9KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites10KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites14KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites24KAvatarComputingCollection { get; } = new();

        public ObservableCollection<AvatarComputing> Favorites48KAvatarComputingCollection { get; } = new();

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
        int _favoritesTabPosition;
        int _lastsTabPosition;
        bool _isAvatarFavorites5KLoading;
        bool _isAvatarFavorites7KLoading;
        bool _isAvatarFavorites9KLoading;
        bool _isAvatarFavorites10KLoading;
        bool _isAvatarFavorites14KLoading;
        bool _isAvatarFavorites24KLoading;
        bool _isAvatarFavorites48KLoading;
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

        async Task LoadAvatarCollection()
        {
            switch (AvatarTabPosition)
            {
                case 0:
                    switch (FavoritesTabPosition)
                    {
                        case 0:
                            IsAvatarFavorites5KLoading = true;
                            var twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/5K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites5KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites5KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites5KLoading = false;
                            break;
                        case 1:
                            IsAvatarFavorites7KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/7K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites7KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites7KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites7KLoading = false;
                            break;
                        case 2:
                            IsAvatarFavorites9KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/9K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites9KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites9KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites9KLoading = false;
                            break;
                        case 3:
                            IsAvatarFavorites10KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/10K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites10KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites10KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites10KLoading = false;
                            break;
                        case 4:
                            IsAvatarFavorites14KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/14K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites14KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites14KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites14KLoading = false;
                            break;
                        case 5:
                            IsAvatarFavorites24KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/24K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites24KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites24KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites24KLoading = false;
                            break;
                        case 6:
                            IsAvatarFavorites48KLoading = true;
                            twilightWwwAvatarFavorites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarFavorite[]>($"{QwilightComponent.QwilightAPI}/avatar/favorites/48K?avatarID={AvatarID}");
                            if (twilightWwwAvatarFavorites != null)
                            {
                                Favorites48KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarFavorites)
                                {
                                    Favorites48KAvatarComputingCollection.Add(new()
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
                            IsAvatarFavorites48KLoading = false;
                            break;
                    }
                    break;
                case 1:
                    switch (LastsTabPosition)
                    {
                        case 0:
                            IsAvatarLasts5KLoading = true;
                            var twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/5K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts5KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts5KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts5KLoading = false;
                            break;
                        case 1:
                            IsAvatarLasts7KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/7K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts7KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts7KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts7KLoading = false;
                            break;
                        case 2:
                            IsAvatarLasts9KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/9K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts9KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts9KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts9KLoading = false;
                            break;
                        case 3:
                            IsAvatarLasts10KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/10K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts10KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts10KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts10KLoading = false;
                            break;
                        case 4:
                            IsAvatarLasts14KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/14K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts14KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts14KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts14KLoading = false;
                            break;
                        case 5:
                            IsAvatarLasts24KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/24K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts24KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts24KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts24KLoading = false;
                            break;
                        case 6:
                            IsAvatarLasts48KLoading = true;
                            twilightWwwAvatarLasts = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarLast[]>($"{QwilightComponent.QwilightAPI}/avatar/lasts/48K?avatarID={AvatarID}");
                            if (twilightWwwAvatarLasts != null)
                            {
                                Lasts48KAvatarComputingCollection.Clear();
                                foreach (var data in twilightWwwAvatarLasts)
                                {
                                    Lasts48KAvatarComputingCollection.Add(new()
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
                            IsAvatarLasts48KLoading = false;
                            break;
                    }
                    break;
                case 2:
                    switch (AbilityTabPosition)
                    {
                        case 0:
                            IsAvatarAbility5KLoading = true;
                            var twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/5K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability5KAvatarComputingCollection.Add(new()
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
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/7K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability7KAvatarComputingCollection.Add(new()
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
                            twilightWwwAvatarAbility = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwAvatarAbility[]>($"{QwilightComponent.QwilightAPI}/avatar/ability/9K?avatarID={AvatarID}");
                            if (twilightWwwAvatarAbility != null)
                            {
                                WwwLevelCollection.Clear();
                                foreach (var data in twilightWwwAvatarAbility)
                                {
                                    Ability9KAvatarComputingCollection.Add(new()
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

        public int FavoritesTabPosition
        {
            get => _favoritesTabPosition;

            set
            {
                if (SetProperty(ref _favoritesTabPosition, value, nameof(FavoritesTabPosition)))
                {
                    _ = LoadAvatarCollection();
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
                    try
                    {
                        AbilityClass5KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass5K={(avatarAbility5KClass < 0 ? avatarAbility5KClass : 100 * avatarAbility5KClass)}"), null);
                    }
                    catch
                    {
                    }
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
                    try
                    {
                        AbilityClass7KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass7K={(avatarAbility7KClass < 0 ? avatarAbility7KClass : 100 * avatarAbility7KClass)}"), null);
                    }
                    catch
                    {
                    }
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
                    try
                    {
                        AbilityClass9KDrawing = DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?abilityClass9K={(avatarAbility9KClass < 0 ? avatarAbility9KClass : 100 * avatarAbility9KClass)}"), null);
                    }
                    catch
                    {
                    }
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