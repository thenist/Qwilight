using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight.ViewModel
{
    public sealed partial class UbuntuViewModel : BaseViewModel
    {
        UbuntuItem _ubuntuItem;
        Timer _getUbuntuHandler;

        public ObservableCollection<UbuntuItem> UbuntuCollection { get; } = new();

        public void OnUbuntuView(KeyEventArgs e)
        {
            var ubuntuID = UbuntuItem?.AvatarID;
            if (e.Key == Key.Delete && ubuntuID != null)
            {
                WeakReferenceMessenger.Default.Send<ICC>(new()
                {
                    IDValue = ICC.ID.ViewAllowWindow,
                    Contents = new object[]
                    {
                        LanguageSystem.Instance.WipeUbuntuNotify,
                        MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1,
                        new Action<MESSAGEBOX_RESULT>(r =>
                        {
                            if (r == MESSAGEBOX_RESULT.IDYES)
                            {
                                TwilightSystem.Instance.SendParallel(Event.Types.EventID.WipeUbuntu, ubuntuID);
                            }
                        })
                    }
                });
            }
        }

        [RelayCommand]
        void OnCallIO()
        {
            var ubuntuID = UbuntuItem?.AvatarID;
            if (ubuntuID != null)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallIo, new
                {
                    avatarID = ubuntuID,
                    ioMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
        }

        [RelayCommand]
        void OnViewBundle()
        {
            var ubuntuID = UbuntuItem?.AvatarID;
            if (ubuntuID != null)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CallBundle, ubuntuID);
            }
        }

        [RelayCommand]
        void OnNewSilentSite()
        {
            var ubuntuID = UbuntuItem?.AvatarID;
            if (ubuntuID != null)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.NewSilentSite, ubuntuID);
            }
        }

        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Top;

        public UbuntuItem UbuntuItem
        {
            get => _ubuntuItem;

            set => SetProperty(ref _ubuntuItem, value, nameof(UbuntuItem));
        }

        public override void OnOpened()
        {
            base.OnOpened();
            _getUbuntuHandler = new(state =>
            {
                if (IsOpened)
                {
                    TwilightSystem.Instance.SendParallel<object>(Event.Types.EventID.CallUbuntu, null);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            _getUbuntuHandler.Dispose();
        }
    }
}