using Pixel.Identity.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for consuming roles api to manage roles
    /// </summary>
    public interface IAuthenticatorService
    {
        /// <summary>
        /// Check if the authenticator application is enabled for user account
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckIsAuthenticatorEnabledAsync();

        /// <summary>
        /// Get the key and Uri required for setting up authenticator for user account
        /// </summary>
        /// <returns></returns>
        Task<EnableAuthenticatorModel> GetAuthenticatorSetupConfigurationAsync();

        /// <summary>
        /// Generate recovery codes for authenticator app
        /// </summary>
        /// <returns></returns>
        Task<string[]> GenerateRecoveryCodesAsync();

        /// <summary>
        /// Get the number of recover codes available
        /// </summary>
        /// <returns></returns>
        Task<int> GetRecoveryCodesCountAsync();

        /// <summary>
        /// Enable authenticator app for user account
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<OperationResult> EnableAuthenticatorAsync(string code);

        /// <summary>
        /// Disable authenticator app for user account
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> DisableAuthenticatorAsync(string code);


        /// <summary>
        /// Reset authenticator app for user account
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> ResetAuthenticatorAsync(string code);

    }


    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public AuthenticatorService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckIsAuthenticatorEnabledAsync()
        {
            return await this.httpClient.GetFromJsonAsync<bool>("api/authenticator/isenabled");
        }

        /// <inheritdoc/>
        public async Task<OperationResult> EnableAuthenticatorAsync(string code)
        {
            var response = await httpClient.PostAsJsonAsync<string>("api/authenticator/enable", code);
            return await OperationResult.FromResponseAsync(response);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> DisableAuthenticatorAsync(string code)
        {
            var response = await httpClient.PostAsJsonAsync<string>("api/authenticator/disable", code);
            return await OperationResult.FromResponseAsync(response);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> ResetAuthenticatorAsync(string code)
        {
            var response = await httpClient.PostAsJsonAsync<string>("api/authenticator/reset", code);
            return await OperationResult.FromResponseAsync(response);
        }

        /// <inheritdoc/>
        public async Task<string[]> GenerateRecoveryCodesAsync()
        {
            return await this.httpClient.GetFromJsonAsync<string[]>("api/authenticator/recoverycodes");
        }

        /// <inheritdoc/>
        public async Task<EnableAuthenticatorModel> GetAuthenticatorSetupConfigurationAsync()
        {
            return await this.httpClient.GetFromJsonAsync<EnableAuthenticatorModel>("api/authenticator");
        }

        /// <inheritdoc/>
        public async Task<int> GetRecoveryCodesCountAsync()
        {
            return await this.httpClient.GetFromJsonAsync<int>("api/authenticator/recoverycodescount");
        }
    }
}
