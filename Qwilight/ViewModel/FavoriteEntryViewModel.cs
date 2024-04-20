using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class FavoriteEntryViewModel : BaseViewModel
    {
        public const int NoteFileMode = 0;
        public const int EntryItemMode = 1;

        public override double TargetLength => 0.2;

        public override double TargetHeight => 0.4;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        int _mode;

        public ObservableCollection<DefaultEntryItem> FavoriteEntryItemCollection { get; } = new();

        public BaseNoteFile NoteFile { get; set; }

        public EntryItem EntryItem { get; set; }

        public int Mode
        {
            get => _mode;

            set => SetProperty(ref _mode, value, nameof(Mode));
        }

        public override void OnOpened()
        {
            base.OnOpened();
            var favoriteEntryItems = Configure.Instance.DefaultEntryItems.Where(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite).ToArray();
            Utility.SetUICollection(FavoriteEntryItemCollection, favoriteEntryItems);
            switch (Mode)
            {
                case NoteFileMode:
                    foreach (var favoriteEntryItem in favoriteEntryItems)
                    {
                        favoriteEntryItem.FavoriteEntryStatus = NoteFile.FavoriteEntryItems.Contains(favoriteEntryItem);
                    }
                    break;
                case EntryItemMode:
                    foreach (var favoriteEntryItem in favoriteEntryItems)
                    {
                        var favoriteEntryValueCount = EntryItem.NoteFiles.Where(noteFile => noteFile.FavoriteEntryItems.Contains(favoriteEntryItem)).Count();
                        if (favoriteEntryValueCount == EntryItem.NoteFiles.Length)
                        {
                            favoriteEntryItem.FavoriteEntryStatus = true;
                        }
                        else if (favoriteEntryValueCount == 0)
                        {
                            favoriteEntryItem.FavoriteEntryStatus = false;
                        }
                        else
                        {
                            favoriteEntryItem.FavoriteEntryStatus = null;
                        }
                    }
                    break;
            }
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            var favoriteEntryItemsModified = new HashSet<DefaultEntryItem>();
            switch (Mode)
            {
                case NoteFileMode:
                    foreach (var favoriteEntryItem in FavoriteEntryItemCollection)
                    {
                        if (favoriteEntryItem.FavoriteEntryStatus.Value)
                        {
                            if (NoteFile.FavoriteEntryItems.Add(favoriteEntryItem))
                            {
                                favoriteEntryItemsModified.Add(favoriteEntryItem);
                                NoteFile.NotifyHasFavoriteEntryItem();
                            }
                        }
                        else
                        {
                            if (NoteFile.FavoriteEntryItems.Remove(favoriteEntryItem))
                            {
                                favoriteEntryItemsModified.Add(favoriteEntryItem);
                                NoteFile.NotifyHasFavoriteEntryItem();
                            }
                        }
                    }
                    foreach (var favoriteEntryItem in NoteFile.FavoriteEntryItems)
                    {
                        favoriteEntryItem.FrontEntryPaths.Add(NoteFile.DefaultEntryItem.DefaultEntryPath);
                    }
                    DB.Instance.SetFavoriteEntry(NoteFile);
                    break;
                case EntryItemMode:
                    foreach (var noteFile in EntryItem.NoteFiles)
                    {
                        if (!noteFile.IsLogical)
                        {
                            foreach (var favoriteEntryItem in FavoriteEntryItemCollection.Where(favoriteEntryItem => favoriteEntryItem.FavoriteEntryStatus.HasValue))
                            {
                                if (favoriteEntryItem.FavoriteEntryStatus.Value)
                                {
                                    if (noteFile.FavoriteEntryItems.Add(favoriteEntryItem))
                                    {
                                        favoriteEntryItemsModified.Add(favoriteEntryItem);
                                        noteFile.NotifyHasFavoriteEntryItem();
                                    }
                                }
                                else
                                {
                                    if (noteFile.FavoriteEntryItems.Remove(favoriteEntryItem))
                                    {
                                        favoriteEntryItemsModified.Add(favoriteEntryItem);
                                        noteFile.NotifyHasFavoriteEntryItem();
                                    }
                                }
                            }
                            foreach (var favoriteEntryItem in noteFile.FavoriteEntryItems)
                            {
                                favoriteEntryItem.FrontEntryPaths.Add(noteFile.DefaultEntryItem.DefaultEntryPath);
                            }
                            DB.Instance.SetFavoriteEntry(noteFile);
                        }
                    }
                    break;
            }
            if (favoriteEntryItemsModified.Contains(Configure.Instance.LastDefaultEntryItem))
            {
                ViewModels.Instance.MainValue.Want();
            }
        }
    }
}