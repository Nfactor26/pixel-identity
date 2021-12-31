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
    public interface IScopeService
    {
        Task<IEnumerable<ScopeViewModel>> GetAllAsync();

        Task<ScopeViewModel> GetByIdAsync(string id);

        Task<OperationResult> AddScopeAsync(ScopeViewModel scope);
       
        Task<OperationResult> UpdateScopeAsync(ScopeViewModel scope);
    }

    public class ScopeService : IScopeService
    {
        private readonly HttpClient httpClient;

        public ScopeService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ScopeViewModel>> GetAllAsync()
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<IEnumerable<ScopeViewModel>>("api/scopes");
                return result ?? Enumerable.Empty<ScopeViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<ScopeViewModel>();
        }

        public async Task<ScopeViewModel> GetByIdAsync(string id)
        {
            try
            {
                var result = await httpClient.GetFromJsonAsync<ScopeViewModel>($"api/scopes/id/{id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<OperationResult> AddScopeAsync(ScopeViewModel scope)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync<ScopeViewModel>("api/scopes/create", scope);
                return await OperationResult.FromResponseAsync(result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }
        }

        public async Task<OperationResult> UpdateScopeAsync(ScopeViewModel scope)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync<ScopeViewModel>("api/scopes/update", scope);
                return await OperationResult.FromResponseAsync(result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }
        }
    }
}
