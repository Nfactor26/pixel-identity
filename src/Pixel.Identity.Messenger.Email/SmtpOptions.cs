namespace Pixel.Identity.Messenger.Email;

public class SmtpOptions
{
    public const string Smtp = "SMTP";

    /// <summary>
    /// SMTP Host address
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// SMTP Port to be used
    /// </summary>
    public string Port { get; set; }

    /// <summary>
    /// UserName for authentication
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// From for sending mail
    /// </summary>
    public string From { get; set; }
   
}
