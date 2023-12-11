using Qwilight.NoteFile;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class DefaultEntryItem : Model
    {
        public static readonly DefaultEntryItem Total = new()
        {
            DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Total,
            DefaultEntryPath = "/"
        };
        public static readonly DefaultEntryItem EssentialBundle = new()
        {
            DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Essential,
            DefaultEntryPath = QwilightComponent.BundleEntryPath
        };

        public EntryItem GetEntryItem(bool isEnter, int entryItemID)
        {
            var entryItem = new EntryItem
            {
                DefaultEntryItem = this,
                Title = isEnter ? ToString() : $"🔙{this}",
                Artist = DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Default || DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Essential ? DefaultEntryPath : string.Empty,
                IsLogical = true,
                EntryPath = DefaultEntryPath,
                LogicalVarietyValue = EntryItem.LogicalVariety.DefaultEntryItem,
                EntryItemID = entryItemID
            };
            var noteFiles = new[]
            {
                new DefaultEntryItemNoteFile(this, entryItem, isEnter)
            };
            entryItem.NoteFiles = noteFiles;
            entryItem.WellNoteFiles = noteFiles.Cast<BaseNoteFile>().ToList();
            return entryItem;
        }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public enum DefaultEntryVariety
        {
            Total, Default, Favorite, Essential, Net
        };

        string _favoriteEntryName = string.Empty;
        bool? _favoriteEntryStatus;

        public string DefaultEntryPath { get; init; }

        public string FavoriteEntryName
        {
            get => _favoriteEntryName;

            set => SetProperty(ref _favoriteEntryName, value, nameof(NameInModifyDefaultEntryWindow));
        }

        public DefaultEntryVariety DefaultEntryVarietyValue { get; init; }

        public HashSet<string> FrontEntryPaths { get; set; } = new();

        [JsonIgnore]
        public bool? FavoriteEntryStatus
        {
            get => _favoriteEntryStatus;

            set => SetProperty(ref _favoriteEntryStatus, value, nameof(FavoriteEntryStatus));
        }

        public string NameInModifyDefaultEntryWindow => DefaultEntryVarietyValue != DefaultEntryVariety.Favorite ? DefaultEntryPath : FavoriteEntryName;

        public override bool Equals(object obj) => obj is DefaultEntryItem defaultEntryItem && DefaultEntryVarietyValue == defaultEntryItem.DefaultEntryVarietyValue && DefaultEntryPath == defaultEntryItem.DefaultEntryPath;

        public override int GetHashCode() => HashCode.Combine(DefaultEntryPath, DefaultEntryVarietyValue);

        public override string ToString()
        {
            switch (DefaultEntryVarietyValue)
            {
                case DefaultEntryVariety.Total:
                    return LanguageSystem.Instance.TotalVarietyContents;
                case DefaultEntryVariety.Default:
                    return Path.GetFileName(DefaultEntryPath);
                case DefaultEntryVariety.Essential when DefaultEntryPath == QwilightComponent.BundleEntryPath:
                    return LanguageSystem.Instance.BundleEntryContents;
                case DefaultEntryVariety.Favorite:
                    return FavoriteEntryName;
                default:
                    return string.Empty;
            }
        }

        public string WipeNotify => DefaultEntryVarietyValue switch
        {
            DefaultEntryVariety.Default => LanguageSystem.Instance.WipeDefaultEntryNotify,
            DefaultEntryVariety.Favorite => LanguageSystem.Instance.WipeFavoriteEntryNotify,
            _ => string.Empty
        };

        public static bool operator ==(DefaultEntryItem left, DefaultEntryItem right) => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        public static bool operator !=(DefaultEntryItem left, DefaultEntryItem right) => !(left == right);
    }
}