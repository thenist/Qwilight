using Qwilight.View;
using System.Collections.ObjectModel;

namespace Qwilight.ViewModel
{
    public sealed class SiteContainerViewModel : BaseViewModel
    {
        Site _siteView;

        public ObservableCollection<Site> SiteCollection { get; } = new();

        public bool HasPointedInput => IsOpened && SiteValue?.HasPointedInput == true;

        public Site SiteView
        {
            get => _siteView;

            set => SetProperty(ref _siteView, value, nameof(SiteView));
        }

        public SiteViewModel SiteValue { get; set; }

        public override bool IsModal => false;

        public void OnSiteView()
        {
            SiteValue = SiteView?.DataContext as SiteViewModel;
            ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.IsOpened = SiteValue == siteViewModel);
            if (IsOpened)
            {
                SiteValue?.OnOpened();
            }
        }

        public void AudioInput(byte[] data, int length)
        {
            var siteViewModel = SiteValue;
            if (siteViewModel?.IsAudioInput == true)
            {
                siteViewModel.AudioInput(data, length);
            }
        }

        public void CallSetNoteFile() => ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.CallSetNoteFile());

        public void CallSetModeComponent() => ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.CallUpdateModeComponent());

        public void SetComputingValues() => ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.SetComputingValues());

        public override void OnOpened()
        {
            base.OnOpened();
            OnPropertyChanged(nameof(Position1));
            if (ViewModels.Instance.HasSiteViewModel())
            {
                HandlingUISystem.Instance.HandleParallel(() => ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.OnOpened()));
            }
            else
            {
                ViewModels.Instance.SiteWindowValue.Open();
            }
        }
    }
}