using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Users;

internal class EditUserPage
{
    private readonly IPage page;
    private readonly DialogComponent dialogComponent;
    private readonly ClaimsManagerComponent claimsManager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public EditUserPage(IPage page)
    {
        this.page = page;
        this.dialogComponent = new DialogComponent(this.page);
        this.claimsManager = new UserClaimsManagerComponent(this.page, this.dialogComponent);
    }

    /// <summary>
    /// Navigate to edit scope page for a given scopeName
    /// </summary>
    /// <param name="userId">Scope to edit</param>
    /// <returns></returns>
    public async Task GoToAsync(string userId)
    {
        await page.GotoAsync($"/users/edit/{userId}");
    }


    /// Assign a new role to user
    /// </summary>
    /// <param name="roleToAdd"></param>
    /// <returns></returns>
    public async Task<bool> TryAddRole(string roleToAdd)
    {
        await this.page.Locator("button#btnAddRole").ClickAsync();
        var dialog = this.page.Locator("div[role='dialog']");
        await dialog.Locator("input.mud-select-input").TypeAsync(roleToAdd);
        await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });       
        var exists = (await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").CountAsync()) == 1;
        if (exists)
        {
            await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").ClickAsync();
            await dialog.Locator("button#btnAddRole").ClickAsync();           
            return await dialogComponent.EnsureSuccessAsync();           
        }
        return false;
    }

    /// Remove assigned role from user
    /// </summary>
    /// <param name="roleToRemove"></param>
    /// <returns></returns>
    public async Task<bool> TryRemoveRole(string roleToRemove)
    {
        int count = await this.page.Locator("div#rolesCollection div.mud-grid-item").CountAsync();
        for (int i = 0; i < count; i++)
        {
            var text = await this.page.Locator("div#rolesCollection div.mud-grid-item").Nth(i).InnerTextAsync();
            if (text.Trim().Equals(roleToRemove))
            {
                var deleteButton = this.page.Locator("div#rolesCollection div.mud-grid-item button").Nth(i);
                await deleteButton.ClickAsync();               
                return await dialogComponent.EnsureSuccessAsync();
            }
        }
        return false;
    }

    /// <summary>
    /// Add a new claim to user
    /// </summary>
    /// <returns></returns>
    public async Task<bool> AddClaimAsync(string type, string value, bool includeInAccessToken, bool includeInIdentityToken)
    {
       return await this.claimsManager.AddClaimAsync(type, value, includeInAccessToken, includeInIdentityToken);
    }

    /// <summary>
    /// Remove existing claims from user matching specified claimType and claimValue.
    /// </summary>
    /// <returns>Number of claims deleted</returns>
    public async Task<int> DeleteClaimsAsync(string claimType, string claimValue)
    {
        return await this.claimsManager.DeleteClaimsAsync(claimType, claimValue);
    }
}
