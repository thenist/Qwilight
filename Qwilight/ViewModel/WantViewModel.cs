using CommunityToolkit.Mvvm.Input;
using Qwilight.NoteFile;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public sealed partial class WantViewModel : BaseViewModel
    {
        public override double TargetLength => 0.8;

        public override double TargetHeight => 0.9;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        [RelayCommand]
        static void OnWantHellBPM() => Configure.Instance.WantHellBPM = !Configure.Instance.WantHellBPM;

        [RelayCommand]
        static void OnWantBanned() => Configure.Instance.WantBannedValue = (Configure.WantBanned)(((int)Configure.Instance.WantBannedValue + 1) % 2);

        [RelayCommand]
        void OnWantNoteVariety(int e)
        {
            Configure.Instance.InputWantNoteVariety[e] = !Configure.Instance.InputWantNoteVariety[e];
            OnPropertyChanged(nameof(NoteVarietyBMS));
            OnPropertyChanged(nameof(NoteVarietyBMSON));
            OnPropertyChanged(nameof(NoteVarietyEventNote));
            OnPropertyChanged(nameof(IsTotalWantNoteVariety));
        }

        [RelayCommand]
        void OnTotalWantNoteVariety(bool? e)
        {
            if (e.HasValue)
            {
                for (var i = Configure.Instance.InputWantNoteVariety.Length - 1; i >= 0; --i)
                {
                    Configure.Instance.InputWantNoteVariety[i] = e.Value;
                }
                OnPropertyChanged(nameof(NoteVarietyBMS));
                OnPropertyChanged(nameof(NoteVarietyBMSON));
                OnPropertyChanged(nameof(NoteVarietyEventNote));
                OnPropertyChanged(nameof(IsTotalWantNoteVariety));
            }
        }

        [RelayCommand]
        void OnWantHandled(int e)
        {
            Configure.Instance.InputWantHandled[e] = !Configure.Instance.InputWantHandled[e];
            OnPropertyChanged(nameof(HandledNot));
            OnPropertyChanged(nameof(HandledAssistClear));
            OnPropertyChanged(nameof(HandledClear));
            OnPropertyChanged(nameof(HandledBand1));
            OnPropertyChanged(nameof(HandledYell1));
            OnPropertyChanged(nameof(HandledF));
            OnPropertyChanged(nameof(HandledHigherClear));
            OnPropertyChanged(nameof(HandledHighestClear));
            OnPropertyChanged(nameof(IsTotalWantHandled));
        }

        [RelayCommand]
        void OnTotalWantHandled(bool? e)
        {
            if (e.HasValue)
            {
                for (var i = Configure.Instance.InputWantHandled.Length - 1; i >= 0; --i)
                {
                    Configure.Instance.InputWantHandled[i] = e.Value;
                }
                OnPropertyChanged(nameof(HandledNot));
                OnPropertyChanged(nameof(HandledAssistClear));
                OnPropertyChanged(nameof(HandledClear));
                OnPropertyChanged(nameof(HandledBand1));
                OnPropertyChanged(nameof(HandledYell1));
                OnPropertyChanged(nameof(HandledF));
                OnPropertyChanged(nameof(HandledHigherClear));
                OnPropertyChanged(nameof(HandledHighestClear));
                OnPropertyChanged(nameof(IsTotalWantHandled));
            }
        }

        [RelayCommand]
        void OnWantInputMode(int e)
        {
            Configure.Instance.InputWantInputMode[e] = !Configure.Instance.InputWantInputMode[e];
            OnPropertyChanged(nameof(InputMode4));
            OnPropertyChanged(nameof(InputMode5));
            OnPropertyChanged(nameof(InputMode6));
            OnPropertyChanged(nameof(InputMode7));
            OnPropertyChanged(nameof(InputMode8));
            OnPropertyChanged(nameof(InputMode9));
            OnPropertyChanged(nameof(InputMode51));
            OnPropertyChanged(nameof(InputMode71));
            OnPropertyChanged(nameof(InputMode10));
            OnPropertyChanged(nameof(InputMode102));
            OnPropertyChanged(nameof(InputMode142));
            OnPropertyChanged(nameof(InputMode242));
            OnPropertyChanged(nameof(InputMode484));
            OnPropertyChanged(nameof(IsTotalWantInputMode));
        }

        [RelayCommand]
        void OnTotalWantInputMode(bool? e)
        {
            if (e.HasValue)
            {
                for (var i = Configure.Instance.InputWantInputMode.Length - 1; i >= 0; --i)
                {
                    Configure.Instance.InputWantInputMode[i] = e.Value;
                }
                OnPropertyChanged(nameof(InputMode4));
                OnPropertyChanged(nameof(InputMode5));
                OnPropertyChanged(nameof(InputMode6));
                OnPropertyChanged(nameof(InputMode7));
                OnPropertyChanged(nameof(InputMode8));
                OnPropertyChanged(nameof(InputMode9));
                OnPropertyChanged(nameof(InputMode51));
                OnPropertyChanged(nameof(InputMode71));
                OnPropertyChanged(nameof(InputMode10));
                OnPropertyChanged(nameof(InputMode102));
                OnPropertyChanged(nameof(InputMode142));
                OnPropertyChanged(nameof(InputMode242));
                OnPropertyChanged(nameof(InputMode484));
                OnPropertyChanged(nameof(IsTotalWantInputMode));
            }
        }

        [RelayCommand]
        void OnWantLevel(int e)
        {
            Configure.Instance.InputWantLevel[e] = !Configure.Instance.InputWantLevel[e];
            OnPropertyChanged(nameof(Level0));
            OnPropertyChanged(nameof(Level1));
            OnPropertyChanged(nameof(Level2));
            OnPropertyChanged(nameof(Level3));
            OnPropertyChanged(nameof(Level4));
            OnPropertyChanged(nameof(Level5));
            OnPropertyChanged(nameof(IsTotalWantLevelID));
        }

        [RelayCommand]
        void OnTotalWantLevel(bool? e)
        {
            if (e.HasValue)
            {
                for (var i = Configure.Instance.InputWantLevel.Length - 1; i >= 0; --i)
                {
                    Configure.Instance.InputWantLevel[i] = e.Value;
                }
                OnPropertyChanged(nameof(Level0));
                OnPropertyChanged(nameof(Level1));
                OnPropertyChanged(nameof(Level2));
                OnPropertyChanged(nameof(Level3));
                OnPropertyChanged(nameof(Level4));
                OnPropertyChanged(nameof(Level5));
                OnPropertyChanged(nameof(IsTotalWantLevelID));
            }
        }

        [RelayCommand]
        static void OnLevelWindow() => ViewModels.Instance.LevelValue.Open();

        public void OnLowestWantLevelTextValue()
        {
            Configure.Instance.HighestWantLevelTextValue = Math.Max(Configure.Instance.LowestWantLevelTextValue, Configure.Instance.HighestWantLevelTextValue);
            Configure.Instance.LowestWantLevelTextValue = Math.Min(Configure.Instance.LowestWantLevelTextValue, Configure.Instance.HighestWantLevelTextValue);
        }

        public void OnHighestWantLevelTextValue()
        {
            Configure.Instance.LowestWantLevelTextValue = Math.Min(Configure.Instance.LowestWantLevelTextValue, Configure.Instance.HighestWantLevelTextValue);
            Configure.Instance.HighestWantLevelTextValue = Math.Max(Configure.Instance.LowestWantLevelTextValue, Configure.Instance.HighestWantLevelTextValue);
        }

        public void OnLowestWantBPM()
        {
            Configure.Instance.HighestWantBPM = Math.Max(Configure.Instance.LowestWantBPM, Configure.Instance.HighestWantBPM);
            Configure.Instance.LowestWantBPM = Math.Min(Configure.Instance.LowestWantBPM, Configure.Instance.HighestWantBPM);
        }

        public void OnHighestWantBPM()
        {
            Configure.Instance.LowestWantBPM = Math.Min(Configure.Instance.LowestWantBPM, Configure.Instance.HighestWantBPM);
            Configure.Instance.HighestWantBPM = Math.Max(Configure.Instance.LowestWantBPM, Configure.Instance.HighestWantBPM);
        }

        public void OnLowestWantAverageInputCount()
        {
            Configure.Instance.HighestWantAverageInputCount = Math.Max(Configure.Instance.LowestWantAverageInputCount, Configure.Instance.HighestWantAverageInputCount);
            Configure.Instance.LowestWantAverageInputCount = Math.Min(Configure.Instance.LowestWantAverageInputCount, Configure.Instance.HighestWantAverageInputCount);
        }

        public void OnHighestWantAverageInputCount()
        {
            Configure.Instance.LowestWantAverageInputCount = Math.Min(Configure.Instance.LowestWantAverageInputCount, Configure.Instance.HighestWantAverageInputCount);
            Configure.Instance.HighestWantAverageInputCount = Math.Max(Configure.Instance.LowestWantAverageInputCount, Configure.Instance.HighestWantAverageInputCount);
        }

        public void OnLowestWantHighestInputCount()
        {
            Configure.Instance.HighestWantHighestInputCount = Math.Max(Configure.Instance.LowestWantHighestInputCount, Configure.Instance.HighestWantHighestInputCount);
            Configure.Instance.LowestWantHighestInputCount = Math.Min(Configure.Instance.LowestWantHighestInputCount, Configure.Instance.HighestWantHighestInputCount);
        }

        public void OnHighestWantHighestInputCount()
        {
            Configure.Instance.LowestWantHighestInputCount = Math.Min(Configure.Instance.LowestWantHighestInputCount, Configure.Instance.HighestWantHighestInputCount);
            Configure.Instance.HighestWantHighestInputCount = Math.Max(Configure.Instance.LowestWantHighestInputCount, Configure.Instance.HighestWantHighestInputCount);
        }

        public Brush NoteVarietyBMS => Paints.PointPaints[Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMS] ? 1 : 0];

        public Brush NoteVarietyBMSON => Paints.PointPaints[Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMSON] ? 1 : 0];

        public Brush NoteVarietyEventNote => Paints.PointPaints[Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote] ? 1 : 0];

        public bool IsTotalWantNoteVariety => Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMS] ||
            Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMSON] ||
            Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote];

        public Brush HandledNot => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Not] ? 1 : 0];

        public Brush HandledAssistClear => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.AssistClear] ? 1 : 0];

        public Brush HandledClear => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Clear] ? 1 : 0];

        public Brush HandledBand1 => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Band1] ? 1 : 0];

        public Brush HandledYell1 => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Yell1] ? 1 : 0];

        public Brush HandledF => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.F] ? 1 : 0];

        public Brush HandledHigherClear => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.HigherClear] ? 1 : 0];

        public Brush HandledHighestClear => Paints.PointPaints[Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.HighestClear] ? 1 : 0];

        public bool IsTotalWantHandled => Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Not] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.AssistClear] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Clear] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Band1] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.F] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.HigherClear] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.HighestClear]||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.ID.Yell1];

        public Brush Level0 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level0] ? 1 : 0];

        public Brush Level1 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level1] ? 1 : 0];

        public Brush Level2 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level2] ? 1 : 0];

        public Brush Level3 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level3] ? 1 : 0];

        public Brush Level4 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level4] ? 1 : 0];

        public Brush Level5 => Paints.PointPaints[Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level5] ? 1 : 0];

        public bool IsTotalWantLevelID => Configure.Instance.InputWantLevel.Any(inputWantLevel => inputWantLevel);

        public Brush InputMode4 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._4] ? 1 : 0];

        public Brush InputMode5 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._5] ? 1 : 0];

        public Brush InputMode6 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._6] ? 1 : 0];

        public Brush InputMode7 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._7] ? 1 : 0];

        public Brush InputMode8 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._8] ? 1 : 0];

        public Brush InputMode9 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._9] ? 1 : 0];

        public Brush InputMode10 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._10] ? 1 : 0];

        public Brush InputMode51 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._5_1] ? 1 : 0];

        public Brush InputMode71 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._7_1] ? 1 : 0];

        public Brush InputMode102 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._10_2] ? 1 : 0];

        public Brush InputMode142 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._14_2] ? 1 : 0];

        public Brush InputMode242 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._24_2] ? 1 : 0];

        public Brush InputMode484 => Paints.PointPaints[Configure.Instance.InputWantInputMode[(int)Component.InputMode._48_4] ? 1 : 0];

        public bool IsTotalWantInputMode => Configure.Instance.InputWantInputMode.Any(inputWantInputMode => inputWantInputMode);

        public override bool ClosingCondition
        {
            get
            {
                if (!IsTotalWantNoteVariety)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantNoteVarietyContents);
                    return false;
                }

                if (!IsTotalWantHandled)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantHandledContents);
                    return false;
                }

                if (!IsTotalWantLevelID)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantLevelContents);
                    return false;
                }

                if (!IsTotalWantInputMode)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantModeContents);
                    return false;
                }

                return base.ClosingCondition;
            }
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            Configure.Instance.Save(true);
            Configure.Instance.NotifyInputWantWindowPaint();
            ViewModels.Instance.MainValue.Want();
        }
    }
}