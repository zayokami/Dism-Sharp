using Microsoft.Win32;

namespace DismSharp.Core.Modules;

/// <summary>右键菜单项信息</summary>
public record ContextMenuItem(
    string Name,
    string Command,
    string KeyPath,
    string FileType,
    bool Enabled);

/// <summary>右键菜单管理器（读写注册表 Shell/ShellEx）</summary>
public static class ContextMenuManager
{
    private static readonly string[] FileTypePaths = [
        @"*\shell",
        @"*\shellex\ContextMenuHandlers",
        @"Directory\shell",
        @"Directory\shellex\ContextMenuHandlers",
        @"Directory\Background\shell",
        @"Folder\shell",
        @"Folder\shellex\ContextMenuHandlers",
        @"DesktopBackground\shell"
    ];

    /// <summary>获取所有右键菜单项</summary>
    public static async Task<List<ContextMenuItem>> GetContextMenuItemsAsync()
    {
        return await Task.Run(() =>
        {
            var items = new List<ContextMenuItem>();

            // 当前用户
            foreach (var basePath in FileTypePaths)
            {
                ScanRegistryKey(Registry.CurrentUser, basePath, $"HKCU\\{basePath}", items);
            }

            // 本地机器
            foreach (var basePath in FileTypePaths)
            {
                ScanRegistryKey(Registry.LocalMachine, basePath, $"HKLM\\{basePath}", items);
            }

            return items;
        }).ConfigureAwait(false);
    }

    private static void ScanRegistryKey(RegistryKey hive, string keyPath, string displayPath, List<ContextMenuItem> items)
    {
        try
        {
            using var key = hive.OpenSubKey(keyPath);
            if (key == null) return;

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                try
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey == null) continue;

                    var command = "";

                    // 检查 shell\Name\command
                    using var cmdKey = subKey.OpenSubKey("command");
                    if (cmdKey != null)
                    {
                        command = cmdKey.GetValue("")?.ToString() ?? "";
                    }

                    // 如果没有 command 子键，取默认值
                    if (string.IsNullOrEmpty(command))
                    {
                        command = subKey.GetValue("")?.ToString() ?? "";
                    }

                    if (!string.IsNullOrEmpty(command))
                    {
                        var fileType = GetFileTypeFromPath(displayPath);
                        items.Add(new ContextMenuItem(subKeyName, command, $"{displayPath}\\{subKeyName}", fileType, true));
                    }
                }
                catch { }
            }
        }
        catch { }
    }

    private static string GetFileTypeFromPath(string path)
    {
        if (path.Contains("*")) return "所有文件";
        if (path.Contains("Directory")) return "文件夹";
        if (path.Contains("DesktopBackground")) return "桌面";
        if (path.Contains("Folder")) return "文件夹";
        return "其他";
    }

    /// <summary>禁用右键菜单项</summary>
    public static async Task DisableMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.KeyPath.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var keyPath = item.KeyPath.Replace("HKLM\\", "").Replace("HKCU\\", "");

            // 获取父键路径和子键名称
            var lastSlash = keyPath.LastIndexOf('\\');
            var parentPath = keyPath[..lastSlash];
            var subKeyName = keyPath[(lastSlash + 1)..];

            using var parentKey = hive.OpenSubKey(parentPath, true);
            if (parentKey == null) return;

            // 添加 LegacyDisable 值来禁用
            using var subKey = parentKey.OpenSubKey(subKeyName, true);
            subKey?.SetValue("LegacyDisable", "", RegistryValueKind.String);
        }).ConfigureAwait(false);
    }

    /// <summary>启用右键菜单项</summary>
    public static async Task EnableMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.KeyPath.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var keyPath = item.KeyPath.Replace("HKLM\\", "").Replace("HKCU\\", "");

            var lastSlash = keyPath.LastIndexOf('\\');
            var parentPath = keyPath[..lastSlash];
            var subKeyName = keyPath[(lastSlash + 1)..];

            using var parentKey = hive.OpenSubKey(parentPath, true);
            if (parentKey == null) return;

            using var subKey = parentKey.OpenSubKey(subKeyName, true);
            subKey?.DeleteValue("LegacyDisable", false);
        }).ConfigureAwait(false);
    }

    /// <summary>删除右键菜单项</summary>
    public static async Task DeleteMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var hive = item.KeyPath.StartsWith("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
            var keyPath = item.KeyPath.Replace("HKLM\\", "").Replace("HKCU\\", "");

            var lastSlash = keyPath.LastIndexOf('\\');
            var parentPath = keyPath[..lastSlash];
            var subKeyName = keyPath[(lastSlash + 1)..];

            using var parentKey = hive.OpenSubKey(parentPath, true);
            parentKey?.DeleteSubKeyTree(subKeyName, false);
        }).ConfigureAwait(false);
    }
}
