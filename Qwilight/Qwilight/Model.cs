using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Qwilight
{
    public class Model : ObservableObject
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e) => HandlingUISystem.Instance.HandleParallel(() => base.OnPropertyChanged(e));

        public virtual void NotifyModel() => OnPropertyChanged(string.Empty);
    }
}
