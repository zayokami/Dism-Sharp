using Microsoft.Win32;

namespace DismSharp.Core.Modules;

public static class CopilotManager
{
    private const string CopilotKey = @"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(CopilotKey);
            var value = key?.GetValue("TurnOffWindowsCopilot")?.ToString();
            return value != "1";
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
            using var key = Registry.LocalMachine.CreateSubKey(CopilotKey);
            key?.SetValue("TurnOffWindowsCopilot", enabled ? "0" : "1", RegistryValueKind.DWord);
        }
        catch { }
    }

    public static bool IsTaskbarButtonEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
            var value = key?.GetValue("ShowCopilotButton")?.ToString();
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
            key?.SetValue("ShowCopilotButton", enabled ? "1" : "0", RegistryValueKind.DWord);
        }
        catch { }
    }
}
