using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight.ViewModel
{
    public sealed partial class LevelViewModel : BaseViewModel
    {
        public ObservableCollection<LevelItem> LevelItemCollection { get; } = new();

        public override double TargetLength => 0.4;

        public override double TargetHeight => 0.7;

        public void OnInput() => OnPropertyChanged(nameof(IsTotalWantLevel));

        public bool IsTotalWantLevel => LevelItemCollection.All(levelItem => levelItem.IsWanted);

        void SetLevelItemCollection()
        {
            LevelItemCollection.Clear();
            foreach (var levelID in LevelSystem.Instance.LevelCollection)
            {
                LevelItemCollection.Add(new LevelItem
                {
                    LevelID = levelID,
                    IsWanted = Configure.Instance.WantLevelIDs.Contains(levelID)
                });
            }
            OnPropertyChanged(nameof(IsTotalWantLevel));
        }

        public void OnNewLevel()
        {
            if (IsLoaded)
            {
                LevelSystem.Instance.LoadJSON(true);
                Configure.Instance.WantLevelIDs = LevelSystem.Instance.LevelCollection.ToArray();
                SetLevelItemCollection();
            }
        }

        [RelayCommand]
        void OnTotalWantLevel(bool? e)
        {
            if (e.HasValue)
            {
                foreach (var levelItem in LevelItemCollection)
                {
                    levelItem.IsWanted = e.Value;
                }
                OnPropertyChanged(nameof(IsTotalWantLevel));
            }
        }

        [RelayCommand]
        async Task OnLoadLevel(string www)
        {
            if (string.IsNullOrEmpty(www))
            {
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.ViewInputWindow,
                    Contents = new object[]
                    {
                        LanguageSystem.Instance.LevelInputContents,
                        string.Empty,
                        new Action<string>(async levelInput =>
                        {
                            if (!string.IsNullOrEmpty(levelInput))
                            {
                                await LevelSystem.Instance.LoadWww(levelInput);
                            }
                        })
                    }
                });
            }
            else
            {
                await LevelSystem.Instance.LoadWww(www);
            }
            LevelSystem.Instance.LoadJSON(true);
        }

        [RelayCommand]
        async Task OnGetLevel()
        {
            if (Configure.Instance.LevelTargetMap.TryGetValue(Configure.Instance.WantLevelName, out var target))
            {
                await LevelSystem.Instance.LoadWww(target);
                LevelSystem.Instance.LoadJSON(true);
            }
            else
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotHaveLevelTarget);
            }
        }

        [RelayCommand]
        void OnWipeLevel() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewAllowWindow,
            Contents = new object[]
            {
                LanguageSystem.Instance.WipeLevelNotify,
                MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                new Action<MESSAGEBOX_RESULT>(r =>
                {
                    if (r == MESSAGEBOX_RESULT.IDYES)
                    {
                        Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"{Configure.Instance.WantLevelName}.json"));
                        Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"#{Configure.Instance.WantLevelName}.json"));
                        Configure.Instance.LevelTargetMap.Remove(Configure.Instance.WantLevelName);
                        var i = LevelSystem.Instance.LevelFileNames.IndexOf(Configure.Instance.WantLevelName);
                        LevelSystem.Instance.LevelFileNames.RemoveAt(i);
                        Configure.Instance.WantLevelName = LevelSystem.Instance.LevelFileNames.ElementAtOrDefault(i) ?? LevelSystem.Instance.LevelFileNames.LastOrDefault();
                    }
                })
            }
        });

        public override void OnOpened()
        {
            base.OnOpened();
            LevelSystem.Instance.LoadLevelFiles();
            SetLevelItemCollection();
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            Configure.Instance.WantLevelIDs = LevelItemCollection.Where(levelItem => levelItem.IsWanted).Select(levelItem => levelItem.LevelID).ToArray();
            ViewModels.Instance.MainValue.Want();
        }
    }
}