using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Pixel.Identity.Provider
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string sendTo, string subject, string htmlMessage)
        {
            using (var smtp = new SmtpClient())
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(configuration["SMTP_USERNAME"]));
                email.To.Add(MailboxAddress.Parse(sendTo));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

                await smtp.ConnectAsync(configuration["SMTP_HOST"], int.Parse(configuration["SMTP_PORT"]));
                await smtp.AuthenticateAsync(configuration["SMTP_USERNAME"], configuration["SMTP_PASSWORD"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }

        }
    }
}
