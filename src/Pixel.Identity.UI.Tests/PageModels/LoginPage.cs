using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels;

internal class LoginPage
{      
    private readonly IPage page;
    
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    public LoginPage(IPage page)
    {
        this.page = page;         
    }

    /// <summary>
    /// Navigate to login page
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task GoToAsync(string baseUrl)
    {
        await page.GotoAsync($"{baseUrl}/Identity/Account/Login");
    }

    /// <summary>
    /// Perform login action on page. Make sure to call AuthorizeRequestedScopesAsync()
    /// </summary>
    /// <param name="userEmail">UserName for account</param>
    /// <param name="password">Password for account</param>
    /// <param name="rememberMe">Indicates whether the remember me checkbox should be checked or not</param>
    /// <returns></returns>
    public async Task LoginAsync(string userEmail, string password, bool rememberMe)
    {
        await this.page.FillAsync("#Input_Email",userEmail);
        await this.page.FillAsync("#Input_Password", password);
        if(rememberMe)
        {
            await this.page.CheckAsync("#Input_RememberMe");
        }
        //await page.RunAndWaitForNavigationAsync(async () =>
        //{
        //    await this.page.ClickAsync("#login-submit");
        //});
        await this.page.ClickAsync("#login-submit");
    }

    /// <summary>
    /// Click on forgot password link
    /// </summary>
    /// <returns></returns>
    public async Task ClickForgotPasswordAsync()
    {
        var forgotPassword = this.page.Locator("#forgot-password");
        await forgotPassword.ClickAsync();
    }

    /// <summary>
    /// Click on resend email confirmation link
    /// </summary>
    /// <returns></returns>
    public async Task ClickResendEmailConfirmationAsync()
    {
        var resentConfirmation = this.page.Locator("#resend-confirmation");
        await resentConfirmation.ClickAsync();
    }

}
