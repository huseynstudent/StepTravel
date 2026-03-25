using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.PipelineBehaviours;
using System.Reflection;
using FluentValidation;
using AutoMapper;
namespace StoreApp.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}