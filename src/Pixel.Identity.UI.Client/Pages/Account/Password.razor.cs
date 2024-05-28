using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    public partial class Password : ComponentBase
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IExternalLoginsService ExternalLoginsService { get; set; }


        bool hasLocalAccount = false;
        List<UserLoginInfo> externalLogins = new List<UserLoginInfo>(4);


        protected override async Task OnInitializedAsync()
        {
            hasLocalAccount = await AccountService.GetHasPasswordAsync();
            var userExternalLogins = await ExternalLoginsService.GetExternalLoginsAsync();
            this.externalLogins.AddRange(userExternalLogins);
            await base.OnInitializedAsync();
        }

        async Task RemoveExternalLoginAsync(UserLoginInfo userLoginInfo)
        {
            var result = await ExternalLoginsService.RemoveExternalLoginAsync(userLoginInfo);
            if (result.IsSuccess)
            {
                SnackBar.Add("External login removed successfully.", Severity.Success);
                this.externalLogins.Remove(userLoginInfo);
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
