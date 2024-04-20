using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight
{
    public sealed partial class ModifyDefaultEntryViewModel : BaseViewModel
    {
        readonly List<DefaultEntryItem> _defaultEntryItemCollection = new();
        readonly bool[] _groupEntryInputs = new bool[2];
        bool _defaultGroupEntry;
        DefaultEntryItem _defaultEntryItem;

        public ObservableCollection<EntryItem> EZ2DJMAXEntryItems0 { get; } = new();

        public ObservableCollection<EntryItem> EZ2DJMAXEntryItems1 { get; } = new();

        public bool GroupEntry0
        {
            get => _groupEntryInputs[0];

            set
            {
                if (SetProperty(ref _groupEntryInputs[0], value, nameof(GroupEntry0)) && value)
                {
                    Configure.Instance.GroupEntry = false;
                }
            }
        }

        public bool GroupEntry1
        {
            get => _groupEntryInputs[1];

            set
            {
                if (SetProperty(ref _groupEntryInputs[1], value, nameof(GroupEntry1)) && value)
                {
                    Configure.Instance.GroupEntry = true;
                }
            }
        }

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.9;

        public ObservableCollection<DefaultEntryItem> DefaultEntryItemCollection { get; } = new();

        public DefaultEntryItem DefaultEntryItemValue
        {
            get => _defaultEntryItem;

            set
            {
                if (SetProperty(ref _defaultEntryItem, value, nameof(DefaultEntryItemValue)))
                {
                    OnPropertyChanged(nameof(IsFavoriteEntry));
                }
            }
        }

        public bool IsFavoriteEntry => DefaultEntryItemValue?.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite;

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up when DefaultEntryItemValue != null:
                    var i = DefaultEntryItemCollection.IndexOf(DefaultEntryItemValue);
                    if (i > 0)
                    {
                        DefaultEntryItemCollection.Move(i, i - 1);
                    }
                    StrongReferenceMessenger.Default.Send(new MoveDefaultEntryView()
                    {
                        Target = DefaultEntryItemValue
                    });
                    e.Handled = true;
                    break;
                case Key.Down when DefaultEntryItemValue != null:
                    i = DefaultEntryItemCollection.IndexOf(DefaultEntryItemValue);
                    if (i < DefaultEntryItemCollection.Count - 1)
                    {
                        DefaultEntryItemCollection.Move(i, i + 1);
                    }
                    StrongReferenceMessenger.Default.Send(new MoveDefaultEntryView()
                    {
                        Target = DefaultEntryItemValue
                    });
                    e.Handled = true;
                    break;
                case Key.Delete when DefaultEntryItemValue != null:
                    if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                    {
                        Text = DefaultEntryItemValue.WipeNotify,
                        Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
                    }) == MESSAGEBOX_RESULT.IDYES)
                    {
                        var defaultEntryItem = DefaultEntryItemValue;
                        i = DefaultEntryItemCollection.IndexOf(defaultEntryItem);
                        DefaultEntryItemCollection.RemoveAt(i);
                        if (i < DefaultEntryItemCollection.Count)
                        {
                            DefaultEntryItemValue = DefaultEntryItemCollection[i];
                        }
                        StrongReferenceMessenger.Default.Send(new MoveDefaultEntryView()
                        {
                            Target = DefaultEntryItemValue
                        });
                        if (Configure.Instance.LastDefaultEntryItem == defaultEntryItem)
                        {
                            Configure.Instance.LastDefaultEntryItem = null;
                            ViewModels.Instance.MainValue.SetDefaultEntryItems();
                        }
                    }
                    break;
            }
        }

        [RelayCommand]
        void OnNewDefaultEntry()
        {
            var entryPath = StrongReferenceMessenger.Default.Send<ViewEntryWindow>();
            if (!string.IsNullOrEmpty(entryPath))
            {
                if (!DefaultEntryItemCollection.Any(defaultEntryItem => defaultEntryItem.DefaultEntryPath == entryPath))
                {
                    DefaultEntryItemCollection.Add(new DefaultEntryItem
                    {
                        DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Default,
                        DefaultEntryPath = entryPath
                    });
                }
            }
        }

        [RelayCommand]
        void OnNewFavoriteEntry()
        {
            var inputTextViewModel = ViewModels.Instance.InputTextValue;
            inputTextViewModel.Text = LanguageSystem.Instance.NewFavoriteEntryContents;
            inputTextViewModel.Input = string.Empty;
            inputTextViewModel.HandleOK = new Action<string>(text =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    DefaultEntryItemCollection.Add(new DefaultEntryItem
                    {
                        DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Favorite,
                        DefaultEntryPath = Guid.NewGuid().ToString(),
                        FavoriteEntryName = text
                    });
                }
            });
            inputTextViewModel.Open();
        }

        [RelayCommand]
        void OnModifyFavoriteEntryName()
        {
            var inputTextViewModel = ViewModels.Instance.InputTextValue;
            inputTextViewModel.Text = LanguageSystem.Instance.ModifyFavoriteEntryNameContents;
            inputTextViewModel.Input = DefaultEntryItemValue.FavoriteEntryName;
            inputTextViewModel.HandleOK = new Action<string>(text =>
            {
                DefaultEntryItemValue.FavoriteEntryName = text;
            });
            inputTextViewModel.Open();
        }

        [RelayCommand]
        void OnModifyFrontEntry()
        {
            ViewModels.Instance.ModifyFrontEntryValue.FavoriteEntryItem = DefaultEntryItemValue;
            ViewModels.Instance.ModifyFrontEntryValue.Open();
        }

        [RelayCommand]
        void OnFitDefaultEntry()
        {
            var defaultEntryCollection = new List<DefaultEntryItem>();
            foreach (var defaultEntryItem in DefaultEntryItemCollection.Where(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Default).OrderBy(defaultEntryItem => defaultEntryItem.ToString()))
            {
                defaultEntryCollection.Add(defaultEntryItem);
            }
            foreach (var defaultEntryItem in DefaultEntryItemCollection.Where(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite).OrderBy(defaultEntryItem => defaultEntryItem.ToString()))
            {
                defaultEntryCollection.Add(defaultEntryItem);
            }
            DefaultEntryItemCollection.Clear();
            foreach (var defaultEntryItem in defaultEntryCollection)
            {
                DefaultEntryItemCollection.Add(defaultEntryItem);
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            _defaultGroupEntry = Configure.Instance.GroupEntry;
            _defaultEntryItemCollection.Clear();
            _defaultEntryItemCollection.AddRange(Configure.Instance.DefaultEntryItems);
            UIHandler.Instance.HandleParallel(() =>
            {
                DefaultEntryItemCollection.Clear();
                foreach (var defaultEntryItem in _defaultEntryItemCollection)
                {
                    DefaultEntryItemCollection.Add(defaultEntryItem);
                }
            });
            GroupEntry0 = !Configure.Instance.GroupEntry;
            GroupEntry1 = Configure.Instance.GroupEntry;

            Random.Shared.Shuffle(EZ2DJMAXNoteFile.Instances);
            Utility.SetUICollection(EZ2DJMAXEntryItems0, [
                NewEntryItem(0, EZ2DJMAXNoteFile.Instances[0][0]),
                NewEntryItem(1, EZ2DJMAXNoteFile.Instances[0][1]),
                NewEntryItem(2, EZ2DJMAXNoteFile.Instances[0][2]),
                NewEntryItem(3, EZ2DJMAXNoteFile.Instances[0][3])
            ]);
            Utility.SetUICollection(EZ2DJMAXEntryItems1, [
                NewEntryItem(0, EZ2DJMAXNoteFile.Instances[0]),
                NewEntryItem(1, EZ2DJMAXNoteFile.Instances[1]),
                NewEntryItem(2, EZ2DJMAXNoteFile.Instances[2]),
                NewEntryItem(3, EZ2DJMAXNoteFile.Instances[3])
            ]);

            static EntryItem NewEntryItem(int entryItemID, params BaseNoteFile[] noteFiles) => new()
            {
                NoteFiles = noteFiles,
                WellNoteFiles = noteFiles.ToList(),
                EntryItemID = Random.Shared.Next(),
                NotePosition = Random.Shared.Next(noteFiles.Length)
            };
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            Configure.Instance.DefaultEntryItems.Clear();
            foreach (var defaultEntryItem in DefaultEntryItemCollection)
            {
                Configure.Instance.DefaultEntryItems.Add(defaultEntryItem);
            }
            if (Utility.IsItemsEqual(DefaultEntryItemCollection, _defaultEntryItemCollection) != true)
            {
                ViewModels.Instance.MainValue.SetDefaultEntryItems();
            }
            if (_defaultGroupEntry != Configure.Instance.GroupEntry)
            {
                ViewModels.Instance.MainValue.WipeLoadedDefaultEntryItems();
            }
        }
    }
}