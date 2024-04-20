using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed partial class SiteWindowViewModel : BaseViewModel
    {
        SiteItem _siteItem;
        Timer _getSitesHandler;

        public ObservableCollection<SiteItem> SiteItemCollection { get; } = new();

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        public SiteItem SiteItem
        {
            get => _siteItem;

            set => SetProperty(ref _siteItem, value, nameof(SiteItem));
        }

        [RelayCommand]
        static void OnNewNetSite()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var entryItem = mainViewModel.EntryItemValue;
            var noteFile = entryItem?.NoteFile;
            if (noteFile?.IsLogical == false)
            {
                var inputPwViewModel = ViewModels.Instance.InputPwValue;
                inputPwViewModel.Text = LanguageSystem.Instance.NewNetSiteContents;
                inputPwViewModel.Input = noteFile.Title;
                inputPwViewModel.IsInputEditable = true;
                inputPwViewModel.HandleOK = new Action<string, string>((input, inputCipher) =>
                {
                    var modeComponent = mainViewModel.ModeComponentValue;
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSite, new
                    {
                        siteName = input,
                        siteCipher = inputCipher,
                        isNetSite = true,
                        data = modeComponent.GetJSON(),
                        noteID = noteFile.GetNoteID512(),
                        noteIDs = noteFile.EntryItem.CompatibleNoteFiles.Select(noteFile => noteFile.GetNoteID512()),
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
                        bundleEntryPath = noteFile.EntryItem.EntryPath,
                        allowedPostableItems = Enumerable.Range(0, PostableItem.Values.Length).ToArray(),
                        postableItemBand = 100
                    });
                });
                inputPwViewModel.Open();
            }
            else
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotValidNoteFile);
            }
        }

        [RelayCommand]
        static void OnNewSite()
        {
            var inputPwViewModel = ViewModels.Instance.InputPwValue;
            inputPwViewModel.Text = LanguageSystem.Instance.NewSiteContents;
            inputPwViewModel.Input = string.Empty;
            inputPwViewModel.IsInputEditable = true;
            inputPwViewModel.HandleOK = new Action<string, string>((input, inputCipher) =>
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSite, new
                {
                    siteName = input,
                    siteCipher = inputCipher,
                    isNetSite = false
                });
            });
            inputPwViewModel.Open();
        }

        public void OnEnterSite()
        {
            if (SiteItem != null)
            {
                if (SiteItem.HasCipher)
                {
                    var inputPwViewModel = ViewModels.Instance.InputPwValue;
                    inputPwViewModel.Text = LanguageSystem.Instance.EnterSiteCipherContents;
                    inputPwViewModel.Input = SiteItem.SiteName;
                    inputPwViewModel.IsInputEditable = false;
                    inputPwViewModel.HandleOK = new Action<string, string>((input, inputCipher) =>
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.EnterSite, new
                        {
                            siteID = SiteItem.SiteID,
                            siteCipher = inputCipher
                        });
                    });
                    inputPwViewModel.Open();
                }
                else
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.EnterSite, new
                    {
                        siteID = SiteItem.SiteID,
                        siteCipher = string.Empty
                    });
                }
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            _getSitesHandler = new(async state =>
            {
                if (IsOpened)
                {
                    var twilightWwwSites = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwSite[]>($"{QwilightComponent.QwilightAPI}/sites").ConfigureAwait(false);
                    if (twilightWwwSites != null)
                    {
                        Utility.SetUICollection(SiteItemCollection, twilightWwwSites.Select(data => new SiteItem
                        {
                            SiteID = data.siteID,
                            SiteName = LanguageSystem.Instance.GetSiteName(data.siteName),
                            SiteConfigure = data.siteConfigure,
                            HasCipher = data.hasCipher,
                            AvatarCountText = data.avatarCount.ToString(LanguageSystem.Instance.AvatarCountContents)
                        }).ToArray(), null, null, (siteItem, targetItem) =>
                        {
                            siteItem.SiteName = targetItem.SiteName;
                            siteItem.AvatarCountText = targetItem.AvatarCountText;
                        });
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            _getSitesHandler.Dispose();
        }
    }
}