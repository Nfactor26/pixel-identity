using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
    /// Gets or set the client application type
    /// </summary>
    public string ApplicationType { get; set; }

    /// <summary>
    /// Gets or sets the application type associated with the application.
    /// </summary>      
    public string ClientType { get; set; }

    /// <summary>
    /// Gets or sets the client secret associated with the application.
    /// Note: depending on the application manager used when creating it,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// public key of the ECDSA private/public key pair used for client assertions
    /// </summary>
    public string? JsonWebKeySet { get; set; }

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

    /// <summary>
    /// Settings can be used to control the lifetime of access tokens, authorization codes, device codes,
    /// identity tokens, refresh tokens and user codes.
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new(StringComparer.Ordinal);

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
            ApplicationType = "web",
            RedirectUris = new List<string> { "http://application-one/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-one/pauth/authentication/logout-callback" },
            Settings = new Dictionary<string, string> { { "AccessToken", "00:30:00" } }
        });
        applications.Add(new Application()
        {
            FromTemplate = "Authorization Code Flow",
            ClientId = "application-02",
            DisplayName = "Application Two",
            ApplicationType = "web",
            RedirectUris = new List<string> { "http://application-two/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-two/pauth/authentication/logout-callback" },
            Settings = new Dictionary<string, string> { { "UserCode", "00:20:00" } }
        });
        applications.Add(new Application()
        {
            FromTemplate = "Client Credentials Flow",
            ClientId = "application-03",
            DisplayName = "Application Three",
            ClientSecret = "application-secret-three",
            ApplicationType = "native",

        });
        applications.Add(new Application()
        {
            FromTemplate = "Client Credentials Flow",
            ClientId = "application-04",
            DisplayName = "Application Four",
            ClientSecret = "application-secret-four",
            ApplicationType = "web",
        });
        applications.Add(new Application()
        {
            FromTemplate = "Device Authorization Flow",
            ClientId = "application-05",
            DisplayName = "Application Five",
            ApplicationType = "web"

        });
        applications.Add(new Application()
        {
            FromTemplate = "Device Authorization Flow",
            ClientId = "application-06",
            DisplayName = "Application Six",
            ApplicationType = "web"
        });
        applications.Add(new Application()
        {
            FromTemplate = "Introspection",
            ClientId = "application-07",
            DisplayName = "Application Seven",
            ClientSecret = "application-secret-seven",
            ApplicationType = "web"

        });
        applications.Add(new Application()
        {
            FromTemplate = "Introspection",
            ClientId = "application-08",
            DisplayName = "Application Eight",
            ClientSecret = "application-secret-eight",
            ApplicationType = "web"
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-09",
            DisplayName = "Application Nine",
            ClientType = "public",
            ApplicationType = "native",
            ConsentType = "explicit",
            Permissions = new List<string>() { "device", "token", "refresh_token", "urn:ietf:params:oauth:grant-type:device_code",
                    "email", "profile", "roles"}
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-10",
            DisplayName = "Application Ten",
            ClientType = "public",
            ConsentType = "explicit",
            ApplicationType = "web",
            RedirectUris = new List<string> { "http://application-ten/pauth/authentication/login-callback" },
            PostLogoutRedirectUris = new List<string> { "http://application-ten/pauth/authentication/logout-callback" },
            Permissions = new List<string>() { "authorization", "logout", "token", "authorization_code", "refresh_token" ,
                    "email", "profile", "roles", "code", "pkce"}
        });
        applications.Add(new Application()
        {
            FromTemplate = "None",
            ClientId = "application-11",
            ApplicationType = "web",
            DisplayName = "Application Eleven",
            ClientType = "confidential",
            ConsentType = "explicit",
            ClientSecret = "application-secret-eleven",
            JsonWebKeySet = $"""
                    -----BEGIN EC PRIVATE KEY-----
                    MHcCAQEEIMGxf/eMzKuW2F8KKWPJo3bwlrO68rK5+xCeO1atwja2oAoGCCqGSM49
                    AwEHoUQDQgAEI23kaVsRRAWIez/pqEZOByJFmlXda6iSQ4QqcH23Ir8aYPPX5lsV
                    nBsExNsl7SOYOiIhgTaX6+PTS7yxTnmvSw==
                    -----END EC PRIVATE KEY-----
                    """,
            Permissions = new List<string>() { "introspection" }
        });        
    }

    public static IEnumerable<Application> GetAllApplications() => applications;
}
