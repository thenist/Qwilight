﻿using Qwilight.ViewModel;
using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class LowerStandPaintModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ViewModels.Instance.ModifyModeComponentValue.ModeComponentValues[(int)parameter]?.Single(modeComponentItem => modeComponentItem.Value == (int)value)?.PointedPaint;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}