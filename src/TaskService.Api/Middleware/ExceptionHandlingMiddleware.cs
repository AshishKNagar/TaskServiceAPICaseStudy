using System.Net;
using System.Text.Json;
using TaskService.Api.Exceptions;

namespace TaskService.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware to handle exceptions that occur during the request processing pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (TaskNotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (TaskCreateFailException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (TaskListNotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}",
                context.TraceIdentifier);

            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    /// <summary>
    ///     Writes an error response to the HTTP context with the specified status code and message.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private static async Task WriteErrorAsync(
        HttpContext context, HttpStatusCode statusCode, string message)
    {
        if (context.Response.HasStarted) return;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            message,
            traceId = context.TraceIdentifier
        }));
    }
}
