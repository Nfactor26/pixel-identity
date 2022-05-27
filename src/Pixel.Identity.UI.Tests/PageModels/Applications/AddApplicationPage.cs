using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels.Applications;

internal class AddApplicationPage : ApplicationPage
{
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public AddApplicationPage(IPage page) : base(page)
    {

    }

    /// <summary>
    /// Click on applications link to navigate to applications/list page if required
    /// and click the new application button on applications/list page
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
                return request.Url.EndsWith("api/applications") && request.Method == "GET";
            });
        }
        await page.ClickAsync("#btnNew");
    }


    /// <summary>
    /// Select a preset in dropdown
    /// </summary>
    /// <returns></returns>
    public async Task ApplyPreset(string preset)
    {
        await this.page.Locator("input#cbPresets+div.mud-select-input").ClickAsync();
        var count = await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").CountAsync();
        for (int i = 0; i < count; i++)
        {
            if ((await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").Nth(i).InnerTextAsync()).Equals(preset))
            {
                await this.page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").Nth(i).ClickAsync();
                break;
            }
        }
    }

    /// <summary>
    /// Add a new application
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public async Task AddApplication(Application application)
    {
        await ApplyPreset(application.FromTemplate);
        await SetClientId(application.ClientId);
        await SetDisplayName(application.DisplayName);
        switch (application.FromTemplate)
        {
            case "Authorization Code Flow":
                await this.AddRedirectUri(application.RedirectUris);
                await this.AddPostLogoutRedirectUri(application.PostLogoutRedirectUris);
                break;
            case "Client Credentials Flow":
            case "Introspection":
                await SetClientSecret(application.ClientSecret);
                break;
            case "Device Authorization Flow":
                break;
            case "None":
                await SetConsentType(application.ConsentType);
                await SetClientType(application.Type);
                if (application.Type == "confidential")
                {
                    await SetClientSecret(application.ClientSecret);
                }
                foreach (var permission in application.Permissions)
                {
                    await AddPermission(permission);
                }
                if (application.RedirectUris.Any() && application.PostLogoutRedirectUris.Any())
                {
                    await AddRedirectUri(application.RedirectUris);
                    await AddPostLogoutRedirectUri(application.RedirectUris);
                }
                break;
        }
        await Submit(!string.IsNullOrEmpty(application.ClientSecret));

    }

    /// <summary>
    /// Click on Create Button and close success alert
    /// </summary>
    /// <param name="hasClientSecret"></param>
    /// <returns></returns>
    public async Task Submit(bool hasClientSecret)
    {
        await page.RunAndWaitForNavigationAsync(async () =>
        {
            await this.page.Locator("button[type='submit']").ClickAsync();
            if (hasClientSecret)
            {
                await this.page.Locator("div[role='dialog'] button").ClickAsync();
            }
        },
       new PageRunAndWaitForNavigationOptions()
       {
           UrlRegex = new System.Text.RegularExpressions.Regex(".*/applications/list")
       });
        //wait for the snackbar to show up
        await page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });
        await page.Locator("div.mud-snackbar.mud-alert-filled-success button").ClickAsync();
        await page.WaitForSelectorAsync("div.mud-snackbar.mud-alert-filled-success", new PageWaitForSelectorOptions()
        {
            State = WaitForSelectorState.Detached,
            Timeout = 5000
        });
    }
}

