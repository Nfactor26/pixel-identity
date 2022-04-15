using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels;

internal class LogoutPage
{
    private readonly IPage page;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public LogoutPage(IPage page)
    {
        this.page = page;
    }

    /// <summary>
    /// Logout using the sign out menu item
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        await this.page.ClickAsync("#signedInMenu");
        await this.page.ClickAsync("#signOutMenuItem");
        await page.RunAndWaitForNavigationAsync(async () =>
        {
            await this.page.ClickAsync("#confirmLogoutButton");
        });       
    }
}
