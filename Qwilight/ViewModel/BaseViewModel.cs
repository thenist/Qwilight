﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.Utilities;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.ViewModel
{
    public partial class BaseViewModel : Model
    {
        static int _lastZvalue;

        double _faint = 1.0;
        int _zValue;
        bool _isControlling;
        bool _wasSilentlyClosed;

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
                    ViewModels.Instance.MainValue.SetWPFViewVisibility();
                }
            }
        }

        public double Length => ViewModels.Instance.MainValue.DefaultLength * TargetLength;

        public double Height => ViewModels.Instance.MainValue.DefaultHeight * TargetHeight;

        public virtual double TargetLength => 1.0;

        public virtual double TargetHeight => 1.0;

        public virtual HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public virtual VerticalAlignment HeightSystem => VerticalAlignment.Center;

        public double Faint
        {
            get => _faint;

            set => SetProperty(ref _faint, value, nameof(Faint));
        }

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
                OnCollapsed();
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
                Zvalue = Interlocked.Increment(ref _lastZvalue);
                IsControlling = true;
                OnOpened();
            }
        }

        public void CloseSilently()
        {
            if (IsOpened)
            {
                Close(false);
                _wasSilentlyClosed = true;
            }
        }

        public void OpenSilently()
        {
            if (_wasSilentlyClosed)
            {
                Open(false);
                _wasSilentlyClosed = false;
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
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Height));
        }

        public virtual void OnOpened()
        {
            StrongReferenceMessenger.Default.Send<PointZMaxView>();
            IsLoaded = true;
        }

        public virtual void OnCollapsed() => StrongReferenceMessenger.Default.Send<PointZMaxView>();
    }
}