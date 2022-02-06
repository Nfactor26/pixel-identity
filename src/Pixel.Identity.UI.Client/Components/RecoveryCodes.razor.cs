using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.UI.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class RecoveryCodes : ComponentBase
    {

        List<string> recoveryCodes = new List<string>();

        int recoveryCodesCount = 0;

        [Inject]
        public IAuthenticatorService Service { get; set; }

        [Inject]
        public ISnackbar SnackBar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                recoveryCodesCount = await Service.GetRecoveryCodesCountAsync();
               
            }
            catch (Exception ex)
            {
                SnackBar.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }


        /// <summary>
        /// Generate new recovery codes 
        /// </summary>
        /// <returns></returns>
        async Task GenerateRecoveryCodesAsync()
        {
            try
            {
                var codes = await Service.GenerateRecoveryCodesAsync();
                this.recoveryCodes.Clear();
                this.recoveryCodes.AddRange(codes);
                this.recoveryCodesCount = this.recoveryCodes.Count;
            }
            catch(Exception ex)
            {
                SnackBar.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }
    }
}
