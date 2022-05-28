using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.ComponentModels;

internal class DialogComponent
{
    protected readonly IPage page;
    
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    /// <param name="table"></param>
    public DialogComponent(IPage page)
    {
        this.page = page;       
    }

    /// <summary>
    /// Return true if success dialog is found else false after closing the success or error dialog.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> EnsureSuccessAsync()
    {
        await page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });

        if (await page.Locator("div.mud-snackbar.mud-alert-filled-success button").IsVisibleAsync())
        {            
            await page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
            return true;
        }
        else if (await page.Locator("div.mud-snackbar.mud-alert-filled-error button").IsVisibleAsync())
        {
            await page.Locator("div.mud-snackbar.mud-alert-filled-error button").ClickAsync();
            return false;
        }       
        return false;
    }

    /// <summary>
    /// Return true if error dialog is found else false after closing the error or success dialog.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> EnsureErrorAsync()
    {
        await page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });

        if (await page.Locator("div.mud-snackbar.mud-alert-filled-error button").IsVisibleAsync())
        {
            await page.Locator("div.mud-snackbar.mud-alert-filled-error button").ClickAsync();
            return true;
        }
        else if (await page.Locator("div.mud-snackbar.mud-alert-filled-success button").IsVisibleAsync())
        {
            await page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
            return false;
        }        
        return false;
    }
}
