using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Pixel.Identity.Provider.Extensions
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
