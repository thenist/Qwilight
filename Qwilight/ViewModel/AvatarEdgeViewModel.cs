using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class AvatarEdgeViewModel : BaseViewModel
    {
        bool _isAvatarEdgeLoading;
        AvatarEdgeItem _avatarEdgeItem;

        public override double TargetLength => 0.4;

        public override double TargetHeight => 0.6;

        public override HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public ObservableCollection<AvatarEdgeItem> AvatarEdgeItemCollection { get; } = new();

        public AvatarEdgeItem AvatarEdgeItemValue
        {
            get => _avatarEdgeItem;

            set => SetProperty(ref _avatarEdgeItem, value, nameof(AvatarEdgeItemValue));
        }

        public bool IsAvatarEdgeLoading
        {
            get => _isAvatarEdgeLoading;

            set => SetProperty(ref _isAvatarEdgeLoading, value, nameof(IsAvatarEdgeLoading));
        }

        public void OnPointLower() => Close();

        public override void OnOpened()
        {
            base.OnOpened();
            UIHandler.Instance.HandleParallel(async () =>
            {
                IsAvatarEdgeLoading = true;
                var edgeIDs = await TwilightSystem.Instance.GetWwwParallel<string[]>($"{QwilightComponent.QwilightAPI}/edges?avatarID={TwilightSystem.Instance.AvatarID}");
                if (edgeIDs != null)
                {
                    Utility.SetUICollection(AvatarEdgeItemCollection, edgeIDs.Select(edgeID => new AvatarEdgeItem
                    {
                        EdgeID = edgeID,
                    }).ToArray());
                }
                IsAvatarEdgeLoading = false;
            });
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            var avatarEdgeItemValue = AvatarEdgeItemValue;
            if (avatarEdgeItemValue != null)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.AvatarEdge, avatarEdgeItemValue.EdgeID);
                ViewModels.Instance.AvatarValue.AvatarWwwValue = new(TwilightSystem.Instance.AvatarID, null, avatarEdgeItemValue.Drawing, true);
            }
        }
    }
}