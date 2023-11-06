using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class SiteItem : Model
    {
        public const int DefaultSite = 0;
        public const int FavorSite = 1;
        public const int NetSite = 2;

        string _siteName;
        string _avatarCountText;

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string SiteID { get; init; }

        public string SiteName
        {
            get => _siteName;

            set => SetProperty(ref _siteName, value, nameof(SiteName));
        }

        public string AvatarCountText
        {
            get => _avatarCountText;

            set => SetProperty(ref _avatarCountText, value, nameof(AvatarCountText));
        }

        public int SiteConfigure { get; init; }

        public bool HasCipher { get; init; }

        public override bool Equals(object obj) => obj is SiteItem siteItem && SiteID == siteItem.SiteID;

        public override int GetHashCode() => HashCode.Combine(SiteID, SiteConfigure, HasCipher);
    }
}