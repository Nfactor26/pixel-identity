using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Pages.Scopes
{
    /// <summary>
    /// Edit scope view allows authorized user to edit an existing <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
    /// </summary>
    public partial class EditScope : ComponentBase
    {
        [Inject]
        public IDialogService Dialog { get; set; }

        [Inject]
        public IScopeService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public string Id { get; set; }

        ScopeViewModel scope;

        bool hasErrors = false;

        /// <summary>
        /// Retrieve the Scope details when Id parameter is set
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            this.scope = await GetScopeDetailsAsync(Id);
        }

        /// <summary>
        /// Update the details of scope 
        /// </summary>
        /// <returns></returns>
        async Task UpdateScopeAsync()
        {
            var result = await Service.UpdateScopeAsync(scope);
            if (result.IsSuccess)
            {
                SnackBar.Add("Updated successfully.", Severity.Success);
                this.scope = await GetScopeDetailsAsync(Id);
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Retrieve scope details given scope id
        /// </summary>
        /// <param name="scopeId"></param>
        /// <returns></returns>
        private async Task<ScopeViewModel> GetScopeDetailsAsync(string scopeId)
        {
            if (!string.IsNullOrEmpty(scopeId))
            {
                try
                {
                    return await Service.GetByIdAsync(scopeId);
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    SnackBar.Add($"Failed to retrieve scope data. {ex.Message}", Severity.Error);
                }
            }
            else
            {
                hasErrors = true;
                SnackBar.Add("No scopeId specified.", Severity.Error);
            }
            return null;
        }
    }
}
