using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Pixel.Identity.Shared.Models;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Provider.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityWithMongo<TUser, TRole>(this IServiceCollection services, IConfiguration Configuration) where TUser : class
                         where TRole : class
        {
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<MongoDbSettings>(mongoDbSettings);

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {              
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                //options.SignIn.RequireConfirmedAccount = true;
                //options.User.RequireUniqueEmail = true;               
            })
            .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
            (
                mongoDbSettings.ConnectionString,
                mongoDbSettings.DatabaseName
            )
            .AddDefaultUI()
            .AddDefaultTokenProviders();
           
            return services;
        }


        public static IServiceCollection AddOpenIdDictWithMongo(this IServiceCollection services,
            IConfiguration Configuration)
        {
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
           
            services.AddOpenIddict()
            // Register the OpenIddict core services.
            .AddCore(options =>
            {
                 // Configure OpenIddict to use the MongoDB stores and models.
                 options.UseMongoDb()
                .UseDatabase(new MongoClient(mongoDbSettings.ConnectionString)
                .GetDatabase(mongoDbSettings.DatabaseName));

                 // Enable Quartz.NET integration.
                 options.UseQuartz();
            })
           // Register the OpenIddict server components.
           .AddServer(options =>
           {
               // Enable the authorization, logout, token and userinfo endpoints.
               options.SetAuthorizationEndpointUris("/connect/authorize")
              .SetLogoutEndpointUris("/connect/logout")
              .SetTokenEndpointUris("/connect/token")
              .SetUserinfoEndpointUris("/connect/userinfo")
              .SetDeviceEndpointUris("/connect/device")
              .SetVerificationEndpointUris("connect/verify");

               //when integration with third-party APIs/resource servers is desired
               options.DisableAccessTokenEncryption();

               // Disables the transport security requirement (HTTPS). Service is supposed
               // to run behind a reverse-proxy with tls termination
               options.UseAspNetCore().DisableTransportSecurityRequirement();

               // Mark the "email", "profile" and "roles" scopes as supported scopes.
               options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

               // Note: this sample only uses the authorization code flow but you can enable
               // the other flows if you need to support implicit, password or client credentials.
               //options.AllowDeviceCodeFlow();
               options.AllowAuthorizationCodeFlow().AllowDeviceCodeFlow().AllowRefreshTokenFlow();

               // Register the signing and encryption credentials.
               options.AddDevelopmentEncryptionCertificate()
              .AddDevelopmentSigningCertificate();

               // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
               options.UseAspNetCore()
              .EnableAuthorizationEndpointPassthrough()
              .EnableLogoutEndpointPassthrough()
              .EnableTokenEndpointPassthrough()
              .EnableUserinfoEndpointPassthrough()
              .EnableStatusCodePagesIntegration();
           })
           // Register the OpenIddict validation components.
           .AddValidation(options =>
           {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
           });
            return services;
        }
    }
}
