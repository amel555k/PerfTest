using EStore.Application.Abstractions;
using EStore.Infrastructure.Persistence;
using EStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextPool<AppDbContext>(options => 
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}

/*
[INFRASTRUCTURE LAYER]

- thats an adapter layer, code thats adapting to outer world(DBs, files, mail)
  and implements interfaces from Application layer
- Only layer who knows about DB (for example: adaptive refactoring project to Dapper)


[NAMING CONVENTIONS]
- EFCore.NamingConventions makes UnitPrice to unit-price, CreatedAt to created_at etc.

*/