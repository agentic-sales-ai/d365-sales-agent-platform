using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Services;
namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services)
{
    services.AddScoped<IDataverseService, DataverseService>();

    return services;
}
}