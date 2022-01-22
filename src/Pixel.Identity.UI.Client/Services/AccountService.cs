using Pixel.Identity.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for consuming account api to manage user account
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Send the password reset link to user registered email
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> SendPasswordResetLinkAsync(ForgotPasswordModel model);

        /// <summary>
        /// Resend the email confirmation link to user registered email
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <returns></returns>
        Task<OperationResult> ResendEmailConfirmationAsync(ResendEmailConfirmationModel model);
    }


    public class AccountService : IAccountService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public AccountService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public async Task<OperationResult> ResendEmailConfirmationAsync(ResendEmailConfirmationModel model)
        {
            var result = await httpClient.PostAsJsonAsync<ResendEmailConfirmationModel>($"api/account/email/resendconfirm", model);
            return await OperationResult.FromResponseAsync(result);
        }


        public async Task<OperationResult> SendPasswordResetLinkAsync(ForgotPasswordModel model)
        {
            var result = await httpClient.PostAsJsonAsync<ForgotPasswordModel>($"api/account/password/sendresetlink", model);
            return await OperationResult.FromResponseAsync(result);
        }
    }
}
