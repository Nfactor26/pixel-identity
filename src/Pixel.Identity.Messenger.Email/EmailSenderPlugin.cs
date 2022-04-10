using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pixel.Identity.Core;
using Pixel.Identity.Core.Plugins;

namespace Pixel.Identity.Messenger.Email
{
    public class EmailSenderPlugin : IServicePlugin
    {     
        public void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {         
            var smtpOptions = new SmtpOptions();
            configuration.GetSection(SmtpOptions.Smtp).Bind(smtpOptions);

            services.AddSingleton<SmtpOptions>(smtpOptions);
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
