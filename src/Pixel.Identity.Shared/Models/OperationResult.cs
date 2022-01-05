using Pixel.Identity.Shared.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.Shared.Models
{
    public class OperationResult
    {
        public HttpStatusCode StatusCode { get; protected set; }

        public List<string> ErrorMessages { get; protected set; } = new List<string>();

        public bool IsSuccess => !ErrorMessages.Any();

        protected OperationResult()
        {

        }

        protected OperationResult(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        protected OperationResult(IEnumerable<string> errorMessages)
        {
            this.ErrorMessages.AddRange(errorMessages);
        }

        protected OperationResult(HttpStatusCode statusCode, IEnumerable<string> errorMessages) : this(statusCode)
        {
            this.ErrorMessages.AddRange(errorMessages);
        }

        public static OperationResult Success(HttpStatusCode statusCode) => new OperationResult(statusCode);

        public static OperationResult Failed(string errorMessage)
       => new OperationResult(new[] { errorMessage });

        public static OperationResult Failed(HttpStatusCode statusCode, string errorMessage)
         => new OperationResult(statusCode, new []{ errorMessage });

        public static OperationResult Failed(HttpStatusCode statusCode, IEnumerable<string> errorMessages)
           => new OperationResult(statusCode ,errorMessages);

        public static async Task<OperationResult> FromResponseAsync(HttpResponseMessage result)
        {
            try
            {
                result.EnsureSuccessStatusCode();
                return Success(result.StatusCode);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await result.Content.ReadFromJsonAsync<BadRequestResponse>();
                return Failed(result.StatusCode, badRequestResponse.Errors);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var notFoundResponse = await result.Content.ReadFromJsonAsync<NotFoundResponse>();
                return Failed(result.StatusCode, notFoundResponse.Message);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var problemResponse = await result?.Content.ReadFromJsonAsync<ProblemResponse>();
                return Failed(result.StatusCode, problemResponse.Message);
            }

        }

        public override string ToString()
        {
            if(IsSuccess)
            {
                return $"{StatusCode} success";
            }
            return $"{StatusCode} : {string.Join(';', this.ErrorMessages)}";
        }
    }
}
