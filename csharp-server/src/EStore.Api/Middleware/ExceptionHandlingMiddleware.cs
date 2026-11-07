using EStore.Api.Contracts;
using EStore.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EStore.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            // Client stops request; theres no one to send the response and thats not an error
        }

        catch (Exception ex) when (!ctx.Response.HasStarted)
        {
            var (status, message) = ex switch
            {
                BadRequestException => (StatusCodes.Status400BadRequest, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal server error")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Unhandled exception");

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(new ErrorResponse(message));
        }


    }
}

/*
[MIDDLEWARE PIPELINE]
- every request is going through component chain, every segment works before next(ctx)
- register sequence is execute sequence, thats why this Middleware is first 
    -- Exception wherever beneath it (service, repo, DB) travels upwards through awaits, beacuse async methods propagates exception to place where someone waits the result
    -- without it, every Controller would have try catch


*/