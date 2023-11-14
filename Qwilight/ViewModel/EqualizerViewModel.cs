using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed partial class EqualizerViewModel : BaseViewModel
    {
        public void OnEqualizer0(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizer(0, (float)e.NewValue);

        public void OnEqualizer1(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizer(1, (float)e.NewValue);

        public void OnEqualizer2(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizer(2, (float)e.NewValue);

        public void OnEqualizer3(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizer(3, (float)e.NewValue);

        public void OnEqualizer4(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizer(4, (float)e.NewValue);

        public void OnEqualizerHz0(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizerHz(0, (float)e.NewValue);

        public void OnEqualizerHz1(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizerHz(1, (float)e.NewValue);

        public void OnEqualizerHz2(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizerHz(2, (float)e.NewValue);

        public void OnEqualizerHz3(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizerHz(3, (float)e.NewValue);

        public void OnEqualizerHz4(RoutedPropertyChangedEventArgs<double> e) => AudioSystem.Instance.SetEqualizerHz(4, (float)e.NewValue);

        public override double TargetLength => 0.4;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        [RelayCommand]
        static void OnInitEqualizer()
        {
            Configure.Instance.InitEqualizers(int.MaxValue);
            AudioSystem.Instance.SetEqualizer(0, (float)Configure.Instance.Equalizer0);
            AudioSystem.Instance.SetEqualizer(1, (float)Configure.Instance.Equalizer1);
            AudioSystem.Instance.SetEqualizer(2, (float)Configure.Instance.Equalizer2);
            AudioSystem.Instance.SetEqualizer(3, (float)Configure.Instance.Equalizer3);
            AudioSystem.Instance.SetEqualizer(4, (float)Configure.Instance.Equalizer4);
            AudioSystem.Instance.SetEqualizerHz(0, (float)Configure.Instance.Equalizer0);
            AudioSystem.Instance.SetEqualizerHz(1, (float)Configure.Instance.Equalizer1);
            AudioSystem.Instance.SetEqualizerHz(2, (float)Configure.Instance.Equalizer2);
            AudioSystem.Instance.SetEqualizerHz(3, (float)Configure.Instance.Equalizer3);
            AudioSystem.Instance.SetEqualizerHz(4, (float)Configure.Instance.Equalizer4);
        }
    }
}