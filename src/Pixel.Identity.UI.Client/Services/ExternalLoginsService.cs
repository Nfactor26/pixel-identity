using Pixel.Identity.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for managing external logins
    /// </summary>
    public interface IExternalLoginsService
    {
        /// <summary>
        /// Get the external logins for user
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserLoginInfo>> GetExternalLoginsAsync();

        /// <summary>
        /// Remove external provider for user account
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="loginProviderKey"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveExternalLoginAsync(UserLoginInfo userloginInfo);
    }

    public class ExternalLoginsService : IExternalLoginsService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public ExternalLoginsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        ///</inheritdoc>
        public async Task<IEnumerable<UserLoginInfo>> GetExternalLoginsAsync()
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<UserLoginInfo>>($"api/externallogins");
        }

        ///</inheritdoc>
        public async Task<OperationResult> RemoveExternalLoginAsync(UserLoginInfo userLoginInfo)
        {
            var result = await httpClient.
                DeleteAsync($"api/externallogins/{userLoginInfo.LoginProvider}/{userLoginInfo.ProviderKey}");
            return await OperationResult.FromResponseAsync(result);
        }
    }
}
