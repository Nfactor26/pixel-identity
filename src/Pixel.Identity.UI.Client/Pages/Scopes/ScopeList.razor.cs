using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    public partial class ScopeList : ComponentBase
    {
        [Inject]
        public IScopeService ScopeService { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public NavigationManager Navigator { get; set; }


        protected string searchString = "";
        protected readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };

        protected IEnumerable<ScopeViewModel> scopes;

        protected override async Task OnInitializedAsync()
        {
            scopes = await ScopeService.GetAllAsync();
        }

        protected void NavigateToAddScopePage()
        {
            Navigator.NavigateTo("scopes/new");
        }

        protected void NavigateToEditScopePage(ScopeViewModel scope)
        {
            Navigator.NavigateTo($"scopes/edit/{scope.Id}");
        }
    }
}
