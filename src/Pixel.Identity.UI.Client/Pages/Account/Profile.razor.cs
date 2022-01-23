using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    /// <summary>
    /// Component to manage profile
    /// </summary>
    public partial class Profile : ComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
     
        [Inject]
        public ISnackbar SnackBar { get; set; }
      
        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IUsersService UsersService { get; set; }

        UserDetailsViewModel user;

        ChangeEmailModel changeEmailModel = new();

        bool isEditingEmail = false;

        /// <summary>
        /// Get the user details for logged in user and show user details.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await authenticationStateTask;
                user =  await UsersService.GetUserByNameAsync(authState.User.Identity.Name);
                changeEmailModel.NewEmail = user.Email;
            }
            catch (Exception ex)
            {
                SnackBar.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });               
            }
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Toggle the visibility of Edit Email form
        /// </summary>
        void ToggleEditEmail()
        {
            isEditingEmail = !isEditingEmail;
        }

        /// <summary>
        /// Sends a verification mail to user to verify new email.
        /// Email and UserName will be updaated once new email is verified.
        /// </summary>
        /// <returns></returns>
        async Task ChangeEmailAsync()
        {
            var result = await AccountService.ChangeEmailAsync(changeEmailModel);
            if (result.IsSuccess)
            {
                SnackBar.Add("Verification email sent. Email will be updated once verified.", Severity.Success);               
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }
}
