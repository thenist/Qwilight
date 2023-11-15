using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
        void OnNewNetSite()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var entryItemValue = mainViewModel.EntryItemValue;
            var noteFile = entryItemValue?.NoteFile;
            if (noteFile?.IsLogical == false)
            {
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.ViewPwWindow,
                    Contents = new object[]
                    {
                        LanguageSystem.Instance.NewNetSiteContents,
                        noteFile.Title,
                        true,
                        new Action<string, string>((input, inputCipher) =>
                        {
                            var modeComponentValue = mainViewModel.ModeComponentValue;
                            TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSite, new
                            {
                                siteName = input,
                                siteCipher = inputCipher,
                                isNetSite = true,
                                data = modeComponentValue.GetJSON(),
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
                                allowedPostableItems = Enumerable.Range(0, PostableItem.Values.Length).ToArray()
                            });
                        })
                    }
                });
            }
            else
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotValidNoteFile);
            }
        }

        [RelayCommand]
        void OnNewSite()
        {
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.ViewPwWindow,
                Contents = new object[]
                {
                    LanguageSystem.Instance.NewSiteContents,
                    string.Empty,
                    true,
                    new Action<string, string>((input, inputCipher) =>
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSite, new
                        {
                            siteName = input,
                            siteCipher = inputCipher,
                            isNetSite = false
                        });
                    })
                }
            });
        }

        public void OnEnterSite()
        {
            var siteItem = SiteItem;
            if (siteItem != null)
            {
                if (siteItem.HasCipher)
                {
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.ViewPwWindow,
                        Contents = new object[]
                        {
                            LanguageSystem.Instance.EnterSiteCipherContents,
                            siteItem.SiteName,
                            false,
                            new Action<string, string>((input, inputCipher) => TwilightSystem.Instance.SendParallel(Event.Types.EventID.EnterSite, new
                            {
                                siteID = siteItem.SiteID,
                                siteCipher = inputCipher
                            }))
                        }
                    });
                }
                else
                {
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.EnterSite, new
                    {
                        siteID = siteItem.SiteID,
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
                            SiteName = Utility.GetSiteName(data.siteName),
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

        public override void OnCollasped()
        {
            base.OnCollasped();
            _getSitesHandler.Dispose();
        }
    }
}