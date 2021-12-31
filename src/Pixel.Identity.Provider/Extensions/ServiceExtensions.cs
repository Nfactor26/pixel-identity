using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Pixel.Identity.Provider.Helpers;
using Pixel.Identity.Shared.Models;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
                options.SignIn.RequireConfirmedAccount = true;
                //options.User.RequireUniqueEmail = true;               
            })
            .AddRoles<ApplicationRole>()
            .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
            (
                mongoDbSettings.ConnectionString,
                mongoDbSettings.DatabaseName
            )
            .AddDefaultUI()
            .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
           
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
              .SetIntrospectionEndpointUris("/connect/introspect")
              .SetDeviceEndpointUris("/connect/device")
              .SetVerificationEndpointUris("connect/verify");

               //when integration with third-party APIs/resource servers is desired
               options.DisableAccessTokenEncryption();

               // Disables the transport security requirement (HTTPS). Service is supposed
               // to run behind a reverse-proxy with tls termination
               options.UseAspNetCore().DisableTransportSecurityRequirement();

               options.DisableScopeValidation();

               // Note: this sample only uses the authorization code flow but you can enable
               // the other flows if you need to support implicit, password or client credentials.
               //options.AllowDeviceCodeFlow();
               options.AllowAuthorizationCodeFlow().AllowDeviceCodeFlow().AllowRefreshTokenFlow().AllowClientCredentialsFlow();

               
               //OpenIdDict uses two types of credentials to secure the token it issues.
               //1.Encryption credentials are used to ensure the content of tokens cannot be read by malicious parties
               if (string.IsNullOrEmpty(Configuration["Identity:Certificates:EncryptionCertificatePath"]))                 
               {
                 
                   var encryptionKeyBytes = File.ReadAllBytes(Configuration["Identity:Certificates:EncryptionCertificatePath"]);
                   X509Certificate2 encryptionKey = new X509Certificate2(encryptionKeyBytes, Configuration["Identity:EncryptionCertificateKey"],
                           X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
               }
               else
               {
                   options.AddDevelopmentEncryptionCertificate();
               }

               //2.Signing credentials are used to protect against tampering
               if (string.IsNullOrEmpty(Configuration["Identity:Certificates:SigningCertificatePath"]))
               {
                  
                   var signingKeyBytes = File.ReadAllBytes(Configuration["Identity:Certificates:SigningCertificatePath"]);
                   X509Certificate2 signingKey = new X509Certificate2(signingKeyBytes, Configuration["Identity:SigningCertificateKey"],
                           X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
                   options.AddSigningCertificate(signingKey);
               }
               else
               {
                   options.AddDevelopmentSigningCertificate();
               }          

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
