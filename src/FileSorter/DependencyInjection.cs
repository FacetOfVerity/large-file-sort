using FileSorter.Interfaces;
using FileSorter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FileSorter;

public static class DependencyInjection
{
    public static IServiceCollection RegisterFileSorterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SortExecutor>();
        services.AddSingleton<SortedChunksGenerator>();
        services.AddSingleton<KWayMergeExecutor>();
        services.TryAddSingleton<IChunkSettingsProvider, ChunkSettingsProvider>();
        services.AddSingleton<ChunkStoreUtility>();

        services.Configure<SortOptions>(configuration.GetSection(SortOptions.ConfigSection));

        return services;
    }
}