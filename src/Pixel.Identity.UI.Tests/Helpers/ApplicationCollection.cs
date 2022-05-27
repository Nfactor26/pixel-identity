using System.Collections.Generic;

namespace Pixel.Identity.UI.Tests.Helpers;


internal class Application
{
    /// <summary>
    /// Template to use while adding application
    /// </summary>
    public string FromTemplate { get; set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the application.
    /// </summary>     
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the application type associated with the application.
    /// </summary>      
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the client secret associated with the application.
    /// Note: depending on the application manager used when creating it,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the consent type associated with the application.
    /// </summary>     
    public string ConsentType { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the application.
    /// </summary>      
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets the permissions associated with the application.
    /// </summary>       
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Gets the callback URLs associated with the application.
    /// </summary>      
    public List<string> RedirectUris { get; set; } = new();


    /// <summary>
    /// Gets the logout callback URLs associated with the application.
    /// </summary>      
    public List<string> PostLogoutRedirectUris { get; set; } = new();

    /// <summary>
    /// Gets the requirements associated with the application.
    /// </summary>       
    public List<string> Requirements { get; set; } = new();

}

internal static class ApplicationCollection
{
    private static readonly List<Application> applications = new();

    static ApplicationCollection()
    {
        applications.Add(new Application()
        {
            FromTemplate = "Authorization Code Flow",
            ClientId = "application-01",
            DisplayName = "Application One",
            RedirectUris = new List<string> { "http://application-one/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-one/pauth/authentication/logout-callback" }
        });
        applications.Add(new Application()
        {
            FromTemplate = "Authorization Code Flow",
            ClientId = "application-02",
            DisplayName = "Application Two",
            RedirectUris = new List<string> { "http://application-two/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-two/pauth/authentication/logout-callback" }
        });
        applications.Add(new Application()
        {
            FromTemplate = "Client Credentials Flow",
            ClientId = "application-03",
            DisplayName = "Application Three",
            ClientSecret = "application-secret-three"

        });
        applications.Add(new Application()
        {
            FromTemplate = "Client Credentials Flow",
            ClientId = "application-04",
            DisplayName = "Application Four",
            ClientSecret = "application-secret-four"
        });
        applications.Add(new Application()
        {
            FromTemplate = "Device Authorization Flow",
            ClientId = "application-05",
            DisplayName = "Application Five"

        });
        applications.Add(new Application()
        {
            FromTemplate = "Device Authorization Flow",
            ClientId = "application-06",
            DisplayName = "Application Six"
        });
        applications.Add(new Application()
        {
            FromTemplate = "Introspection",
            ClientId = "application-07",
            DisplayName = "Application Seven",
            ClientSecret = "application-secret-seven"

        });
        applications.Add(new Application()
        {
            FromTemplate = "Introspection",
            ClientId = "application-08",
            DisplayName = "Application Eight",
            ClientSecret = "application-secret-eight"
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-09",
            DisplayName = "Application Nine",
            Type = "public",
            ConsentType = "explicit",
            Permissions = new List<string>() { "device", "token", "refresh_token", "urn:ietf:params:oauth:grant-type:device_code",
                    "email", "profile", "roles"}
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-10",
            DisplayName = "Application Ten",
            Type = "public",
            ConsentType = "explicit",
            RedirectUris = new List<string> { "http://application-ten/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-ten/pauth/authentication/logout-callback" },
            Permissions = new List<string>() { "authorization", "logout", "token", "authorization_code", "refresh_token" ,
                    "email", "profile", "roles", "code", "pkce"}
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-11",
            DisplayName = "Application Eleven",
            Type = "confidential",
            ConsentType = "explicit",
            ClientSecret = "application-secret-eleven",
            Permissions = new List<string>() { "introspection" }
        });
    }

    public static IEnumerable<Application> GetAllApplications() => applications;
}
