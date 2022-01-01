using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Roles
{
    public partial class RoleList : ComponentBase
    {
        [Inject]
        public IUserRolesService UserRolesService { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        protected string searchString = "";
        protected readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };

        protected IEnumerable<UserRoleViewModel> roles;

        protected override async Task OnInitializedAsync()
        {
            roles = await UserRolesService.GetAll();
        }

        protected void NavigateToAddRolePage()
        {
            Navigator.NavigateTo("roles/new");
        }

        protected void NavigateToEditRolePage(UserRoleViewModel role)
        {
            Navigator.NavigateTo($"roles/edit/{role.RoleName}");
        }
    }
}
