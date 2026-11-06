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


/*
[Scoped filetime] 

AddScoped means: one instance per HTTP request
AddSingleton means: one instance per lifetime (once created, lives unless system goes down)
AddTransirent means: new instance is created every single time is requseted from de DI container

In this case: 
- Scoped is best option becaues it holds DbContext which is Scoped by default
- Theres no reason for "captive dependency" (that means one parent object holds whole lifetime other object that meant to be dead already on next request)

*same Lifetimes in most of the cases*




[Dependency Inversion (D from SOLID)]

- Application has interface IOrderRepository and Infrastructure layer implements it
Exec. flow is: API -> Application -> Infrastructure
Dep. flow is: Infrastructure -> Application 


*/

