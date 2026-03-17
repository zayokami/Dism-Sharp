using System.Globalization;
using System.Windows.Data;
using DismSharp.Core.Native;

namespace DismSharp.UI.Converters;

/// <summary>将 DismDriverSignature 转换为中文显示文本</summary>
public class DriverSignatureToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DismDriverSignature sig)
        {
            return sig switch
            {
                DismDriverSignature.DismDriverSignatureSigned => "已签名",
                DismDriverSignature.DismDriverSignatureUnsigned => "未签名",
                DismDriverSignature.DismDriverSignatureUnknown => "未知",
                _ => sig.ToString()
            };
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
