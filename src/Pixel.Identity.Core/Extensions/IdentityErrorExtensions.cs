using Microsoft.AspNetCore.Identity;

namespace Pixel.Identity.Core.Extensions
{
    public static class IdentityErrorExtensions
    {
        public static IEnumerable<string> GetErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors.Select(s => $"{s.Code}:{s.Description}").Distinct();
        }
    }
}
