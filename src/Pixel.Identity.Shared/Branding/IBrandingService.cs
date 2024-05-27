using System.Threading.Tasks;

namespace Pixel.Identity.Shared.Branding;

public interface IBrandingService
{
    Task<Brand> GetBrandAsync();
}
