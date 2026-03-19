using System.IO;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace DismSharp.UI.Services;

/// <summary>Serilog 配置和日志服务</summary>
public static class LoggingService
{
    private static ILoggerFactory? _loggerFactory;

    /// <summary>初始化 Serilog 日志系统</summary>
    public static void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()  // VS 输出窗口
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "dismsharp-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _loggerFactory = new LoggerFactory();
        _loggerFactory.AddSerilog(Log.Logger);
    }

    /// <summary>获取指定类型的 ILogger</summary>
    public static Microsoft.Extensions.Logging.ILogger<T> GetLogger<T>()
    {
        if (_loggerFactory == null)
            Initialize();
        return _loggerFactory!.CreateLogger<T>();
    }

    /// <summary>关闭日志系统</summary>
    public static void Shutdown()
    {
        Log.CloseAndFlush();
        _loggerFactory?.Dispose();
    }
}
