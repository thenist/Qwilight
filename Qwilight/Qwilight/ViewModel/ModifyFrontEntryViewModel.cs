using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.MoveFrontEntryView,
                        Contents = FrontEntryItemValue
                    });
                    e.Handled = true;
                    break;
                case Key.Down when FrontEntryItemValue.HasValue:
                    i = FrontEntryItemCollection.IndexOf(FrontEntryItemValue.Value);
                    if (i < FrontEntryItemCollection.Count - 1)
                    {
                        FrontEntryItemCollection.Move(i, i + 1);
                    }
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.MoveFrontEntryView,
                        Contents = FrontEntryItemValue
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
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.MoveFrontEntryView,
                        Contents = FrontEntryItemValue
                    });
                    break;
            }
        }

        [RelayCommand]
        void OnNewFrontEntry() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.ViewEntryWindow,
            Contents = new Action<string>(fileName =>
            {
                if (!FrontEntryItemCollection.Any(frontEntryItem => frontEntryItem.FrontEntryPath == fileName))
                {
                    FrontEntryItemCollection.Add(new FrontEntryItem
                    {
                        FrontEntryPath = fileName,
                    });
                }
            })
        });

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

        public override void OnCollasped()
        {
            base.OnCollasped();
            FavoriteEntryItem.FrontEntryPaths.Clear();
            foreach (var frontEntryItemValue in FrontEntryItemCollection)
            {
                FavoriteEntryItem.FrontEntryPaths.Add(frontEntryItemValue.FrontEntryPath);
            }
        }
    }
}