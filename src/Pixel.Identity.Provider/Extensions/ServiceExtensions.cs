using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pixel.Identity.Core.Plugins;
using System;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// Load a service plugin from plugins directory and invoke the ConfigureService() method on plugin.
        /// This will allow plugin to register the required services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pluginName"></param>
        /// <param name="sharedTypes"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlugin(this IServiceCollection services, string pluginType, string pluginName, Type[] sharedTypes)
        {         
            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, "Plugins", "", pluginType, pluginName);           
            if (Directory.Exists(pluginsDirectory))
            {
                var pluginFile = Directory.GetFiles(pluginsDirectory, "*.dll").Where(f => Path.GetFileNameWithoutExtension(f).Equals(pluginName)).Single();
                var loader = PluginLoader.CreateFromAssemblyFile(pluginFile, sharedTypes: sharedTypes);
                foreach (var type in loader.LoadDefaultAssembly().GetTypes()
                    .Where(t => typeof(IServicePlugin).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    var servicePlugin =  (IServicePlugin)Activator.CreateInstance(type);
                    servicePlugin.ConfigureService(services);
                }
            }
            return services;
        }
    }
}
