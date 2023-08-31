using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.Application;

public static class ServicesExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServicesExtensions).Assembly));
    }
}