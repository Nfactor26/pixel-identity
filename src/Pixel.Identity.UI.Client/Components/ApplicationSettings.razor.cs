using Microsoft.AspNetCore.Components;
using MudBlazor;
using Pixel.Identity.Shared.Helpers;
using System;
using System.Collections.Generic;

namespace Pixel.Identity.UI.Client.Components;

public partial class ApplicationSettings : ComponentBase
{
    IEnumerable<string> AllSettings { get; } = new List<string>(TokenLifeTimesHelper.TokenLifeTimeNames);
   
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public IEnumerable<string> ConfiguredSettings { get; set; }

    string selectedSetting;
    string configuredValue;
    string error;

    void AddNewSetting()
    {
        if (string.IsNullOrEmpty(configuredValue))
        {
            error = "Value is required";
            return;
        }
        if(!TimeSpan.TryParse(configuredValue, out _ ))
        {
            error = "Failed to convert value to TimeSpan";
            return;
        }
        MudDialog.Close(DialogResult.Ok<KeyValuePair<string,string>>(new KeyValuePair<string, string>(TokenLifeTimesHelper.GetValueFromName(selectedSetting), configuredValue)));
        return;
    }  

    void Cancel() => MudDialog.Cancel();
}
