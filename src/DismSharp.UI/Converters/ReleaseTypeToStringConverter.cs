using System.Globalization;
using System.Windows.Data;
using DismSharp.Core.Native;

namespace DismSharp.UI.Converters;

/// <summary>将 DismReleaseType 转换为中文显示文本</summary>
public class ReleaseTypeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DismReleaseType type)
        {
            return type switch
            {
                DismReleaseType.DismReleaseTypeCriticalUpdate => "关键更新",
                DismReleaseType.DismReleaseTypeDriver => "驱动",
                DismReleaseType.DismReleaseTypeFeaturePack => "功能包",
                DismReleaseType.DismReleaseTypeHotfix => "热修复",
                DismReleaseType.DismReleaseTypeSecurityUpdate => "安全更新",
                DismReleaseType.DismReleaseTypeSoftwareUpdate => "软件更新",
                DismReleaseType.DismReleaseTypeUpdate => "更新",
                DismReleaseType.DismReleaseTypeUpdateRollup => "更新汇总",
                DismReleaseType.DismReleaseTypeLanguagePack => "语言包",
                DismReleaseType.DismReleaseTypeFoundation => "基础",
                DismReleaseType.DismReleaseTypeServicePack => "服务包",
                DismReleaseType.DismReleaseTypeProduct => "产品",
                DismReleaseType.DismReleaseTypeLocalPack => "本地包",
                DismReleaseType.DismReleaseTypeOther => "其他",
                DismReleaseType.DismReleaseTypeOnDemandPack => "按需功能包",
                _ => type.ToString()
            };
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
