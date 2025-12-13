using AuthServer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Infrastructure.DI
{
    public static class InfrastructureServicesExtension
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("AuthServerDb");

            services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseSqlServer(cs);
            });

            services.AddHostedService<Seeding.OpenIddictSeeder>();

            return services;
        }
    }
}
