using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Users
{
    /// <summary>
    /// Component for listing users
    /// </summary>
    public partial class UserList : ComponentBase
    {
        [Inject]
        public IUsersService Service { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }
       
        private MudTable<UserDetailsViewModel> usersTable;
        private GetUsersRequest usersRequest = new GetUsersRequest();
        private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
        private bool resetCurrentPage = false;

        /// <summary>
        /// Get roles from api endpoint for the current page of the data table
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task<TableData<UserDetailsViewModel>> GetUsersDataAsync(TableState state)
        {
            try
            {
                usersRequest.CurrentPage = resetCurrentPage ? 1 : (state.Page + 1);
                resetCurrentPage = false;
                usersRequest.PageSize = state.PageSize;
                var sessionPage = await Service.GetUsersAsync(usersRequest);

                return new TableData<UserDetailsViewModel>
                {
                    Items = sessionPage.Items,
                    TotalItems = sessionPage.ItemsCount
                };
            }
            catch (System.Exception ex)
            {
                SnackBar.Add($"Error while retrieving users.{ex.Message}", Severity.Error);
            }
            return new TableData<UserDetailsViewModel>
            {
                Items = Enumerable.Empty<UserDetailsViewModel>(),
                TotalItems = 0
            };
        }

        /// <summary>
        /// Refresh data for the search query
        /// </summary>
        /// <param name="text"></param>
        private void OnSearch(string text)
        {
            usersRequest.UsersFilter = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                usersRequest.UsersFilter = text;
            }
            resetCurrentPage = true;
            usersTable.ReloadServerData();
        }


        /// <summary>
        /// Navigate to the edit user page
        /// </summary>
        void EditUser(UserDetailsViewModel userDetails)
        {
            Navigator.NavigateTo($"users/edit/{userDetails.Id}");
        }
         
        /// <summary>
        /// Permanently delete user from system. 
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        async Task DeleteUserAsync(UserDetailsViewModel userDetails)
        {
            bool? dialogResult = await DialogService.ShowMessageBox("Warning", "Delete can't be undone !!", 
                yesText: "Delete!", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
            if(dialogResult.GetValueOrDefault())
            {
                var result = await Service.DeleteUserAsync(userDetails);
                if (result.IsSuccess)
                {
                    SnackBar.Add("Deleted successfully.", Severity.Success);
                    await usersTable.ReloadServerData();
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
}
