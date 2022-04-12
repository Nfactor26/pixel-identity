using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    /// <summary>
    /// List scope view allows user to see all the existing <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
    /// </summary>
    public partial class ScopeList : ComponentBase
    {
        [Inject]
        public IScopeService Service { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }      

        [Inject]
        public NavigationManager Navigator { get; set; }


        private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
        private MudTable<ScopeViewModel> scopesTable;
        private GetScopesRequest scopesRequest = new GetScopesRequest();
        private bool resetCurrentPage = false;

        /// <summary>
        /// Get scopes from api service endpoint for the current page of the data table
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task<TableData<ScopeViewModel>> GetScopesDataAsync(TableState state)
        {
            try
            {
                scopesRequest.CurrentPage = resetCurrentPage ? 1 : (state.Page + 1);
                resetCurrentPage = false;
                scopesRequest.PageSize = state.PageSize;
                var sessionPage = await Service.GetScopesAsync(scopesRequest);

                return new TableData<ScopeViewModel>
                {
                    Items = sessionPage.Items,
                    TotalItems = sessionPage.ItemsCount
                };
            }
            catch (System.Exception ex)
            {
                SnackBar.Add($"Error while retrieving scopes.{ex.Message}", Severity.Error);
            }
            return new TableData<ScopeViewModel>
            {
                Items = Enumerable.Empty<ScopeViewModel>(),
                TotalItems = 0
            };
        }

        /// <summary>
        /// Refresh data for the search query
        /// </summary>
        /// <param name="text"></param>
        private void OnSearch(string text)
        {
            scopesRequest.ScopesFilter = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                scopesRequest.ScopesFilter = text;
            }
            resetCurrentPage = true;
            scopesTable.ReloadServerData();
        }

        /// <summary>
        /// Permanently delete user from system. 
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        async Task DeleteScopeAsync(ScopeViewModel scope)
        {
            bool? dialogResult = await Dialog.ShowMessageBox("Warning", "Are you sure you want to delete ?",
                yesText: "Delete", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
            if (dialogResult.GetValueOrDefault())
            {
                var result = await Service.DeleteScopeAsync(scope);
                if (result.IsSuccess)
                {
                    SnackBar.Add("Deleted successfully.", Severity.Success);
                    await scopesTable.ReloadServerData();
                    return;
                }
                SnackBar.Add(result.ToString(), Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }

        /// <summary>
        /// Navigate to the add scope page
        /// </summary>
        protected void NavigateToAddScopePage()
        {
            Navigator.NavigateTo("scopes/new");
        }

        /// <summary>
        /// Navigate to the edit scope page
        /// </summary>
        protected void NavigateToEditScopePage(ScopeViewModel scope)
        {
            Navigator.NavigateTo($"scopes/edit/{scope.Id}");
        }
    }
}
