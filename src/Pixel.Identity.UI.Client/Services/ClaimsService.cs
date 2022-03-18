using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    public interface IClaimsService
    {
        /// <summary>
        /// Add a new claim to owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="claimToAdd"></param>
        /// <returns></returns>
        Task<OperationResult> AddClaimAsync(string owner, ClaimViewModel claimToAdd);

        /// <summary>
        /// Remove claim from owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="claimToRemove"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveClaimAsync(string owner, ClaimViewModel claimToRemove);

        /// <summary>
        /// Update details of an existing claim on owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="original"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        Task<OperationResult>UpdateClaimAsync(string owner, ClaimViewModel original, ClaimViewModel modified);
    }

    public interface IUserClaimsService : IClaimsService { }

    public interface IRoleClaimsService : IClaimsService { }

    public abstract class ClaimsService : IClaimsService
    {
        protected readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public ClaimsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        protected abstract string GetEndPoint();

        /// <inheritdoc/>
        public async Task<OperationResult> AddClaimAsync(string owner, ClaimViewModel claimToAdd)
        {
            var request = new AddClaimRequest(owner, claimToAdd);
            var result = await httpClient.PostAsJsonAsync<AddClaimRequest>($"{GetEndPoint()}/add/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> RemoveClaimAsync(string owner, ClaimViewModel claimToRemove)
        {
            var request = new RemoveClaimRequest(owner, claimToRemove);
            var result = await httpClient.PostAsJsonAsync<RemoveClaimRequest>($"{GetEndPoint()}/delete/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }


        /// <inheritdoc/>
        public async Task<OperationResult> UpdateClaimAsync(string owner, ClaimViewModel original, ClaimViewModel modified)
        {
            var request = new UpdateClaimRequest(owner, original, modified);
            var result = await httpClient.PostAsJsonAsync<UpdateClaimRequest>($"{GetEndPoint()}/update/claim", request);
            return await OperationResult.FromResponseAsync(result);
        }
    }

    public class UserClaimsService : ClaimsService, IUserClaimsService
    {
        public UserClaimsService(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override string GetEndPoint()
        {
            return $"api/users";
        }
    }

    public class RoleClaimsService : ClaimsService, IRoleClaimsService
    {
        public RoleClaimsService(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override string GetEndPoint()
        {
            return $"api/roles";
        }
    }
}
