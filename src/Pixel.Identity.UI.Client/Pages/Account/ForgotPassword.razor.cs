using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    public partial class ForgotPassword : ComponentBase
    {       
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        ForgotPasswordModel model = new (); 

        /// <summary>
        /// Send password reset link
        /// </summary>
        /// <returns></returns>
        async Task SendPasswordResetLinkAsync()
        {
            var result = await AccountService.SendPasswordResetLinkAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Please check your mail for password reset link.", Severity.Success);
                model.Email = string.Empty;
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
