using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pixel.Identity.Core;
using Pixel.Identity.Store.Sql.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Store.Sql
{
    /// <summary>
    /// Configure Pixel Identity to use the MongoDb backend for asp.net identity and OpenIddict
    /// </summary>
    public class SqlConfigurator : IConfigurator
    {
        ///<inheritdoc/>
        public void ConfigureAutoMap(IServiceCollection services)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapProfile>();
            });

            #if DEBUG
            configuration.AssertConfigurationIsValid();
            #endif

            var mapper = configuration.CreateMapper();
            services.AddSingleton(mapper);
        }

        ///<inheritdoc/>
        public IdentityBuilder ConfigureIdentity(IConfiguration configuration, IServiceCollection services)
        {
            return services.AddDbContext<ApplicationDbContext>(options =>
            {
                string entityFrameworkProvider = configuration["EntityFrameworkProvider"] ??
                    throw new InvalidOperationException("EntityFrameworkProvider is not configured");
              
                if (string.Equals(entityFrameworkProvider, "sqlserver", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
                }                         
                else if (string.Equals(entityFrameworkProvider, "postgresql", StringComparison.OrdinalIgnoreCase))
                {                  
                    options.UseNpgsql(configuration.GetConnectionString("PostgreServerConnection"));
                }
                else
                {
                    throw new ArgumentException($"{entityFrameworkProvider} is not a supported provider");
                }
              
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            })
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.SignIn.RequireConfirmedAccount = true;
            })
           .AddRoles<ApplicationRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        ///<inheritdoc/>
        public OpenIddictBuilder ConfigureOpenIdDictStore(IConfiguration configuration, OpenIddictBuilder builder)
        {
           return builder.AddCore(options =>
           {
               // Configure OpenIddict to use the Entity Framework Core stores and models.
               // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
               options.UseEntityFrameworkCore()
                      .UseDbContext<ApplicationDbContext>();

               // Enable Quartz.NET integration.
               options.UseQuartz();
           });
        }

        ///<inheritdoc/>
        public void AddServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }
}
}
