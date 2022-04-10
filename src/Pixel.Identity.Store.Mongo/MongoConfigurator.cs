using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Conventions;
using Pixel.Identity.Store.Mongo.Extensions;
using Pixel.Identity.Store.Mongo.Models;

namespace Pixel.Identity.Store.Mongo
{
    /// <summary>
    /// Configure Pixel Identity to use the MongoDb backend for asp.net identity and OpenIddict
    /// </summary>
    public class MongoConfigurator : IDataStoreConfigurator
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

            BsonClassMap.RegisterClassMap<IdentityUserClaim<ObjectId>>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.Id);
                cm.UnmapMember(m => m.UserId);
            });

            BsonClassMap.RegisterClassMap<IdentityRoleClaim<ObjectId>>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.Id);
                cm.UnmapMember(m => m.RoleId);
            });

            var identityOptions = new IdentityOptions();
            configuration.GetSection(nameof(IdentityOptions)).Bind(identityOptions);

            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            return services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Role;

                options.SignIn.RequireConfirmedPhoneNumber = identityOptions.SignIn.RequireConfirmedPhoneNumber;
                options.SignIn.RequireConfirmedEmail = identityOptions.SignIn.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedAccount = identityOptions.SignIn.RequireConfirmedAccount;

                options.User.AllowedUserNameCharacters = identityOptions.User.AllowedUserNameCharacters;
                options.User.RequireUniqueEmail = identityOptions.User.RequireUniqueEmail;

                options.Password.RequiredLength = identityOptions.Password.RequiredLength;
                options.Password.RequiredUniqueChars = identityOptions.Password.RequiredUniqueChars;              
                options.Password.RequireNonAlphanumeric = identityOptions.Password.RequireNonAlphanumeric;                         
                options.Password.RequireLowercase = identityOptions.Password.RequireLowercase;
                options.Password.RequireUppercase = identityOptions.Password.RequireUppercase;
                options.Password.RequireDigit = identityOptions.Password.RequireDigit;

                options.Lockout.AllowedForNewUsers = identityOptions.Lockout.AllowedForNewUsers;
                options.Lockout.MaxFailedAccessAttempts = identityOptions.Lockout.MaxFailedAccessAttempts;  
                options.Lockout.DefaultLockoutTimeSpan = identityOptions.Lockout.DefaultLockoutTimeSpan;
            })
            .AddRoles<ApplicationRole>()
            .AddMongoDbStore<ApplicationUser, ApplicationRole, 
                IdentityUserClaim, IdentityUserLogin<ObjectId>, IdentityUserToken<ObjectId>, IdentityRoleClaim, ObjectId>(mongoDbSettings);
        }

        ///<inheritdoc/>
        public OpenIddictBuilder ConfigureOpenIdDictStore(IConfiguration configuration, OpenIddictBuilder builder)
        {
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            return builder.AddCore(options =>
            {
                // Configure OpenIddict to use the MongoDB stores and models.
                options.UseMongoDb()
               .UseDatabase(new MongoClient(mongoDbSettings.ConnectionString)
               .GetDatabase(mongoDbSettings.DatabaseName));

                //// Enable Quartz.NET integration.
                options.UseQuartz();
            });
        }

        ///<inheritdoc/>
        public void AddServices(IServiceCollection services)
        {        
            services.AddControllersWithViews()
                 .AddApplicationPart(typeof(ApplicationUser).Assembly)
                 .AddRazorPagesOptions(options =>
                 {
                     options.Conventions.Add(new IdentityPageModelConvention<ApplicationUser, ObjectId>());
                 });            
            services.AddHostedService<Worker>();
        }

    }
}
