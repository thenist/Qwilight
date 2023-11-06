using Microsoft.UI;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class AvatarTitleViewModel : BaseViewModel
    {
        bool _isAvatarTitleLoading;
        AvatarTitleItem? _avatarTitleItem;

        public override double TargetLength => 0.4;

        public override double TargetHeight => 0.6;

        public override HorizontalAlignment TargetLengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Bottom;

        public ObservableCollection<AvatarTitleItem> AvatarTitleItemCollection { get; } = new();

        public AvatarTitleItem? AvatarTitleItemValue
        {
            get => _avatarTitleItem;

            set => SetProperty(ref _avatarTitleItem, value, nameof(AvatarTitleItemValue));
        }

        public bool IsAvatarTitleLoading
        {
            get => _isAvatarTitleLoading;

            set => SetProperty(ref _isAvatarTitleLoading, value, nameof(IsAvatarTitleLoading));
        }

        public void OnPointLower() => Close();

        public override async void OnOpened()
        {
            base.OnOpened();
            IsAvatarTitleLoading = true;

            var twilightWwwTitles = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwTitles[]>($"{QwilightComponent.QwilightAPI}/titles?avatarID={TwilightSystem.Instance.AvatarID}&language={Configure.Instance.Language}");
            if (twilightWwwTitles != null)
            {
                Utility.SetUICollection(AvatarTitleItemCollection, twilightWwwTitles.Prepend(new JSON.TwilightWwwTitles
                {
                    titleID = string.Empty,
                    title = LanguageSystem.Instance.NotAvatarTitle,
                    titleColor = nameof(Colors.White)
                }).Select(data => new AvatarTitleItem
                {
                    Title = data.title,
                    TitleID = data.titleID,
                    TitlePaint = Utility.GetTitlePaint(data.titleColor),
                    TitleColor = Utility.GetTitleColor(data.titleColor)
                }).ToArray());
            }

            IsAvatarTitleLoading = false;
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            var avatarTitleItem = AvatarTitleItemValue;
            if (avatarTitleItem.HasValue)
            {
                var avatarTitleItemValue = avatarTitleItem.Value;
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.AvatarTitle, avatarTitleItemValue.TitleID);
                ViewModels.Instance.AvatarValue.InitAvatarWwwValue(TwilightSystem.Instance.AvatarID, new AvatarTitle(avatarTitleItemValue.Title, avatarTitleItemValue.TitlePaint, avatarTitleItemValue.TitleColor));
            }
        }
    }
}