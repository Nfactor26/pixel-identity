using System;

namespace Pixel.Identity.Shared.Branding;

public static class BrandingProperties
{
    public static String Name { get; set; } = BrandingDefaults.Name;
    public static String ShortName { get; set; } = BrandingDefaults.ShortName;
    public static String LogoUriDark { get; set; } = BrandingDefaults.LogoUriDark;
    public static String LogoUriLight { get; set; } = BrandingDefaults.LogoUriLight;
}
