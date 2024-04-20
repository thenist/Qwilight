using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
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

        public override double TargetHeight => 0.6;

        public void OnInput() => OnPropertyChanged(nameof(IsTotalWantLevelID));

        public bool IsTotalWantLevelID => LevelItemCollection.All(levelItem => levelItem.IsWanted);

        public void OnNewLevel()
        {
            if (IsLoaded)
            {
                LevelSystem.Instance.LoadJSON(true);
                LevelItemCollection.Clear();
                foreach (var levelID in LevelSystem.Instance.LevelIDCollection)
                {
                    LevelItemCollection.Add(new LevelItem
                    {
                        LevelID = levelID,
                        IsWanted = Configure.Instance.LastWantLevelIDs.Contains(levelID)
                    });
                }
                OnPropertyChanged(nameof(IsTotalWantLevelID));
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
                OnPropertyChanged(nameof(IsTotalWantLevelID));
            }
        }

        [RelayCommand]
        static async Task OnLoadLevel(string www)
        {
            if (string.IsNullOrEmpty(www))
            {
                var inputTextViewModel = ViewModels.Instance.InputTextValue;
                inputTextViewModel.Text = LanguageSystem.Instance.LevelInputContents;
                inputTextViewModel.Input = string.Empty;
                inputTextViewModel.HandleOK = new Action<string>(async text =>
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        await LevelSystem.Instance.LoadWww(text);
                        LevelSystem.Instance.LoadJSON(true);
                    }
                });
                inputTextViewModel.Open();
            }
            else
            {
                await LevelSystem.Instance.LoadWww(www);
                LevelSystem.Instance.LoadJSON(true);
            }
        }

        [RelayCommand]
        static async Task OnGetLevel()
        {
            if (Configure.Instance.LevelTargetMap.TryGetValue(Configure.Instance.LastWantLevelName, out var target))
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
        static void OnWipeLevel()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.WipeLevelNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"{Configure.Instance.LastWantLevelName}.json"));
                Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"#{Configure.Instance.LastWantLevelName}.json"));
                Configure.Instance.LevelTargetMap.Remove(Configure.Instance.LastWantLevelName);
                var i = LevelSystem.Instance.LevelFileNames.IndexOf(Configure.Instance.LastWantLevelName);
                LevelSystem.Instance.LevelFileNames.RemoveAt(i);
                Configure.Instance.LastWantLevelName = LevelSystem.Instance.LevelFileNames.ElementAtOrDefault(i) ?? LevelSystem.Instance.LevelFileNames.LastOrDefault();
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            LevelSystem.Instance.LoadLevelFiles();
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            Configure.Instance.LastWantLevelIDs = LevelItemCollection.Where(levelItem => levelItem.IsWanted).Select(levelItem => levelItem.LevelID).ToArray();
            Configure.Instance.Save(true);
            ViewModels.Instance.MainValue.Want();
        }
    }
}