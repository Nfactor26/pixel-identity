using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels;

internal class ListRolesPage
{
    private readonly IPage page;
    private readonly TableComponent rolesTable;
    private readonly string requestUrl = "api/roles";

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public ListRolesPage(IPage page)
    {
        this.page = page;     
        this.rolesTable = new TableComponent(page, page.Locator("#tblRoles"));
    }

    /// <summary>
    /// Click on roles link to navigate to roles/list page if required   
    /// </summary>  
    /// <returns></returns>
    public async Task GoToAsync()
    {
        if(!this.page.Url.EndsWith("roles/list"))
        {
            await this.page.RunAndWaitForRequestAsync(async () =>
            {
                await this.page.ClickAsync("a[href='./roles/list']");
            }, request =>
            {                
                return request.Url.Contains("api/roles?") && request.Method == "GET";
            });
        }       
    }

    /// <summary>
    /// Click on add new button on list page to navigate to add new role page
    /// </summary>
    /// <returns></returns>
    public async Task AddNew()
    {
        await this.page.ClickAsync("#btnNewRole");
    }

    /// <summary>
    /// Search roles matching specified filter
    /// </summary>
    /// <param name="filter">search text</param>
    /// <returns></returns>
    public async Task SearchAsync(string filter)
    {
      await this.rolesTable.SearchAndWaitForRequestAsync(filter, requestUrl);       
    }

    /// <summary>
    /// Get the number of roles avaialbe on roles table
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetRolesCountAsync()
    {
        return await this.rolesTable.GetRowCountAsync();
    }

    /// <summary>
    /// Search for specified role and click the edit button
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> EditRoleAsync(string roleName)
    {
        await this.SearchAsync(roleName);
        return await this.rolesTable.EditFirstMatchingRowAsync(async (row) => (await row.Locator("#tdRoleName").TextContentAsync())?.Equals(roleName) 
                                                                               ?? false, requestUrl);
    }

    /// <summary>
    /// Search for specifed role and click on delete button
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        await this.SearchAsync(roleName);
        return await this.rolesTable.DeleteFirstMatchingRowAsync(async (row) => (await row.Locator("#tdRoleName").TextContentAsync())?.Equals(roleName) 
                                                                                ?? false , requestUrl);        
    }

    /// <summary>
    /// Set page size on table
    /// </summary>
    /// <param name="pageSize">PageSize e.g. 10, 20</param>
    /// <returns></returns>
    public async Task<bool> SetPageSizeAsync(string pageSize)
    {
        return await this.rolesTable.SetPageSizeAndWaitForRequestAsync(pageSize, requestUrl);  
    }

    /// <summary>
    /// Check if it possible to navigate to next page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CanNavigateToNext()
    {
        return await this.rolesTable.CanNavigateToNext();
    }

    /// <summary>
    /// Navigate to next page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> NavigateToNextAsync()
    {
       return await this.rolesTable.NavigateToNextAndWaitForRequestAsync(requestUrl);
    }

    /// <summary>
    /// Check if it is possible to navigate to previous page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CanNavigateToPrevious()
    {
       return await this.rolesTable.CanNavigateToPrevious();
    }

    /// <summary>
    /// Navigate to previous page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> NavigateToPreviousAsync()
    {
        return await this.rolesTable.NavigateToPreviousAndWaitForRequestAsync(requestUrl);
    }
}
