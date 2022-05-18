using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Scopes
{
    internal class AddScopePage
    {
        private readonly IPage page;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public AddScopePage(IPage page)
        {
            this.page = page;
        }

        /// <summary>
        /// Click on scopes link to navigate to scopes/list page if required
        /// and click the new scope button on scopes/list page
        /// </summary> 
        /// <returns></returns>
        public async Task GoToAsync()
        {
            if (!page.Url.EndsWith("scopes/list"))
            {
                await page.RunAndWaitForRequestAsync(async () =>
                {
                    await page.ClickAsync("a[href='./scopes/list']");
                }, request =>
                {
                    return request.Url.EndsWith("api/scopes") && request.Method == "GET";
                });
            }
            await page.ClickAsync("#btnNew");
        }

        /// <summary>
        /// Add a new scope and wait for navigation to scopes/list page.
        /// Close the success snack alert shown after creating the scope succesfully.
        /// </summary>
        /// <param name="scopeName"></param>
        /// <param name="description"></param>
        /// <param name="resources"></param>
        /// <returns></returns>
        public async Task CreateScope(string scopeName, string description, params string[] resources)
        {
            await page.RunAndWaitForNavigationAsync(async () =>
            {
                await page.FillAsync("#inputScopeName", scopeName);
                await page.FillAsync("#inputDisplayName", scopeName);
                await page.FillAsync("#inputDescription", description);
                foreach (var resource in resources)
                {
                    await page.ClickAsync("#btnAddResources");
                    await page.FillAsync("#inputResourceName", resource);
                    await page.ClickAsync("#btnAddResource");
                }
                await page.ClickAsync("#btnAddScope");
            }, new PageRunAndWaitForNavigationOptions()
            {
                UrlRegex = new System.Text.RegularExpressions.Regex(".*/scopes/list")
            });
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
}
