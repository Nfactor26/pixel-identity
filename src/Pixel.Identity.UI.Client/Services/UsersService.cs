using Microsoft.AspNetCore.WebUtilities;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for consuming users api to manage users
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Get all the available roles
        /// </summary>
        /// <returns></returns>
        Task<PagedList<UserDetailsViewModel>> GetUsersAsync(GetUsersRequest request);

        /// <summary>
        /// Get user details given user name
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <returns></returns>
        Task<UserDetailsViewModel> GetUserByNameAsync(string userName);
      
        Task<bool> UpdateUserAsync(UserDetailsViewModel userDetails);
    }

    public class UsersService : IUsersService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public UsersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<PagedList<UserDetailsViewModel>> GetUsersAsync(GetUsersRequest request)
        {

            var queryStringParam = new Dictionary<string, string>
            {
                ["currentPage"] = request.CurrentPage.ToString(),
                ["pageSize"] = request.PageSize.ToString()
            };
            if (!string.IsNullOrEmpty(request.UsersFilter))
            {
                queryStringParam.Add(nameof(GetUsersRequest.UsersFilter), request.UsersFilter);
            }
            return await this.httpClient.GetFromJsonAsync<PagedList<UserDetailsViewModel>>(QueryHelpers.AddQueryString("api/users", queryStringParam));       
          
        }

        /// <inheritdoc/>
        public async Task<UserDetailsViewModel> GetUserByNameAsync(string userName)
        {
            return await httpClient.GetFromJsonAsync<UserDetailsViewModel>($"api/users/{userName}");
        }

        public async Task<bool> UpdateUserAsync(UserDetailsViewModel userDetails)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync<UserDetailsViewModel>("api/users", userDetails);
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
