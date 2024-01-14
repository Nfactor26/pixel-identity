using System;
using System.Collections.Generic;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Pixel.Identity.Shared.Helpers;

public static class TokenLifeTimesHelper
{
    public static IEnumerable<string> TokenLifeTimeNames { get; } = new List<string>()
    {
        nameof(Settings.TokenLifetimes.AccessToken),
        nameof(Settings.TokenLifetimes.AuthorizationCode),
        nameof(Settings.TokenLifetimes.DeviceCode),
        nameof(Settings.TokenLifetimes.IdentityToken),
        nameof(Settings.TokenLifetimes.RefreshToken),
        nameof(Settings.TokenLifetimes.UserCode)
    };

    public static string GetValueFromName(string  settingName)
    {
        switch (settingName)
        {
            case nameof(Settings.TokenLifetimes.AccessToken):
                return Settings.TokenLifetimes.AccessToken;
            case nameof(Settings.TokenLifetimes.AuthorizationCode):
                return Settings.TokenLifetimes.AuthorizationCode;
            case nameof(Settings.TokenLifetimes.DeviceCode):
                return Settings.TokenLifetimes.DeviceCode;
            case nameof(Settings.TokenLifetimes.IdentityToken):
                return Settings.TokenLifetimes.IdentityToken;
            case nameof(Settings.TokenLifetimes.RefreshToken):
                return Settings.TokenLifetimes.RefreshToken;
            case nameof(Settings.TokenLifetimes.UserCode):
                return Settings.TokenLifetimes.UserCode;
            default:
                throw new ArgumentException($"{settingName} doesn't exist", nameof(settingName));
        }
    }

    public static string GetNameFromValue(string settingValue)
    {
        switch (settingValue)
        {
            case Settings.TokenLifetimes.AccessToken:
                return nameof(Settings.TokenLifetimes.AccessToken);              
            case Settings.TokenLifetimes.AuthorizationCode:
                return nameof(Settings.TokenLifetimes.AuthorizationCode);            
            case Settings.TokenLifetimes.DeviceCode:
                return nameof(Settings.TokenLifetimes.DeviceCode);
            case Settings.TokenLifetimes.IdentityToken:
                return nameof(Settings.TokenLifetimes.IdentityToken);
            case Settings.TokenLifetimes.RefreshToken:
                return nameof(Settings.TokenLifetimes.RefreshToken);         
            case Settings.TokenLifetimes.UserCode:
                return nameof(Settings.TokenLifetimes.UserCode);
            default:
                throw new ArgumentException($"{settingValue} doesn't exist", nameof(settingValue));
        }
    }
}
