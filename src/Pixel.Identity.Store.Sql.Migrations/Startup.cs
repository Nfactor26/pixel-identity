using Microsoft.EntityFrameworkCore;
using Pixel.Identity.Store.Sql.Shared.Models;
using Pixel.Identity.Store.Sql.Shared.Stores;


namespace Pixel.Identity.Store.Sql.Migrations;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        string dbStore = Configuration.GetRequiredSection("dbStore")?.Value ?? "SqlServer";
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            switch(dbStore)
            {
                case "SqlServer":
                    string sqlServerConnectionString = Configuration.GetConnectionString("SqlServerConnection") ?? throw new ArgumentNullException(nameof(sqlServerConnectionString), "Sql server connection string is not configured");
                    options.UseSqlServer(sqlServerConnectionString, b => b.MigrationsAssembly("Pixel.Identity.Store.SqlServer"));
                    break;
                case "PostgreSql":
                    string postgreConnectionString = Configuration.GetConnectionString("PostgreServerConnection") ?? throw new ArgumentNullException(nameof(postgreConnectionString), "Postgres connection string is not configured");
                    options.UseNpgsql(postgreConnectionString, b => b.MigrationsAssembly("Pixel.Identity.Store.PostgreSQL"));
                    break;
            };               
            options.UseOpenIddict();
        })
        .AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
           
        })
       .AddRoles<ApplicationRole>()
       .AddRoleStore<ApplicationRoleStore>()
       .AddUserStore<ApplicationUserStore>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();       
        app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
    }
}