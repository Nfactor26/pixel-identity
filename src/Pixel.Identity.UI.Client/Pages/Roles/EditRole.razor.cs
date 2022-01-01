using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    public partial class EditRole : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUserRolesService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public string Name { get; set; }

        UserRoleViewModel model;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                model = await Service.GetRoleByName(Name);
                if (model == null)
                {
                    SnackBar.Add("Failed to retrieve role.", Severity.Success, config =>
                    {
                        config.ShowCloseIcon = true;
                    });
                }
            }
        }

        async Task UpdateRoleAsync()
        {
            var result = await Service.UpdateRoleAsync(model);
            if (result.IsSuccess)
            {
                SnackBar.Add("Updated successfully.", Severity.Success, config =>
                {
                    config.ShowCloseIcon = true;
                });
                return;
            }
            foreach (var error in result.ErrorMessages)
            {
                SnackBar.Add(error, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }
    }
}
