namespace Pixel.Identity.Shared.Request
{
    public class GetApplicationsRequest : PagedDataRequest
    {
        public string ApplicationFilter { get; set; }
    }
}
