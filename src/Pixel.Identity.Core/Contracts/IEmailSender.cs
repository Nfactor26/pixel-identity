namespace Pixel.Identity.Core
{
    /// <summary>
    /// IEmailSender is used for sending messages through email
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
