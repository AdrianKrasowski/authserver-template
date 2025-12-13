using AuthServer.Infrastructure.Context;
using AuthServer.Infrastructure.Models;

namespace AuthServer.Api.DI
{
    public static class AuthExtension
    {
        public static IServiceCollection AddOpenIdDictServices(this IServiceCollection services)
        {
            services
                .AddIdentityCore<ApplicationUser>(opt =>
                {
                    opt.User.RequireUniqueEmail = true;

                    opt.Password.RequireNonAlphanumeric = true;
                    opt.Password.RequiredLength = 8;
                    opt.Password.RequireUppercase = true;
                    opt.Password.RequireDigit = true;

                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<AuthDbContext>();


            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<AuthDbContext>();
                })
                .AddServer(options =>
                {
                    options.SetTokenEndpointUris("/connect/token")
                           .SetAuthorizationEndpointUris("/connect/authorize")
                           .SetUserInfoEndpointUris("/connect/userinfo");
                    options.AllowPasswordFlow()
                           .AllowAuthorizationCodeFlow()
                           .AllowRefreshTokenFlow();
                    options.RegisterScopes("email", "profile", "roles");
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableUserInfoEndpointPassthrough();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });
            return services;
        }
    }
}
