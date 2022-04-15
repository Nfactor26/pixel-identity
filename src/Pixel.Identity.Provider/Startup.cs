using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Plugins;
using Pixel.Identity.Provider.Extensions;
using Pixel.Identity.Shared;
using Quartz;
using Serilog;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Pixel.Identity.Provider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.ConfigureHttpLogging(Configuration);

            var pluginsOptions = new PluginOptions();
            Configuration.GetSection(PluginOptions.Plugins).Bind(pluginsOptions);

            //To forward the scheme from the proxy in non - IIS scenarios
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            //Add plugin assembly type to application part so that controllers in this assembly can be discovered by asp.net
            services.AddControllersWithViews();               
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
           
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
            });

            ConfigureCors(services);

            services.AddPlugin<IServicePlugin>(pluginsOptions["EmailSender"].Single(), (p, s) => 
            {
                p.ConfigureService(s, this.Configuration);
            });

            var authenticationBuilder = services.AddAuthentication();
            foreach (var externalProvider in pluginsOptions["OAuthProvider"])
            {
                services.AddPlugin<IExternalAuthProvider>(externalProvider, (p, s) =>
                { 
                    p.AddProvider(this.Configuration, authenticationBuilder); 
                });              
            }        
       
            services.AddPlugin<IDataStoreConfigurator>(pluginsOptions["DbStore"].Single(), (p, s) =>
            {
                p.ConfigureAutoMap(s);
                ConfigureOpenIddict(s, p);
                p.AddServices(services);
            });

            ConfigureAuthorizationPolicy(services);
            ConfigureQuartz(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UsePathBase("/pauth");
           
            app.UseSerilogRequestLogging();
           
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pixel Persistence V1");
            });

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
         
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }      

        /// <summary>
        /// Configure the Cors so that different clients can consume api
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        var allowedOrigins = Configuration["AllowedOrigins"];
                        foreach (var item in allowedOrigins?.Split(';') ?? Enumerable.Empty<string>())
                        {
                            builder.WithOrigins(item);
                        }
                        //This is required for pre-flight request for CORS
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                    });
            });
        }

        /// <summary>
        /// Configure the authorization policioes used by Pixel Identity for access control
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureAuthorizationPolicy(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CanManageApplications, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(Claims.ReadWriteClaim, "applications");
                });
                options.AddPolicy(Policies.CanManageScopes, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(Claims.ReadWriteClaim, "scopes");
                });
                options.AddPolicy(Policies.CanManageUsers, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(Claims.ReadWriteClaim, "users");
                });
                options.AddPolicy(Policies.CanManageRoles, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(Claims.ReadWriteClaim, "roles");
                });
            });
        }

        /// <summary>
        /// Configure core setup for OpenIddict and delegate database configuration to DbStore plugin
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurator"></param>
        private void ConfigureOpenIddict(IServiceCollection services, IDataStoreConfigurator configurator)
        {
            //Configure Identity will call services.AddIdentity which will AddAuthentication  
            configurator.ConfigureIdentity(this.Configuration, services) 
            .AddSignInManager()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts => {
                opts.LoginPath = "/Identity/Account/Login";                
            });           

            var openIdBuilder = services.AddOpenIddict()        
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

                options.AllowAuthorizationCodeFlow().AllowDeviceCodeFlow()
                    .AllowRefreshTokenFlow().AllowClientCredentialsFlow().AllowPasswordFlow();

                //https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html
                //OpenIdDict uses two types of credentials to secure the token it issues.
                //1.Encryption credentials are used to ensure the content of tokens cannot be read by malicious parties
                if (string.IsNullOrEmpty(Configuration["Identity:Certificates:EncryptionCertificatePath"]))
                {
                    var encryptionKeyBytes = File.ReadAllBytes(Configuration["Identity:Certificates:EncryptionCertificatePath"]);
                    X509Certificate2 encryptionKey = new X509Certificate2(encryptionKeyBytes, Configuration["Identity:EncryptionCertificateKey"],
                         X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
                    options.AddEncryptionCertificate(encryptionKey);
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
            configurator.ConfigureOpenIdDictStore(this.Configuration, openIdBuilder);
        }

        /// <summary>
        /// Configure Quartz which is a scheduler. OpenIddict uses this for cleanup of database periodically
        /// to remove outdated tokens, etc.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureQuartz(IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }
    }
}
