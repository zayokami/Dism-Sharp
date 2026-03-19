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
    // 主要的右键菜单位置
    private static readonly (string Path, string FileType)[] ScanPaths = [
        (@"*\shell", "所有文件"),
        (@"*\shellex\ContextMenuHandlers", "所有文件(SH)"),
        (@"*\shellex\PropertySheetHandlers", "所有文件(属性)"),
        (@"AllFilesystemObjects\shell", "文件系统对象"),
        (@"Directory\shell", "文件夹"),
        (@"Directory\shellex\ContextMenuHandlers", "文件夹(SH)"),
        (@"Directory\Background\shell", "文件夹背景"),
        (@"Folder\shell", "文件夹(旧)"),
        (@"Folder\shellex\ContextMenuHandlers", "文件夹(旧)(SH)"),
        (@"DesktopBackground\shell", "桌面"),
        (@"DesktopBackground\shellex\ContextMenuHandlers", "桌面(SH)"),
        (@"LibraryFolder\shell", "库文件夹"),
        (@"LibraryFolder\Background\shell", "库背景"),
        (@"Drive\shell", "磁盘驱动器"),
        (@"Drive\shellex\ContextMenuHandlers", "磁盘(SH)"),
    ];

    // 常见文件扩展名的右键菜单
    private static readonly string[] CommonExtensions = [
        @".txt", @".html", @".htm", @".pdf", @".jpg", @".jpeg", @".png",
        @".gif", @".bmp", @".mp3", @".mp4", @".avi", @".zip", @".rar",
        @".exe", @".dll", @".sys", @".log", @".xml", @".json", @".csv"
    ];

    /// <summary>获取所有右键菜单项</summary>
    public static async Task<List<ContextMenuItem>> GetContextMenuItemsAsync()
    {
        return await Task.Run(() =>
        {
            var items = new List<ContextMenuItem>();

            // 扫描 HKEY_CLASSES_ROOT（合并视图）
            foreach (var (path, fileType) in ScanPaths)
            {
                ScanShellKey(Registry.ClassesRoot, path, fileType, items);
            }

            // 扫描 HKCU 自定义菜单（Software\Classes 下用户覆盖）
            foreach (var (path, fileType) in ScanPaths)
            {
                ScanShellKey(Registry.CurrentUser, @"Software\Classes\" + path, fileType, items);
            }

            // 扫描用户自定义菜单（文件扩展关联覆盖）
            ScanShellKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts", "文件扩展关联", items);

            // 扫描常见扩展名
            foreach (var ext in CommonExtensions)
            {
                ScanShellKey(Registry.ClassesRoot, ext + @"\shell", $"扩展名({ext})", items);
            }

            return items;
        }).ConfigureAwait(false);
    }

    private static void ScanShellKey(RegistryKey hive, string keyPath, string fileType, List<ContextMenuItem> items)
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

                    var command = GetCommand(subKey);
                    var displayName = GetDisplayName(subKey, subKeyName);

                    // 检查是否被禁用
                    var isEnabled = subKey.GetValue("LegacyDisable") == null
                                 && subKey.GetValue("ProgrammaticAccessOnly") == null;

                    if (!string.IsNullOrEmpty(command))
                    {
                        items.Add(new ContextMenuItem(displayName, command, $"{keyPath}\\{subKeyName}", fileType, isEnabled));
                    }
                }
                catch { }
            }
        }
        catch { }
    }

    private static string GetCommand(RegistryKey subKey)
    {
        // 1. 检查 command 子键
        using var cmdKey = subKey.OpenSubKey("command");
        if (cmdKey != null)
        {
            var cmd = cmdKey.GetValue("")?.ToString();
            if (!string.IsNullOrEmpty(cmd)) return cmd;
        }

        // 2. 检查 DelegateExecute（常见的替代方式）
        var delegateExecute = subKey.GetValue("DelegateExecute")?.ToString();
        if (!string.IsNullOrEmpty(delegateExecute))
            return $"[Delegate: {delegateExecute[..Math.Min(32, delegateExecute.Length)]}...]";

        // 3. 检查默认值
        var defaultCmd = subKey.GetValue("")?.ToString();
        if (!string.IsNullOrEmpty(defaultCmd) && defaultCmd.Length > 3)
            return defaultCmd;

        // 4. 检查 Icon（可能是 ShellEx 类型）
        var icon = subKey.GetValue("Icon")?.ToString();
        if (!string.IsNullOrEmpty(icon))
            return $"[ShellEx]";

        return "";
    }

    private static string GetDisplayName(RegistryKey subKey, string fallbackName)
    {
        // 优先用 MUIVerb（多语言显示名）
        var muiVerb = subKey.GetValue("MUIVerb")?.ToString();
        if (!string.IsNullOrEmpty(muiVerb))
        {
            // 解析 @resource.dll,-ID 格式
            if (muiVerb.StartsWith('@'))
                return fallbackName; // 无法解析资源，用键名
            return muiVerb;
        }

        // 用默认值（如果不是命令）
        var defaultValue = subKey.GetValue("")?.ToString();
        if (!string.IsNullOrEmpty(defaultValue)
            && !defaultValue.Contains('%')
            && !defaultValue.Contains("rundll32")
            && defaultValue.Length < 100)
            return defaultValue;

        return fallbackName;
    }

    /// <summary>禁用右键菜单项</summary>
    public static async Task DisableMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var (hive, subPath) = ParseKeyPath(item.KeyPath);
            var (parentPath, subKeyName) = SplitPath(subPath);

            using var parentKey = hive.OpenSubKey(parentPath, true);
            if (parentKey == null) return;

            using var subKey = parentKey.OpenSubKey(subKeyName, true);
            subKey?.SetValue("LegacyDisable", "", RegistryValueKind.String);
        }).ConfigureAwait(false);
    }

    /// <summary>启用右键菜单项</summary>
    public static async Task EnableMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var (hive, subPath) = ParseKeyPath(item.KeyPath);
            var (parentPath, subKeyName) = SplitPath(subPath);

            using var parentKey = hive.OpenSubKey(parentPath, true);
            if (parentKey == null) return;

            using var subKey = parentKey.OpenSubKey(subKeyName, true);
            subKey?.DeleteValue("LegacyDisable", false);
            subKey?.DeleteValue("ProgrammaticAccessOnly", false);
        }).ConfigureAwait(false);
    }

    /// <summary>删除右键菜单项</summary>
    public static async Task DeleteMenuItemAsync(ContextMenuItem item)
    {
        await Task.Run(() =>
        {
            var (hive, subPath) = ParseKeyPath(item.KeyPath);
            var (parentPath, subKeyName) = SplitPath(subPath);

            using var parentKey = hive.OpenSubKey(parentPath, true);
            parentKey?.DeleteSubKeyTree(subKeyName, false);
        }).ConfigureAwait(false);
    }

    private static (RegistryKey hive, string subPath) ParseKeyPath(string fullPath)
    {
        if (fullPath.StartsWith("HKLM\\"))
            return (Registry.LocalMachine, fullPath[5..]);
        if (fullPath.StartsWith("HKCU\\"))
            return (Registry.CurrentUser, fullPath[5..]);
        if (fullPath.StartsWith("HKCR\\"))
            return (Registry.ClassesRoot, fullPath[5..]);
        return (Registry.CurrentUser, fullPath);
    }

    private static (string parent, string child) SplitPath(string path)
    {
        var lastSlash = path.LastIndexOf('\\');
        return (path[..lastSlash], path[(lastSlash + 1)..]);
    }
}
