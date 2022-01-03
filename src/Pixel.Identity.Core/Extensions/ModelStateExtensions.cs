using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pixel.Identity.Core
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<string> GetValidationErrors(this ModelStateDictionary modelState)
        {
            if(!modelState.IsValid)
            {
                return modelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage);
            }
            return Enumerable.Empty<string>();
        }
    }
}
