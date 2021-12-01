using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pixel.Identity.Provider.Extensions;
using Pixel.Identity.Shared.Models;
using Quartz;
using System;

namespace Pixel.Identity.Provider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpLogging(logging =>
            {               
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("Referer");
                logging.RequestHeaders.Add("Origin");
                logging.RequestHeaders.Add("X-Forwarded-For");
                logging.RequestHeaders.Add("X-Forwarded-Host");
                logging.RequestHeaders.Add("X-Forwarded-Proto");
                logging.RequestHeaders.Add("Upgrade-Insecure-Requests");
                logging.RequestHeaders.Add("Sec-Fetch-Site");
                logging.RequestHeaders.Add("Sec-Fetch-Mode");
                logging.RequestHeaders.Add("Sec-Fetch-Dest");              
                logging.RequestHeaders.Add("Access-Control-Request-Method");
                logging.RequestHeaders.Add("Access-Control-Request-Headers");
                logging.ResponseHeaders.Add("Access-Control-Allow-Origin");
                logging.ResponseHeaders.Add("Access-Control-Allow-Methods");
                logging.ResponseHeaders.Add("Access-Control-Request-Headers");
                logging.ResponseHeaders.Add("Access-Control-Allow-Credentials");
                logging.ResponseHeaders.Add("Access-Control-Max-Age");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });

            //To forward the scheme from the proxy in non - IIS scenarios
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

         
            //// configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            //services.Configure<IISOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});

            //// configures IIS in-proc settings
            //services.Configure<IISServerOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.AddScheme(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme, options =>
                //{
                //    options.DisplayName = "Windows";                  
                //});
            });
            //.AddNegotiate()
            //.AddNegotiate(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme, "Windows", options =>
            //{
            //    //options.ForwardAuthenticate = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;             
            //    options.Events = new NegotiateEvents()
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            // context.SkipHandler();
            //            return Task.CompletedTask;
            //        }
            //    };
            //})
            //.AddGoogle(options =>
            //{
            //    options.ClientId = Configuration["google-client-id"];
            //    options.ClientSecret = Configuration["google-secret"];
            //});

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        var allowedOrigins = Configuration["AllowedOrigins"];
                        foreach(var item in allowedOrigins.Split(';'))
                        {
                            builder.WithOrigins(item);                           
                        }
                        //This is required for pre-flight request for CORS
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();                       
                        builder.AllowCredentials();                        
                    });
            });

            services.AddIdentityWithMongo<ApplicationUser, ApplicationRole>(this.Configuration);
            services.AddOpenIdDictWithMongo(this.Configuration);

            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddIdentityServer()
            //    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAutoMapper();

            services.AddHostedService<Worker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {         
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseHttpLogging();
            app.UseRouting();
            app.UseCors();

            //app.Use((context, next) =>
            //{
            //    context.Request.Scheme = "https";
            //    return next();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
