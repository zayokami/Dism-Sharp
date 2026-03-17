using System.Globalization;
using System.Windows.Data;

namespace DismSharp.UI.Converters;

/// <summary>将 bool 取反</summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is not true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is not true;
}
