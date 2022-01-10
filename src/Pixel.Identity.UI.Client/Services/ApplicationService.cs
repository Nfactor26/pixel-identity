using Microsoft.AspNetCore.WebUtilities;
using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.Request;
using Pixel.Identity.Shared.Responses;
using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    /// <summary>
    /// Service contract for consuming applications api to manage applications
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Get all the available applications based on request
        /// </summary>
        /// <returns></returns>
        Task<PagedList<ApplicationViewModel>> GetApplicationsAsync(GetApplicationsRequest request);
      
        /// <summary>
        /// Get application details given client id of the application
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<ApplicationViewModel> GetByClientIdAsync(string clientId);

        /// <summary>
        /// Add a new application
        /// </summary>
        /// <param name="applicationDescriptor"></param>
        /// <returns></returns>
        Task<OperationResult> AddApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor);

        /// <summary>
        /// Update details of an existing application
        /// </summary>
        /// <param name="applicationDescriptor"></param>
        /// <returns></returns>
        Task<OperationResult> UpdateApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor);
       
    }

    public class ApplicationService : IApplicationService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public ApplicationService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<PagedList<ApplicationViewModel>> GetApplicationsAsync(GetApplicationsRequest request)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["currentPage"] = request.CurrentPage.ToString(),
                ["pageSize"] = request.PageSize.ToString()
            };
            return await this.httpClient.GetFromJsonAsync<PagedList<ApplicationViewModel>>(QueryHelpers.AddQueryString("api/applications", queryStringParam));           
        }

        /// <inheritdoc/>
        public async Task<ApplicationViewModel> GetByClientIdAsync(string clientId)
        {
            return await httpClient.GetFromJsonAsync<ApplicationViewModel>($"api/applications/clientid/{clientId}");          
        }

        /// <inheritdoc/>
        public async Task<OperationResult> AddApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            var result = await httpClient.PostAsJsonAsync<ApplicationViewModel>("api/applications/create", applicationDescriptor);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> UpdateApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            var result = await httpClient.PostAsJsonAsync<ApplicationViewModel>("api/applications/update", applicationDescriptor);
            return await OperationResult.FromResponseAsync(result);
        }
    }
}
