using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Shared.ViewModels
{
    public class ApplicationViewModel
    {        
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if application is a web or native application.
        /// Native applications have relaxed redirect_uri comparisons
        /// </summary>
        [Required]
        public string ApplicationType { get; set; } = ApplicationTypes.Web;

        /// <summary>
        /// Gets or sets the client identifier associated with the application.
        /// </summary>
        [Required]
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the application type associated with the application.
        /// </summary>
        [Required]
        public string ClientType { get; set; } = string.Empty;
               
        [IgnoreDataMember]
        public bool IsConfidentialClient => ClientType?.Equals(ClientTypes.Confidential) ?? false;

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
        [Required]
        public string ConsentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name associated with the application.
        /// </summary>
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the permissions associated with the application.
        /// </summary>
        [Required]
        public List<string> Permissions { get; set; } = [];

        /// <summary>
        /// Gets the callback URLs associated with the application.
        /// </summary>      
        public List<Uri> RedirectUris { get; set; } = [];

        /// <summary>
        /// Gets the logout callback URLs associated with the application.
        /// </summary>      
        public List<Uri> PostLogoutRedirectUris { get; set; } = [];

        /// <summary>
        /// Gets the requirements associated with the application.
        /// </summary>
        [Required]
        public List<string> Requirements { get; set; } = [];

        /// <summary>
        /// Settings can be used to control the lifetime of access tokens, authorization codes, device codes,
        /// identity tokens, refresh tokens and user codes.
        /// </summary>
        [Required]
        public Dictionary<string, string> Settings { get; set; } = new(StringComparer.Ordinal);

    }
}
