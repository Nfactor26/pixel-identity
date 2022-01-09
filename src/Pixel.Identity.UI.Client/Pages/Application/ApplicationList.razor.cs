using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
    public partial class ApplicationList : ComponentBase
    {
        [Inject]
        public IApplicationService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }       
       
        private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
        private MudTable<ApplicationViewModel> applicationsTable;
        private GetApplicationsRequest applicationsRequest = new GetApplicationsRequest();
        private bool resetCurrentPage = false;

        /// <summary>
        /// Get roles from api endpoint for the current page of the data table
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task<TableData<ApplicationViewModel>> GetApplicationsDataAsync(TableState state)
        {
            try
            {
                applicationsRequest.CurrentPage = resetCurrentPage ? 1 : (state.Page + 1);
                resetCurrentPage = false;
                applicationsRequest.PageSize = state.PageSize;
                var sessionPage = await Service.GetApplicationsAsync(applicationsRequest);

                return new TableData<ApplicationViewModel>
                {
                    Items = sessionPage.Items,
                    TotalItems = sessionPage.ItemsCount
                };
            }
            catch (Exception ex)
            {
                SnackBar.Add($"Error while retrieving applications.{ex.Message}", Severity.Error);
            }
            return new TableData<ApplicationViewModel>
            {
                Items = Enumerable.Empty<ApplicationViewModel>(),
                TotalItems = 0
            };
        }
        
        void EditApplication(ApplicationViewModel application)
        {
            Navigator.NavigateTo($"applications/edit/{application.ClientId}");
        }

        void DeleteApplication(ApplicationViewModel application)
        {
           
        }

        void AddNewApplication()
        {
            Navigator.NavigateTo($"applications/new");
        }
    }
}
