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

        public async Task OnNewLevel()
        {
            if (IsLoaded)
            {
                await LevelSystem.Instance.LoadJSON(true);
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
                        await LevelSystem.Instance.LoadWww(text).ConfigureAwait(false);
                        await LevelSystem.Instance.LoadJSON(true);
                    }
                });
                inputTextViewModel.Open();
            }
            else
            {
                await LevelSystem.Instance.LoadWww(www).ConfigureAwait(false);
                await LevelSystem.Instance.LoadJSON(true);
            }
        }

        [RelayCommand]
        static async Task OnGetLevel()
        {
            if (Configure.Instance.LevelTargetMap.TryGetValue(Configure.Instance.WantLevelName, out var target))
            {
                await LevelSystem.Instance.LoadWww(target).ConfigureAwait(false);
                await LevelSystem.Instance.LoadJSON(true);
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
                Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"{Configure.Instance.WantLevelName}.json"));
                Utility.WipeFile(Path.Combine(LevelSystem.EntryPath, $"#{Configure.Instance.WantLevelName}.json"));
                Configure.Instance.LevelTargetMap.Remove(Configure.Instance.WantLevelName);
                var i = LevelSystem.Instance.LevelFileNames.IndexOf(Configure.Instance.WantLevelName);
                LevelSystem.Instance.LevelFileNames.RemoveAt(i);
                Configure.Instance.WantLevelName = LevelSystem.Instance.LevelFileNames.ElementAtOrDefault(i) ?? LevelSystem.Instance.LevelFileNames.LastOrDefault();
            }
        }

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