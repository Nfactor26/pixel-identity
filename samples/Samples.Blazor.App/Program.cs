using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Samples.Blazor.App;
using Samples.Blazor.App.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//we need to use a CustomAuthorizationMessageHandler as default BaseAddressAuthorizationMessageHandler
//won't pass the token if service base uri is different then host uri
//see https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-3.1#attach-tokens-to-outgoing-requests
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("WeatherForecast", client => client.BaseAddress = new Uri("http://localhost:5216/"))
            .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WeatherForecast"));

builder.Services.AddHttpClient<IWeatherService, WeatherService>(
          client => client.BaseAddress = new Uri("http://localhost:5216/"))
           .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.ClientId = "sample-blazor-app";
    options.ProviderOptions.Authority = "http://localhost:44382/pauth";
    options.ProviderOptions.ResponseType = "code";

    options.ProviderOptions.ResponseMode = "query";
    options.AuthenticationPaths.RemoteRegisterPath = "./Identity/Account/Register";

    options.UserOptions.RoleClaim = "role";
    options.ProviderOptions.DefaultScopes.Add("roles");
    options.ProviderOptions.DefaultScopes.Add("service-api-scope");
}).AddAccountClaimsPrincipalFactory<IdentityClaimsPrincipalFactory<RemoteUserAccount>>();

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("ReadWeatherDataPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("read-weather", "true");
    });    
});


await builder.Build().RunAsync();
