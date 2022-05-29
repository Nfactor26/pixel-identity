using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Scopes
{
    internal class ListScopesPage : ListPage
    {        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public ListScopesPage(IPage page) :base(page, new TableComponent(page, page.Locator("#tblScopes")))
        {
           
        }

        protected override string GetRequestUrl()
        {
            return "api/scopes";
        }

        /// <summary>
        /// Click on roles link to navigate to roles/list page if required   
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
                    return request.Url.Contains("api/scopes?") && request.Method == "GET";
                });
                await this.page.Locator("div#tblScopes table tbody tr").Nth(0).WaitForAsync(new LocatorWaitForOptions()
                {
                    Timeout = 5000
                });
            }
        }       
    }
}
