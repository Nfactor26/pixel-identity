using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Roles;

internal class EditRolePage
{
    private readonly IPage page;
    private readonly TableComponent claimsTable;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public EditRolePage(IPage page)
    {
        this.page = page;
        claimsTable = new TableComponent(page, page.Locator("#tblClaims"));
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
        await page.ClickAsync("#btnNewClaim");
        await page.FillAsync("#txtClaimType", type);
        await page.FillAsync("#txtClaimValue", value);
        await page.SetCheckedAsync("#cbIncludeInAccessToken", includeInAccessToken);
        await page.SetCheckedAsync("#cbIncludeInIdentityToken", includeInIdentityToken);
        await page.RunAndWaitForRequestAsync(async () =>
        {
            await page.Locator("#btnAddNewClaim").ClickAsync();
        }, request =>
        {
            return request.Url.EndsWith($"/api/roles/add/claim") && request.Method == "POST";
        });

        //wait for success dialog to popup for 5 second
        await page.Locator("div.mud-snackbar.mud-alert-filled-success button").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });
        if (await page.Locator("div.mud-snackbar.mud-alert-filled-success button").IsVisibleAsync())
        {
            await page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
            return true;
        }

        //close the claim dialog and return false indicating failure
        await page.Locator("div[role='dialog'] button[aria-label='close'])").ClickAsync();
        return false;
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
        await claimsTable.SearchAsync(claimType);
        var rows = await claimsTable.GetMatchingRowsAsync(async (row) =>
        {
            bool isClaimTypeMatch = (await row.Locator("#tdType").TextContentAsync())?.Equals(claimType) ?? false;
            bool isClaimValueMatch = (await row.Locator("#tdValue").TextContentAsync())?.Equals(claimValue) ?? false;
            return isClaimTypeMatch && isClaimValueMatch;
        });
        foreach (var row in rows)
        {
            await row.Locator("#btnEdit").ClickAsync();
            await row.Locator("#txtClaimType").FillAsync(newClaimType);
            await row.Locator("#txtClaimValue").FillAsync(newClaimValue);
            await row.Locator("#cbIncludeInAccessToken").SetCheckedAsync(newIncludeInAccessToken);
            await row.Locator("#cbIncludeInIdentityToken").SetCheckedAsync(newIncludeInIdentityToken);
            await page.RunAndWaitForRequestAsync(async () =>
            {
                await row.Locator("button").Nth(0).ClickAsync();
            }, request =>
            {
                return request.Url.EndsWith("api/roles/update/claim") && request.Method == "POST";
            });
            await page.ClickAsync("div.mud-snackbar.mud-alert-filled-success button");
        }
        return rows.Count();
    }

    /// <summary>
    /// Delete all the claims matching claimType and claimValue
    /// </summary>
    /// <param name="claimType"></param>
    /// <param name="claimValue"></param>
    /// <returns>Number of deleted rows</returns>
    public async Task<int> DeleteClaimsAsync(string claimType, string claimValue)
    {
        await claimsTable.SearchAsync(claimType);
        var rows = await claimsTable.GetMatchingRowsAsync(async (row) =>
        {
            bool isClaimTypeMatch = (await row.Locator("#tdType").TextContentAsync())?.Equals(claimType) ?? false;
            bool isClaimValueMatch = (await row.Locator("#tdValue").TextContentAsync())?.Equals(claimValue) ?? false;
            return isClaimTypeMatch && isClaimValueMatch;
        });
        foreach (var row in rows)
        {
            await page.RunAndWaitForRequestFinishedAsync(async () =>
            {
                await row.Locator("#btnDelete").ClickAsync();
            },
            new PageRunAndWaitForRequestFinishedOptions()
            {
                Predicate = request => request.Url.EndsWith("api/roles/delete/claim") && request.Method == "POST"
            });
            await page.ClickAsync("div.mud-snackbar.mud-alert-filled-success button");
        }
        return rows.Count();
    }
}
