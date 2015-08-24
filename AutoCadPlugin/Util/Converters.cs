using System;
using System.Windows.Data;

namespace AutoCadPlugin.Util
{
    /// <summary>
    /// Класс для конвертирования наличия выделенного объекта в булевый тип.
    /// </summary>
    public class SelectedItemToVisibility : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            bool visible = true;

            if (value == null)
            {
                visible = false;
            }

            if (visible)
                return System.Windows.Visibility.Visible;
            else
                return System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }    
}
