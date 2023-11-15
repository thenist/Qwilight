using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
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

        public override double TargetHeight => 0.8;

        public ObservableCollection<DefaultEntryItem> DefaultEntryItemCollection { get; } = new();

        public DefaultEntryItem DefaultEntryItem
        {
            get => _defaultEntryItem;

            set
            {
                if (SetProperty(ref _defaultEntryItem, value, nameof(DefaultEntryItem)))
                {
                    OnPropertyChanged(nameof(IsFavoriteEntry));
                }
            }
        }

        public bool IsFavoriteEntry => DefaultEntryItem?.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite;

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up when DefaultEntryItem != null:
                    var i = DefaultEntryItemCollection.IndexOf(DefaultEntryItem);
                    if (i > 0)
                    {
                        DefaultEntryItemCollection.Move(i, i - 1);
                    }
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.MoveDefaultEntryView,
                        Contents = DefaultEntryItem
                    });
                    e.Handled = true;
                    break;
                case Key.Down when DefaultEntryItem != null:
                    i = DefaultEntryItemCollection.IndexOf(DefaultEntryItem);
                    if (i < DefaultEntryItemCollection.Count - 1)
                    {
                        DefaultEntryItemCollection.Move(i, i + 1);
                    }
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.MoveDefaultEntryView,
                        Contents = DefaultEntryItem
                    });
                    e.Handled = true;
                    break;
                case Key.Delete when DefaultEntryItem != null:
                    if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                    {
                        Text = DefaultEntryItem.WipeNotify,
                        Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
                    }) == MESSAGEBOX_RESULT.IDYES)
                    {
                        i = DefaultEntryItemCollection.IndexOf(DefaultEntryItem);
                        DefaultEntryItemCollection.RemoveAt(i);
                        if (i < DefaultEntryItemCollection.Count)
                        {
                            DefaultEntryItem = DefaultEntryItemCollection[i];
                        }
                        WeakReferenceMessenger.Default.Send<ICC>(new()
                        {
                            IDValue = ICC.ID.MoveDefaultEntryView,
                            Contents = DefaultEntryItem
                        });
                    }
                    break;
            }
        }

        [RelayCommand]
        async Task OnNewDefaultEntry()
        {
            var fileName = await StrongReferenceMessenger.Default.Send<ViewEntryWindow>();
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!DefaultEntryItemCollection.Any(defaultEntryItem => defaultEntryItem.DefaultEntryPath == fileName))
                {
                    DefaultEntryItemCollection.Add(new DefaultEntryItem
                    {
                        DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Default,
                        DefaultEntryPath = fileName
                    });
                }
            }
        }

        [RelayCommand]
        void OnNewFavoriteEntry()
        {
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.ViewInputWindow,
                Contents = new object[]
                {
                    LanguageSystem.Instance.NewFavoriteEntryContents,
                    string.Empty,
                    new Action<string>(favoriteEntryName =>
                    {
                        DefaultEntryItemCollection.Add(new DefaultEntryItem
                        {
                            DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Favorite,
                            DefaultEntryPath = Guid.NewGuid().ToString(),
                            FavoriteEntryName = favoriteEntryName
                        });
                    })
                }
            });
        }

        [RelayCommand]
        void OnModifyFavoriteEntryName() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewInputWindow,
            Contents = new object[]
            {
                LanguageSystem.Instance.ModifyFavoriteEntryNameContents,
                DefaultEntryItem.FavoriteEntryName,
                new Action<string>(favoriteEntryName => DefaultEntryItem.FavoriteEntryName = favoriteEntryName)
            }
        });

        [RelayCommand]
        void OnModifyFrontEntry()
        {
            ViewModels.Instance.ModifyFrontEntryValue.FavoriteEntryItem = DefaultEntryItem;
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
            HandlingUISystem.Instance.HandleParallel(() =>
            {
                DefaultEntryItemCollection.Clear();
                foreach (var defaultEntryItem in _defaultEntryItemCollection)
                {
                    DefaultEntryItemCollection.Add(defaultEntryItem);
                }
            });
            GroupEntry0 = !Configure.Instance.GroupEntry;
            GroupEntry1 = Configure.Instance.GroupEntry;
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            for (var i = DefaultEntryItemCollection.Count - 1; i >= 0; --i)
            {
                DefaultEntryItemCollection[i].Layer = i;
            }
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