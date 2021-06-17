using Microsoft.AspNetCore.Components;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Application
{
    public partial class ApplicationList : ComponentBase
    {
        [Inject]
        public IApplicationService Service { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }
       
        IEnumerable<ApplicationViewModel> applications;
        protected string searchString = "";
        protected readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };

        protected override async Task OnInitializedAsync()
        {
            applications = await Service.GetAllAsync();
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
