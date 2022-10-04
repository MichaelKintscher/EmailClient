using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Converters
{
    internal class BoolToInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = (bool)value;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool convertedValue;

            if (value is bool == false)
            {
                return DependencyProperty.UnsetValue;
            }

            convertedValue = !(bool)value;

            return convertedValue;
        }
    }
}
