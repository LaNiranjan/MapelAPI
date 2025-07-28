using MapelRestAPI.Identity;
using MapelRestAPI.Interfaces;

namespace MapelRestAPI.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, GraphUserService>();
            return services;
        }
    }
}
