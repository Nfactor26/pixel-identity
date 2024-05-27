using Pixel.Identity.Shared.Branding;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services;

public class RemoteBrandService(HttpClient httpClient) : IBrandingService
{
    private readonly HttpClient httpClient = httpClient;
    private Brand _brand;

    public async Task<Brand> GetBrandAsync()
    {
        if (_brand == null)
        {
            _brand = await httpClient.GetFromJsonAsync<Brand>("api/branding");
        }

        return _brand;
    }
}
