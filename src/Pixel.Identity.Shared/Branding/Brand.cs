using System;

namespace Pixel.Identity.Shared.Branding;

public record Brand(String Name, String ShortName, String LogoUriDark, String LogoUriLight)
{
    public static readonly Brand Empty = new(BrandingDefaults.PleaseWait, BrandingDefaults.PleaseWait, String.Empty, String.Empty);
}
