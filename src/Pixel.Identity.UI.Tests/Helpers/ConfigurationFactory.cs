using Microsoft.Extensions.Configuration;

namespace Pixel.Identity.UI.Tests.Helpers;

internal class ConfigurationFactory
{
    private static IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                              .AddEnvironmentVariables().Build();

    public static IConfiguration Create() => configuration;

}
