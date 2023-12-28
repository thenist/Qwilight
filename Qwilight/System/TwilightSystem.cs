using CommunityToolkit.Mvvm.Messaging;
using Google.Protobuf;
using Ionic.Zip;
using Qwilight.Compute;
using Qwilight.MSG;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.View;
using Qwilight.ViewModel;
using System.Buffers;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight
{
    public sealed class TwilightSystem : Model, IDisposable
    {
        const int NotEstablished = 0;
        const int NotSignedIn = 1;
        const int SignedIn = 2;

        public static readonly TwilightSystem Instance = QwilightComponent.GetBuiltInData<TwilightSystem>(nameof(TwilightSystem));

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(TwilightSystem));

        readonly Action<object> _handleSendParallel;
        readonly ConcurrentDictionary<string, NotifyItem> _savingBundleItems = new();
        readonly HttpClient _wwwClient = new()
        {
            DefaultRequestVersion = HttpVersion.Version30
        };
        SslStream _ss;
        int _twilightSituation;
        bool _isAvailable = true;

        string QwilightName => QwilightComponent.IsValve ? ValveSystem.Instance.ValveName : Environment.UserName;

        public TwilightSystem()
        {
            _wwwClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _handleSendParallel = obj =>
            {
                var eventItem = obj as Event;
                if (eventItem != null)
                {
                    eventItem.Millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    eventItem.AvatarID = AvatarID;
                    try
                    {
                        lock (_ss)
                        {
                            eventItem.WriteDelimitedTo(_ss);
                        }
                    }
                    catch
                    {
                    }
                }
            };
        }

        public AvatarWww AvatarWwwValue { get; set; }

        public string Totem { get; set; } = string.Empty;

        public string AvatarID { get; set; } = string.Empty;

        public string AvatarName { get; set; } = string.Empty;

        public string GetAvatarName() => TwilightSituation == NotEstablished ? QwilightName : AvatarName;

        public int TwilightSituation
        {
            get => _twilightSituation;

            set
            {
                if (value != SignedIn)
                {
                    AvatarDrawingSystem.Instance.WipeAvatarDrawings();
                    AvatarEdgeSystem.Instance.WipeAvatarEdges();
                    AvatarTitleSystem.Instance.WipeAvatarTitles();
                }
                if (SetProperty(ref _twilightSituation, value, nameof(TwilightSituation)))
                {
                    OnPropertyChanged(nameof(IsEstablished));
                    OnPropertyChanged(nameof(IsSignedIn));
                    ViewModels.Instance.MainValue.NotifyCanSaveAsBundle();
                    ViewModels.Instance.MainValue.NotifyCanTwilightCommentary();
                    ViewModels.Instance.MainValue.NotifyCanTwilightFavor();
                    ViewModels.Instance.ConfigureValue.NotifyCanGetDefaultText();
                    ViewModels.Instance.ConfigureValue.NotifyCanSaveAsBundle();
                    ViewModels.Instance.BundleValue.NotifyIsMe();
                    ViewModels.Instance.AvatarValue.NotifyIsMe();
                    ViewModels.Instance.AvatarValue.NotifyIsLevelVSVisible();
                }
            }
        }

        public void StopBundle(string siteID)
        {
            if (_savingBundleItems.TryGetValue(siteID, out var savingBundleItem))
            {
                savingBundleItem.OnStop(false);
            }
        }

        public bool IsEstablished => TwilightSituation != NotEstablished;

        public bool IsSignedIn => TwilightSituation == SignedIn;

        public void NotifyAvatarWwwValue()
        {
            AvatarWwwValue = new AvatarWww(AvatarID);
            OnPropertyChanged(nameof(AvatarWwwValue));
        }

        public override void NotifyModel()
        {
            base.NotifyModel();
            NotifyAvatarWwwValue();
        }

        static Event NewEvent<T>(Event.Types.EventID eventID, T text, ByteString[] data)
        {
            var eventItem = new Event
            {
                EventID = eventID,
                Text = text != null ? text as string ?? Utility.SetJSON(text) : string.Empty
            };
            if (data != null)
            {
                eventItem.Data.AddRange(data);
            }
            return eventItem;
        }

        void Send<T>(Event.Types.EventID eventID, T text, params ByteString[] data)
        {
            if (IsEstablished)
            {
                _handleSendParallel(NewEvent<T>(eventID, text, data));
            }
        }

        public void SendParallel(Event.Types.EventID eventID, string text, params ByteString[] data)
        {
            if (IsEstablished || eventID == Event.Types.EventID.Establish)
            {
                Task.Factory.StartNew(_handleSendParallel, NewEvent(eventID, text, data));
            }
        }

        public void SendParallel<T>(Event.Types.EventID eventID, T text, params ByteString[] data)
        {
            if (IsEstablished || eventID == Event.Types.EventID.Establish)
            {
                Task.Factory.StartNew(_handleSendParallel, NewEvent(eventID, text, data));
            }
        }

        public void SendParallel(Event eventItem)
        {
            if (IsEstablished)
            {
                Task.Factory.StartNew(_handleSendParallel, eventItem);
            }
        }

        void AutoEnter(Func<int, bool> onAutoEnter)
        {
            if (onAutoEnter(Configure.Instance.AutoEnterNotifySite.Data))
            {
                SendParallel(Event.Types.EventID.EnterSite, new
                {
                    siteID = "00000000-0000-0000-0000-000000000000",
                    siteCipher = string.Empty
                });
            }
            if (onAutoEnter(Configure.Instance.AutoEnterDefaultSite.Data))
            {
                SendParallel(Event.Types.EventID.EnterSite, new
                {
                    siteID = "00000000-0000-0000-0000-000000000001",
                    siteCipher = string.Empty
                });
            }
            if (onAutoEnter(Configure.Instance.AutoEnterPlatformSite.Data))
            {
                SendParallel(Event.Types.EventID.EnterSite, new
                {
                    siteID = "00000000-0000-0000-0000-000000000002",
                    siteCipher = string.Empty
                });
            }
        }

        public void HandleSystem()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var signInViewModel = ViewModels.Instance.SignInValue;
            var bundleViewModel = ViewModels.Instance.BundleValue;
            var ubuntuViewModel = ViewModels.Instance.UbuntuValue;
            var toNotifyViewModel = ViewModels.Instance.NotifyValue;
            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
            var twilightConfigureViewModel = ViewModels.Instance.TwilightConfigure;
            var netSiteCommentViewModel = ViewModels.Instance.NetSiteCommentValue;
            var wasMITM = false;
            while (true)
            {
                string textClose = null;
                try
                {
                    using (var tc = new TcpClient(QwilightComponent.TaehuiNetDDNS, 6101))
                    using (_ss = QwilightComponent.IsVS ? new(tc.GetStream(), false, (sender, certificate, chain, sslPolicyErrors) => true) : new(tc.GetStream()))
                    {
                        _ss.AuthenticateAsClient("taehui.ddns.net");
                        wasMITM = false;
                        var textEstablish = new
                        {
                            hash = QwilightComponent.HashText,
                            date = QwilightComponent.DateText,
                            language = Configure.Instance.Language,
                            qwilightName = QwilightName,
                            amd64 = QwilightComponent.AMD64Name,
                            os = QwilightComponent.OSName,
                            ram = QwilightComponent.RAMName,
                            vga = QwilightComponent.GPUName,
                            m2 = QwilightComponent.M2Name,
                            audio = QwilightComponent.AudioName,
                            tv = QwilightComponent.TVName,
                            lan = QwilightComponent.LANName,
                        };
                        if (ValveSystem.Instance.ValveDrawing != null)
                        {
                            SendParallel(Event.Types.EventID.Establish, textEstablish, ValveSystem.Instance.ValveDrawing);
                        }
                        else
                        {
                            SendParallel(Event.Types.EventID.Establish, textEstablish);
                        }
                        while (_isAvailable)
                        {
                            var eventItem = Event.Parser.ParseDelimitedFrom(_ss);
                            var eventItemText = eventItem.Text;
                            var eventItemData = eventItem.Data;
                            var defaultComputer = mainViewModel.Computer;
                            switch (eventItem.EventID)
                            {
                                case Event.Types.EventID.Close:
                                    textClose = eventItemText;
                                    break;
                                case Event.Types.EventID.Establish:
                                    var twilightEstablish = Utility.GetJSON<JSON.TwilightEstablish>(eventItemText);
                                    Totem = string.Empty;
                                    AvatarID = twilightEstablish.avatarID;
                                    AvatarName = twilightEstablish.avatarName;
                                    TwilightSituation = NotSignedIn;
                                    mainViewModel.LoadCommentItemCollection();
                                    Configure.Instance.CommentViewTabPosition = Configure.Instance.CommentViewTabPosition;
                                    AutoEnter(autoEnter => autoEnter == AutoEnterSite.AutoEnter);
                                    if (Configure.Instance.AutoSignIn)
                                    {
                                        var avatarCipher = Configure.Instance.GetCipher();
                                        if (!string.IsNullOrEmpty(avatarCipher))
                                        {
                                            SendParallel(Event.Types.EventID.SignIn, new
                                            {
                                                avatarID = Configure.Instance.AvatarID,
                                                avatarCipher
                                            });
                                        }
                                    }
                                    _ = GetDefaultNoteDate(Configure.Instance.DefaultNoteDate, Configure.Instance.AutoGetDefaultNote);
                                    _ = GetDefaultUIDate(Configure.Instance.DefaultUIDate, Configure.Instance.AutoGetDefaultUI);
                                    break;
                                case Event.Types.EventID.UnavailableDate:
                                    if (!Configure.Instance.AutoGetQwilight)
                                    {
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, eventItemText);
                                    }
                                    break;
                                case Event.Types.EventID.SignIn:
                                    signInViewModel.Close();
                                    var twilightSignIn = Utility.GetJSON<JSON.TwilightSignIn>(eventItemText);
                                    Totem = twilightSignIn.totem;
                                    AvatarID = twilightSignIn.avatarID;
                                    AvatarName = twilightSignIn.avatarName;
                                    TwilightSituation = SignedIn;
                                    NotifyAvatarWwwValue();
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SuccessfullySignedInContents, GetAvatarName()), false, "Sign in");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.SignIn);
                                    AutoEnter(autoEnter => autoEnter != AutoEnterSite.WaitSite);
                                    break;
                                case Event.Types.EventID.NotSignIn:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.NotSignedInContents, GetAvatarName()), false, "Not Sign in");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.NotSignIn);
                                    var twilightNotSignIn = Utility.GetJSON<JSON.TwilightNotSignIn>(eventItemText);
                                    Totem = string.Empty;
                                    AvatarID = twilightNotSignIn.avatarID;
                                    AvatarName = twilightNotSignIn.avatarName;
                                    TwilightSituation = NotSignedIn;
                                    AutoEnter(autoEnter => autoEnter == AutoEnterSite.AutoEnter);
                                    break;
                                case Event.Types.EventID.QuitSite:
                                    var toQuitSiteViewModel = ViewModels.Instance.WipeSiteViewModel(eventItemText);
                                    if (toQuitSiteViewModel != null)
                                    {
                                        UIHandler.Instance.HandleParallel(() => siteContainerViewModel.SiteViewCollection.Remove(toQuitSiteViewModel.View));
                                    }
                                    break;
                                case Event.Types.EventID.Warning:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, eventItemText);
                                    break;
                                case Event.Types.EventID.NotifyInfo:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, eventItemText);
                                    break;
                                case Event.Types.EventID.NotifyYes:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, eventItemText);
                                    break;
                                case Event.Types.EventID.LevelUp:
                                    var twilightLevelUp = Utility.GetJSON<JSON.TwilightLevelUp>(eventItemText);
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, $"LV. {twilightLevelUp.from} → LV. {twilightLevelUp.to}", true, "Level Up");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.LevelUp);
                                    break;
                                case Event.Types.EventID.AbilityUp:
                                    var twilightAbilityUp = Utility.GetJSON<JSON.TwilightAbilityUp>(eventItemText);
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, twilightAbilityUp.ToString(), true, "Ability Up");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.AbilityUp);
                                    break;
                                case Event.Types.EventID.WwwLevel:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.WwwLevelClearContents, eventItemText), true, "Www Level Clear");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.LevelClear);
                                    break;
                                case Event.Types.EventID.NewTitle:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.NewTitleContents, eventItemText), true, "New Title");
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.NewTitle);
                                    break;
                                case Event.Types.EventID.SiteYell:
                                    var twilightSiteYell = Utility.GetJSON<JSON.TwilightSiteYell>(eventItemText);
                                    var siteYellSiteViewModel = ViewModels.Instance.GetSiteViewModel(twilightSiteYell.siteID);
                                    if (siteYellSiteViewModel != null)
                                    {
                                        var siteYell = new JSON.TwilightSiteYellItem
                                        {
                                            avatarID = twilightSiteYell.avatarID,
                                            avatarName = twilightSiteYell.avatarName,
                                            date = twilightSiteYell.date,
                                            siteYell = twilightSiteYell.siteYell,
                                            siteYellID = twilightSiteYell.siteYellID
                                        };
                                        siteYellSiteViewModel.NewSiteYells([siteYell], false);
                                        siteYellSiteViewModel.SetNew();
                                        if (twilightSiteYell.avatarName != "@Enter" || twilightSiteYell.siteYell != GetAvatarName())
                                        {
                                            siteYellSiteViewModel.Notify(siteYell);
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.ModifySiteYell:
                                    var twilightModifySiteYell = Utility.GetJSON<JSON.TwilightModifySiteYell>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightModifySiteYell.siteID)?.ModifySiteYell(twilightModifySiteYell);
                                    break;
                                case Event.Types.EventID.WipeSiteYell:
                                    var twilightWipeSiteYell = Utility.GetJSON<JSON.TwilightWipeSiteYell>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightWipeSiteYell.siteID)?.WipeSiteYell(twilightWipeSiteYell);
                                    break;
                                case Event.Types.EventID.GetSiteYells:
                                    var twilightGetSiteYells = Utility.GetJSON<JSON.TwilightGetSiteYells>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightGetSiteYells.siteID)?.NewSiteYells(twilightGetSiteYells.data, true);
                                    break;
                                case Event.Types.EventID.EnterSite:
                                    var twilightEnterSite = Utility.GetJSON<JSON.TwilightEnterSite>(eventItemText);
                                    var toEnterSiteID = twilightEnterSite.siteID;
                                    var toEnterSiteViewModel = ViewModels.Instance.NewSiteViewModel(toEnterSiteID);
                                    toEnterSiteViewModel.SiteID = toEnterSiteID;
                                    toEnterSiteViewModel.SiteNotify = twilightEnterSite.siteNotify;
                                    toEnterSiteViewModel.IsGetNotify = twilightEnterSite.isGetNotify;
                                    toEnterSiteViewModel.IsEditable = twilightEnterSite.isEditable;
                                    toEnterSiteViewModel.CanAudioInput = twilightEnterSite.isAudioInput;
                                    toEnterSiteViewModel.IsAudioInput = twilightEnterSite.isAudioInput;
                                    toEnterSiteViewModel.NewSiteYells(twilightEnterSite.data, false);
                                    toEnterSiteViewModel.IsNetSite = twilightEnterSite.isNetSite;
                                    if (toEnterSiteViewModel.IsNetSite)
                                    {
                                        toEnterSiteViewModel.SetComputingValues(twilightEnterSite);
                                        toEnterSiteViewModel.IsFavorNoteFile = twilightEnterSite.isFavorNoteFile;
                                        toEnterSiteViewModel.IsFavorModeComponent = twilightEnterSite.isFavorModeComponent;
                                        toEnterSiteViewModel.IsFavorAudioMultiplier = twilightEnterSite.isFavorAudioMultiplier;
                                        toEnterSiteViewModel.IsAutoSiteHand = twilightEnterSite.isAutoSiteHand;
                                        toEnterSiteViewModel.ValidHunterMode = twilightEnterSite.validHunterMode;
                                        toEnterSiteViewModel.ValidNetModeValue = new()
                                        {
                                            Data = twilightEnterSite.validNetMode
                                        };
                                        toEnterSiteViewModel.SetAllowedPostableItems(twilightEnterSite);
                                        toEnterSiteViewModel.ModeComponentValue.CopyAsJSON(twilightEnterSite.modeComponentData);
                                    }
                                    UIHandler.Instance.HandleParallel(() =>
                                    {
                                        var toEnterSiteView = new SiteView
                                        {
                                            DataContext = toEnterSiteViewModel
                                        };
                                        toEnterSiteViewModel.View = toEnterSiteView;
                                        siteContainerViewModel.SiteViewCollection.Add(toEnterSiteView);
                                        siteContainerViewModel.SiteViewValue = toEnterSiteView;
                                    });
                                    break;
                                case Event.Types.EventID.CallBundle:
                                    var twilightCallBundle = Utility.GetJSON<JSON.TwilightCallBundle>(eventItemText);
                                    var targetAvatar = twilightCallBundle.targetAvatar;
                                    if (!twilightCallBundle.isSilent || (bundleViewModel.IsOpened && bundleViewModel.TargetBundleAvatar == targetAvatar))
                                    {
                                        var noteFilesBundleItems = new List<BundleItem>();
                                        var noteFileBundleItems = new List<BundleItem>();
                                        var valueUIBundleItems = new List<BundleItem>();
                                        var qwilightBundleItems = new List<BundleItem>();
                                        var eventNoteBundleItems = new List<BundleItem>();
                                        foreach (var bundleDataItem in twilightCallBundle.data)
                                        {
                                            ((BundleItem.BundleVariety)bundleDataItem.bundleVariety switch
                                            {
                                                BundleItem.BundleVariety.Note => noteFileBundleItems,
                                                BundleItem.BundleVariety.UI => valueUIBundleItems,
                                                BundleItem.BundleVariety.Qwilight => qwilightBundleItems,
                                                BundleItem.BundleVariety.EventNote => eventNoteBundleItems,
                                                _ => default
                                            })?.Add(new(new()
                                            {
                                                Data = bundleDataItem.bundleCompetence
                                            })
                                            {
                                                Avatar = targetAvatar,
                                                Date = DateTimeOffset.FromUnixTimeMilliseconds(bundleDataItem.date).LocalDateTime.ToLongDateString(),
                                                Name = bundleDataItem.bundleName,
                                                Length = bundleDataItem.bundleLength,
                                                Variety = (BundleItem.BundleVariety)bundleDataItem.bundleVariety
                                            });
                                        }
                                        var noteFileBundleItemCollection = bundleViewModel.NoteFileBundleItemCollection;
                                        var valueUIBundleItemCollection = bundleViewModel.UIBundleItemCollection;
                                        var qwilightBundleItemCollection = bundleViewModel.QwilightBundleItemCollection;
                                        var eventNoteBundleItemCollection = bundleViewModel.EventNoteBundleItemCollection;
                                        UIHandler.Instance.HandleParallel(() =>
                                        {
                                            noteFileBundleItemCollection.Clear();
                                            foreach (var noteFileBundleItem in noteFileBundleItems)
                                            {
                                                noteFileBundleItem.BundleItemCollection = noteFileBundleItemCollection;
                                                noteFileBundleItemCollection.Add(noteFileBundleItem);
                                            }
                                            valueUIBundleItemCollection.Clear();
                                            foreach (var valueUIBundleItem in valueUIBundleItems)
                                            {
                                                valueUIBundleItem.BundleItemCollection = valueUIBundleItemCollection;
                                                valueUIBundleItemCollection.Add(valueUIBundleItem);
                                            }
                                            qwilightBundleItemCollection.Clear();
                                            foreach (var qwilightBundleItem in qwilightBundleItems)
                                            {
                                                qwilightBundleItem.BundleItemCollection = qwilightBundleItemCollection;
                                                qwilightBundleItemCollection.Add(qwilightBundleItem);
                                            }
                                            eventNoteBundleItemCollection.Clear();
                                            foreach (var eventNoteBundleItem in eventNoteBundleItems)
                                            {
                                                eventNoteBundleItem.BundleItemCollection = eventNoteBundleItemCollection;
                                                eventNoteBundleItemCollection.Add(eventNoteBundleItem);
                                            }
                                        });
                                        bundleViewModel.TargetBundleAvatar = targetAvatar;
                                        bundleViewModel.TargetValue = twilightCallBundle.targetValue;
                                        bundleViewModel.BundleLength = twilightCallBundle.bundleLength;
                                        bundleViewModel.NotifyUI();
                                        bundleViewModel.Open();
                                    }
                                    break;
                                case Event.Types.EventID.CallConfigure:
                                    var twilightCallConfigure = Utility.GetJSON<JSON.TwilightCallConfigure>(eventItemText);
                                    twilightConfigureViewModel.SilentSiteCompetence = new()
                                    {
                                        Data = twilightCallConfigure.silentSiteCompetence
                                    };
                                    twilightConfigureViewModel.NotifyUbuntuCompetence = new()
                                    {
                                        Data = twilightCallConfigure.toNotifyUbuntuCompetence
                                    };
                                    twilightConfigureViewModel.DefaultBundleCompetence = new()
                                    {
                                        Data = twilightCallConfigure.defaultBundleCompetence
                                    };
                                    twilightConfigureViewModel.IOCompetence = new()
                                    {
                                        Data = twilightCallConfigure.ioCompetence
                                    };
                                    twilightConfigureViewModel.NotifySaveBundleCompetence = new()
                                    {
                                        Data = twilightCallConfigure.toNotifySaveBundle
                                    };
                                    twilightConfigureViewModel.Open();
                                    break;
                                case Event.Types.EventID.CallUbuntu:
                                    Utility.SetUICollection(ubuntuViewModel.UbuntuCollection, Utility.GetJSON<JSON.TwilightCallUbuntu[]>(eventItemText).Select(data => new UbuntuItem(data.ubuntuID)
                                    {
                                        AvatarName = data.ubuntuName,
                                        UbuntuSituationValue = (UbuntuItem.UbuntuSituation)data.situationValue,
                                        SituationText = data.situationText
                                    }).ToArray(), null, null, (valueItem, targetItem) =>
                                    {
                                        valueItem.AvatarName = targetItem.AvatarName;
                                        valueItem.UbuntuSituationValue = targetItem.UbuntuSituationValue;
                                        valueItem.SituationText = targetItem.SituationText;
                                    });
                                    break;
                                case Event.Types.EventID.CallSiteAvatar:
                                    var twilightCallSiteAvatar = Utility.GetJSON<JSON.TwilightCallSiteAvatar>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightCallSiteAvatar.siteID)?.SetSiteAvatar(twilightCallSiteAvatar);
                                    break;
                                case Event.Types.EventID.CallSiteNet:
                                    var twilightCallSiteNet = Utility.GetJSON<JSON.TwilightCallSiteNet>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightCallSiteNet.siteID)?.SetSiteNet(twilightCallSiteNet);
                                    break;
                                case Event.Types.EventID.CallSiteModeComponent:
                                    var twilightCallSiteModeComponent = Utility.GetJSON<JSON.TwilightCallSiteModeComponent>(eventItemText);
                                    ViewModels.Instance.GetSiteViewModel(twilightCallSiteModeComponent.siteID)?.SetSiteModeComponent(twilightCallSiteModeComponent);
                                    break;
                                case Event.Types.EventID.LevyNet:
                                    var twilightLevyNet = Utility.GetJSON<JSON.TwilightLevyNet>(eventItemText);
                                    var toLevyNetSiteViewModel = ViewModels.Instance.GetSiteViewModel(twilightLevyNet.siteID);
                                    if (toLevyNetSiteViewModel != null)
                                    {
                                        if (mainViewModel.NoteID512s.TryGetValue(toLevyNetSiteViewModel.ComputingValue.NoteID, out var toLevyNoteFile))
                                        {
                                            var isFavorModeComponent = twilightLevyNet.isFavorModeComponent;
                                            toLevyNetSiteViewModel.IsFavorModeComponent = isFavorModeComponent;
                                            toLevyNetSiteViewModel.IsFavorAudioMultiplier = twilightLevyNet.isFavorAudioMultiplier;
                                            var toLevyModeComponentValue = toLevyNetSiteViewModel.ModeComponentValue;
                                            toLevyModeComponentValue.ComputingValue = toLevyNoteFile;
                                            toLevyModeComponentValue.CopyAsJSON(twilightLevyNet.modeComponentData);

                                            mainViewModel.IsCommentMode = false;
                                            var modeComponentValue = mainViewModel.ModeComponentValue;
                                            var defaultModeComponentValue = modeComponentValue.Clone();
                                            var defaultMultiplierValue = modeComponentValue.MultiplierValue;
                                            modeComponentValue.ComputingValue = toLevyNoteFile;
                                            if (!isFavorModeComponent)
                                            {
                                                modeComponentValue.CopyAs(toLevyModeComponentValue);
                                            }
                                            modeComponentValue.CanModifyMultiplier = true;
                                            modeComponentValue.MultiplierValue = defaultMultiplierValue;
                                            modeComponentValue.CanModifyAudioMultiplier = toLevyNetSiteViewModel.IsFavorAudioMultiplier;
                                            if (modeComponentValue.CanModifyAudioMultiplier)
                                            {
                                                modeComponentValue.AudioMultiplier = mainViewModel.ModeComponentValue.AudioMultiplier;
                                            }
                                            else
                                            {
                                                modeComponentValue.AudioMultiplier = toLevyModeComponentValue.AudioMultiplier;
                                            }
                                            if (twilightLevyNet.validNetMode > 0)
                                            {
                                                modeComponentValue.AudioMultiplier = 1.0;
                                                modeComponentValue.FaintNoteModeValue = ModeComponent.FaintNoteMode.Default;
                                                modeComponentValue.JudgmentModeValue = ModeComponent.JudgmentMode.Default;
                                                modeComponentValue.HitPointsModeValue = ModeComponent.HitPointsMode.Default;
                                                modeComponentValue.NoteMobilityModeValue = ModeComponent.NoteMobilityMode.Default;
                                            }
                                            Utility.HandleUIAudio("Levy Note File");
                                            mainViewModel.SetComputingMode(new NetCompute([toLevyNoteFile], twilightLevyNet.validNetMode == 0 && isFavorModeComponent ? null : defaultModeComponentValue, AvatarID, GetAvatarName(), twilightLevyNet));
                                        }
                                        else
                                        {
                                            SendParallel(Event.Types.EventID.Compiled, new
                                            {
                                                siteID = toLevyNetSiteViewModel.SiteID,
                                                twilightLevyNet.handlerID,
                                                isCompiled = false
                                            });
                                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.HasNotNetNoteFile);
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.Compiled:
                                    if (defaultComputer is NetCompute && defaultComputer.HandlerID == eventItemText)
                                    {
                                        defaultComputer.HandleComputer();
                                        defaultComputer.WaitingTwilightLevel = DefaultCompute.WaitingTwilight.Default;
                                    }
                                    break;
                                case Event.Types.EventID.CallNet:
                                    var twilightCallNet = eventItem.TwilightCallNet;
                                    if ((defaultComputer is NetCompute || defaultComputer is IOCompute) && defaultComputer.HandlerID == twilightCallNet.HandlerID && defaultComputer.IsHandling)
                                    {
                                        lock (defaultComputer.IsTwilightNetItemsCSX)
                                        {
                                            if (defaultComputer.IsTwilightNetItems)
                                            {
                                                foreach (var netItem in defaultComputer.NetItems)
                                                {
                                                    foreach (var data in twilightCallNet.Data)
                                                    {
                                                        if (data.AvatarID == netItem.AvatarID)
                                                        {
                                                            if (data.IsFailed)
                                                            {
                                                                netItem.IsFailedStatus = 100.0;
                                                            }
                                                            netItem.TargetPosition = data.TargetPosition;
                                                            netItem.LastJudged = (Component.Judged)data.LastJudged;
                                                            netItem.AvatarNetStatus = data.AvatarNetStatus;
                                                            netItem.HitPointsModeValue = (ModeComponent.HitPointsMode)data.HitPointsMode;
                                                            netItem.SetValue(data.Stand, data.Band, data.Point, data.HitPoints, 1, data.Drawings, data.DrawingComponent);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var date = DateTime.Now;
                                                defaultComputer.IsTwilightNetItems = true;
                                                defaultComputer.LevyingNetPosition = 0;
                                                defaultComputer.QuitNetPosition = defaultComputer.SetNetItems(twilightCallNet.Data.Select((data, i) =>
                                                {
                                                    var avatarID = data.AvatarID;
                                                    return new NetItem(avatarID, data.AvatarName, date, data.Stand, data.Band, data.Point, data.HitPoints)
                                                    {
                                                        TargetPosition = i,
                                                        AvatarNetStatus = data.AvatarNetStatus,
                                                        IsMyNetItem = avatarID == defaultComputer.AvatarID,
                                                        IsFailedStatus = data.IsFailed ? 100.0 : 0.0,
                                                        LastJudged = (Component.Judged)data.LastJudged,
                                                        QuitValue = Utility.GetQuitStatusValue(data.Point, data.Stand, data.HitPoints, 1),
                                                        HitPointsModeValue = (ModeComponent.HitPointsMode)data.HitPointsMode
                                                    };
                                                }).ToList()) - 1;
                                            }
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.QuitNet:
                                    var twilightQuitNet = Utility.GetJSON<JSON.TwilightQuitNet>(eventItemText);
                                    if (defaultComputer is NetCompute && defaultComputer.HandlerID == twilightQuitNet.handlerID)
                                    {
                                        if (twilightQuitNet.quitNetItems != null)
                                        {
                                            defaultComputer.PendingQuitNetItems = twilightQuitNet.quitNetItems;
                                            defaultComputer.PendingQuitNetComments = eventItemData.Select(data => Comment.Parser.ParseFrom(data)).ToArray();
                                            defaultComputer.SetQuitMode();
                                        }
                                        else
                                        {
                                            defaultComputer.SetNoteFileMode();
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.CallNetSiteComments:
                                    var twilightCallNetSiteComments = Utility.GetJSON<JSON.TwilightCallNetSiteComments[]>(eventItemText);
                                    if (twilightCallNetSiteComments != null)
                                    {
                                        var netSIteCommentItemCollection = new Dictionary<long, NetSIteCommentItems>();
                                        foreach (var twilightCallNetSiteComment in twilightCallNetSiteComments)
                                        {
                                            var date = twilightCallNetSiteComment.date;
                                            if (!netSIteCommentItemCollection.TryGetValue(date, out var netSiteCommentItems))
                                            {
                                                netSiteCommentItems = new()
                                                {
                                                    NetSiteCommentItems = new(),
                                                    Date = date
                                                };
                                            }
                                            foreach (var toCallNetSiteCommentItem in twilightCallNetSiteComment.data)
                                            {
                                                netSiteCommentItems.NetSiteCommentItems.Add(new(toCallNetSiteCommentItem));
                                            }
                                            netSIteCommentItemCollection[date] = netSiteCommentItems;
                                        }
                                        var netSiteCommentItemsCollection = netSiteCommentViewModel.NetSiteCommentItemsCollection;
                                        UIHandler.Instance.HandleParallel(() =>
                                        {
                                            netSiteCommentItemsCollection.Clear();
                                            foreach (var date in netSIteCommentItemCollection.Values)
                                            {
                                                netSiteCommentItemsCollection.Add(date);
                                            }
                                            netSiteCommentViewModel.NetSiteCommentItemsValue = netSiteCommentItemsCollection.LastOrDefault();
                                        });
                                        netSiteCommentViewModel.Open();
                                    }
                                    break;
                                case Event.Types.EventID.SaveAsBundle:
                                    var twilightSaveAsBundle = Utility.GetJSON<JSON.TwilightSaveAsBundle>(eventItemText);
                                    var bundleEntryPath = twilightSaveAsBundle.bundleEntryPath;
                                    var saveAsBundleID = twilightSaveAsBundle.bundleID;
                                    var saveAsBundleVariety = (BundleItem.BundleVariety)twilightSaveAsBundle.bundleVariety;
                                    var savingBundleItem = new NotifyItem
                                    {
                                        Text = LanguageSystem.Instance.SaveAsBundleContents,
                                        Variety = NotifySystem.NotifyVariety.Levying,
                                        Data = ArrayPool<byte>.Shared.Rent(QwilightComponent.SendUnit),
                                        OnStop = wipeTotal =>
                                        {
                                            if (wipeTotal)
                                            {
                                                return !_savingBundleItems.ContainsKey(saveAsBundleID);
                                            }
                                            else
                                            {
                                                if (_savingBundleItems.Remove(saveAsBundleID, out var savingBundleItem))
                                                {
                                                    savingBundleItem.Variety = NotifySystem.NotifyVariety.Stopped;
                                                    savingBundleItem.Text = LanguageSystem.Instance.StopSavingAsBundleContents;
                                                    savingBundleItem.IsStopped = true;
                                                    SendParallel(Event.Types.EventID.StopSavingAsBundle, saveAsBundleID);
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    };
                                    _savingBundleItems[saveAsBundleID] = savingBundleItem;
                                    UIHandler.Instance.HandleParallel(() => toNotifyViewModel.NotifyItemCollection.Insert(0, savingBundleItem));
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            using var rms = PoolSystem.Instance.GetDataFlow();
                                            using (var zipFile = new ZipFile
                                            {
                                                AlternateEncoding = Encoding.UTF8,
                                                AlternateEncodingUsage = ZipOption.Always,
                                            })
                                            {
                                                switch (saveAsBundleVariety)
                                                {
                                                    case BundleItem.BundleVariety.Net:
                                                        zipFile.AddDirectory(bundleEntryPath);
                                                        zipFile.SaveProgress += OnSavingZipFile;
                                                        zipFile.Save(rms);
                                                        break;
                                                    case BundleItem.BundleVariety.UI:
                                                        if (Path.GetFileName(twilightSaveAsBundle.bundleName).StartsWith('@'))
                                                        {
                                                            BaseUI.ZipUIFile(zipFile, new()
                                                            {
                                                                UIEntry = Path.GetDirectoryName(bundleEntryPath),
                                                                YamlName = Path.GetFileNameWithoutExtension(bundleEntryPath)
                                                            }, string.Empty);
                                                        }
                                                        else
                                                        {
                                                            UI.ZipUIFile(zipFile, new()
                                                            {
                                                                UIEntry = Path.GetDirectoryName(bundleEntryPath),
                                                                YamlName = Path.GetFileNameWithoutExtension(bundleEntryPath)
                                                            }, string.Empty);
                                                        }
                                                        zipFile.SaveProgress += OnSavingZipFile;
                                                        zipFile.Save(rms);
                                                        break;
                                                    case BundleItem.BundleVariety.Qwilight:
                                                        Configure.Instance.Save(false);
                                                        GPUConfigure.Instance.Save(false);
                                                        DB.Instance.Save();
                                                        if (Directory.Exists(QwilightComponent.CommentEntryPath))
                                                        {
                                                            zipFile.AddDirectory(QwilightComponent.CommentEntryPath, "Comment");
                                                        }
                                                        var levelFilePath = Path.Combine(LevelSystem.EntryPath, $"{Configure.Instance.WantLevelName}.json");
                                                        if (File.Exists(levelFilePath))
                                                        {
                                                            zipFile.AddFile(levelFilePath, "Level");
                                                        }
                                                        levelFilePath = Path.Combine(LevelSystem.EntryPath, $"#{Configure.Instance.WantLevelName}.json");
                                                        if (File.Exists(levelFilePath))
                                                        {
                                                            zipFile.AddFile(levelFilePath, "Level");
                                                        }
                                                        if (Configure.Instance.BaseUIItemValue.UIEntry != "@Default" || Configure.Instance.BaseUIItemValue.YamlName != "@Default")
                                                        {
                                                            BaseUI.ZipUIFile(zipFile, Configure.Instance.BaseUIItemValue, "UI");
                                                        }
                                                        if (Configure.Instance.UIItemValue.UIEntry != "Default" || Configure.Instance.UIItemValue.YamlName != "Default")
                                                        {
                                                            UI.ZipUIFile(zipFile, Configure.Instance.UIItemValue, "UI");
                                                        }
                                                        zipFile.AddFile(Path.Combine(QwilightComponent.QwilightEntryPath, "Configure.json"), string.Empty);
                                                        zipFile.AddFile(Path.Combine(QwilightComponent.QwilightEntryPath, "GPU Configure.json"), string.Empty);
                                                        zipFile.AddFile(Path.Combine(QwilightComponent.QwilightEntryPath, "DB.db"), string.Empty);
                                                        zipFile.SaveProgress += OnSavingZipFile;
                                                        zipFile.Save(rms);
                                                        break;
                                                    case BundleItem.BundleVariety.EventNote:
                                                        var eventNoteData = Encoding.UTF8.GetBytes(bundleEntryPath);
                                                        rms.Write(eventNoteData, 0, eventNoteData.Length);
                                                        break;
                                                    case BundleItem.BundleVariety.Note:
                                                        twilightSaveAsBundle.bundleEntryPath = Path.GetDirectoryName(bundleEntryPath)!;
                                                        zipFile.AddFile(bundleEntryPath, string.Empty);
                                                        zipFile.SaveProgress += OnSavingZipFile;
                                                        zipFile.Save(rms);
                                                        break;
                                                }
                                            }

                                            if (!savingBundleItem.IsStopped)
                                            {
                                                savingBundleItem.Text = LanguageSystem.Instance.SavingAsBundleContents;
                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                savingBundleItem.MaxStatus = rms.Length;
                                                rms.Position = 0;
                                                while (_savingBundleItems.TryGetValue(saveAsBundleID, out var savingBundleItem))
                                                {
                                                    var length = (int)Math.Min(savingBundleItem.Data.Length, rms.Length - rms.Position);
                                                    rms.Read(savingBundleItem.Data, 0, length);
                                                    savingBundleItem.Status += length;
                                                    if (length < savingBundleItem.Data.Length)
                                                    {
                                                        Send(Event.Types.EventID.SavedAsBundle, saveAsBundleID, UnsafeByteOperations.UnsafeWrap(savingBundleItem.Data.AsMemory(0, length)));
                                                        if (_savingBundleItems.Remove(saveAsBundleID, out _))
                                                        {
                                                            savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                            savingBundleItem.Text = LanguageSystem.Instance.SavedAsBundleContents;
                                                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                        }
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        Send(Event.Types.EventID.SavingAsBundle, saveAsBundleID, UnsafeByteOperations.UnsafeWrap(savingBundleItem.Data));
                                                    }
                                                }
                                            }

                                            void OnSavingZipFile(object sender, SaveProgressEventArgs e)
                                            {
                                                if (savingBundleItem.IsStopped)
                                                {
                                                    e.Cancel = true;
                                                    savingBundleItem.Dispose();
                                                }
                                                else
                                                {
                                                    if (e.EntriesTotal > 0)
                                                    {
                                                        savingBundleItem.Status = e.EntriesSaved;
                                                        savingBundleItem.MaxStatus = e.EntriesTotal;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            savingBundleItem.OnStop(false);
                                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SaveAsBundleFault, e.Message));
                                        }
                                        finally
                                        {
                                            savingBundleItem.Dispose();
                                        }
                                    });
                                    break;
                                case Event.Types.EventID.StopSavingAsBundle:
                                    if (_savingBundleItems.TryGetValue(eventItemText, out savingBundleItem))
                                    {
                                        savingBundleItem.Variety = NotifySystem.NotifyVariety.Stopped;
                                        savingBundleItem.Text = LanguageSystem.Instance.StopSavingAsBundleContents;
                                    }
                                    break;
                                case Event.Types.EventID.SaveBundle:
                                    var twilightSaveBundle = Utility.GetJSON<JSON.TwilightSaveBundle>(eventItemText);
                                    var saveBundleID = twilightSaveBundle.bundleID;
                                    var saveBundleVariety = (BundleItem.BundleVariety)twilightSaveBundle.bundleVariety;
                                    var bundleLength = twilightSaveBundle.bundleLength;
                                    savingBundleItem = new NotifyItem
                                    {
                                        Text = LanguageSystem.Instance.SaveBundleContents,
                                        Variety = NotifySystem.NotifyVariety.Levying,
                                        DataFlow = PoolSystem.Instance.GetDataFlow(bundleLength),
                                        OnStop = wipeTotal =>
                                        {
                                            if (wipeTotal)
                                            {
                                                return !_savingBundleItems.ContainsKey(saveBundleID);
                                            }
                                            else
                                            {
                                                if (_savingBundleItems.Remove(saveBundleID, out var savingBundleItem))
                                                {
                                                    savingBundleItem.Variety = NotifySystem.NotifyVariety.Stopped;
                                                    savingBundleItem.Text = LanguageSystem.Instance.StopSavingBundleContents;
                                                    savingBundleItem.IsStopped = true;
                                                    SendParallel(Event.Types.EventID.StopSavingBundle, saveBundleID);
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        },
                                        MaxStatus = bundleLength
                                    };
                                    _savingBundleItems[saveBundleID] = savingBundleItem;
                                    UIHandler.Instance.HandleParallel(() => toNotifyViewModel.NotifyItemCollection.Insert(0, savingBundleItem));
                                    if (saveBundleVariety != BundleItem.BundleVariety.DefaultNotes && saveBundleVariety != BundleItem.BundleVariety.DefaultUI)
                                    {
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                    }
                                    SendParallel(Event.Types.EventID.SavingBundle, saveBundleID);
                                    break;
                                case Event.Types.EventID.SavingBundle:
                                    if (_savingBundleItems.TryGetValue(eventItemText, out savingBundleItem))
                                    {
                                        if (savingBundleItem.IsStopped)
                                        {
                                            savingBundleItem.Dispose();
                                        }
                                        else
                                        {
                                            eventItemData[0].WriteTo(savingBundleItem.DataFlow);
                                        }
                                        savingBundleItem.Status += eventItemData[0].Length;
                                    }
                                    break;
                                case Event.Types.EventID.SavedBundle:
                                    var twilightSavedBundle = Utility.GetJSON<JSON.TwilightSavedBundle>(eventItemText);
                                    if (_savingBundleItems.Remove(twilightSavedBundle.bundleID, out savingBundleItem))
                                    {
                                        if (savingBundleItem.IsStopped)
                                        {
                                            savingBundleItem.Dispose();
                                        }
                                        else
                                        {
                                            eventItemData[0].WriteTo(savingBundleItem.DataFlow);
                                            Utility.HandleParallelly(() =>
                                            {
                                                var dataFlow = savingBundleItem.DataFlow;
                                                dataFlow.Position = 0;
                                                var bundleVariety = (BundleItem.BundleVariety)twilightSavedBundle.bundleVariety;
                                                var isNotDefaultBundle = bundleVariety != BundleItem.BundleVariety.DefaultNotes && bundleVariety != BundleItem.BundleVariety.DefaultUI;
                                                try
                                                {
                                                    switch (bundleVariety)
                                                    {
                                                        case BundleItem.BundleVariety.DefaultNotes:
                                                        case BundleItem.BundleVariety.Net:
                                                            var bundleEntryPath = Path.Combine(QwilightComponent.BundleEntryPath, twilightSavedBundle.bundleName);
                                                            using (var zipFile = ZipFile.Read(dataFlow))
                                                            {
                                                                savingBundleItem.Text = LanguageSystem.Instance.SavingBundleContents;
                                                                if (isNotDefaultBundle)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                }
                                                                zipFile.ExtractProgress += OnSavingZipFile;
                                                                zipFile.ExtractAll(bundleEntryPath, ExtractExistingFileAction.OverwriteSilently);
                                                            }
                                                            if (!savingBundleItem.IsStopped)
                                                            {
                                                                mainViewModel.LoadEntryItem(DefaultEntryItem.EssentialBundle, bundleEntryPath, isNotDefaultBundle);
                                                                savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                                savingBundleItem.Text = LanguageSystem.Instance.SavedBundleContents;
                                                                if (isNotDefaultBundle)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                }
                                                                else if (twilightSavedBundle.isLastDefault)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, LanguageSystem.Instance.SavedDefaultNoteContents);
                                                                }
                                                            }
                                                            break;
                                                        case BundleItem.BundleVariety.DefaultUI:
                                                        case BundleItem.BundleVariety.UI:
                                                            using (var zipFile = ZipFile.Read(dataFlow))
                                                            {
                                                                savingBundleItem.Text = LanguageSystem.Instance.SavingBundleContents;
                                                                if (isNotDefaultBundle)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                }
                                                                zipFile.ExtractProgress += OnSavingZipFile;
                                                                zipFile.ExtractAll(string.IsNullOrEmpty(Path.GetDirectoryName(zipFile.Where(zipEntry => zipEntry.FileName.IsTailCaselsss(".yaml")).OrderBy(zipEntry => zipEntry.FileName).FirstOrDefault().FileName)) ? Path.Combine(QwilightComponent.UIEntryPath, twilightSavedBundle.bundleName) : QwilightComponent.UIEntryPath, ExtractExistingFileAction.OverwriteSilently);
                                                            }
                                                            if (!savingBundleItem.IsStopped)
                                                            {
                                                                savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                                savingBundleItem.Text = LanguageSystem.Instance.SavedBundleContents;
                                                                if (isNotDefaultBundle)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                }
                                                                else if (twilightSavedBundle.isLastDefault)
                                                                {
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, LanguageSystem.Instance.SavedDefaultUIContents);
                                                                }
                                                            }
                                                            break;
                                                        case BundleItem.BundleVariety.Qwilight:
                                                            using (var fs = File.OpenWrite(Path.Combine(QwilightComponent.QwilightEntryPath, "Qwilight.zip")))
                                                            {
                                                                dataFlow.CopyTo(fs);
                                                            }
                                                            savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                            savingBundleItem.Text = LanguageSystem.Instance.SavedQwilightBundleContents;
                                                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                            break;
                                                        case BundleItem.BundleVariety.EventNote:
                                                            try
                                                            {
                                                                var eventNoteID = Encoding.UTF8.GetString(dataFlow.ToArray()).Split('.')[0];
                                                                var eventNoteName = Path.GetFileNameWithoutExtension(twilightSavedBundle.bundleName);
                                                                var eventNoteVariety = twilightSavedBundle.etc switch
                                                                {
                                                                    "MD5" => DB.EventNoteVariety.MD5,
                                                                    "Qwilight" => DB.EventNoteVariety.Qwilight,
                                                                    _ => default
                                                                };
                                                                var date = DateTime.Now;
                                                                DB.Instance.SetEventNote(eventNoteID, eventNoteName, date, eventNoteVariety);
                                                                savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                                savingBundleItem.Text = LanguageSystem.Instance.SavedBundleContents;
                                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                mainViewModel.LoadEventNoteEntryItems();
                                                                mainViewModel.Want();
                                                            }
                                                            catch (SQLiteException)
                                                            {
                                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.BeforeEventNoteContents);
                                                            }
                                                            break;
                                                        case BundleItem.BundleVariety.Note:
                                                            var noteID = twilightSavedBundle.etc.Split('/').FirstOrDefault(noteID512s => mainViewModel.NoteID512s.ContainsKey(noteID512s));
                                                            if (noteID != null && mainViewModel.NoteID512s.TryGetValue(noteID, out var noteFile))
                                                            {
                                                                using (var zipFile = ZipFile.Read(dataFlow))
                                                                {
                                                                    savingBundleItem.Text = LanguageSystem.Instance.SavingBundleContents;
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                    zipFile.ExtractProgress += OnSavingZipFile;
                                                                    zipFile.ExtractAll(noteFile.EntryItem.EntryPath, ExtractExistingFileAction.OverwriteSilently);
                                                                }
                                                                if (!savingBundleItem.IsStopped)
                                                                {
                                                                    mainViewModel.LoadEntryItem(noteFile.DefaultEntryItem, noteFile.EntryItem.EntryPath);
                                                                    savingBundleItem.Variety = NotifySystem.NotifyVariety.Quit;
                                                                    savingBundleItem.Text = LanguageSystem.Instance.SavedBundleContents;
                                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                savingBundleItem.Text = LanguageSystem.Instance.NotHaveNoteFileEntryText;
                                                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.NotSave, savingBundleItem.Text);
                                                            }
                                                            break;
                                                    }

                                                    void OnSavingZipFile(object sender, ExtractProgressEventArgs e)
                                                    {
                                                        if (savingBundleItem.IsStopped)
                                                        {
                                                            e.Cancel = true;
                                                        }
                                                        else
                                                        {
                                                            if (e.EntriesTotal > 0)
                                                            {
                                                                savingBundleItem.Status = e.EntriesExtracted;
                                                                savingBundleItem.MaxStatus = e.EntriesTotal;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    savingBundleItem.OnStop(false);
                                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SaveBundleFault, e.Message));
                                                }
                                                finally
                                                {
                                                    savingBundleItem.Dispose();
                                                }
                                            }, false);
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.PostFile:
                                    var toPostFileSiteViewModel = ViewModels.Instance.SiteContainerValue.SiteValue;
                                    if (toPostFileSiteViewModel != null)
                                    {
                                        toPostFileSiteViewModel.Input = eventItemText;
                                    }
                                    break;
                                case Event.Types.EventID.CallIo:
                                    var twilightCallIO = Utility.GetJSON<JSON.TwilightCallIO>(eventItemText);
                                    if (defaultComputer?.CanIO == true && defaultComputer.IsHandling)
                                    {
                                        SendParallel(Event.Types.EventID.CallIoComponent, new
                                        {
                                            noteID = defaultComputer.NoteFile.GetNoteID512(),
                                            handlerID = defaultComputer.HandlerID,
                                            twilightCallIO.avatarID,
                                            data = defaultComputer.ModeComponentValue.GetJSON(),
                                            ioHandlerID = twilightCallIO.handlerID,
                                            isFailMode = defaultComputer.IsFailMode,
                                            ioMillis = twilightCallIO.ioMillis,
                                            targetIOMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                        });
                                    }
                                    else
                                    {
                                        SendParallel(Event.Types.EventID.IoNot, new
                                        {
                                            avatarID = twilightCallIO.avatarID,
                                            handlerID = twilightCallIO.handlerID
                                        });
                                    }
                                    break;
                                case Event.Types.EventID.IoNot:
                                    if (string.IsNullOrEmpty(eventItemText))
                                    {
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.CannotIOContents);
                                    }
                                    break;
                                case Event.Types.EventID.CallIoComponent:
                                    var twilightCallIOComponent = Utility.GetJSON<JSON.TwilightCallIOComponent>(eventItemText);
                                    if (mainViewModel.NoteID512s.TryGetValue(twilightCallIOComponent.noteID, out var ioNoteFile))
                                    {
                                        mainViewModel.IsCommentMode = false;
                                        var modeComponentValue = mainViewModel.ModeComponentValue;
                                        var defaultModeComponentValue = modeComponentValue.Clone();
                                        modeComponentValue.ComputingValue = ioNoteFile;
                                        modeComponentValue.CopyAsJSON(twilightCallIOComponent.data);
                                        modeComponentValue.CanModifyMultiplier = false;
                                        modeComponentValue.CanModifyAudioMultiplier = false;
                                        mainViewModel.SetComputingMode(new IOCompute([ioNoteFile], defaultModeComponentValue, twilightCallIOComponent));
                                    }
                                    else
                                    {
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.HasNotIONoteFile);
                                        if (!string.IsNullOrEmpty(twilightCallIOComponent.ioHandlerID))
                                        {
                                            mainViewModel.Computer.SetNoteFileMode();
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.CompiledIo:
                                    var twilightCompiledIO = Utility.GetJSON<JSON.TwilightCompiledIO>(eventItemText);
                                    if (defaultComputer.HandlerID == twilightCompiledIO.handlerID)
                                    {
                                        if (defaultComputer.CanIO && defaultComputer.IsHandling)
                                        {
                                            defaultComputer.TwilightCompiledIOQueue.Enqueue(twilightCompiledIO);
                                        }
                                        else
                                        {
                                            SendParallel(Event.Types.EventID.IoQuit, new
                                            {
                                                twilightCompiledIO.handlerID,
                                                avatarIDs = new[] { twilightCompiledIO.avatarID },
                                                isBanned = true
                                            });
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.LevyIo:
                                    var twilightLevyIO = Utility.GetJSON<JSON.TwilightLevyIO>(eventItemText);
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightLevyIO.handlerID)
                                    {
                                        var comment = Comment.Parser.ParseFrom(eventItemData[0]);
                                        defaultComputer.IOLazyInit = new(() =>
                                        {
                                            defaultComputer.LastStand = twilightLevyIO.lastStand;
                                            defaultComputer.IsF.SetValue(twilightLevyIO.isF);
                                            var sentMultiplier = twilightLevyIO.multiplier;
                                            defaultComputer.ModeComponentValue.SentMultiplier = sentMultiplier;
                                            defaultComputer.ModeComponentValue.MultiplierValue = defaultComputer.ModeComponentValue.BPM * defaultComputer.ModeComponentValue.AudioMultiplier * sentMultiplier;
                                            defaultComputer.ModeComponentValue.AudioMultiplier = twilightLevyIO.audioMultiplier;
                                        });
                                        defaultComputer.IOMillis += twilightLevyIO.ioMillis;
                                        defaultComputer.LevyingWait = twilightLevyIO.levyingWait;
                                        defaultComputer.WaitingTwilightLevel = DefaultCompute.WaitingTwilight.Default;
                                        defaultComputer.EventComment = comment;
                                        if (defaultComputer.IsHandling)
                                        {
                                            defaultComputer.SetUndo = true;
                                        }
                                        else
                                        {
                                            defaultComputer.HandleComputer();
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoInput:
                                    var twilightIOInput = eventItem.TwilightIOInput;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOInput.HandlerID)
                                    {
                                        var twilightIOInputQueue = defaultComputer.TwilightIOInputQueue;
                                        lock (twilightIOInputQueue)
                                        {
                                            twilightIOInputQueue.Enqueue(Elapsable<Event.Types.TwilightIOInput>.GetElapsable(twilightIOInput));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoJudge:
                                    var twilightIOJudge = eventItem.TwilightIOJudge;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOJudge.HandlerID)
                                    {
                                        var twilightIOJudgeQueue = defaultComputer.TwilightIOJudgeQueue;
                                        lock (twilightIOJudgeQueue)
                                        {
                                            twilightIOJudgeQueue.Enqueue(Elapsable<Event.Types.TwilightIOJudge>.GetElapsable(twilightIOJudge));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoNoteVisibility:
                                    var twilightIONoteVisibility = eventItem.TwilightIONoteVisibility;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIONoteVisibility.HandlerID)
                                    {
                                        var twilightIONoteVisibilityQueue = defaultComputer.TwilightIONoteVisibilityQueue;
                                        lock (twilightIONoteVisibilityQueue)
                                        {
                                            twilightIONoteVisibilityQueue.Enqueue(Elapsable<Event.Types.TwilightIONoteVisibility>.GetElapsable(twilightIONoteVisibility));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoJudgmentMeter:
                                    var twilightIOJudgmentMeter = eventItem.TwilightIOJudgmentMeter;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOJudgmentMeter.HandlerID)
                                    {
                                        var twilightIOJudgmentMeterQueue = defaultComputer.TwilightIOJudgmentMeterQueue;
                                        lock (twilightIOJudgmentMeterQueue)
                                        {
                                            twilightIOJudgmentMeterQueue.Enqueue(Elapsable<Event.Types.TwilightIOJudgmentMeter>.GetElapsable(twilightIOJudgmentMeter));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoMultiplier:
                                    var twilightIOMultiplier = eventItem.TwilightIOMultiplier;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOMultiplier.HandlerID)
                                    {
                                        var twilightIOMultiplierQueue = defaultComputer.TwilightIOMultiplierQueue;
                                        lock (twilightIOMultiplierQueue)
                                        {
                                            twilightIOMultiplierQueue.Enqueue(Elapsable<Event.Types.TwilightIOMultiplier>.GetElapsable(twilightIOMultiplier));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoAudioMultiplier:
                                    var twilightIOAudioMultiplier = eventItem.TwilightIOAudioMultiplier;
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOAudioMultiplier.HandlerID)
                                    {
                                        var twilightAudioMultiplierQueue = defaultComputer.TwilightIOAudioMultiplierQueue;
                                        lock (twilightAudioMultiplierQueue)
                                        {
                                            twilightAudioMultiplierQueue.Enqueue(Elapsable<Event.Types.TwilightIOAudioMultiplier>.GetElapsable(twilightIOAudioMultiplier));
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.IoPause:
                                    var twilightIOPause = Utility.GetJSON<JSON.TwilightIOPause>(eventItemText);
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOPause.handlerID)
                                    {
                                        defaultComputer.SetPause = twilightIOPause.isPaused;
                                    }
                                    break;
                                case Event.Types.EventID.IoQuit:
                                    var twilightIOQuit = Utility.GetJSON<JSON.TwilightIOQuit>(eventItemText);
                                    if (defaultComputer is IOCompute && defaultComputer.HandlerID == twilightIOQuit.handlerID)
                                    {
                                        if (string.IsNullOrEmpty(twilightIOQuit.avatarID))
                                        {
                                            defaultComputer.SetNoteFileMode();
                                        }
                                        else
                                        {
                                            defaultComputer.IOLazyInit = new(() =>
                                            {
                                                defaultComputer.SetPause = true;
                                            });
                                            defaultComputer.LevyingWait = double.NaN;
                                            defaultComputer.SetUndo = true;
                                            defaultComputer.WaitingTwilightLevel = DefaultCompute.WaitingTwilight.CallIO;
                                        }
                                    }
                                    break;
                                case Event.Types.EventID.AudioInput:
                                    var twilightAudioInput = eventItem.TwilightAudioInput;
                                    var audioInputSiteViewModel = ViewModels.Instance.GetSiteViewModel(twilightAudioInput.SiteID);
                                    if (audioInputSiteViewModel?.IsAudioInput == true)
                                    {
                                        AudioInputSystem.Instance.Handle(twilightAudioInput.AvatarID, eventItemData[0].ToByteArray(), eventItemData[0].Length);
                                    }
                                    break;
                                case Event.Types.EventID.InvalidateAvatarDrawing:
                                    AvatarDrawingSystem.Instance.WipeAvatarDrawing(eventItemText);
                                    break;
                                case Event.Types.EventID.InvalidateAvatarTitle:
                                    AvatarTitleSystem.Instance.WipeAvatarTitle(eventItemText);
                                    break;
                                case Event.Types.EventID.InvalidateAvatarEdge:
                                    AvatarEdgeSystem.Instance.WipeAvatarEdge(eventItemText);
                                    break;
                                case Event.Types.EventID.AlreadyLoadingBundle:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.AlreadyLoadingBundle);
                                    ViewModels.Instance.NotifyValue.Open();
                                    break;
                                case Event.Types.EventID.PostItem:
                                    var twilightPostItem = eventItem.TwilightPostItem;
                                    if (defaultComputer is NetCompute && defaultComputer.IsPostableItemMode && defaultComputer.HandlerID == twilightPostItem.HandlerID)
                                    {
                                        defaultComputer.TwilightPostItemQueue.Enqueue(twilightPostItem);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (AuthenticationException e)
                {
                    if (!wasMITM)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.MITMFaultText, e.Message));
                        wasMITM = true;
                    }
                }
                catch (SocketException)
                {
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    if (_isAvailable)
                    {
                        if (textClose == null)
                        {
                            var (faultFilePath, _) = Utility.SaveFaultFile(FaultEntryPath, e);
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, e.Message, false, null, () => Utility.OpenAs(faultFilePath));
                        }
                        else if (textClose.Length > 0)
                        {
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, string.Format($"{LanguageSystem.Instance.TwilightCloseContents} ({{0}})", textClose), false);
                        }
                        else
                        {
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.TwilightCloseContents, false);
                        }
                    }
                }
                finally
                {
                    if (IsEstablished)
                    {
                        ViewModels.Instance.WipeSiteViewModels();
                        UIHandler.Instance.HandleParallel(siteContainerViewModel.SiteViewCollection.Clear);
                        foreach (var bundleID in _savingBundleItems.Keys)
                        {
                            if (_savingBundleItems.Remove(bundleID, out var savingBundleItem))
                            {
                                savingBundleItem.OnStop(false);
                            }
                        }
                        Totem = string.Empty;
                        AvatarID = string.Empty;
                        AvatarName = string.Empty;
                        TwilightSituation = NotEstablished;
                    }
                }
            }
        }

        public void Stop()
        {
            _ss?.Dispose();
        }

        public void Dispose()
        {
            _isAvailable = false;
            Stop();
        }

        public async ValueTask<Stream> GetWwwParallel(string target)
        {
            if (!target.IsFrontCaselsss(QwilightComponent.QwilightAPI) || IsEstablished)
            {
                try
                {
                    var dataGet = new HttpRequestMessage(HttpMethod.Get, target);
                    var www = await _wwwClient.SendAsync(dataGet).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                    return await www.Content.ReadAsStreamAsync().ConfigureAwait(false);
                }
                catch
                {
                }
            }

            return Stream.Null;
        }

        public async ValueTask<T> GetWwwParallel<T>(string target)
        {
            if (target.EqualsCaseless($"{QwilightComponent.TaehuiNetFE}/qwilight/qwilight.json") || !target.IsFrontCaselsss(QwilightComponent.TaehuiNetFE) || IsEstablished)
            {
                try
                {
                    var dataGet = new HttpRequestMessage(HttpMethod.Get, target);
                    using var www = await _wwwClient.SendAsync(dataGet).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                    var text = await www.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(text))
                    {
                        return Utility.GetJSON<T>(text);
                    }
                }
                catch
                {
                }
            }

            return default(T);
        }

        public async Task<bool> PostWwwParallel(string target, string data, string dataVariety = "text/plain")
        {
            if (IsEstablished)
            {
                try
                {
                    var dataPost = new HttpRequestMessage(HttpMethod.Post, target)
                    {
                        Content = new StringContent(data, Encoding.UTF8, dataVariety)
                    };
                    dataPost.Headers.Add("millis", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                    using var www = await _wwwClient.SendAsync(dataPost).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public async ValueTask<bool> PostAvatarDrawingParallel(string target, string fileName)
        {
            if (IsEstablished)
            {
                try
                {
                    var dataContents = new ByteArrayContent(await File.ReadAllBytesAsync(fileName).ConfigureAwait(false));
                    dataContents.Headers.ContentDisposition = new("form-data")
                    {
                        Name = "data",
                        FileName = Path.GetFileName(fileName)
                    };
                    dataContents.Headers.ContentType = new("image/png");
                    var dataPost = new HttpRequestMessage(HttpMethod.Post, target)
                    {
                        Content = new MultipartFormDataContent
                        {
                            dataContents
                        }
                    };
                    dataPost.Headers.Add("millis", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                    dataPost.Headers.Add("totem", Totem);
                    using var www = await _wwwClient.SendAsync(dataPost).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public async Task<string> PostWwwParallel(string target, byte[] data)
        {
            if (IsEstablished)
            {
                try
                {
                    var dataPost = new HttpRequestMessage(HttpMethod.Post, target)
                    {
                        Content = new ByteArrayContent(data)
                    };
                    dataPost.Headers.Add("millis", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                    using var www = await _wwwClient.SendAsync(dataPost).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                    return await www.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        public async Task PutAvatarParallel(string target, string avatarIntro)
        {
            if (!target.IsFrontCaselsss(QwilightComponent.TaehuiNetFE) || IsEstablished)
            {
                try
                {
                    var dataPut = new HttpRequestMessage(HttpMethod.Put, target)
                    {
                        Content = new StringContent(avatarIntro)
                    };
                    dataPut.Headers.Add("millis", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                    dataPut.Headers.Add("totem", Totem);
                    using var www = await _wwwClient.SendAsync(dataPut).ConfigureAwait(false);
                    www.EnsureSuccessStatusCode();
                }
                catch
                {
                }
            }
        }

        public async Task GetDefaultNoteDate(long defaultNoteDate, bool isSilent)
        {
            var twilightWwwDefaultDate = await GetWwwParallel<JSON.TwilightWwwDefaultDate?>($"{QwilightComponent.QwilightAPI}/defaultNoteDate?date={defaultNoteDate}").ConfigureAwait(false);
            if (twilightWwwDefaultDate.HasValue)
            {
                var date = twilightWwwDefaultDate.Value.date;
                Configure.Instance.DefaultNoteDate = date;
                if (isSilent)
                {
                    GetDefaultNote();
                }
                else
                {
                    if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                    {
                        Text = string.Format(LanguageSystem.Instance.SaveDefaultNoteNotify, DateTimeOffset.FromUnixTimeMilliseconds(date).LocalDateTime.ToLongDateString()),
                        Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
                    }) == MESSAGEBOX_RESULT.IDYES)
                    {
                        GetDefaultNote();
                    }
                }

                void GetDefaultNote()
                {
                    SendParallel(Event.Types.EventID.SaveDefaultNote, Utility.GetEntry(QwilightComponent.BundleEntryPath).Select(bundleEntryPath => Path.GetFileName(bundleEntryPath)));
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.SavingDefaultNoteContents);
                }
            }
        }

        public async Task GetDefaultUIDate(long defaultUIDate, bool isSilent)
        {
            var twilightWwwDefaultDate = await GetWwwParallel<JSON.TwilightWwwDefaultDate?>($"{QwilightComponent.QwilightAPI}/defaultUIDate?date={defaultUIDate}").ConfigureAwait(false);
            if (twilightWwwDefaultDate.HasValue && !ViewModels.Instance.MainValue.IsDNDMode)
            {
                var date = twilightWwwDefaultDate.Value.date;
                Configure.Instance.DefaultUIDate = date;
                if (isSilent)
                {
                    GetDefaultUI();
                }
                else
                {
                    if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                    {
                        Text = string.Format(LanguageSystem.Instance.SaveDefaultUINotify, DateTimeOffset.FromUnixTimeMilliseconds(date).LocalDateTime.ToLongDateString()),
                        Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
                    }) == MESSAGEBOX_RESULT.IDYES)
                    {
                        GetDefaultUI();
                    }
                }

                void GetDefaultUI()
                {
                    SendParallel(Event.Types.EventID.SaveDefaultUi, bool.TrueString);
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.SavingDefaultUIContents);
                }
            }
        }
    }
}