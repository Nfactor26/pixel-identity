using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Scopes
{
    internal class EditScopePage
    {
        private readonly IPage page;
       
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public EditScopePage(IPage page)
        {
            this.page = page;           
        }

        /// <summary>
        /// Navigate to edit scope page for a given scopeName
        /// </summary>
        /// <param name="scopeName">Scope to edit</param>
        /// <returns></returns>
        public async Task GoToAsync(string scopeName)
        {
            await page.GotoAsync($"/scopes/edit/{scopeName}");
        }

        public async Task<bool> UpdateAsync(string newDisplayName, string newDescription)
        {
            await page.FillAsync("#inputDisplayName", newDisplayName);
            await page.FillAsync("#inputDescription", newDescription);
            await page.ClickAsync("#btnUpdateScope");
          
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


        public async Task<bool> CheckIfResourceExists(string resource)
        {
            var resourceCount = await this.page.Locator("div.mud-grid > div.mud-grid-item").CountAsync();
            for (int i = 0; i < resourceCount; i++)
            {
                var resourceItem = this.page.Locator("div.mud-grid > div.mud-grid-item").Nth(i);
                if ((await resourceItem.TextContentAsync())?.Equals(resource) ?? false)
                {                   
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddResourceAsync(string resource)
        {
            await page.ClickAsync("#btnAddResources");
            await page.FillAsync("#inputResourceName", resource);
            await page.ClickAsync("#btnAddResource");
            await page.ClickAsync("#btnUpdateScope");
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

        public async Task<bool> DeleteResourceAsync(string resource)
        { 
            var resourceCount =  await this.page.Locator("div.mud-grid > div.mud-grid-item").CountAsync();
            for(int i=0; i<resourceCount; i++)
            {
                var resourceItem = this.page.Locator("div.mud-grid > div.mud-grid-item").Nth(i);
                if((await resourceItem.TextContentAsync())?.Equals(resource) ?? false)
                {
                    await resourceItem.Locator("button").ClickAsync();
                    return true;
                }
            }
            return false;
        }

    }
}
