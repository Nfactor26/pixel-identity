using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Pixel.Identity.Store.SqlServer.Data;
using Pixel.Identity.Store.Sql.Shared;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Store.SqlServer
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public Worker(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = this.serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await applicationManager.FindByClientIdAsync("pixel-identity-ui") is null)
            {
                await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "pixel-identity-ui",
                    ConsentType = ConsentTypes.Explicit,
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
            }

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            if(await scopeManager.CountAsync() == 0 )
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
            }

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            if(roleManager.Roles.Count() == 0)
            {
                await roleManager.CreateAsync(new ApplicationRole() { Name = "IdentityAdmin" });
                var role = await roleManager.FindByNameAsync("IdentityAdmin");
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("rc_read_write", "users"));
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("rc_read_write", "roles"));
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("rc_read_write", "applications"));
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("rc_read_write", "scopes"));
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if(userManager.Users.Count() == 0)
            {
                var adminUser = new ApplicationUser();
                await userManager.SetUserNameAsync(adminUser, "admin@pixel.com");
                await userManager.SetEmailAsync(adminUser, "admin@pixel.com");
                await userManager.CreateAsync(adminUser, "Admi9@pixel");
                await userManager.ConfirmEmailAsync(adminUser, await userManager.GenerateEmailConfirmationTokenAsync(adminUser));
                //assign IdentityAdmin role to user
                await userManager.AddToRoleAsync(adminUser, "IdentityAdmin");
            }           
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
