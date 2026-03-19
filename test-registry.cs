using Microsoft.Win32;

Console.WriteLine("=== 诊断注册表右键菜单 ===\n");

// 测试常见路径
string[] paths = [
    @"*\shell",
    @"*\shellex\ContextMenuHandlers",
    @"Directory\shell",
    @"Directory\shellex\ContextMenuHandlers",
    @"Directory\Background\shell",
    @"DesktopBackground\shell",
    @"AllFilesystemObjects\shell",
    @"Drive\shell"
];

Console.WriteLine("--- HKCR (ClassesRoot) ---");
foreach (var path in paths)
{
    try
    {
        using var key = Registry.ClassesRoot.OpenSubKey(path);
        if (key != null)
        {
            var subKeys = key.GetSubKeyNames();
            Console.WriteLine($"  {path}: ✓ 存在, {subKeys.Length} 个子键");
            if (subKeys.Length > 0 && subKeys.Length <= 5)
            {
                foreach (var sk in subKeys)
                    Console.WriteLine($"    - {sk}");
            }
        }
        else
        {
            Console.WriteLine($"  {path}: ✗ 不存在");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {path}: ⚠ 错误: {ex.Message}");
    }
}

Console.WriteLine("\n--- HKCU (CurrentUser) ---");
foreach (var path in paths)
{
    try
    {
        using var key = Registry.CurrentUser.OpenSubKey(path);
        if (key != null)
        {
            var subKeys = key.GetSubKeyNames();
            Console.WriteLine($"  {path}: ✓ 存在, {subKeys.Length} 个子键");
        }
        else
        {
            Console.WriteLine($"  {path}: ✗ 不存在");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {path}: ⚠ 错误: {ex.Message}");
    }
}

Console.WriteLine("\n--- HKLM (LocalMachine) ---");
foreach (var path in paths)
{
    try
    {
        var fullPath = $@"SOFTWARE\Classes\{path}";
        using var key = Registry.LocalMachine.OpenSubKey(fullPath);
        if (key != null)
        {
            var subKeys = key.GetSubKeyNames();
            Console.WriteLine($"  {fullPath}: ✓ 存在, {subKeys.Length} 个子键");
        }
        else
        {
            Console.WriteLine($"  {fullPath}: ✗ 不存在");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {path}: ⚠ 错误: {ex.Message}");
    }
}

Console.WriteLine("\n--- 检查 HKCR\\*\\shell 子键详情 ---");
try
{
    using var shellKey = Registry.ClassesRoot.OpenSubKey(@"*\shell");
    if (shellKey != null)
    {
        foreach (var name in shellKey.GetSubKeyNames())
        {
            try
            {
                using var subKey = shellKey.OpenSubKey(name);
                if (subKey == null) continue;

                // 检查 command
                using var cmdKey = subKey.OpenSubKey("command");
                var cmd = cmdKey?.GetValue("")?.ToString();

                // 检查默认值
                var defaultVal = subKey.GetValue("")?.ToString();

                // 检查 DelegateExecute
                var delegateVal = subKey.GetValue("DelegateExecute")?.ToString();

                Console.WriteLine($"  {name}:");
                if (!string.IsNullOrEmpty(cmd))
                    Console.WriteLine($"    command: {cmd}");
                if (!string.IsNullOrEmpty(defaultVal) && defaultVal != cmd)
                    Console.WriteLine($"    default: {defaultVal}");
                if (!string.IsNullOrEmpty(delegateVal))
                    Console.WriteLine($"    delegate: {delegateVal}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {name}: ⚠ {ex.Message}");
            }
        }
    }
    else
    {
        Console.WriteLine("  不存在");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"  ⚠ 错误: {ex.Message}");
}

Console.WriteLine("\n完成");
