using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("Pixel.Identity.UI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Pixel.Identity.UI"));

            //builder.Services.AddOidcAuthentication(options =>
            //{
            //    builder.Configuration.Bind("PixelIdentityUI", options.ProviderOptions);                
            //});

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.ClientId = "pixel-identity-ui";
                options.ProviderOptions.Authority = "https://localhost:44382/";
                options.ProviderOptions.ResponseType = "code";              
                //options.ProviderOptions.DefaultScopes.Add("pixel_identity_api");

                // Note: response_mode=fragment is the best option for a SPA. Unfortunately, the Blazor WASM
                // authentication stack is impacted by a bug that prevents it from correctly extracting
                // authorization error responses (e.g error=access_denied responses) from the URL fragment.
                // For more information about this bug, visit https://github.com/dotnet/aspnetcore/issues/28344.
                //
                options.ProviderOptions.ResponseMode = "query";
                options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:44382/Identity/Account/Register";
            });

            builder.Services.AddApiAuthorization();
            builder.Services.AddMudServices();
            await builder.Build().RunAsync();
        }
    }
}
