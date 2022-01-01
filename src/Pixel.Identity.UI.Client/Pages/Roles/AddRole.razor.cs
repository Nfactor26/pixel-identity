using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    public partial class AddRole : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IUserRolesService UserRolesService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }
      
        UserRoleViewModel model = new UserRoleViewModel(string.Empty);


        async Task AddRoleAsync()
        {
            if (!string.IsNullOrEmpty(model.RoleName))
            {
                var result = await UserRolesService.CreateRole(model);
                if (result != null)
                {
                    SnackBar.Add("Added successfully.", Severity.Success, config =>
                    {
                        config.ShowCloseIcon = true;
                    });
                    model = new UserRoleViewModel(string.Empty);
                    return;
                }
                //foreach (var error in result.ErrorMessages)
                //{
                //    SnackBar.Add(error, Severity.Error, config =>
                //    {
                //        config.ShowCloseIcon = true;
                //        config.RequireInteraction = true;
                //    });
                //}
            }
        }
    }
}
