using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace StoreApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
     
        return services;
    }
}
