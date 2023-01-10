using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Converters
{
    internal class ColorTypeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ColorType)value switch
            {
                ColorType.C1 => Brushes.White,
                ColorType.C2 => Brushes.Green,
                ColorType.C3 => Brushes.Red,
                ColorType.C4 => Brushes.Blue,
                ColorType.C5 => Brushes.Orange,
                ColorType.C6 => Brushes.Yellow,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
