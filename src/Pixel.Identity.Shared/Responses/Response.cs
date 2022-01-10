using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pixel.Identity.Shared.Responses
{
    public class ResponseBase
    {
        [Required]
        public int StatusCode { get; set; }

        public string Message { get; set; }
      
        public ResponseBase()
        {
        }

        public ResponseBase(int statusCode, string message)
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
        public object Result { get; set; }

        public OkResponse() : base()
        {
        }

        public OkResponse(object result) : base(200, string.Empty)
        {
            Result = result;
        }      
    }

    public class BadRequestResponse : ResponseBase
    {
        public List<string> Errors { get; set; } = new List<string>();

        public BadRequestResponse() : base()
        {
        }

        public BadRequestResponse(IEnumerable<string> errors) : base(400, string.Empty)
        {
            this.Errors.AddRange(errors);
        }
    }

    public class NotFoundResponse : ResponseBase
    {
        public NotFoundResponse() : base()
        {
        }

        public NotFoundResponse(string message) : base(404, message)
        {           
        }
    }

    public class ProblemResponse : ResponseBase
    {
        public ProblemResponse() : base()
        {
        }

        public ProblemResponse(string error) : base(500, error)
        {            
        }
    }
}
