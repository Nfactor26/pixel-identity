using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Pixel.Identity.UI.Client.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDetailsViewModel>> GetUsersAsync();

        Task<UserDetailsViewModel> GetUserByNameAsync(string userName);
        Task<bool> UpdateUserAsync(UserDetailsViewModel userDetails);
    }

    public class UsersService : IUsersService
    {
        private readonly HttpClient httpClient;

        public UsersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<UserDetailsViewModel>> GetUsersAsync()
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<UserDetailsViewModel>>("api/users");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<UserDetailsViewModel>();
        }

        public async Task<UserDetailsViewModel> GetUserByNameAsync(string userName)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<UserDetailsViewModel>($"api/users/{userName}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
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
