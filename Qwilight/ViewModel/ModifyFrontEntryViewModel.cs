using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class ModifyFrontEntryViewModel : BaseViewModel
    {
        FrontEntryItem? _frontEntryItemValue;

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.4;

        public ObservableCollection<FrontEntryItem> FrontEntryItemCollection { get; } = new();

        public DefaultEntryItem FavoriteEntryItem { get; set; }

        public FrontEntryItem? FrontEntryItemValue
        {
            get => _frontEntryItemValue;

            set => SetProperty(ref _frontEntryItemValue, value, nameof(FrontEntryItemValue));
        }

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up when FrontEntryItemValue.HasValue:
                    var i = FrontEntryItemCollection.IndexOf(FrontEntryItemValue.Value);
                    if (i > 0)
                    {
                        FrontEntryItemCollection.Move(i, i - 1);
                    }
                    StrongReferenceMessenger.Default.Send(new MoveFrontEntryView
                    {
                        Target = FrontEntryItemValue
                    });
                    e.Handled = true;
                    break;
                case Key.Down when FrontEntryItemValue.HasValue:
                    i = FrontEntryItemCollection.IndexOf(FrontEntryItemValue.Value);
                    if (i < FrontEntryItemCollection.Count - 1)
                    {
                        FrontEntryItemCollection.Move(i, i + 1);
                    }
                    StrongReferenceMessenger.Default.Send(new MoveFrontEntryView
                    {
                        Target = FrontEntryItemValue
                    });
                    e.Handled = true;
                    break;
                case Key.Delete when FrontEntryItemValue.HasValue:
                    i = FrontEntryItemCollection.IndexOf(FrontEntryItemValue.Value);
                    FrontEntryItemCollection.RemoveAt(i);
                    if (i < FrontEntryItemCollection.Count)
                    {
                        FrontEntryItemValue = FrontEntryItemCollection[i];
                    }
                    StrongReferenceMessenger.Default.Send(new MoveFrontEntryView
                    {
                        Target = FrontEntryItemValue
                    });
                    break;
            }
        }

        [RelayCommand]
        void OnNewFrontEntry()
        {
            var entryPath = StrongReferenceMessenger.Default.Send<ViewEntryWindow>();
            if (!string.IsNullOrEmpty(entryPath) && !FrontEntryItemCollection.Any(frontEntryItem => frontEntryItem.FrontEntryPath == entryPath))
            {
                FrontEntryItemCollection.Add(new FrontEntryItem
                {
                    FrontEntryPath = entryPath,
                });
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            FrontEntryItemCollection.Clear();
            foreach (var frontEntryPath in FavoriteEntryItem.FrontEntryPaths)
            {
                FrontEntryItemCollection.Add(new FrontEntryItem
                {
                    FrontEntryPath = frontEntryPath
                });
            }
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            FavoriteEntryItem.FrontEntryPaths.Clear();
            foreach (var frontEntryItemValue in FrontEntryItemCollection)
            {
                FavoriteEntryItem.FrontEntryPaths.Add(frontEntryItemValue.FrontEntryPath);
            }
        }
    }
}