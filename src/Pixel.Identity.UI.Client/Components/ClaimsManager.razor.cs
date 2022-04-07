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

        private MudTable<ClaimViewModel> table;


        private ClaimViewModel selectedClaim = null;
        private ClaimViewModel elementBeforeEdit;
        private string searchString = "";

        void EditItem(ClaimViewModel model)
        {
            selectedClaim = model;
            table.SetEditingItem(model);
            BackupItem(model);
        }

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
            if(element is ClaimViewModel beforeEdit)
            {
                elementBeforeEdit = new()
                {
                    Type = beforeEdit.Type,
                    Value = beforeEdit.Value,
                    IncludeInAccessToken = beforeEdit.IncludeInAccessToken,
                    IncludeInIdentityToken = beforeEdit.IncludeInIdentityToken
                };
            }
           
        }

        void ResetItemToOriginalValues(object element)
        {
            ((ClaimViewModel)element).Type = elementBeforeEdit.Type;
            ((ClaimViewModel)element).Value = elementBeforeEdit.Value;
            ((ClaimViewModel)element).IncludeInAccessToken = elementBeforeEdit.IncludeInAccessToken;
            ((ClaimViewModel)element).IncludeInIdentityToken = elementBeforeEdit.IncludeInIdentityToken;
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
