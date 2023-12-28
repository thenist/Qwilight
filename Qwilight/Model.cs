using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Qwilight
{
    public class Model : ObservableObject
    {
        public virtual void NotifyModel() => OnPropertyChanged(string.Empty);

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                base.OnPropertyChanged(e);
            }
            catch (Win32Exception)
            {
            }
        }
    }
}
