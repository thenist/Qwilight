using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Google.Protobuf;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.View;
using Qwilight.View.SiteYell;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;

namespace Qwilight.ViewModel
{
    public sealed partial class SiteViewModel : Model
    {
        public sealed class NetSiteComputing : Computing
        {
            public override BaseNoteFile.NoteVariety NoteVarietyValue => default;

            public string NoteID { get; set; }

            public string WantLevelID { get; set; } = string.Empty;

            public BaseNoteFile.Handled HandledValue { get; set; }

            public ImageSource HandledWallDrawing => BaseUI.Instance.HandledWallDrawings[(int)HandledValue];

            public bool NotHaveIt { get; set; }

            public bool IsDefault { get; set; }

            public override void OnCompiled()
            {
            }

            public override void OnFault(Exception e)
            {
            }

            public override bool Equals(object obj) => obj is NetSiteComputing netSiteComputing && NoteID == netSiteComputing.NoteID;

            public override int GetHashCode() => NoteID.GetHashCode();
        }

        public enum SiteSituation
        {
            Default, Compiling, Net
        };

        readonly ConcurrentDictionary<int, ISiteYell> _siteYellsMap = new();
        readonly LinkedList<Func<ISiteYell>> _pendingNewSiteYellQueue = new();
        readonly List<Action> _pendingModifySiteYellQueue = new();
        readonly List<Action> _pendingWipeSiteYellQueue = new();
        NetSiteComputing _valueComputing;
        NetSiteComputing _defaultNetSiteComputing;
        string[] _netSiteNoteID;
        string _bundleEntryPath;
        double _lastPosition1BeforeCall;
        bool _isSiteYellsViewLowest = true;
        string _siteName;
        string _siteNotify;
        string _input = string.Empty;
        string _siteHand;
        bool _isFavorNoteFile = true;
        bool _isAutoNetLevying;
        bool _isNew;
        bool _isNetSite;
        bool _isFavorModeComponent = true;
        bool _isFavorAudioMultiplier;
        AvatarGroup _avatarGroupValue;
        bool _isAutoSiteHand;
        bool _allowTotalLevying = true;
        bool _isGetNotify;
        bool _isAudioInput;
        SiteSituation _siteSituationValue;
        int _validHunterMode;
        ValidNetMode _validNetMode;
        string _bundleName;
        AvatarItem _avatarItemValue;
        bool _hasPendingNew;

        public PostableUIItem[] PostableUIItemCollection { get; } = Enumerable.Range(0, PostableItem.Values.Length).Select(i => new PostableUIItem
        {
            PostableItemValue = PostableItem.Values[i]
        }).ToArray();

        public bool IsTotalWantPostableUIItem => PostableUIItemCollection.All(postableUIItem => postableUIItem.IsWanted);

        public bool IsOpened { get; set; }

        [RelayCommand]
        static void OnTwilightConfigure()
        {
            if (TwilightSystem.Instance.IsSignedIn)
            {
                TwilightSystem.Instance.SendParallel<object>(Event.Types.EventID.CallConfigure, null);
            }
            else
            {
                ViewModels.Instance.TwilightConfigure.Open();
            }
        }

        [RelayCommand]
        static void OnSite() => ViewModels.Instance.SiteWindowValue.Toggle();

        [RelayCommand]
        static void OnUbuntu() => ViewModels.Instance.UbuntuValue.Toggle();

        [RelayCommand]
        void OnQuit()
        {
            TwilightSystem.Instance.StopBundle(SiteID);
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.QuitSite, SiteID);
        }

        [RelayCommand]
        void OnTotalWantPostableItem(bool? e)
        {
            if (e.HasValue)
            {
                foreach (var postableUIItem in PostableUIItemCollection)
                {
                    postableUIItem.IsWanted = e.Value;
                }
                OnInputPostableItem();
            }
        }

        [RelayCommand]
        void OnViewBundle() => AvatarItemValue?.AvatarWwwValue?.ViewBundleCommand?.Execute(null);

        [RelayCommand]
        void OnNewUbuntu() => AvatarItemValue?.AvatarWwwValue?.NewUbuntuCommand?.Execute(null);

        [RelayCommand]
        void OnViewAvatar() => AvatarItemValue?.AvatarWwwValue?.ViewAvatarCommand?.Execute(null);

