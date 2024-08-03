using BG.Model.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BG.Serv;
public static class ConfigExtensions
{
    //public static IServiceCollection AddBGServices_ForAdo(
    //                                    this IServiceCollection services)
    //{
    //    services.AddScoped<ICoreData, BG.Repo.Ado.Core.CoreData>()
    //        .AddScoped<ICoreServices, Core.CoreServices>();

    //    return services;
    //}
    public static IServiceCollection AddBGServices_ForRestApi(
                                            this IServiceCollection services)
    {
        services.AddScoped<ICoreData, BG.Repo.RestApi.Core.CoreData>()
            .AddScoped<ICoreServices, Core.CoreServices>();

        return services;
    }
    public static IServiceCollection AddBGServices_ForEF(
                                            this IServiceCollection services)
    {
        services.AddScoped<ICoreData, BG.Repo.EF.Core.CoreData>()
            .AddScoped<ICoreServices, Core.CoreServices>();

        return services;
    }
    //public static IServiceCollection AddBGServices_ForInMem(
    //                                        this IServiceCollection services)
    //{
    //    services.AddSingleton<ICoreData, BG.Repo.InMem.Core.CoreData>()
    //        .AddSingleton<ICoreServices, Core.CoreServices>();

    //    return services;
    //}
}
