using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pixel.Identity.Shared.Responses
{
    public class ResponseBase
    {       
        public int Status { get; set; }

        public string Message { get; set; }
      
        public ResponseBase()
        {
        }

        public ResponseBase(int status, string message)
        {
            Status = status;
            Message = message ?? GetDefaultMessageForStatusCode(status);
        }

        private static string GetDefaultMessageForStatusCode(int status)
        {
            switch (status)
            {
                case 400:
                    return "Bad Request";
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                default:
                    return status.ToString();
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
        public string? Type { get; set; }      
        
        public string? Title { get; set; }        
     
        public string? Detail { get; set; }     
     
        public string? Instance { get; set; }
     
        [JsonExtensionData]
        public IDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

        public ProblemResponse() : base()
        {
        }

        public ProblemResponse(string error) : base(500, error)
        {            
        }
    }
}
