namespace Pixel.Identity.Core.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool ShowStackTrace => !string.IsNullOrEmpty(StackTrace);

    }
}
