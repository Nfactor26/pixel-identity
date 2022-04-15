using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.PageModels;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.Tests;

internal class RegisterTests : PageTest
{
    private readonly string baseUrl;

    public RegisterTests()
    {
        this.baseUrl = ConfigurationFactory.Create()["BaseUrl"];
    }
    /// <summary>
    /// Validate that email shows a validation error when the entered value is not a valid email.
    /// </summary>
    /// <returns></returns>
    [Test, Order(1)]
    public async Task Validate_That_UserEmail_Requires_Valid_Email()
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);
        await registrationPage.GoToAsync();
        await registrationPage.RegisterAsync("invalid-email-id", "secret");
        await Expect(this.Page.Locator("#Input_Email-error")).ToHaveTextAsync("The Email field is not a valid e-mail address.");       
    }

    /// <summary>
    /// Validate that user email and credentials are required for registration
    /// </summary>
    /// <returns></returns>
    [Test, Order(2)]
    public async Task Validate_That_UserEmail_And_Credentials_Are_Required()
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);
        await registrationPage.GoToAsync();
        await registrationPage.RegisterAsync(string.Empty, string.Empty);
        await Expect(this.Page.Locator("#Input_Email-error")).ToHaveTextAsync("The Email field is required.");
        await Expect(this.Page.Locator("#Input_Password-error")).ToHaveTextAsync("The Password field is required.");
    }


    /// <summary>
    /// Validate that password requires minimum number of characters as configured by policy.
    /// This is validate on client side
    /// </summary>
    /// <returns></returns>
    [Test, Order(3)]
    public async Task Validate_That_Password_Requires_Configured_Minimum_Length()
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);
        await registrationPage.GoToAsync();
        // Default configuration is to have minimum of 6 chars 
        await registrationPage.RegisterAsync("invalid-email-id", "aaaaa");
        await Expect(this.Page.Locator("#Input_Password-error")).ToHaveTextAsync("The Password must be at least 6 and at max 100 characters long.");
    }

    /// <summary>
    /// Validate that password field enforces uppercase, lowercase and digit requirements configured by policy.
    /// By default, password requires 1 lowercase, 1 uppercase and 1 digit and a non alphanumeric.     
    /// </summary>
    /// <returns></returns>
    [Test, Order(4)]
    public async Task Validate_That_Password_Requires_UpperCase_LowerCase_And_Digit()
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);
        await registrationPage.GoToAsync();
        // Default configuration is to have minimum of 6 chars 
        await registrationPage.RegisterAsync("test@pixel.com", "aaaaaaa");
        await Expect(this.Page.Locator("#registerForm>div.text-danger li:nth-child(1)")).ToHaveTextAsync("Passwords must have at least one non alphanumeric character.");
        await Expect(this.Page.Locator("#registerForm>div.text-danger li:nth-child(2)")).ToHaveTextAsync("Passwords must have at least one digit ('0'-'9').");
        await Expect(this.Page.Locator("#registerForm>div.text-danger li:nth-child(3)")).ToHaveTextAsync("Passwords must have at least one uppercase ('A'-'Z').");

    }

    /// <summary>
    /// Validate that account can be registerd by providing a valid email and password that matches configured policy.   
    /// </summary>
    /// <returns></returns>
    [Test, Order(5)]
    [TestCaseSource(typeof(UserCollection), nameof(UserCollection.GetAllUsers))]
    public async Task Validate_That_Can_Register_Account(User user)
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);       
        await registrationPage.GoToAsync();
        await registrationPage.RegisterAsync(user.Email, user.Password);     
        //After login, user should be able to see home, profile, logins and authenticator link without
        //any additional claims
        await Expect(this.Page.Locator("div.mud-navmenu.mud-navmenu-default a")).ToHaveCountAsync(4);
    }

    /// <summary>
    /// Validate that account can not be registered again with an existing email
    /// </summary>
    /// <returns></returns>
    [Test, Order(6)]
    public async Task Validate_That_Can_Not_Register_Account_With_Existing_Email()
    {
        await this.Page.GotoAsync(this.baseUrl);
        var registrationPage = new RegisterPage(this.Page);      
        var user = UserCollection.GetAllUsers().First();
        await registrationPage.GoToAsync();
        await registrationPage.RegisterAsync(user.Email, user.Password);    
        await Expect(Page.Locator("#registerForm>div.text-danger li")).ToHaveTextAsync($"Username '{user.Email}' is already taken.");
    }
}
