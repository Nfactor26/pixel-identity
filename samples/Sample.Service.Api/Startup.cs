using IdentityModel.AspNetCore.OAuth2Introspection;
using Sample.Service.Api;

namespace Samples.Service.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //configure cors to allow requests from http://localhost:5239 which is our client application
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://localhost:5239");                       
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
        });

        //Configure Authentication to use introspection i.e. API will check with OAuth2 introspection endpoint to validate 
        //if request is authenticated.
        services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
                   .AddOAuth2Introspection(options =>
                   {
                       options.Authority = "http://localhost:44382/pauth";
                       options.ClientId = "sample-service-api";
                       options.ClientSecret = "service-secret";
                   });

        //Configure authorization policy that requires read-weather = true claim to read weather data
        services.AddAuthorizationCore(options =>
        {
            //Add a policy to require read-weather claim
            options.AddPolicy(Policies.ReadWeatherDataPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });
        });
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
     
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();            
        }     

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast v1");
        });
 
        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {               
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
