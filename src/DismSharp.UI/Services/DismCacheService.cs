using System.Collections.Concurrent;
using DismSharp.Core;
using DismSharp.Core.Modules;

namespace DismSharp.UI.Services;

/// <summary>DISM 数据缓存服务，避免重复查询</summary>
public static class DismCacheService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static readonly ConcurrentDictionary<string, (DateTime Time, object Data)> _cache = new();

    /// <summary>获取缓存的数据，如果过期则重新查询</summary>
    public static async Task<List<T>> GetOrLoadAsync<T>(
        string key,
        Func<DismSharpSession, List<T>> loader)
    {
        if (_cache.TryGetValue(key, out var cached) && DateTime.Now - cached.Time < CacheDuration)
        {
            return (List<T>)cached.Data;
        }

        var data = await Task.Run(() =>
        {
            using var session = DismSharpSession.OpenOnline();
            return loader(session);
        });

        _cache[key] = (DateTime.Now, data);
        return data;
    }

    /// <summary>清除指定键的缓存</summary>
    public static void Invalidate(string key)
    {
        _cache.TryRemove(key, out _);
    }

    /// <summary>清除所有缓存</summary>
    public static void InvalidateAll()
    {
        _cache.Clear();
    }

    /// <summary>获取缓存的 Features</summary>
    public static Task<List<Core.FeatureInfo>> GetFeaturesAsync()
        => GetOrLoadAsync("Features", s => s.GetFeatures());

    /// <summary>获取缓存的 Drivers</summary>
    public static Task<List<Core.DriverPackageInfo>> GetDriversAsync(bool allDrivers = false)
        => GetOrLoadAsync(allDrivers ? "Drivers_All" : "Drivers_ThirdParty", s => s.GetDrivers(allDrivers));

    /// <summary>获取缓存的 Packages</summary>
    public static Task<List<Core.PackageBasicInfo>> GetPackagesAsync()
        => GetOrLoadAsync("Packages", s => s.GetPackages());

    /// <summary>获取 Features/Drivers/Packages 的数量（用于 Dashboard）</summary>
    public static async Task<(int Features, int Drivers, int Packages)> GetCountsAsync()
    {
        var featuresTask = GetFeaturesAsync();
        var driversTask = GetDriversAsync(allDrivers: true);
        var packagesTask = GetPackagesAsync();

        await Task.WhenAll(featuresTask, driversTask, packagesTask);

        return (
            Features: featuresTask.Result.Count,
            Drivers: driversTask.Result.Count,
            Packages: packagesTask.Result.Count
        );
    }
}
