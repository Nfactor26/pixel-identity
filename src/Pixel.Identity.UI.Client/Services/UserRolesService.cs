using Microsoft.AspNetCore.WebUtilities;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for consuming roles api to manage roles
    /// </summary>
    public interface IUserRolesService
    {
        /// <summary>
        /// Get all the available roles
        /// </summary>
        /// <returns></returns>
        Task<PagedList<UserRoleViewModel>> GetRolesAsync(GetRolesRequest request);
    
        /// <summary>
        /// Get role given role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<UserRoleViewModel> GetRoleByNameAsync(string roleName);

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="userRoleViewModel"></param>
        /// <returns></returns>
        Task<OperationResult> CreateRoleAsync(UserRoleViewModel userRoleViewModel);

        /// <summary>
        /// Delete an existing role with specified name
        /// </summary>
        /// <param name="roleName">Name of the role to delete</param>
        /// <returns></returns>
        Task<OperationResult> DeleteRoleAsync(string roleName);

        /// <summary>
        /// Add a new claim to role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimToAdd"></param>
        /// <returns></returns>
        Task<OperationResult> AddClaimToRoleAsync(string roleName, ClaimViewModel claimToAdd);

        /// <summary>
        /// Modify an existing claim details
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="original"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        Task<OperationResult> UpdateClaimForRoleAsync(string roleName, ClaimViewModel original,
            ClaimViewModel modified);

        /// <summary>
        /// Remove an existing claim from role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimToRemove"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveClaimFromRoleAsync(string roleName, ClaimViewModel claimToRemove);

        /// <summary>
        /// Assign role to a user
        /// </summary>
        /// <param name="userName">Name of the user to assign role to</param>
        /// <param name="rolesToAssign">Name of the role to assign</param>
        /// <returns></returns>
        Task<OperationResult> AssignRolesToUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToAssign);

        /// <summary>
        /// Remove role from a user
        /// </summary>
        /// <param name="userName">Name of the user for which role should be removed</param>
        /// <param name="rolesToRemove">Name of the role to remove</param>
        /// <returns></returns>
        Task<OperationResult> RemoveRolesFromUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToRemove);
    }

    /// <summary>
    /// Implementation of <see cref="IUserRolesService"/>
    /// </summary>
    public class UserRolesService : IUserRolesService
    {
        private readonly HttpClient httpClient;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public UserRolesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;            
        }

        /// <inheritdoc/>
        public async Task<PagedList<UserRoleViewModel>> GetRolesAsync(GetRolesRequest request)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["currentPage"] = request.CurrentPage.ToString(),
                ["pageSize"] = request.PageSize.ToString()
            };
            if (!string.IsNullOrEmpty(request.RoleFilter))
            {
                queryStringParam.Add("roleFilter", request.RoleFilter);
            }
            return await this.httpClient.GetFromJsonAsync<PagedList<UserRoleViewModel>>(QueryHelpers.AddQueryString("api/roles", queryStringParam));
        }

        /// <inheritdoc/>
        public async Task<UserRoleViewModel> GetRoleByNameAsync(string roleName)
        {
            return await JsonSerializer.DeserializeAsync<UserRoleViewModel>
                      (await httpClient.GetStreamAsync($"api/roles/{roleName}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        /// <inheritdoc/>
        public async Task<OperationResult> CreateRoleAsync(UserRoleViewModel userRoleViewModel)
        {
            var response = await httpClient.PostAsJsonAsync<UserRoleViewModel>("api/roles", userRoleViewModel);
            return await OperationResult.FromResponseAsync(response);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> DeleteRoleAsync(string roleName)
        {
            var result = await httpClient.DeleteAsync($"api/roles/{roleName}");          
            return await OperationResult.FromResponseAsync(result);            
        }

        /// <inheritdoc/>
        public async Task<OperationResult> AddClaimToRoleAsync(string roleName, ClaimViewModel claimToAdd)
        {
            var request = new AddClaimRequest(roleName, claimToAdd);
            var result = await httpClient.PostAsJsonAsync<AddClaimRequest>("api/roles/add/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> RemoveClaimFromRoleAsync(string roleName, ClaimViewModel claimToRemove)
        {
            var request = new RemoveClaimRequest(roleName, claimToRemove);
            var result = await httpClient.PostAsJsonAsync<RemoveClaimRequest>("api/roles/delete/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> UpdateClaimForRoleAsync(string roleName, ClaimViewModel original,
            ClaimViewModel modified)
        {
            var request = new UpdateClaimRequest(roleName, original, modified);
            var result = await httpClient.PostAsJsonAsync<UpdateClaimRequest>("api/roles/update/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> AssignRolesToUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToAssign)
        {
            var request = new AddUserRolesRequest(userName, rolesToAssign);
            var result = await httpClient.PostAsJsonAsync<AddUserRolesRequest>("api/roles/assign", request);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> RemoveRolesFromUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToRemove)
        {
            var request = new RemoveUserRolesRequest(userName, rolesToRemove);
            var result = await httpClient.PostAsJsonAsync<RemoveUserRolesRequest>("api/roles/remove", request);
            return await OperationResult.FromResponseAsync(result);
        }    
    }
}
