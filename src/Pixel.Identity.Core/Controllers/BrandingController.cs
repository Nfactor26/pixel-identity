using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pixel.Identity.Shared.Branding;

namespace Pixel.Identity.Core.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class BrandingController(IBrandingService brandService) : Controller
{
    private readonly IBrandingService _brandService = brandService;

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var brand = await _brandService.GetBrandAsync();
        return Ok(brand);
    }
}
