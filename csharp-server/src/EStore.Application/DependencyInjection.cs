using EStore.Application.Abstractions;
using EStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}

