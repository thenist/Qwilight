using CommunityToolkit.Mvvm.Input;
using Qwilight.NoteFile;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed partial class WantViewModel : BaseViewModel
    {
        public override double TargetLength => 0.9;

        [RelayCommand]
        void OnWantHellBPM() => Configure.Instance.WantHellBPM = !Configure.Instance.WantHellBPM;

        [RelayCommand]
        void OnWantBanned() => Configure.Instance.WantBannedValue = (Configure.WantBanned)(((int)Configure.Instance.WantBannedValue + 1) % 2);

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
            OnPropertyChanged(nameof(HandledClear));
            OnPropertyChanged(nameof(HandledBand1));
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
                OnPropertyChanged(nameof(HandledClear));
                OnPropertyChanged(nameof(HandledBand1));
                OnPropertyChanged(nameof(HandledF));
                OnPropertyChanged(nameof(HandledHigherClear));
                OnPropertyChanged(nameof(HandledHighestClear));
                OnPropertyChanged(nameof(IsTotalWantHandled));
            }
        }

        [RelayCommand]
        void OnWantMode(int e)
        {
            Configure.Instance.InputWantMode[e] = !Configure.Instance.InputWantMode[e];
            OnPropertyChanged(nameof(Mode4));
            OnPropertyChanged(nameof(Mode5));
            OnPropertyChanged(nameof(Mode6));
            OnPropertyChanged(nameof(Mode7));
            OnPropertyChanged(nameof(Mode8));
            OnPropertyChanged(nameof(Mode9));
            OnPropertyChanged(nameof(Mode51));
            OnPropertyChanged(nameof(Mode71));
            OnPropertyChanged(nameof(Mode10));
            OnPropertyChanged(nameof(Mode102));
            OnPropertyChanged(nameof(Mode142));
            OnPropertyChanged(nameof(Mode242));
            OnPropertyChanged(nameof(Mode484));
            OnPropertyChanged(nameof(IsTotalWantMode));
        }

        [RelayCommand]
        void OnTotalWantMode(bool? e)
        {
            if (e.HasValue)
            {
                for (var i = Configure.Instance.InputWantMode.Length - 1; i >= 0; --i)
                {
                    Configure.Instance.InputWantMode[i] = e.Value;
                }
                OnPropertyChanged(nameof(Mode4));
                OnPropertyChanged(nameof(Mode5));
                OnPropertyChanged(nameof(Mode6));
                OnPropertyChanged(nameof(Mode7));
                OnPropertyChanged(nameof(Mode8));
                OnPropertyChanged(nameof(Mode9));
                OnPropertyChanged(nameof(Mode51));
                OnPropertyChanged(nameof(Mode71));
                OnPropertyChanged(nameof(Mode10));
                OnPropertyChanged(nameof(Mode102));
                OnPropertyChanged(nameof(Mode142));
                OnPropertyChanged(nameof(Mode242));
                OnPropertyChanged(nameof(Mode484));
                OnPropertyChanged(nameof(IsTotalWantMode));
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
            OnPropertyChanged(nameof(IsTotalWantLevel));
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
                OnPropertyChanged(nameof(IsTotalWantLevel));
            }
        }

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

        [RelayCommand]
        void OnLevelWindow() => ViewModels.Instance.LevelValue.Open();

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

        public Thickness NoteVarietyBMS => new Thickness(Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMS] ? 1.0 : 0.0);

        public Thickness NoteVarietyBMSON => new Thickness(Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMSON] ? 1.0 : 0.0);

        public Thickness NoteVarietyEventNote => new Thickness(Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote] ? 1.0 : 0.0);

        public bool IsTotalWantNoteVariety => Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMS] ||
            Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMSON] ||
            Configure.Instance.InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote];

        public Thickness HandledNot => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Not] ? 1.0 : 0.0);

        public Thickness HandledClear => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Clear] ? 1.0 : 0.0);

        public Thickness HandledBand1 => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Band1] ? 1.0 : 0.0);

        public Thickness HandledF => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.F] ? 1.0 : 0.0);

        public Thickness HandledHigherClear => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.HigherClear] ? 1.0 : 0.0);

        public Thickness HandledHighestClear => new Thickness(Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.HighestClear] ? 1.0 : 0.0);

        public bool IsTotalWantHandled => Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Not] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Clear] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.Band1] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.F] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.HigherClear] ||
            Configure.Instance.InputWantHandled[(int)BaseNoteFile.Handled.HighestClear];

        public Thickness Level0 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level0] ? 1.0 : 0.0);

        public Thickness Level1 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level1] ? 1.0 : 0.0);

        public Thickness Level2 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level2] ? 1.0 : 0.0);

        public Thickness Level3 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level3] ? 1.0 : 0.0);

        public Thickness Level4 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level4] ? 1.0 : 0.0);

        public Thickness Level5 => new Thickness(Configure.Instance.InputWantLevel[(int)BaseNoteFile.Level.Level5] ? 1.0 : 0.0);

        public bool IsTotalWantLevel => Configure.Instance.InputWantLevel.Any(inputWantLevel => inputWantLevel);

        public Thickness Mode4 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode4] ? 1.0 : 0.0);

        public Thickness Mode5 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode5] ? 1.0 : 0.0);

        public Thickness Mode6 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode6] ? 1.0 : 0.0);

        public Thickness Mode7 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode7] ? 1.0 : 0.0);

        public Thickness Mode8 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode8] ? 1.0 : 0.0);

        public Thickness Mode9 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode9] ? 1.0 : 0.0);

        public Thickness Mode51 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode51] ? 1.0 : 0.0);

        public Thickness Mode71 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode71] ? 1.0 : 0.0);

        public Thickness Mode10 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode10] ? 1.0 : 0.0);

        public Thickness Mode102 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode102] ? 1.0 : 0.0);

        public Thickness Mode142 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode142] ? 1.0 : 0.0);

        public Thickness Mode242 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode242] ? 1.0 : 0.0);

        public Thickness Mode484 => new Thickness(Configure.Instance.InputWantMode[(int)Component.InputMode.InputMode484] ? 1.0 : 0.0);

        public bool IsTotalWantMode => Configure.Instance.InputWantMode.Any(inputWantMode => inputWantMode);

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

                if (!IsTotalWantLevel)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantLevelContents);
                    return false;
                }

                if (!IsTotalWantMode)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotWantModeContents);
                    return false;
                }

                return base.ClosingCondition;
            }
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            Configure.Instance.NotifyInputWantWindowPaint();
            ViewModels.Instance.MainValue.Want();
        }
    }
}