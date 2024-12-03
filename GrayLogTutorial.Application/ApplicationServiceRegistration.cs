using GrayLogTutorial.Application.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;

namespace GrayLogTutorial.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}