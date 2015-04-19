using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpBoiler.Converter
{
    class IsLessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int parameterInt;
            int.TryParse((string)parameter, out parameterInt);

            return ((int)value < (int)parameterInt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int parameterInt;
            int.TryParse((string)parameter, out parameterInt);
            return ((int)value < (int)parameterInt);
        }
    }
}
