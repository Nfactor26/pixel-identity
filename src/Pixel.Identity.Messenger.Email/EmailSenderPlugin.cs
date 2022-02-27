using Microsoft.Extensions.DependencyInjection;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Plugins;

namespace Pixel.Identity.Messenger.Email
{
    public class EmailSenderPlugin : IServicePlugin
    {
        public void ConfigureService(IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
