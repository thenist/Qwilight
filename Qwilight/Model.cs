using CommunityToolkit.Mvvm.ComponentModel;

namespace Qwilight
{
    public class Model : ObservableObject
    {
        public virtual void NotifyModel() => OnPropertyChanged(string.Empty);
    }
}
