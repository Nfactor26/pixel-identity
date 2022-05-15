using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using Pixel.Identity.Store.Mongo.Models;
using System.Configuration;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Store.Mongo;

/// <summary>
/// Worker is reponsible for setting up the initial values in database
/// </summary>
public class Worker : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly IOptions<CorsOptions> corsOptions;
    private readonly ILogger<Worker> logger;

    public Worker(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<CorsOptions> corsOptions, ILogger<Worker> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.corsOptions = corsOptions;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this.serviceProvider.CreateScope();
        
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await applicationManager.FindByClientIdAsync("pixel-identity-ui") is null)
        {
            if (!string.IsNullOrEmpty(configuration["IdentityHost"]))
            {
                await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "pixel-identity-ui",
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "Pixel Identity",
                    Type = ClientTypes.Public,
                    PostLogoutRedirectUris =
            {
                new Uri($"{configuration["IdentityHost"]}/authentication/logout-callback")
            },
                    RedirectUris =
            {
                new Uri($"{configuration["IdentityHost"]}/authentication/login-callback")
            },
                    Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles
            },
                    Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
                });
                logger.LogInformation("Added application descriptor for pixel-identity-ui");
            }
            else
            {
                throw new ConfigurationErrorsException("A non-empty value is required for 'IdentityHost'");
            }

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            if (await scopeManager.CountAsync() == 0)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "persistence-api",
                    DisplayName = "Persistence Api"
                });

                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "offline_access",
                    DisplayName = "Offline Access"
                });
                logger.LogInformation("Added persistence-api and offline_access scopes");
            }

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            if (roleManager.Roles.Count() == 0)
            {
                await roleManager.CreateAsync(new ApplicationRole() { Name = "IdentityAdmin" });
                var role = await roleManager.FindByNameAsync("IdentityAdmin");
               
                await AddClaimAsync("identity_read_write", "users");
                await AddClaimAsync("identity_read_write", "roles");
                await AddClaimAsync("identity_read_write", "applications");
                await AddClaimAsync("identity_read_write", "scopes");
               
                async Task AddClaimAsync(string type, string value)
                {
                    var claim = new Claim(type, value);
                    claim.Properties.Add("IncludeInAccessToken", "true");
                    claim.Properties.Add("IncludeInIdentityToken", "true");
                    await roleManager.AddClaimAsync(role, claim);
                }
                logger.LogInformation("Added role IdentityAdmin and required claims");
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManager.Users.Count() == 0)
            {
                if (!string.IsNullOrEmpty(configuration["InitAdminUser"]) && !string.IsNullOrEmpty(configuration["InitAdminUserPass"]))
                {
                    var adminUser = new ApplicationUser();
                    await userManager.SetUserNameAsync(adminUser, configuration["InitAdminUser"]);
                    await userManager.SetEmailAsync(adminUser, configuration["InitAdminUser"]);
                    await userManager.CreateAsync(adminUser, configuration["InitAdminUserPass"]);
                    await userManager.ConfirmEmailAsync(adminUser, await userManager.GenerateEmailConfirmationTokenAsync(adminUser));
                    //assign IdentityAdmin role to user
                    await userManager.AddToRoleAsync(adminUser, "IdentityAdmin");
                    logger.LogInformation("Added InitAdminUser and assigned IdentityAdmin role to it.");
                }
                else
                {
                    throw new ConfigurationErrorsException("A non - empty value is required for 'InitAdminUser' and 'InitAdminUserPass'") ;
                }
            }
        }

        //For each application, add redirect uri to allowed origin list on default cors policy
        var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);
        Func<IQueryable<object>, IQueryable<OpenIddictMongoDbApplication>> query = (apps) =>
        {
            return apps.Where(app => true).Select(s => s as OpenIddictMongoDbApplication);
        };      
        await foreach (var app in applicationManager.ListAsync(query, CancellationToken.None))
        {
            var redirectUris = await applicationManager.GetRedirectUrisAsync(app);
            foreach (var uri in redirectUris.Select(s => new Uri(s)))
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (!defaultCorsPolicy.Origins.Contains(origin))
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
