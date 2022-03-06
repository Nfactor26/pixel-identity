using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Pixel.Identity.Core;

namespace Pixel.Identity.Messenger.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EmailSender> logger;
     
        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string sendTo, string subject, string htmlMessage)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogWarning($"There was an error sending email to {sendTo} with subject {subject}");
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
