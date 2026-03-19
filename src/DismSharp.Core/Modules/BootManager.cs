using Microsoft.Win32;

namespace DismSharp.Core.Modules;

/// <summary>启动项信息</summary>
public record StartupItem(
    string Name,
    string Command,
    string Location,
    bool Enabled);

/// <summary>启动项管理器（读写注册表 Run/RunOnce）</summary>
public static class BootManager
{
    private static readonly string[] RunKeys = [
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\RunOnce"
    ];

    /// <summary>获取所有启动项</summary>
    public static async Task<List<StartupItem>> GetStartupItemsAsync()
    {
        return await Task.Run(() =>
        {
            var items = new List<StartupItem>();

            // 当前用户
            foreach (var keyPath in RunKeys)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(keyPath);
                    if (key == null) continue;

                    foreach (var name in key.GetValueNames())
                    {
                        var command = key.GetValue(name)?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(command))
                        {
                            items.Add(new StartupItem(name, command, $"HKCU\\{keyPath}", true));
                        }
                    }
                }
                catch { }
            }

            // 本地机器（需要管理员权限）
            foreach (var keyPath in RunKeys)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                    if (key == null) continue;

                    foreach (var name in key.GetValueNames())
                    {
                        var command = key.GetValue(name)?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(command))
                        {
                            items.Add(new StartupItem(name, command, $"HKLM\\{keyPath}", true));
                        }
                    }
                }
                catch { }
            }

            return items;
        }).ConfigureAwait(false);
    }

    /// <summary>禁用启动项（移到 Run-Disabled 备份键）</summary>
    public static async Task DisableStartupItemAsync(StartupItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.Location.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var sourcePath = item.Location.Replace("HKLM\\", "").Replace("HKCU\\", "");
            var backupPath = sourcePath + "-Disabled";

            using var sourceKey = hive.OpenSubKey(sourcePath, true);
            if (sourceKey == null) return;

            var command = sourceKey.GetValue(item.Name)?.ToString();
            if (command == null) return;

            // 备份到 Disabled 键
            using var backupKey = hive.CreateSubKey(backupPath);
            backupKey.SetValue(item.Name, command);

            // 删除原键值
            sourceKey.DeleteValue(item.Name, false);
        }).ConfigureAwait(false);
    }

    /// <summary>启用启动项（从 Run-Disabled 恢复）</summary>
    public static async Task EnableStartupItemAsync(StartupItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.Location.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var backupPath = item.Location.Replace("HKLM\\", "").Replace("HKCU\\", "") + "-Disabled";
            var targetPath = item.Location.Replace("HKLM\\", "").Replace("HKCU\\", "");

            using var backupKey = hive.OpenSubKey(backupPath, true);
            if (backupKey == null) return;

            var command = backupKey.GetValue(item.Name)?.ToString();
            if (command == null) return;

            // 恢复到原键
            using var targetKey = hive.CreateSubKey(targetPath);
            targetKey.SetValue(item.Name, command);

            // 删除备份
            backupKey.DeleteValue(item.Name, false);
        }).ConfigureAwait(false);
    }

    /// <summary>删除启动项</summary>
    public static async Task DeleteStartupItemAsync(StartupItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.Location.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var keyPath = item.Location.Replace("HKLM\\", "").Replace("HKCU\\", "");

            using var key = hive.OpenSubKey(keyPath, true);
            key?.DeleteValue(item.Name, false);
        }).ConfigureAwait(false);
    }
}
