using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    public interface IUserRolesService
    {
        Task<IEnumerable<UserRoleViewModel>> GetAll();
    
        Task<UserRoleViewModel> GetRoleByName(string roleName);

        Task<UserRoleViewModel> CreateRole(UserRoleViewModel userRoleViewModel);

        Task<OperationResult> UpdateRoleAsync(UserRoleViewModel userRoleViewModel);
      
        Task<bool> AssignRolesToUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToAssign);

        Task<bool> RemoveRolesFromUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToRemove);
    }

    public class UserRolesService : IUserRolesService
    {
        private readonly HttpClient httpClient;

        public UserRolesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<UserRoleViewModel>> GetAll()
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<UserRoleViewModel>>
                        (await httpClient.GetStreamAsync($"api/roles"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<UserRoleViewModel>();
        }

        public async Task<UserRoleViewModel> GetRoleByName(string roleName)
        {
            return await JsonSerializer.DeserializeAsync<UserRoleViewModel>
                (await httpClient.GetStreamAsync($"api/roles/{roleName}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<UserRoleViewModel> CreateRole(UserRoleViewModel userRoleViewModel)
        {
            try
            {               
                var response = await httpClient.PostAsJsonAsync<UserRoleViewModel>("api/roles", userRoleViewModel);
                response.EnsureSuccessStatusCode();
               return await JsonSerializer.DeserializeAsync<UserRoleViewModel>(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<OperationResult> UpdateRoleAsync(UserRoleViewModel userRoleViewModel)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync<UserRoleViewModel>("api/roles", userRoleViewModel);
                return await OperationResult.FromResponseAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }          
        }

        public async Task<bool> AssignRolesToUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToAssign)
        {
            try
            {
                var request = new AddUserRolesRequest(userName, rolesToAssign);
                var result = await httpClient.PostAsJsonAsync<AddUserRolesRequest>("api/roles/assign", request);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<bool> RemoveRolesFromUserAsync(string userName, IEnumerable<UserRoleViewModel> rolesToRemove)
        {
            try
            {
                var request = new RemoveUserRolesRequest(userName, rolesToRemove);
                var result = await httpClient.PostAsJsonAsync<RemoveUserRolesRequest>("api/roles/remove", request);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
       
    }
}
