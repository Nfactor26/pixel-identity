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
    /// Component to list scopes
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
        /// Get scopes from api endpoint for the current page of the data table
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
