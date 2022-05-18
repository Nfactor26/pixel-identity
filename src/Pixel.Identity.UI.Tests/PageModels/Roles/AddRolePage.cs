using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Roles;

internal class AddRolePage
{
    private readonly IPage page;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public AddRolePage(IPage page)
    {
        this.page = page;
    }

    /// <summary>
    /// Click on roles link to navigate to roles/list page if required
    /// and click the new role button on roles/list page
    /// </summary> 
    /// <returns></returns>
    public async Task GoToAsync()
    {
        if (!page.Url.EndsWith("roles/list"))
        {
            await page.RunAndWaitForRequestAsync(async () =>
            {
                await page.ClickAsync("a[href='./roles/list']");
            }, request =>
            {
                return request.Url.EndsWith("api/roles") && request.Method == "GET";
            });
        }
        await page.ClickAsync("#btnNew");
    }

    /// <summary>
    /// Add a new role and wait for navigation to roles/list page.
    /// Close the success snack alert shown after creating the role succesfully.
    /// </summary>
    /// <param name="roleName">Name of the role to create</param>
    /// <returns></returns>
    public async Task CreateRole(string roleName)
    {
        await page.RunAndWaitForNavigationAsync(async () =>
        {
            await page.FillAsync("#inputRoleName", roleName);
            await page.ClickAsync("#btnCreate");
        }, new PageRunAndWaitForNavigationOptions()
        {
            UrlRegex = new System.Text.RegularExpressions.Regex(".*/roles/list")
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
