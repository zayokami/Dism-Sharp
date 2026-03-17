using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DismSharp.Core.Native;

namespace DismSharp.UI.Converters;

/// <summary>将 DismPackageFeatureState 转换为状态指示颜色</summary>
public class FeatureStateToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Green = new(Color.FromRgb(0x4C, 0xAF, 0x50));
    private static readonly SolidColorBrush Gray = new(Color.FromRgb(0x9E, 0x9E, 0x9E));
    private static readonly SolidColorBrush Orange = new(Color.FromRgb(0xFF, 0x98, 0x00));
    private static readonly SolidColorBrush Red = new(Color.FromRgb(0xF4, 0x43, 0x36));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DismPackageFeatureState state)
        {
            return state switch
            {
                DismPackageFeatureState.DismStateInstalled => Green,
                DismPackageFeatureState.DismStateNotPresent => Gray,
                DismPackageFeatureState.DismStateStaged => Gray,
                DismPackageFeatureState.DismStateRemoved => Gray,
                DismPackageFeatureState.DismStateInstallPending => Orange,
                DismPackageFeatureState.DismStateUninstallPending => Orange,
                DismPackageFeatureState.DismStateSuperseded => Orange,
                DismPackageFeatureState.DismStatePartiallyInstalled => Red,
                _ => Gray
            };
        }
        return Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
