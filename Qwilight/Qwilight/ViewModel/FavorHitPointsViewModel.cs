using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class FavorHitPointsViewModel : BaseViewModel
    {
        string _favorHitPointsName;
        FavorHitPoints? _favorHitPointsValue;

        public override double TargetLength => 0.3;

        public override double TargetHeight => 0.6;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Bottom;

        public FavorHitPoints? FavorHitPointsValue
        {
            get => _favorHitPointsValue;

            set
            {
                if (SetProperty(ref _favorHitPointsValue, value, nameof(FavorHitPointsValue)) && value.HasValue)
                {
                    var favorHitPointsValue = value.Value.Value;
                    ViewModels.Instance.MainValue.ModeComponentValue.HighestHitPoints0 = favorHitPointsValue[(int)Component.Judged.Highest][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.HigherHitPoints0 = favorHitPointsValue[(int)Component.Judged.Higher][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.HighHitPoints0 = favorHitPointsValue[(int)Component.Judged.High][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowHitPoints0 = favorHitPointsValue[(int)Component.Judged.Low][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowerHitPoints0 = favorHitPointsValue[(int)Component.Judged.Lower][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowestHitPoints0 = favorHitPointsValue[(int)Component.Judged.Lowest][0];

                    ViewModels.Instance.MainValue.ModeComponentValue.HighestHitPoints1 = favorHitPointsValue[(int)Component.Judged.Highest][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.HigherHitPoints1 = favorHitPointsValue[(int)Component.Judged.Higher][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.HighHitPoints1 = favorHitPointsValue[(int)Component.Judged.High][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowHitPoints1 = favorHitPointsValue[(int)Component.Judged.Low][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowerHitPoints1 = favorHitPointsValue[(int)Component.Judged.Lower][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowestHitPoints1 = favorHitPointsValue[(int)Component.Judged.Lowest][1];

                    OnPropertyChanged(nameof(CanWipe));
                }
            }
        }

        public bool CanWipe => FavorHitPointsValue?.IsDefault == false;

        public ObservableCollection<FavorHitPoints> FavorHitPointsCollection { get; } = new();

        public string FavorHitPointsName
        {
            get => _favorHitPointsName;

            set => SetProperty(ref _favorHitPointsName, value, nameof(FavorHitPointsName));
        }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSetFavorHitPoints();
            }
        }

        [RelayCommand]
        void OnSetFavorHitPoints()
        {
            var value = new double[6][];
            var favorHitPoints = ViewModels.Instance.MainValue.ModeComponentValue.FavorHitPoints;
            for (var i = favorHitPoints.Length - 1; i >= 0; --i)
            {
                value[i] = new double[favorHitPoints[i].Length];
                for (var j = favorHitPoints[i].Length - 1; j >= 0; --j)
                {
                    value[i][j] = favorHitPoints[i][j];
                }
            }
            var favorHitPointsValue = new FavorHitPoints
            {
                Name = FavorHitPointsName,
                Value = value
            };
            Configure.Instance.FavorHitPoints.Add(favorHitPointsValue);
            SetFavorHitPointsCollection();
            FavorHitPointsValue = favorHitPointsValue;
            FavorHitPointsName = string.Empty;
        }

        [RelayCommand]
        void OnQuitFavorHitPoints()
        {
            Configure.Instance.FavorHitPoints.Remove(FavorHitPointsValue.Value);
            SetFavorHitPointsCollection();
        }

        public void SetFavorHitPointsCollection() => Utility.SetUICollection(FavorHitPointsCollection, Configure.Instance.FavorHitPoints);

        public override void OnOpened()
        {
            base.OnOpened();
            SetFavorHitPointsCollection();
        }
    }
}