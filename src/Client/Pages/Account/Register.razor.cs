using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Account
{
    public partial class Register : ComponentBase
    {
        [Inject]
        public IAccountsService AccountService { get; set; }

        protected RegisterViewModel model = new RegisterViewModel();
        protected bool success;

        private async Task OnValidSubmit(EditContext context)
        {
            success = await AccountService.RegisterAsync(model);
            StateHasChanged();
        }
    }
}
