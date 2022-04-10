using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Plugins;

namespace Pixel.Identity.Messenger.Console;

public class EmailSenderPlugin : IServicePlugin
{     
    public void ConfigureService(IServiceCollection services, IConfiguration configuration)
    {                
        services.AddTransient<IEmailSender, EmailSender>();
    }
}
