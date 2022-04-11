using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpenIddict.Abstractions;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Components
{
    public partial class ApplicationForm : ComponentBase
    {      
        [Parameter]
        public IDialogService Dialog { get; set; }

        [Parameter]
        public ISnackbar SnackBar { get; set; }
      
        [CascadingParameter]
        public ApplicationViewModel Application { get; set; }

        List<SwitchItemViewModel> endPointPermissions = new List<SwitchItemViewModel>()
        {
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Authorization, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Device, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Introspection, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Logout, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Revocation, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Endpoints.Token, false))
        };

        List<SwitchItemViewModel> grantTypePermissions = new List<SwitchItemViewModel>()
        {
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials, false)),          
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.Implicit, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.Password, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.RefreshToken, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.GrantTypes.DeviceCode, false))
        };

        List<SwitchItemViewModel> responseTypePermissions = new List<SwitchItemViewModel>()
        {
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.ResponseTypes.Code, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.ResponseTypes.CodeToken, false))
        };

        List<SwitchItemViewModel> scopePermissions = new List<SwitchItemViewModel>()
        {
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Scopes.Address, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Scopes.Email, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Scopes.Phone, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Scopes.Profile, false)),
            (new SwitchItemViewModel(OpenIddictConstants.Permissions.Scopes.Roles, false))
        };

        List<SwitchItemViewModel> requirements = new List<SwitchItemViewModel>()
        {
            (new SwitchItemViewModel(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange, false))
        };

        protected override void OnParametersSet()
        {
            if(Application != null)
            {               
                AddCustomScopes();
                InitializePermissionState(endPointPermissions);
                InitializePermissionState(grantTypePermissions);
                InitializePermissionState(responseTypePermissions);
                InitializePermissionState(scopePermissions);
                InitializeRequirementState(requirements);
            }

            //While editing an application back, we need to add any custom scope to the scopePermissions that was previously added 
            void AddCustomScopes()
            {
                var scopes = Application.Permissions.Where(p => p.StartsWith("scp:"));
                foreach (var scope in scopes)
                {
                    if (this.scopePermissions.Any(sp => sp.ItemValue.Equals(scope)))
                    {
                        continue;
                    }
                    this.scopePermissions.Add(new SwitchItemViewModel(scope, true));
                }
            }
        }

        void InitializePermissionState(List<SwitchItemViewModel> permissions)
        {
            foreach (var item in permissions)
            {
                item.IsSelected = false;
                if (Application.Permissions.Contains(item.ItemValue))
                {
                    item.IsSelected = true;
                }
            }
        }

        void InitializeRequirementState(List<SwitchItemViewModel> requirements)
        {
            foreach (var item in requirements)
            {
                item.IsSelected = false;
                if (Application.Requirements.Contains(item.ItemValue))
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Add a new scope the application permissions list
        /// </summary>
        /// <returns></returns>
        async Task AddScope()
        {
            var parameters = new DialogParameters();
            var dialog = Dialog.Show<AddScopeDialog>("Add New Scope", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is ScopeViewModel customScope)
            {
                if(Application.Permissions.Contains(customScope.Name))
                {
                    SnackBar.Add($"Selected scope {customScope.DisplayName} already exists in application permission.", Severity.Error);
                    return;
                }
                var scopeSwitchItem = new SwitchItemViewModel(customScope.DisplayName, $"scp:{customScope.Name}", false);
                scopePermissions.Add(scopeSwitchItem);
                TogglePermission(scopeSwitchItem);
            }
        }

        async Task AddRedirectUri()
        {
            var parameters = new DialogParameters();
            parameters.Add("ExistingUris", Application.RedirectUris);
            var dialog = Dialog.Show<AddUriComponent>("Add New Uri", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true }) ;
            var result = await dialog.Result;
            if (!result.Cancelled && result.Data is Uri uriToAdd)
            {
                Application.RedirectUris.Add(uriToAdd);
            }
        }

        void RemoveRedirectUri(Uri uri)
        {
            if (Application.RedirectUris.Contains(uri))
            {
                Application.RedirectUris.Remove(uri);
            }
        }

        async Task AddPostLogoutRedirectUri()
        {
            var parameters = new DialogParameters();           
            parameters.Add("ExistingUris", Application.PostLogoutRedirectUris);
            var dialog = Dialog.Show<AddUriComponent>("Add New Uri", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if(!result.Cancelled && result.Data is Uri uriToAdd)
            {
               Application.PostLogoutRedirectUris.Add(uriToAdd);
            }            
        }

        void RemovePostLogoutRedirectUri(Uri uri)
        {
            if (Application.PostLogoutRedirectUris.Contains(uri))
            {
                Application.PostLogoutRedirectUris.Remove(uri);
            }
        }

        void TogglePermission(SwitchItemViewModel permission)
        {
            ToggleSwitch(permission, Application.Permissions);
        }

        void ToggleRequirement(SwitchItemViewModel requirement)
        {
            ToggleSwitch(requirement, Application.Requirements);
        }

        void ToggleSwitch(SwitchItemViewModel item, List<string> targetCollection)
        {
            if (!targetCollection.Contains(item.ItemValue))
            {
                targetCollection.Add(item.ItemValue);
            }
            else if (targetCollection.Contains(item.ItemValue))
            {
                targetCollection.Remove(item.ItemValue);
            }
            item.IsSelected = !item.IsSelected;
        }

        string passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        bool isPasswordVisible = false;
        InputType passwordInputFieldType = InputType.Password;
        void OnTogglePasswordVisibility()
        {
            if (isPasswordVisible)
            {
                isPasswordVisible = false;
                passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                passwordInputFieldType = InputType.Password;
            }
            else
            {
                isPasswordVisible = true;
                passwordInputIcon = Icons.Material.Filled.Visibility;
                passwordInputFieldType = InputType.Text;
            }
        }
    }
}
