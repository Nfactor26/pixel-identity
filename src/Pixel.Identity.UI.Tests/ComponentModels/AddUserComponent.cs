using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.Helpers;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.ComponentModels
{
    internal class AddUserComponent
    {
        private readonly IPage page;
        private readonly DialogComponent dialogComponent;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public AddUserComponent(IPage page)
        {
            this.page = page;
            this.dialogComponent = new DialogComponent(page);
        }

        /// <summary>
        /// Fill user email and password on create new user dialog screen
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public async Task FillNewUserDetails(User userDetails)
        {            
            var dialog = this.page.Locator("div[role='dialog']");
            await dialog.Locator("#txtEmail").FillAsync(userDetails.Email);
            await dialog.Locator("#txtPassword").FillAsync(userDetails.Password);
            await dialog.Locator("#txtConfirmPassword").FillAsync(userDetails.Password);
        }

        /// <summary>
        /// Click Create button to submit the form
        /// </summary>
        /// <returns></returns>
        public async Task SumbitFormAsync()
        {
            var dialog = this.page.Locator("div[role='dialog']");
            await dialog.Locator("#btnAddUser").ClickAsync();
        }

        /// <summary>
        /// Click Create button to submit the form
        /// </summary>
        /// <returns></returns>
        public async Task CloseDialogAsync()
        {
            var dialog = this.page.Locator("div[role='dialog']");
            await dialog.Locator("button[aria-label='close']").ClickAsync();
        }

        /// <summary>
        /// Check if a given error message is displayed on form
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public async Task<bool> HasErrorMessage(string errorMessage)
        {
            var dialog = this.page.Locator("div[role='dialog']");
            var errors = await dialog.Locator("div.me-auto").AllAsync();
            foreach (var error in errors)
            {
               var errorDisplay = await error.TextContentAsync();
               if(errorDisplay.Equals(errorMessage)) 
               { 
                    return true;
               }
            }
            return false;
        }

        /// <summary>
        /// Click Create button to submit the form and close notification
        /// </summary>
        /// <returns>true if user was created successfully else false</returns>
        public async Task<bool> SubmitAndCloseNotificationAsync()
        {             
            await page.RunAndWaitForRequestAsync(async () =>
            {
                var dialog = this.page.Locator("div[role='dialog']");
                await dialog.Locator("#btnAddUser").ClickAsync();
            }, request =>
            {
                return request.Url.EndsWith($"api/users") && request.Method == "POST";
            });

            return await dialogComponent.EnsureSuccessAsync();           
        }
    }
}
