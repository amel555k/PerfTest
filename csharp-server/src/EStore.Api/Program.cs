using EStore.Api.Contracts;
using EStore.Api.Middleware;
using EStore.Application;
using EStore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings: Default is not configured");

builder.Services
    .AddApplication()
    .AddInfrastructure(connectionString);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions( o =>
    // JSON malfunction (bad JSON, "page=abc", empty body) recieves our format, not ProblemDetails
    o.InvalidModelStateResponseFactory = _ =>
        new BadRequestObjectResult(new ErrorResponse("Validation failed: invalid request")));

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>(); // first in chain, covers all beneath itself
    app.MapControllers();

    app.Run();


/*
[API LAYER]
- meeting place of all layers
- encapsulation tells that if someone calls IProductService, give ProductService 
- Every Layer register themself with extensions AddApplication, AddInfrastructure, so API layer doesnt see the internal classes

- DI Container recursively builds the chain (controller -> service -> repo -> Dbcontext)

*/