using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Roles;

internal class ListRolesPage : ListPage
{       
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public ListRolesPage(IPage page) : base(page, new TableComponent(page, page.Locator("#tblRoles")))
    {
     
    }

    protected override string GetRequestUrl()
    {
        return "api/roles";
    }

    /// <summary>
    /// Click on roles link to navigate to roles/list page if required   
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
                return request.Url.Contains("api/roles?") && request.Method == "GET";
            });
        }
    }
}
