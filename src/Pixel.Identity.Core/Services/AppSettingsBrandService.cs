using Pixel.Identity.Shared.Branding;

namespace Pixel.Identity.UI.Client.Services;

public class AppSettingsBrandService(IConfiguration configuration) : IBrandingService
{
    private readonly IConfiguration _configuration = configuration;

    private Brand _brand;

    public async Task<Brand> GetBrandAsync() => _brand ??= await BuildAsync();

    private Task<Brand> BuildAsync()
    {
        var name = _configuration["Brand:Name"] ?? BrandingProperties.Name;
        var shortName = _configuration["Brand:ShortName"] ?? BrandingProperties.ShortName;
        var logoUriDark = _configuration["Brand:LogoUriDark"] ?? BrandingProperties.LogoUriDark;
        var logoUriLight = _configuration["Brand:LogoUriLight"] ?? BrandingProperties.LogoUriLight;

        return Task.FromResult(new Brand(name, shortName, logoUriDark, logoUriLight));
    }
}
