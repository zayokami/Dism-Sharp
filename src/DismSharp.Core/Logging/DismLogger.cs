using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DismSharp.Core.Logging;

public static class DismLogger
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    public static void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
    }

    public static ILogger<T> GetLogger<T>() => _loggerFactory.CreateLogger<T>();

    public static ILogger GetLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);
}
