using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Infrastructure.Seeding;

public sealed class OpenIddictSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public OpenIddictSeeder(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await scopeManager.FindByNameAsync("api", cancellationToken) is null)
        {
            await SeedApiClient(scopeManager, cancellationToken);
        }

        if (await appManager.FindByClientIdAsync("postman", cancellationToken) is null)
        {
            await SeedPostmanClient(appManager, cancellationToken);
        }

        if (await appManager.FindByClientIdAsync("spa", cancellationToken) is null)
        {
            await SeedSPAClient(appManager, cancellationToken);
        }
    }

    private static async Task SeedSPAClient(IOpenIddictApplicationManager appManager, CancellationToken cancellationToken)
    {
        await appManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "spa",
            DisplayName = "SPA (dev)",

            RedirectUris =
                {
                    new Uri("https://localhost:4200/auth/callback"),
                    new Uri("http://localhost:4200/auth/callback")
                },

            PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:4200/"),
                    new Uri("http://localhost:4200/")
                },

            Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,

                    Permissions.GrantTypes.AuthorizationCode,

                    Permissions.ResponseTypes.Code,

                    Permissions.Prefixes.Scope + "api",
                },

            Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
        }, cancellationToken);
    }

    private static async Task SeedPostmanClient(IOpenIddictApplicationManager appManager, CancellationToken cancellationToken)
    {
        await appManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "postman",
            ClientSecret = "postman-secret-very-dev",
            DisplayName = "Postman (dev)",

            Permissions =
                {
                    Permissions.Endpoints.Token,

                    Permissions.GrantTypes.ClientCredentials,

                    Permissions.Prefixes.Scope + "api"
                }
        }, cancellationToken);
    }

    private static async Task SeedApiClient(IOpenIddictScopeManager scopeManager, CancellationToken cancellationToken)
    {
        await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
        {
            Name = "api",
            DisplayName = "Main API access",
            Resources = { "resource_server" }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}