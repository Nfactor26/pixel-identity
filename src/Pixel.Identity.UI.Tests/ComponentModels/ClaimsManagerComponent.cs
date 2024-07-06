using Microsoft.Playwright;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.ComponentModels;

internal abstract class ClaimsManagerComponent
{
    private readonly IPage page;
    private readonly TableComponent claimsTable;
    private readonly DialogComponent dialogComponent;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public ClaimsManagerComponent(IPage page) : this(page, new DialogComponent(page))
    {
    }
      

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public ClaimsManagerComponent(IPage page, DialogComponent dialogComponent)
    {
        this.page = page;
        this.dialogComponent = dialogComponent;
        this.claimsTable = new TableComponent(page, page.Locator("#tblClaims"));
    }

    protected abstract string GetApiBasePath();  

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
            return request.Url.EndsWith($"{GetApiBasePath()}/add/claim") && request.Method == "POST";
        });

        var success = await dialogComponent.EnsureSuccessAsync();

        if(!success)
        {
            //close the claim dialog and return false indicating failure
            await page.Locator("div[role='dialog'] button[aria-label='Close dialog'])").ClickAsync();            
        }
        return success;
    }

    /// <summary>
    /// Search for claim matching specified filter
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
                return request.Url.EndsWith($"{GetApiBasePath()}/update/claim") && request.Method == "POST";
            });
            await dialogComponent.EnsureSuccessAsync();
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
                Predicate = request => request.Url.EndsWith($"{GetApiBasePath()}/delete/claim") && request.Method == "POST"
            });
            await dialogComponent.EnsureSuccessAsync();
        }
        return rows.Count();
    }
}

internal class UserClaimsManagerComponent : ClaimsManagerComponent
{
    public UserClaimsManagerComponent(IPage page) : base(page)
    {
    }
  
    public UserClaimsManagerComponent(IPage page, DialogComponent dialogComponent) : base(page, dialogComponent)
    {
       
    }

    protected override string GetApiBasePath()
    {
        return "/api/users";
    }
}

internal class RoleClaimsManagerComponent : ClaimsManagerComponent
{
    public RoleClaimsManagerComponent(IPage page) : base(page)
    {
    }

    public RoleClaimsManagerComponent(IPage page, DialogComponent dialogComponent) : base(page, dialogComponent)
    {

    }

    protected override string GetApiBasePath()
    {
        return "/api/roles";
    }
}
