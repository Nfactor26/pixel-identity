using Microsoft.AspNetCore.Components;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Users
{
    public partial class UserList : ComponentBase
    {
        [Inject]
        public IUsersService UsersService { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }

        IEnumerable<UserDetailsViewModel> users;
        protected string searchString = "";
        protected readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };

        protected override async Task OnInitializedAsync()
        {
            users = await UsersService.GetUsersAsync();
        }

        void EditUser(UserDetailsViewModel userDetails)
        {
            Navigator.NavigateTo($"users/edit/{userDetails.UserName}");
        }

        void DeleteUser(UserDetailsViewModel userDetails)
        {

        }
    }
}
