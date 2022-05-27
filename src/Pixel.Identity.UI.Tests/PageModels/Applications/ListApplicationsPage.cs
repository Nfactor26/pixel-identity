using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Applications;

internal class ListApplicationsPage : ListPage
{
    private readonly string requestUrl = "api/applications";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public ListApplicationsPage(IPage page) : base(page, new TableComponent(page, page.Locator("#tblApplications")))
    {

    }

    protected override string GetRequestUrl()
    {
        return "api/applications";
    }

    /// <summary>
    /// Click on roles link to navigate to roles/list page if required   
    /// </summary>  
    /// <returns></returns>
    public async Task GoToAsync()
    {
        if (!page.Url.EndsWith("applications/list"))
        {
            await page.RunAndWaitForRequestAsync(async () =>
            {
                await page.ClickAsync("a[href='./applications/list']");
            }, request =>
            {
                return request.Url.Contains("api/applications?") && request.Method == "GET";
            });
        }
    }
}