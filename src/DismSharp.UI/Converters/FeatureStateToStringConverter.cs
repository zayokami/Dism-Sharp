using System.Globalization;
using System.Windows.Data;
using DismSharp.Core.Native;

namespace DismSharp.UI.Converters;

/// <summary>将 DismPackageFeatureState 转换为中文显示文本</summary>
public class FeatureStateToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DismPackageFeatureState state)
        {
            return state switch
            {
                DismPackageFeatureState.DismStateInstalled => "已启用",
                DismPackageFeatureState.DismStateNotPresent => "未启用",
                DismPackageFeatureState.DismStateStaged => "已暂存",
                DismPackageFeatureState.DismStateInstallPending => "启用挂起",
                DismPackageFeatureState.DismStateUninstallPending => "禁用挂起",
                DismPackageFeatureState.DismStateSuperseded => "已被取代",
                DismPackageFeatureState.DismStatePartiallyInstalled => "部分安装",
                DismPackageFeatureState.DismStateRemoved => "已移除",
                _ => state.ToString()
            };
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
