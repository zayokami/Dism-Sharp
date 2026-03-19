using Microsoft.Win32;

namespace DismSharp.Core.Modules;

public static class WidgetsManager
{
    private const string WidgetsKey = @"SOFTWARE\Microsoft\PolicyManager\default\NewsAndInterests\AllowNewsAndInterests";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(WidgetsKey);
            var value = key?.GetValue("Behavior")?.ToString();
            return value != "0";
        }
        catch
        {
            return true;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        try
        {
            using var key = Registry.LocalMachine.CreateSubKey(WidgetsKey);
            key?.SetValue("Behavior", enabled ? "1" : "0", RegistryValueKind.DWord);
        }
        catch { }
    }

    public static bool IsTaskbarButtonEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
            var value = key?.GetValue("TaskbarDa")?.ToString();
            return value != "0";
        }
        catch
        {
            return true;
        }
    }

    public static void SetTaskbarButtonEnabled(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
            key?.SetValue("TaskbarDa", enabled ? "1" : "0", RegistryValueKind.DWord);
        }
        catch { }
    }
}
