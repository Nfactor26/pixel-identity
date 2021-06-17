using Pixel.Identity.Shared.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    public interface IAccountsService
    {
        Task<bool> RegisterAsync(RegisterViewModel registerViewModel);
    }

    public class AccountsService : IAccountsService
    {
        private readonly HttpClient httpClient;

        public AccountsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> RegisterAsync(RegisterViewModel registerViewModel)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/account/create", registerViewModel);
                if (response.IsSuccessStatusCode)
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