        [RelayCommand]
        void OnCallIO()
        {
            var siteAvatarID = AvatarItemValue?.AvatarID;
            if (!string.IsNullOrEmpty(siteAvatarID))
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallIo, new
                {
                    avatarID = siteAvatarID,
                    ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
        }

        [RelayCommand]
        void OnNewSilentSite()
        {
            var siteAvatarID = AvatarItemValue?.AvatarID;
            if (!string.IsNullOrEmpty(siteAvatarID))
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSilentSite, siteAvatarID);
            }
        }

        [RelayCommand]
        void OnExileAvatar()
        {
            var siteAvatarID = AvatarItemValue?.AvatarID;
            if (!string.IsNullOrEmpty(siteAvatarID))
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.ExileAvatar, new
                {
                    siteID = SiteID,
                    avatarID = siteAvatarID
                });
            }
        }

        [RelayCommand]
        void OnSetSiteHand()
        {
            var siteAvatarID = AvatarItemValue?.AvatarID;
            if (!string.IsNullOrEmpty(siteAvatarID))
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSiteHand, new
                {
                    siteID = SiteID,
                    avatarID = siteAvatarID
                });
            }
        }

        [RelayCommand]
        void OnLevying() => HandleLevyingInput(false);

        [RelayCommand]
        void OnSaveAsNetBundle() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
        {
            bundleVariety = BundleItem.BundleVariety.Net,
            bundleEntryPath = _bundleEntryPath,
            etc = SiteID
        });

        [RelayCommand]
        void OnSaveNetBundle() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveBundle, new
        {
            etc = SiteID,
            bundleName = Path.GetFileNameWithoutExtension(BundleName)
        });

        [RelayCommand]
        void OnStopSiteNet() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.QuitNet, SiteID);

        [RelayCommand]
        void OnSetValidHunterMode() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetValidHunterMode, SiteID);

        [RelayCommand]
        void OnNetSIteComments() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallNetSiteComments, SiteID);

        [RelayCommand]
        void OnSetSiteName() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewInputWindow,
            Contents = new object[]
            {
                LanguageSystem.Instance.SetSiteNameContents,
                SiteName,
                new Action<string>(siteName => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSiteName, new
                {
                    siteID = SiteID,
                    siteName = siteName
                }))
            }
        });

        [RelayCommand]
        static void OnPostFile() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewFileWindow,
            Contents = new object[]
            {
                Array.Empty<string>(),
                new Action<string>(async fileName =>TwilightSystem.Instance.SendParallel(Event.Types.EventID.PostFile, Path.GetFileName(fileName), UnsafeByteOperations.UnsafeWrap(await File.ReadAllBytesAsync(fileName))))
            }
        });

        public void OnSetFavorNoteFile() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetFavorNoteFile, SiteID);

        public void OnSetFavorModeComponent() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetFavorModeComponent, SiteID);

        public void OnSetFavorAudioMultiplier() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetFavorAudioMultiplier, SiteID);

        public void OnSetAutoSiteHand() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetAutoSiteHand, SiteID);

        public void OnInputPostableItem()
        {
            OnPropertyChanged(nameof(IsTotalWantPostableUIItem));
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetAllowedPostableItems, new
            {
                siteID = SiteID,
                allowedPostableItems = PostableUIItemCollection.Where(postableUIItem => postableUIItem.IsWanted).Select(postableUIitem => (int)postableUIitem.PostableItemValue.VarietyValue).ToArray()
            });
        }
        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(Input))
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.SiteYell, new
                    {
                        siteID = SiteID,
                        siteYell = Input
                    });
                }
                Input = string.Empty;
            }
        }

        public async ValueTask OnEssentialInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.V && Utility.HasInput(VirtualKey.LeftControl) && string.IsNullOrEmpty(Input))
            {
                try
                {
                    using var ras = await (await Clipboard.GetContent().GetBitmapAsync()).OpenReadAsync();
                    var data = ArrayPool<byte>.Shared.Rent((int)ras.Size);
                    try
                    {
                        await ras.ReadAsync(data.AsBuffer(), (uint)ras.Size, InputStreamOptions.None);
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.PostFile, ras.ContentType.Split("/")[1], UnsafeByteOperations.UnsafeWrap(data));
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(data);
                    }
                    e.Handled = true;
                }
                catch
                {
                }
            }
        }

        public void OnPointedModified(bool e) => IsInputPointed = e;

        public void OnSiteYellsViewerMove(ScrollChangedEventArgs e)
        {
            if (_lastPosition1BeforeCall > 0.0)
            {
                View.SiteYellsViewer.ScrollToVerticalOffset(View.SiteYellsViewer.ExtentHeight - View.SiteYellsViewer.ActualHeight - _lastPosition1BeforeCall);
                _lastPosition1BeforeCall = 0.0;
            }
            else
            {
                var siteYellID = (SiteYellCollection.FirstOrDefault() as ISiteYell)?.SiteYellID;
                if (siteYellID.HasValue && siteYellID.Value > 0 && View.SiteYellsViewer.VerticalOffset == 0.0)
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.GetSiteYells, new
                    {
                        siteID = SiteID,
                        siteYellID = siteYellID.Value
                    });
                }
            }
            _isSiteYellsViewLowest = View.SiteYellsViewer.VerticalOffset + View.SiteYellsViewer.ActualHeight >= View.SiteYellsViewer.ExtentHeight;
        }

        public ModeComponent ModeComponentValue { get; } = new();

        public ObservableCollection<ISiteYell> SiteYellCollection { get; } = new();

        public ObservableCollection<AvatarItem> AvatarItemCollection { get; } = new();

        public ObservableCollection<NetSiteComputing> ComputingValues { get; } = new();

        public AvatarGroup[] AvatarGroupCollection { get; } = Enumerable.Range(0, 5).Select(data => new AvatarGroup
        {
            Data = data
        }).ToArray();

        public ValidNetMode[] ValidNetModeCollection { get; } = Enumerable.Range(0, 5).Select(data => new ValidNetMode
        {
            Data = data
        }).ToArray();

        public NetSiteComputing ComputingValue
        {
            get => _valueComputing;

            set
            {
                if (SetProperty(ref _valueComputing, value, nameof(ComputingValue)))
                {
                    OnPropertyChanged(nameof(LengthText));
                    NotifyComputedValues();
                }
            }
        }

        public bool AllowSiteHandLevying => !AllowTotalLevying || AvatarItemCollection.All(avatarItem => avatarItem.IsMe || avatarItem.AvatarID.StartsWith('@') || avatarItem.AvatarConfigureValue == AvatarItem.AvatarConfigure.Levying);

        public bool IsSiteHand => SiteHand == TwilightSystem.Instance.AvatarID;

        public bool IsInputPointed { get; set; }

        public bool IsGetNotify
        {
            get => _isGetNotify;

            set => SetProperty(ref _isGetNotify, value, nameof(IsGetNotify));
        }

        public bool IsAudioInput
        {
            get => _isAudioInput;

            set
            {
                if (SetProperty(ref _isAudioInput, value, nameof(IsAudioInput)))
                {
                    NotifyIsSendingAudioInput();
                }
            }
        }

        public bool IsFavorNoteFile
        {
            get => _isFavorNoteFile;

            set => SetProperty(ref _isFavorNoteFile, value, nameof(IsFavorNoteFile));
        }

        public bool IsAutoNetLevying
        {
            get => _isAutoNetLevying;

            set => SetProperty(ref _isAutoNetLevying, value, nameof(IsAutoNetLevying));
        }

        public bool IsFavorModeComponent
        {
            get => _isFavorModeComponent;

            set => SetProperty(ref _isFavorModeComponent, value, nameof(IsFavorModeComponent));
        }

        public bool IsFavorAudioMultiplier
        {
            get => _isFavorAudioMultiplier;

            set
            {
                if (SetProperty(ref _isFavorAudioMultiplier, value, nameof(IsFavorAudioMultiplier)))
                {
                    OnPropertyChanged(nameof(BPMText));
                }
            }
        }

        public bool IsAutoSiteHand
        {
            get => _isAutoSiteHand;

            set => SetProperty(ref _isAutoSiteHand, value, nameof(IsAutoSiteHand));
        }

        public bool AllowTotalLevying
        {
            get => _allowTotalLevying;

            set
            {
                if (SetProperty(ref _allowTotalLevying, value, nameof(AllowTotalLevying)))
                {
                    OnPropertyChanged(nameof(AllowSiteHandLevying));
                }
            }
        }

        public int ValidHunterMode
        {
            get => _validHunterMode;

            set
            {
                if (SetProperty(ref _validHunterMode, value, nameof(ValidHunterMode)))
                {
                    OnPropertyChanged(nameof(ValidHunterModeText));
                }
            }
        }

        public string ValidHunterModeText => ValidHunterMode switch
        {
            0 => LanguageSystem.Instance.StandValidHunterModeText,
            1 => LanguageSystem.Instance.PointValidHunterModeText,
            2 => LanguageSystem.Instance.BandValidHunterModeText,
            _ => default,
        };

        public string BundleName
        {
            get => _bundleName;

            set
            {
                if (SetProperty(ref _bundleName, value, nameof(BundleName)))
                {
                    OnPropertyChanged(nameof(CanSaveNetBundle));
                }
            }
        }

        public AvatarGroup AvatarGroupValue
        {
            get => _avatarGroupValue;

            set
            {
                if (SetProperty(ref _avatarGroupValue, value, nameof(AvatarGroupValue)))
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetAvatarGroup, new
                    {
                        siteID = SiteID,
                        avatarGroup = value.Data
                    });
                }
            }
        }

        public ValidNetMode ValidNetModeValue
        {
            get => _validNetMode;

            set
            {
                if (SetProperty(ref _validNetMode, value, nameof(ValidNetModeValue)))
                {
                    OnPropertyChanged(nameof(CanSetPostableItems));
                    if (IsSiteHand)
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetValidNetMode, new
                        {
                            siteID = SiteID,
                            validNetMode = value.Data
                        });
                    }
                }
            }
        }

        public bool WasNotify { get; set; }

        public bool IsEditable { get; set; }

        public bool CanAudioInput { get; set; }

        public Site View { get; set; }

        public string HighestInputCountText => Utility.GetHighestInputCountText(ComputingValue?.AverageInputCount ?? 0.0, ComputingValue?.HighestInputCount ?? 0, IsFavorAudioMultiplier ? 1.0 : ModeComponentValue.AudioMultiplier);

        public string BPMText => IsFavorAudioMultiplier ? $"{ComputingValue?.BPM ?? Component.StandardBPM} BPM" : Utility.GetBPMText(ComputingValue?.BPM ?? Component.StandardBPM, ModeComponentValue.AudioMultiplier);

        public string LengthText => Utility.GetLengthText(ComputingValue?.Length ?? 0.0);

        public bool IsSendingAudioInput => IsAudioInput && Configure.Instance.AudioInput;

        public void NotifyComputedValues()
        {
            OnPropertyChanged(nameof(HighestInputCountText));
            OnPropertyChanged(nameof(BPMText));
        }

        public void NotifyIsSendingAudioInput() => OnPropertyChanged(nameof(IsSendingAudioInput));

        public bool IsNew
        {
            get => _isNew;

            set => SetProperty(ref _isNew, value, nameof(SiteName));
        }

        public bool IsNetSite
        {
            get => _isNetSite;

            set => SetProperty(ref _isNetSite, value, nameof(IsNetSite));
        }

        public string SiteName
        {
            get => IsNew ? $"{_siteName} 💬" : _siteName;

            set => SetProperty(ref _siteName, value, nameof(SiteName));
        }

        public string SiteNotify
        {
            get => _siteNotify;

            set => SetProperty(ref _siteNotify, value, nameof(SiteNotify));
        }

        public string SiteID { get; set; }

        public string Input
        {
            get => _input;

            set => SetProperty(ref _input, value, nameof(Input));
        }

        public bool IsIdle => SiteSituationValue == SiteSituation.Default;

        public bool CanSaveNetBundle => !string.IsNullOrEmpty(BundleName) && IsIdle;

        public bool CanSetTwilightConfigures => IsSiteHand && IsIdle;

        public bool CanSetPostableItems => IsSiteHand && IsIdle && ValidNetModeValue.Data > 0;

        public bool CanSiteHandLevying => IsSiteHand && IsIdle;

        public bool CanStopSiteNet => IsSiteHand && !IsIdle;

        public SiteSituation SiteSituationValue
        {
            get => _siteSituationValue;

            set
            {
                if (SetProperty(ref _siteSituationValue, value, nameof(SiteSituationValue)))
                {
                    OnPropertyChanged(nameof(IsIdle));
                    OnPropertyChanged(nameof(CanSaveNetBundle));
                    OnPropertyChanged(nameof(CanSetTwilightConfigures));
                    OnPropertyChanged(nameof(CanSetPostableItems));
                    OnPropertyChanged(nameof(CanSiteHandLevying));
                    OnPropertyChanged(nameof(CanStopSiteNet));
                }
            }
        }

        public AvatarItem AvatarItemValue
        {
            get => _avatarItemValue;

            set => SetProperty(ref _avatarItemValue, value, nameof(AvatarItemValue));
        }

        public string SiteHand
        {
            get => _siteHand;

            set
            {
                if (SetProperty(ref _siteHand, value))
                {
                    OnPropertyChanged(nameof(IsSiteHand));
                    OnPropertyChanged(nameof(IsIdle));
                    OnPropertyChanged(nameof(CanSetTwilightConfigures));
                    OnPropertyChanged(nameof(CanSetPostableItems));
                    OnPropertyChanged(nameof(CanSiteHandLevying));
                    OnPropertyChanged(nameof(CanStopSiteNet));
                }
            }
        }

        public JSON.TwilightCallSiteAvatar? PendingCallSiteAvatarData { get; set; }

        public void SetCallSiteAvatarData(JSON.TwilightCallSiteAvatar pendingCallSiteAvatarData)
        {
            var siteHand = pendingCallSiteAvatarData.siteHand;
            SiteName = Utility.GetSiteName(pendingCallSiteAvatarData.siteName);
            SiteHand = siteHand;
            SiteSituationValue = (SiteSituation)pendingCallSiteAvatarData.situationValue;
            Utility.SetUICollection(AvatarItemCollection, pendingCallSiteAvatarData.data.Select(data =>
            {
                var avatarID = data.avatarID;
                return new AvatarItem(avatarID)
                {
                    AvatarConfigureValue = (AvatarItem.AvatarConfigure)data.avatarConfigure,
                    IsSiteHand = !string.IsNullOrEmpty(siteHand) && avatarID == siteHand,
                    IsMe = Utility.GetDefaultAvatarID(avatarID) == TwilightSystem.Instance.AvatarID,
                    AvatarName = data.avatarName,
                    AvatarGroupValue = new()
                    {
                        Data = data.avatarGroup
                    },
                    IsValve = data.isValve,
                    IsAudioInput = data.isAudioInput
                };
            }).ToArray(), null, null, (valueItem, targetItem) =>
            {
                valueItem.AvatarConfigureValue = targetItem.AvatarConfigureValue;
                valueItem.IsSiteHand = targetItem.IsSiteHand;
                valueItem.IsMe = targetItem.IsMe;
                valueItem.AvatarGroupValue = targetItem.AvatarGroupValue;
                valueItem.IsAudioInput = targetItem.IsAudioInput;
            });
            OnPropertyChanged(nameof(AllowSiteHandLevying));
            if (pendingCallSiteAvatarData.setNoteFile)
            {
                CallSetNoteFile();
            }
        }

        public JSON.TwilightCallSiteNet PendingCallSiteNetData { get; set; }

        public void SetComputingValues(JSON.TwilightCallSiteNet twilightCallSiteNet = null)
        {
            if (IsNetSite)
            {
                var mainViewModel = ViewModels.Instance.MainValue;
                if (twilightCallSiteNet != null)
                {
                    var noteID = twilightCallSiteNet.noteID;
                    _defaultNetSiteComputing = new()
                    {
                        NoteID = noteID,
                        IsDefault = true,
                        Title = twilightCallSiteNet.title,
                        Artist = twilightCallSiteNet.artist,
                        LevelText = twilightCallSiteNet.levelText,
                        LevelValue = twilightCallSiteNet.level,
                        Genre = twilightCallSiteNet.genre,
                        JudgmentStage = twilightCallSiteNet.judgmentStage,
                        HitPointsValue = twilightCallSiteNet.hitPointsValue,
                        TotalNotes = twilightCallSiteNet.totalNotes,
                        LongNotes = twilightCallSiteNet.longNotes,
                        AutoableNotes = twilightCallSiteNet.autoableNotes,
                        TrapNotes = twilightCallSiteNet.trapNotes,
                        HighestInputCount = twilightCallSiteNet.highestInputCount,
                        Length = twilightCallSiteNet.length,
                        BPM = twilightCallSiteNet.bpm,
                        LowestBPM = twilightCallSiteNet.lowestBPM,
                        HighestBPM = twilightCallSiteNet.highestBPM,
                        InputMode = twilightCallSiteNet.inputMode,
                        WantLevelID = twilightCallSiteNet.wantLevelID,
                        HandledValue = mainViewModel.NoteID512s.TryGetValue(noteID, out var netSiteNoteFile) ? netSiteNoteFile.HandledValue : BaseNoteFile.Handled.Not,
                        IsAutoLongNote = twilightCallSiteNet.isAutoLongNote
                    };
                    _netSiteNoteID = twilightCallSiteNet.noteIDs;
                    _bundleEntryPath = twilightCallSiteNet.bundleEntryPath;
                    BundleName = twilightCallSiteNet.bundleName;
                    ModeComponentValue.ComputingValue = _defaultNetSiteComputing;
                }
                _defaultNetSiteComputing.NotHaveIt = !mainViewModel.NoteID512s.ContainsKey(_defaultNetSiteComputing.NoteID);
                HandlingUISystem.Instance.HandleParallel(() =>
                {
                    var noteID = ComputingValue?.NoteID;
                    ComputingValues.Clear();
                    ComputingValues.Add(_defaultNetSiteComputing);
                    foreach (var netSiteNoteID in _netSiteNoteID)
                    {
                        if (netSiteNoteID != _defaultNetSiteComputing.NoteID && mainViewModel.NoteID512s.TryGetValue(netSiteNoteID, out var netSiteNoteFile))
                        {
                            var netSiteComputing = new NetSiteComputing();
                            netSiteComputing.NoteID = netSiteNoteID;
                            netSiteComputing.Title = netSiteNoteFile.Title;
                            netSiteComputing.Artist = netSiteNoteFile.Artist;
                            netSiteComputing.LevelText = netSiteNoteFile.LevelText;
                            netSiteComputing.LevelValue = netSiteNoteFile.LevelValue;
                            netSiteComputing.Genre = netSiteNoteFile.Genre;
                            netSiteComputing.JudgmentStage = netSiteNoteFile.JudgmentStage;
                            netSiteComputing.HitPointsValue = netSiteNoteFile.HitPointsValue;
                            netSiteComputing.TotalNotes = netSiteNoteFile.TotalNotes;
                            netSiteComputing.LongNotes = netSiteNoteFile.LongNotes;
                            netSiteComputing.AutoableNotes = netSiteNoteFile.AutoableNotes;
                            netSiteComputing.TrapNotes = netSiteNoteFile.TrapNotes;
                            netSiteComputing.HighestInputCount = netSiteNoteFile.HighestInputCount;
                            netSiteComputing.Length = netSiteNoteFile.Length;
                            netSiteComputing.BPM = netSiteNoteFile.BPM;
                            netSiteComputing.LowestBPM = netSiteNoteFile.LowestBPM;
                            netSiteComputing.HighestBPM = netSiteNoteFile.HighestBPM;
                            netSiteComputing.InputMode = netSiteNoteFile.InputMode;
                            netSiteComputing.WantLevelID = netSiteNoteFile.WantLevelID;
                            netSiteComputing.HandledValue = netSiteNoteFile.HandledValue;
                            ComputingValues.Add(netSiteComputing);
                        }
                    }
                    ComputingValue = IsSiteHand ? _defaultNetSiteComputing : ComputingValues.FirstOrDefault(valueComputing => valueComputing.NoteID == noteID) ?? ComputingValues.First();
                });
            }
        }

        public void SetAllowedPostableItems(JSON.TwilightCallSiteNet twilightCallSiteNet)
        {
            foreach (var postableUIItem in PostableUIItemCollection)
            {
                postableUIItem.IsWanted = twilightCallSiteNet.allowedPostableItems.Contains((int)postableUIItem.PostableItemValue.VarietyValue);
            }
            OnPropertyChanged(nameof(IsTotalWantPostableUIItem));
        }

        public void SetCallSiteNetData(JSON.TwilightCallSiteNet pendingTwilightCallSiteNetData)
        {
            IsFavorNoteFile = pendingTwilightCallSiteNetData.isFavorNoteFile;
            IsFavorModeComponent = pendingTwilightCallSiteNetData.isFavorModeComponent;
            IsFavorAudioMultiplier = pendingTwilightCallSiteNetData.isFavorAudioMultiplier;
            ValidHunterMode = pendingTwilightCallSiteNetData.validHunterMode;
            ValidNetModeValue = new()
            {
                Data = pendingTwilightCallSiteNetData.validNetMode
            };
            SetAllowedPostableItems(pendingTwilightCallSiteNetData);
            IsAutoSiteHand = pendingTwilightCallSiteNetData.isAutoSiteHand;
            if (IsSiteHand)
            {
                SetComputingValues(pendingTwilightCallSiteNetData);
            }
            else
            {
                HandlingUISystem.Instance.HandleParallel(() =>
                {
                    var lastComputingValues = ComputingValues.ToArray();
                    SetComputingValues(pendingTwilightCallSiteNetData);
                    if (IsAutoNetLevying && lastComputingValues.Except(ComputingValues).Any())
                    {
                        HandleLevyingInput(true);
                    }
                    if (ViewModels.Instance.SiteContainerValue.IsOpened && IsOpened)
                    {
                        var mainViewModel = ViewModels.Instance.MainValue;
                        if (mainViewModel.NoteID512s.TryGetValue(ComputingValue.NoteID, out var noteFile))
                        {
                            mainViewModel.EntryItemValue = noteFile.EntryItem;
                        }
                    }
                });
            }
        }

        public JSON.TwilightCallSiteModeComponent? PendingCallSiteModeComponentData { get; set; }

        public void SetCallSiteModeComponentData(JSON.TwilightCallSiteModeComponent pendingTwilightCallModeComponentData)
        {
            ModeComponentValue.CopyAsJSON(pendingTwilightCallModeComponentData.modeComponentData);
            NotifyComputedValues();
        }

        public void HandleLevyingInput(bool isSilent)
        {
            if (ComputingValue != null && ViewModels.Instance.MainValue.NoteID512s.ContainsKey(ComputingValue.NoteID))
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.LevyNet, SiteID);
            }
            else if (!isSilent)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.HasNotNetNoteFile);
            }
        }

        public void CallSetNoteFile()
        {
            if (IsNetSite && IsSiteHand)
            {
                var entryItemValue = ViewModels.Instance.MainValue.EntryItemValue;
                var noteFile = entryItemValue?.NoteFile;
                if (noteFile?.IsLogical == false)
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetNoteFile, new
                    {
                        siteID = SiteID,
                        noteID = noteFile.GetNoteID512(),
                        noteIDs = IsFavorNoteFile ? noteFile.EntryItem.CompatibleNoteFiles.Select(noteFile => noteFile.GetNoteID512()) : new[] { noteFile.GetNoteID512() },
                        title = noteFile.Title,
                        artist = noteFile.Artist,
                        genre = noteFile.Genre,
                        levelText = noteFile.LevelText,
                        level = noteFile.LevelValue,
                        wantLevelID = noteFile.WantLevelID,
                        judgmentStage = noteFile.JudgmentStage,
                        hitPointsValue = noteFile.HitPointsValue,
                        totalNotes = noteFile.TotalNotes,
                        longNotes = noteFile.LongNotes,
                        autoableNotes = noteFile.AutoableNotes,
                        trapNotes = noteFile.TrapNotes,
                        highestInputCount = noteFile.HighestInputCount,
                        length = noteFile.Length,
                        bpm = noteFile.BPM,
                        lowestBPM = noteFile.LowestBPM,
                        highestBPM = noteFile.HighestBPM,
                        inputMode = noteFile.InputMode,
                        isAutoLongNote = noteFile.IsAutoLongNote,
                        bundleEntryPath = noteFile.EntryItem.EntryPath
                    });
                }
            }
        }

        public void CallUpdateModeComponent()
        {
            if (IsNetSite && IsSiteHand)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetModeComponent, new
                {
                    siteID = SiteID,
                    data = ViewModels.Instance.MainValue.ModeComponentValue.GetJSON()
                });
            }
        }

        public void NewSiteYells(ICollection<JSON.TwilightSiteYellItem> siteYells, bool isGetSiteYell)
        {
            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
            var targetAvatarID = TwilightSystem.Instance.AvatarID;
            var isMySiteYell = siteYells.Any(data => data.avatarID == targetAvatarID);
            var pendingNewSiteYells = siteYells.Select(data => new Func<ISiteYell>(() =>
            {
                var avatarID = data.avatarID;
                var avatarName = data.avatarName;
                var date = DateTimeOffset.FromUnixTimeMilliseconds(data.date).LocalDateTime.ToLongTimeString();
                var siteYellID = data.siteYellID;
                var siteYell = data.siteYell;
                switch (avatarName)
                {
                    case "@Enter":
                        return new EnterSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@Quit":
                        return new QuitSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@Site":
                        return new NewSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@Net":
                        return new NewNetSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@Comment":
                        return new CommentSiteYell(siteYell, date, siteYellID);
                    case "@Ability":
                        return new AbilitySiteYell(siteYell, date, siteYellID);
                    case "@Level":
                        return new LevelSiteYell(siteYell, date, siteYellID);
                    case "@Notify":
                        SiteNotify = siteYell;
                        return new TaehuiSiteYell(siteYell, date, siteYellID);
                    case "@Invite":
                        return new InviteSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@TV":
                        return new TVSiteYell(avatarID, siteYell, date, siteYellID);
                    case "@Wiped":
                        return new WipedSiteYell(avatarID, siteYell, date, siteYellID);
                    case "":
                        return new NotifySiteYell(siteYell, siteYellID);
                    default:
                        var href = Utility.CompileSiteYells(siteYell);
                        var position = href.IndexOf('?');
                        switch (Utility.GetAvailable(position != -1 ? href.Substring(0, position) : href))
                        {
                            case Utility.AvailableFlag.Audio:
                                return new AudioSiteYell(avatarID, avatarName, date, siteYell, href, siteYellID);
                            case Utility.AvailableFlag.Media:
                                return new MediaSiteYell(avatarID, avatarName, date, siteYell, href, siteYellID);
                            case Utility.AvailableFlag.Drawing:
                                return new DrawingSiteYell(avatarID, avatarName, date, siteYell, href, siteYellID);
                            default:
                                return new DefaultSiteYell(avatarID, avatarName, date, siteYell, href, siteYellID);
                        }
                }
            }));
            if (siteContainerViewModel.IsOpened && IsOpened)
            {
                HandlingUISystem.Instance.HandleParallel(() =>
                {
                    if (isGetSiteYell)
                    {
                        foreach (var pendingNewSiteYell in pendingNewSiteYells.Reverse())
                        {
                            SiteYellCollection.Insert(0, PutPlatformSiteYell(pendingNewSiteYell()));
                        }
                    }
                    else
                    {
                        foreach (var pendingNewSiteYell in pendingNewSiteYells)
                        {
                            SiteYellCollection.Add(PutPlatformSiteYell(pendingNewSiteYell()));
                        }
                    }
                    if (isGetSiteYell)
                    {
                        _lastPosition1BeforeCall = View.SiteYellsViewer.ExtentHeight - View.SiteYellsViewer.ActualHeight;
                    }
                    else if (isMySiteYell || _isSiteYellsViewLowest)
                    {
                        View.SiteYellsViewer.ScrollToEnd();
                    }
                });
            }
            else
            {
                lock (_pendingNewSiteYellQueue)
                {
                    if (isGetSiteYell)
                    {
                        foreach (var pendingNewSiteYell in pendingNewSiteYells.Reverse())
                        {
                            _pendingNewSiteYellQueue.AddFirst(pendingNewSiteYell);
                        }
                    }
                    else
                    {
                        foreach (var putSiteYell in pendingNewSiteYells)
                        {
                            _pendingNewSiteYellQueue.AddLast(putSiteYell);
                        }
                    }
                }
            }
        }

        public void ModifySiteYell(JSON.TwilightModifySiteYell twilightModifySiteYell)
        {
            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
            var targetAvatarID = TwilightSystem.Instance.AvatarID;
            var pendingModifySiteYell = new Action(() =>
            {
                if (_siteYellsMap.TryGetValue(twilightModifySiteYell.siteYellID, out var siteYell))
                {
                    siteYell.SiteYell = twilightModifySiteYell.siteYell;
                }
            });
            if (siteContainerViewModel.IsOpened && IsOpened)
            {
                pendingModifySiteYell();
            }
            else
            {
                lock (_pendingModifySiteYellQueue)
                {
                    _pendingModifySiteYellQueue.Add(pendingModifySiteYell);
                }
            }
        }

        public void WipeSiteYell(JSON.TwilightWipeSiteYell twilightWipeSiteYell)
        {
            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
            var targetAvatarID = TwilightSystem.Instance.AvatarID;
            var pendingWipeSiteYell = new Action(() =>
            {
                if (_siteYellsMap.TryGetValue(twilightWipeSiteYell.siteYellID, out var siteYell))
                {
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        var i = SiteYellCollection.IndexOf(siteYell);
                        SiteYellCollection.Remove(siteYell);
                        SiteYellCollection.Insert(i, new WipedSiteYell(siteYell.AvatarID, siteYell.AvatarName, siteYell.Date, siteYell.SiteYellID));
                    });
                }
            });
            if (siteContainerViewModel.IsOpened && IsOpened)
            {
                pendingWipeSiteYell();
            }
            else
            {
                lock (_pendingWipeSiteYellQueue)
                {
                    _pendingWipeSiteYellQueue.Add(pendingWipeSiteYell);
                }
            }
        }

        public void Notify(JSON.TwilightSiteYellItem siteYell)
        {
            if (IsGetNotify)
            {
                if (!WasNotify && !ViewModels.Instance.SiteContainerValue.IsOpened)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, siteYell.ToString(), false);
                    WasNotify = true;
                }
            }
        }

        ISiteYell PutPlatformSiteYell(ISiteYell siteYell)
        {
            var siteYellID = siteYell.SiteYellID;
            if (siteYellID != -1)
            {
                _siteYellsMap[siteYellID] = siteYell;
            }
            return siteYell;
        }

        public void OnOpened()
        {
            if (IsOpened)
            {
                WasNotify = false;
                lock (_pendingNewSiteYellQueue)
                {
                    if (_pendingNewSiteYellQueue.Count > 0)
                    {
                        foreach (var pendingNewSiteYell in _pendingNewSiteYellQueue)
                        {
                            SiteYellCollection.Add(PutPlatformSiteYell(pendingNewSiteYell()));
                        }
                        _pendingNewSiteYellQueue.Clear();
                    }
                }
                lock (_pendingModifySiteYellQueue)
                {
                    if (_pendingModifySiteYellQueue.Count > 0)
                    {
                        foreach (var pendingModifySiteYell in _pendingModifySiteYellQueue)
                        {
                            pendingModifySiteYell();
                        }
                        _pendingModifySiteYellQueue.Clear();
                    }
                }
                lock (_pendingWipeSiteYellQueue)
                {
                    if (_pendingWipeSiteYellQueue.Count > 0)
                    {
                        foreach (var pendingWipeSiteYell in _pendingWipeSiteYellQueue)
                        {
                            pendingWipeSiteYell();
                        }
                        _pendingWipeSiteYellQueue.Clear();
                    }
                }
                if (_isSiteYellsViewLowest)
                {
                    View.SiteYellsViewer.ScrollToEnd();
                }
                if (IsNew)
                {
                    IsNew = false;
                }
            }
            if (PendingCallSiteAvatarData.HasValue)
            {
                var pendingCallSiteAvatarData = PendingCallSiteAvatarData.Value;
                PendingCallSiteAvatarData = null;
                SetCallSiteAvatarData(pendingCallSiteAvatarData);
            }
            if (PendingCallSiteNetData != null)
            {
                var pendingSetSiteNetData = PendingCallSiteNetData;
                PendingCallSiteNetData = null;
                SetCallSiteNetData(pendingSetSiteNetData);
            }
            if (PendingCallSiteModeComponentData.HasValue)
            {
                var pendingCallSiteModeComponentData = PendingCallSiteModeComponentData.Value;
                PendingCallSiteModeComponentData = null;
                SetCallSiteModeComponentData(pendingCallSiteModeComponentData);
            }
            if (_hasPendingNew)
            {
                _hasPendingNew = false;
                IsNew = true;
            }
        }

        public void SetSiteAvatar(JSON.TwilightCallSiteAvatar pendingCallSiteAvatarData)
        {
            if (ViewModels.Instance.SiteContainerValue.IsOpened)
            {
                SetCallSiteAvatarData(pendingCallSiteAvatarData);
            }
            else
            {
                PendingCallSiteAvatarData = pendingCallSiteAvatarData;
            }
        }

        public void SetSiteNet(JSON.TwilightCallSiteNet pendingCallSiteNetData)
        {
            if (ViewModels.Instance.SiteContainerValue.IsOpened)
            {
                SetCallSiteNetData(pendingCallSiteNetData);
            }
            else
            {
                PendingCallSiteNetData = pendingCallSiteNetData;
            }
        }

        public void SetSiteModeComponent(JSON.TwilightCallSiteModeComponent pendingCallSiteModeComponentData)
        {
            if (ViewModels.Instance.SiteContainerValue.IsOpened)
            {
                SetCallSiteModeComponentData(pendingCallSiteModeComponentData);
            }
            else
            {
                PendingCallSiteModeComponentData = pendingCallSiteModeComponentData;
            }
        }

        public void SetNew()
        {
            if (!IsOpened)
            {
                if (ViewModels.Instance.SiteContainerValue.IsOpened)
                {
                    IsNew = true;
                }
                else
                {
                    _hasPendingNew = true;
                }
            }
        }

        public void AudioInput(byte[] data, int length) => TwilightSystem.Instance.SendParallel(Event.Types.EventID.AudioInput, SiteID, UnsafeByteOperations.UnsafeWrap(data.AsMemory(0, length)));
    }
}