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