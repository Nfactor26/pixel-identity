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

        /// <summary>
        /// Delete an existing application
        /// </summary>
        /// <param name="applicationDescriptor"></param>
        /// <returns></returns>
        Task<OperationResult> DeleteApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor);

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
            if(!string.IsNullOrEmpty(request.ApplicationFilter))
            {
                queryStringParam.Add("applicationFilter", request.ApplicationFilter);
            }
            return await this.httpClient.GetFromJsonAsync<PagedList<ApplicationViewModel>>(QueryHelpers.AddQueryString("api/applications", queryStringParam));           
        }

        /// <inheritdoc/>
        public async Task<ApplicationViewModel> GetByClientIdAsync(string clientId)
        {
            return await httpClient.GetFromJsonAsync<ApplicationViewModel>($"api/applications/{clientId}");          
        }

        /// <inheritdoc/>
        public async Task<OperationResult> AddApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            var result = await httpClient.PostAsJsonAsync<ApplicationViewModel>("api/applications", applicationDescriptor);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> UpdateApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            var result = await httpClient.PutAsJsonAsync<ApplicationViewModel>("api/applications", applicationDescriptor);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> DeleteApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            var result = await httpClient.DeleteAsync($"api/applications/{applicationDescriptor.ClientId}");
            return await OperationResult.FromResponseAsync(result);
        }
    }
}
