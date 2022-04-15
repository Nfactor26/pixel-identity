using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels
{
    internal class RegisterPage
    {
        private readonly IPage page;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public RegisterPage(IPage page)
        {
            this.page = page;
        }

        /// <summary>
        /// Click the register button to navigate to registration page
        /// </summary>
        /// <returns></returns>
        public async Task GoToAsync()
        {           
            await page.ClickAsync("#registerPageLink");
        }

        /// <summary>
        /// Fill out required details and click the register button to complete registration
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task RegisterAsync(string userEmail, string password)
        {
            await page.FillAsync("#Input_Email", userEmail);
            await page.FillAsync("#Input_Password", password);
            await page.FillAsync("#Input_ConfirmPassword", password);
            await page.ClickAsync("#registerSubmit");
        }
    }
}
