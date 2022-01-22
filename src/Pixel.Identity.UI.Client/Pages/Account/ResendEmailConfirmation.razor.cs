using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    public partial class ResendEmailConfirmation : ComponentBase
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        ResendEmailConfirmationModel model = new ();

        /// <summary>
        /// Send password reset link
        /// </summary>
        /// <returns></returns>
        async Task ResendEmailConfirmationLinkAsync()
        {
            var result = await AccountService.ResendEmailConfirmationAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Please check your mail for email confirmation link.", Severity.Success);
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
