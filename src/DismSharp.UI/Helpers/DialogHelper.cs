using System.Windows;

namespace DismSharp.UI.Helpers;

public static class DialogHelper
{
    public static bool Confirm(string message, string title = "确认")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public static bool ConfirmDangerous(string message, string title = "警告")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    public static void ShowInfo(string message, string title = "提示")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public static void ShowError(string message, string title = "错误")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
