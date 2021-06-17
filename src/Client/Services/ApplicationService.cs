using Pixel.Identity.Shared.Models;
using Pixel.Identity.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationViewModel>> GetAllAsync();
      
        Task<ApplicationViewModel> GetByClientIdAsync(string clientId);

        Task<OperationResult> AddApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor);

        Task<OperationResult> UpdateApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor);
       
    }

    public class ApplicationService : IApplicationService
    {
        private readonly HttpClient httpClient;

        public ApplicationService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ApplicationViewModel>> GetAllAsync()
        {           
            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<ApplicationViewModel>>("api/applications");
                return result ?? Enumerable.Empty<ApplicationViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<ApplicationViewModel>();
        }

        public async Task<ApplicationViewModel> GetByClientIdAsync(string clientId)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<ApplicationViewModel>($"api/applications/clientid/{clientId}");
                return result;              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<OperationResult> AddApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync<ApplicationViewModel>("api/applications/create", applicationDescriptor);
                return await OperationResult.FromResponseAsync(result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }
        }

        public async Task<OperationResult> UpdateApplicationDescriptorAsync(ApplicationViewModel applicationDescriptor)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync<ApplicationViewModel>("api/applications/update", applicationDescriptor);
                return await OperationResult.FromResponseAsync(result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }
        }
    }
}
