using FluentValidation;
using StoreApp.Comman.GlobalResponse.Generics;
using System.Net;
using System.Text.Json;
using static StoreApp.Comman.GlobalResponse.Generics.ErrorResponse;

namespace StoreApp.WebApi.Infastructure.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var message = new ErrorResponse(ex.Message, ErrorTypes.ValidationError);
            await WriteError(context, HttpStatusCode.BadRequest, message);

        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        static async Task WriteError(HttpContext context, HttpStatusCode httpStatusCode, ErrorResponse errorResponse)
        {
            var statusCode = (int)httpStatusCode;
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(errorResponse, options);
            await context.Response.WriteAsync(json);
        }
    }
}
