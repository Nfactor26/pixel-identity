using Microsoft.Extensions.Configuration;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.PageModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.Tests;

public class LoginTests : PageTest
{
    private IConfiguration configuration;
    private string baseUrl;

    /// <summary>
    /// constructor
    /// </summary>
    public LoginTests()
    {
        configuration = ConfigurationFactory.Create();
        baseUrl = configuration["BaseUrl"];
    }

    /// <summary>
    /// Validate that email and password fields are required for sign in 
    /// </summary>
    /// <returns></returns>
    [Test, Order(1)]
    public async Task Validate_That_UserEmail_And_Credentials_Are_Required()
    {
        var loginPage = new LoginPage(this.Page);
        await loginPage.GoToAsync(this.baseUrl);
        await loginPage.LoginAsync(string.Empty, string.Empty, false);
        await Expect(Page.Locator("#Input_Email-error")).ToHaveTextAsync("The Email field is required.");
        await Expect(Page.Locator("#Input_Password-error")).ToHaveTextAsync("The Password field is required.");
    }

    /// <summary>
    /// Validate that user can not login with incorrect credentials
    /// </summary>
    /// <returns></returns>
    [Test, Order(2)]
    public async Task Validate_That_User_Can_Not_LogIn_With_Incorrect_Credentials()
    {
        var loginPage = new LoginPage(this.Page);
        await loginPage.GoToAsync(this.baseUrl);
        await loginPage.LoginAsync(this.configuration["UserEmail"], "unknown-secret", false);
        await Expect(Page.Locator("#account>div.text-danger")).ToHaveTextAsync("Invalid login attempt.");
    }

    /// <summary>
    /// validate taht user can login with correct email and password combination
    /// </summary>
    /// <returns></returns>
    [Test, Order(3)]
    public async Task Validate_That_User_Can_LogIn_With_Correct_Credentials()
    {        
        var loginPage = new LoginPage(this.Page);      
        await loginPage.GoToAsync(baseUrl);
        await loginPage.LoginAsync(configuration["UserEmail"], configuration["UserSecret"], false);       
        await Expect(this.Page.Locator("#signedInMenu")).ToBeVisibleAsync();
    }
}
