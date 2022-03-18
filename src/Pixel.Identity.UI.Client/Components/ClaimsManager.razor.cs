using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class ClaimsManager : ComponentBase
    {
        [Inject]
        public ISnackbar SnackBar { get; set; }

        [Parameter]
        public IEnumerable<ClaimViewModel> Claims { get; set; }

        [Parameter]
        public EventCallback OnAddItem { get; set; }

        [Parameter]
        public EventCallback<ClaimViewModel> OnDeleteItem { get; set; }

        [Parameter]
        public Func<ClaimViewModel, ClaimViewModel, Task<bool>> OnUpdateItem { get; set; }


        private ClaimViewModel selectedClaim = null;
        private ClaimViewModel elementBeforeEdit;
        private string searchString = "";

        async Task UpdateItemAsync()
        {
            var success = await this.OnUpdateItem(elementBeforeEdit, selectedClaim);
            if(!success)
            {
                ResetItemToOriginalValues(selectedClaim);
            }    
        }

        void BackupItem(object element)
        {
            elementBeforeEdit = new()
            {
                Type = ((ClaimViewModel)element).Type,
                Value = ((ClaimViewModel)element).Value               
            };
        }

        void ResetItemToOriginalValues(object element)
        {
            ((ClaimViewModel)element).Type = elementBeforeEdit.Type;
            ((ClaimViewModel)element).Value = elementBeforeEdit.Value;          
        }

        private bool FilterFunc(ClaimViewModel element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Type.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Value.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
