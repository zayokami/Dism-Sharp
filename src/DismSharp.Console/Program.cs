using DismSharp.Core;
using DismSharp.Core.Helpers;
using DismSharp.Core.Native;

// 检查管理员权限
if (!PrivilegeHelper.IsRunningAsAdmin())
{
    Console.WriteLine("错误：请以管理员身份运行此程序！");
    return 1;
}

try
{
    // 初始化 DISM
    DismSharpSession.Initialize(DismLogLevel.DismLogErrors);
    Console.WriteLine("DISM 初始化成功");

    // 打开在线镜像会话
    using var session = DismSharpSession.OpenOnline();
    Console.WriteLine("在线镜像会话已打开");

    // 获取并打印前 10 个功能
    var features = session.GetFeatures();
    Console.WriteLine($"\n系统功能总数: {features.Count}");
    Console.WriteLine("前 10 个功能:");
    foreach (var feature in features.Take(10))
    {
        Console.WriteLine($"  {feature.FeatureName} - {feature.State}");
    }

    // 获取第三方驱动数量
    var drivers = session.GetDrivers(allDrivers: false);
    Console.WriteLine($"\n第三方驱动数量: {drivers.Count}");

    // 获取包信息
    var packages = session.GetPackages();
    Console.WriteLine($"已安装包数量: {packages.Count}");
}
catch (DismSharpException ex)
{
    Console.WriteLine($"DISM 错误: 0x{ex.HResult:X8} - {ex.Message}");
    return 1;
}
finally
{
    // 关闭 DISM
    DismSharpSession.Shutdown();
    Console.WriteLine("\nDISM 已关闭");
}

return 0;
