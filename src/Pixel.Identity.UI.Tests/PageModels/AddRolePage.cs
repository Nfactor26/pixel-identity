using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels;

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
        if (!this.page.Url.EndsWith("roles/list"))
        {
            await this.page.RunAndWaitForRequestAsync(async () =>
            {
                await this.page.ClickAsync("a[href='./roles/list']");
            }, request =>
            {
                return request.Url.EndsWith("api/roles") && request.Method == "GET";
            });
        }       
        await this.page.ClickAsync("#btnNewRole");
    }

    /// <summary>
    /// Add a new role and wait for navigation to roles/list page.
    /// Close the success snack alert shown after creating the role succesfully.
    /// </summary>
    /// <param name="roleName">Name of the role to create</param>
    /// <returns></returns>
    public async Task CreateRole(string roleName)
    {
        await this.page.RunAndWaitForNavigationAsync(async () =>
        {
            await this.page.FillAsync("#inputRoleName", roleName);
            await this.page.ClickAsync("#btnCreate");
        }, new PageRunAndWaitForNavigationOptions()
        {
            UrlRegex = new System.Text.RegularExpressions.Regex(".*/roles/list")
        });      
        //wait for the snackbar to show up
        await this.page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });
        await this.page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
        await this.page.WaitForSelectorAsync("div.mud-snackbar.mud-alert-filled-success",new PageWaitForSelectorOptions()
        {
             State = WaitForSelectorState.Detached,
             Timeout = 5000
        });
    }
}
