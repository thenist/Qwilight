using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.Utilities;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public partial class BaseViewModel : Model
    {
        static int _lastZvalue;

        int _zValue;
        bool _isControlling;

        public bool IsLoaded { get; set; }

        public bool IsOpened => IsControlling && OpeningCondition;

        public bool IsControlling
        {
            get => _isControlling;

            set
            {
                if (SetProperty(ref _isControlling, value, nameof(IsControlling)))
                {
                    NotifyIsOpened();
                    ViewModels.Instance.MainValue.OnWPFViewVisibilityModified();
                }
            }
        }

        public double Length => ViewModels.Instance.MainValue.DefaultLength * TargetLength;

        public double Height => ViewModels.Instance.MainValue.DefaultHeight * TargetHeight;

        public double Position0 => TargetLengthSystem switch
        {
            HorizontalAlignment.Right => ViewModels.Instance.MainValue.DefaultLength - Length,
            HorizontalAlignment.Center => (ViewModels.Instance.MainValue.DefaultLength - Length) / 2,
            _ => 0.0
        };

        public double Position1 => TargetHeightSystem switch
        {
            VerticalAlignment.Bottom => ViewModels.Instance.MainValue.DefaultHeight - Height,
            VerticalAlignment.Center => (ViewModels.Instance.MainValue.DefaultHeight - Height) / 2,
            _ => 0.0
        };

        public virtual double TargetLength => 1.0;

        public virtual double TargetHeight => 1.0;

        public virtual HorizontalAlignment TargetLengthSystem => HorizontalAlignment.Center;

        public virtual VerticalAlignment TargetHeightSystem => VerticalAlignment.Center;

        public int Zvalue
        {
            get => _zValue;

            set => SetProperty(ref _zValue, value, nameof(Zvalue));
        }

        public virtual bool ClosingCondition => true;

        public virtual bool OpeningCondition => true;

        public virtual bool IsModal => true;

        public Brush ModalPaint => IsModal ? Paints.ModalPaint : null;

        [RelayCommand]
        void OnClose() => Close();

        public void Close(bool handleAudio = true)
        {
            if (IsOpened && ClosingCondition)
            {
                if (handleAudio)
                {
                    Utility.HandleUIAudio("Window 0");
                }
                IsControlling = false;
                OnCollasped();
            }
        }

        public void Open(bool handleAudio = true)
        {
            if (!IsOpened && OpeningCondition)
            {
                if (handleAudio)
                {
                    Utility.HandleUIAudio("Window 1");
                }
                Zvalue = ++_lastZvalue;
                IsControlling = true;
                OnOpened();
            }
        }

        public void Toggle()
        {
            if (IsOpened)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void NotifyIsOpened() => OnPropertyChanged(nameof(IsOpened));

        public void NotifyArea()
        {
            OnPropertyChanged(nameof(Position0));
            OnPropertyChanged(nameof(Position1));
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Height));
        }

        public virtual void OnOpened()
        {
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.PointZMaxView
            });
            IsLoaded = true;
        }

        public virtual void OnCollasped() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.PointZMaxView
        });
    }
}