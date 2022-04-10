using Microsoft.Extensions.Logging;
using Pixel.Identity.Core;
using System.Text;

namespace Pixel.Identity.Messenger.Console;

/// <summary>
/// <see cref="IEmailSender"/> implementation that prints the message as information
/// on console and log and doesn't actually send the mail.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        this.logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"----------------------------------------------------");
        sb.AppendLine($"{email}:{subject}");
        sb.AppendLine("message:[redacted]");
        sb.AppendLine($"----------------------------------------------------");       
        this.logger.LogInformation(sb.ToString());
        System.Console.WriteLine(sb.ToString());        
        return Task.CompletedTask;
    }
}
