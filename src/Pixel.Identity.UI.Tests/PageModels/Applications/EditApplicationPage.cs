using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.Helpers;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Applications;

internal class EditApplicationPage : ApplicationPage
{
    public EditApplicationPage(IPage page) : base(page)
    {      
    }

    /// <summary>
    /// Navigate to edit application page
    /// </summary>
    /// <param name="clientId">ClientId of the application to edit</param>
    /// <returns></returns>
    public async Task GoToAsync(string baseUrl, string clientId)
    {
        await page.GotoAsync($"{baseUrl}/applications/edit/{clientId}");
    }

    /// <summary>
    /// Click Update button and close success alert
    /// </summary>
    /// <param name="hasClientSecret"></param>
    /// <returns></returns>
    public async Task Submit(bool hasClientSecret)
    {
        await this.page.Locator("button[type='submit']").ClickAsync();
        if (hasClientSecret)
        {
            await this.page.Locator("div[role='dialog'] button").ClickAsync();
        }
        //wait for the snackbar to show up
        await page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });
        await page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
        await page.WaitForSelectorAsync("div.mud-snackbar.mud-alert-filled-success", new PageWaitForSelectorOptions()
        {
            State = WaitForSelectorState.Detached,
            Timeout = 5000
        });
    }
}

