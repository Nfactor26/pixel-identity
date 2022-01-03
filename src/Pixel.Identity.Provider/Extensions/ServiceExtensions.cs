using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pixel.Identity.Provider.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureHttpLogging(this IServiceCollection services, IConfiguration Configuration)
        {
            return services.AddHttpLogging(logging =>
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
        }
    }
}
