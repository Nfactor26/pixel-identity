using OpenIddict.Abstractions;

namespace Pixel.Identity.Shared.ViewModels
{
    public enum ApplicationPreset
    {
        AuthorizationCodeFlow,
        ClientCredentialsFlow,
        DeviceAuthorizationFlow,
        ResourceOwnerPasswordCredentialsFlow,
        None
    }

    public static class ApplicationPresetManager
    {        
        public static void ApplyPreset(this ApplicationViewModel applicationViewModel, ApplicationPreset preset)
        {
            applicationViewModel.Permissions.Clear();
            applicationViewModel.Requirements.Clear();
            applicationViewModel.Type = OpenIddictConstants.ClientTypes.Public;
            applicationViewModel.ConsentType = OpenIddictConstants.ConsentTypes.Explicit;

            switch (preset)
            {
                case ApplicationPreset.AuthorizationCodeFlow:
                    applicationViewModel.Requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    break;
                case ApplicationPreset.ClientCredentialsFlow:
                    applicationViewModel.Type = OpenIddictConstants.ClientTypes.Confidential;
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);                  
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                    break;
                case ApplicationPreset.DeviceAuthorizationFlow:                   
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);                   
                    break;
            }

            applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
            applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
            applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);
        }

        public static string ToDisplayString(this ApplicationPreset preset)
        {            
            return preset switch
            {
                ApplicationPreset.AuthorizationCodeFlow => "Authorization Code Flow",
                ApplicationPreset.ClientCredentialsFlow => "Client Credentials Flow",
                ApplicationPreset.DeviceAuthorizationFlow => "Device Authorization Flow",
                ApplicationPreset.ResourceOwnerPasswordCredentialsFlow => "Resource Owner Password Credentials Flow",
                ApplicationPreset.None => "None",
                _ => preset.ToString()
            };
        }
    }
}
