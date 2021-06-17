using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Responses
{
    public class ResponseBase
    {
        [Required]
        public int StatusCode { get; }

        public string Message { get; }

        public ResponseBase(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad Request";
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                default:
                    return statusCode.ToString();
            }
        }
    }

    public class OkResponse : ResponseBase
    {
        public object Result { get; }

        public OkResponse(object result)
            : base(200)
        {
            Result = result;
        }
    }

    public class BadRequestResponse : ResponseBase
    {
        public List<string> Errors { get; } = new List<string>();

        public BadRequestResponse(IEnumerable<string> errors) : base(400)
        {
            this.Errors.AddRange(errors);
        }
    }

    public class NotFoundResponse : ResponseBase
    {
        public NotFoundResponse(string message) : base(404, message)
        {
           
        }
    }

    public class ProblemResponse : ResponseBase
    {
        public ProblemResponse(string error) : base(500, error)
        {            
        }
    }
}
