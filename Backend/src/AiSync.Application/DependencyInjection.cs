using AiSync.Application.Interfaces;
using AiSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiSync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}
