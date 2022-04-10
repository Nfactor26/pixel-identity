using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sample.Blazor.App;
using System.Net.Http.Json;

namespace Samples.Blazor.App.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetAllAsync();
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public WeatherService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<WeatherForecast>> GetAllAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/WeatherForecast")
                      ?? Enumerable.Empty<WeatherForecast>();
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return Enumerable.Empty<WeatherForecast>();
        }
    }
}
