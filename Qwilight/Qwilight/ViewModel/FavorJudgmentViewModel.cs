using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class FavorJudgmentViewModel : BaseViewModel
    {
        string _favorJudgmentName;
        FavorJudgment? _favorJudgment;

        public override double TargetLength => 0.3;

        public override double TargetHeight => 0.55;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Bottom;

        public FavorJudgment? FavorJudgmentValue
        {
            get => _favorJudgment;

            set
            {
                if (SetProperty(ref _favorJudgment, value, nameof(FavorJudgmentValue)) && value.HasValue)
                {
                    var favorJudgmentValue = value.Value.Value;
                    ViewModels.Instance.MainValue.ModeComponentValue.HighestJudgment0 = favorJudgmentValue[(int)Component.Judged.Highest][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.HigherJudgment0 = favorJudgmentValue[(int)Component.Judged.Higher][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.HighJudgment0 = favorJudgmentValue[(int)Component.Judged.High][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowJudgment0 = favorJudgmentValue[(int)Component.Judged.Low][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowerJudgment0 = favorJudgmentValue[(int)Component.Judged.Lower][0];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowestJudgment0 = favorJudgmentValue[(int)Component.Judged.Lowest][0];

                    ViewModels.Instance.MainValue.ModeComponentValue.HighestJudgment1 = favorJudgmentValue[(int)Component.Judged.Highest][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.HigherJudgment1 = favorJudgmentValue[(int)Component.Judged.Higher][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.HighJudgment1 = favorJudgmentValue[(int)Component.Judged.High][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowJudgment1 = favorJudgmentValue[(int)Component.Judged.Low][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowerJudgment1 = favorJudgmentValue[(int)Component.Judged.Lower][1];
                    ViewModels.Instance.MainValue.ModeComponentValue.LowestJudgment1 = favorJudgmentValue[(int)Component.Judged.Lowest][1];

                    OnPropertyChanged(nameof(CanWipe));
                }
            }
        }

        public bool CanWipe => FavorJudgmentValue?.IsDefault == false;

        public ObservableCollection<FavorJudgment> FavorJudgmentCollection { get; } = new();

        public string FavorJudgmentName
        {
            get => _favorJudgmentName;

            set => SetProperty(ref _favorJudgmentName, value, nameof(FavorJudgmentName));
        }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSetFavorJudgment();
            }
        }

        [RelayCommand]
        void OnSetFavorJudgment()
        {
            var value = new double[6][];
            var favorJudgments = ViewModels.Instance.MainValue.ModeComponentValue.FavorJudgments;
            for (var i = favorJudgments.Length - 1; i >= 0; --i)
            {
                value[i] = new double[favorJudgments[i].Length];
                for (var j = favorJudgments[i].Length - 1; j >= 0; --j)
                {
                    value[i][j] = favorJudgments[i][j];
                }
            }
            var favorJudgmentValue = new FavorJudgment
            {
                Name = FavorJudgmentName,
                Value = value
            };
            Configure.Instance.FavorJudgments.Add(favorJudgmentValue);
            SetFavorHitPointsCollection();
            FavorJudgmentValue = favorJudgmentValue;
            FavorJudgmentName = string.Empty;
        }

        [RelayCommand]
        void OnWipeFavorJudgment()
        {
            Configure.Instance.FavorJudgments.Remove(FavorJudgmentValue.Value);
            SetFavorHitPointsCollection();
        }

        public void SetFavorHitPointsCollection() => Utility.SetUICollection(FavorJudgmentCollection, Configure.Instance.FavorJudgments);

        public override void OnOpened()
        {
            base.OnOpened();
            SetFavorHitPointsCollection();
        }
    }
}