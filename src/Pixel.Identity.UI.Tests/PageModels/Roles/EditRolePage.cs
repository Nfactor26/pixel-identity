using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Roles;

internal class EditRolePage
{
    private readonly IPage page;
    private readonly TableComponent claimsTable;
    private readonly DialogComponent dialogComponent;
    private readonly ClaimsManagerComponent claimsManager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public EditRolePage(IPage page)
    {
        this.page = page;
        this.claimsTable = new TableComponent(page, page.Locator("#tblClaims"));
        this.dialogComponent = new DialogComponent(this.page);
        this.claimsManager = new RoleClaimsManagerComponent(this.page, this.dialogComponent);
    }

    /// <summary>
    /// Navigate to edit role page for a given roleName
    /// </summary>
    /// <param name="roleName">Role to edit</param>
    /// <returns></returns>
    public async Task GoToAsync(string roleName)
    {
        await page.GotoAsync($"/roles/edit/{roleName}");
    }

    /// <summary>
    /// Rename the role being edited to a new value
    /// </summary>
    /// <param name="newRoleName">New role name</param>
    /// <returns>True if rename was successful. False otherwise</returns>
    public async Task<bool> RenameRoleAsync(string newRoleName)
    {
        await page.ClickAsync("#toggleRoleNameEdit");
        await page.FillAsync("#txtRoleName", newRoleName);
        await page.RunAndWaitForRequestAsync(async () =>
        {
            await page.Locator("#btnUpdateRoleName").ClickAsync();
        }, request =>
        {
            return request.Url.EndsWith("/api/roles") && request.Method == "PUT";
        });
        //wait for the snackbar to show up
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
    /// Add a new claim to role
    /// </summary>
    /// <param name="type">type of claim</param>
    /// <param name="value">value of claim</param>
    /// <param name="includeInAccessToken">Indicate if claim should be included in access token</param>
    /// <param name="includeInIdentityToken">Indicate if claim should be included in identity token</param>
    /// <returns></returns>
    public async Task<bool> AddClaimAsync(string type, string value, bool includeInAccessToken, bool includeInIdentityToken)
    {
        return await this.claimsManager.AddClaimAsync(type, value, includeInAccessToken, includeInIdentityToken);
    }

    /// <summary>
    /// Search for claim matchin specified filter
    /// </summary>
    /// <param name="filter">filter to apply</param>
    /// <returns></returns>
    public async Task SearchForClaimAsync(string filter)
    {
        await claimsTable.SearchAsync(filter);
    }

    /// <summary>
    /// Update 
    /// </summary>
    /// <param name="claimType">current type of claim</param>
    /// <param name="claimValue">current value of claim</param>
    /// <param name="newClaimType">new type of claim</param>
    /// <param name="newClaimValue">new value of claim</param>
    /// <param name="newIncludeInAccessToken">new value of whether to include claim in access token</param>
    /// <param name="newIncludeInIdentityToken">new value of whether to include claim in identity token</param>
    /// <returns></returns>
    public async Task<int> EditClaimsAsync(string claimType, string claimValue, string newClaimType, string newClaimValue,
        bool newIncludeInAccessToken, bool newIncludeInIdentityToken)
    {
        return await this.claimsManager.EditClaimsAsync(claimType, claimValue, newClaimType, newClaimValue, 
            newIncludeInAccessToken, newIncludeInIdentityToken);
    }

    /// <summary>
    /// Delete all the claims matching claimType and claimValue
    /// </summary>
    /// <param name="claimType"></param>
    /// <param name="claimValue"></param>
    /// <returns>Number of deleted rows</returns>
    public async Task<int> DeleteClaimsAsync(string claimType, string claimValue)
    {
        return await this.claimsManager.DeleteClaimsAsync(claimType, claimValue);
    }
}
