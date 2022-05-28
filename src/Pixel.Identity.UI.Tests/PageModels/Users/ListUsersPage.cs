using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Users;

internal class ListUsersPage : ListPage
{
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    /// <param name="table"></param>
    public ListUsersPage(IPage page) : base(page, new TableComponent(page, page.Locator("#tblUsers")))
    {
    }

    protected override string GetRequestUrl()
    {
        return "api/users";
    }

    /// <summary>
    /// Click on roles link to navigate to roles/list page if required   
    /// </summary>  
    /// <returns></returns>
    public async Task GoToAsync()
    {
        if (!page.Url.EndsWith("users/list"))
        {
            await page.RunAndWaitForRequestAsync(async () =>
            {
                await page.ClickAsync("a[href='./users/list']");
            }, request =>
            {
                return request.Url.Contains("api/users?") && request.Method == "GET";
            });
            await this.page.Locator("div#tblUsers table tbody tr").Nth(0).WaitForAsync(new LocatorWaitForOptions()
            {
                Timeout = 5000
            });
        }
    }
}
