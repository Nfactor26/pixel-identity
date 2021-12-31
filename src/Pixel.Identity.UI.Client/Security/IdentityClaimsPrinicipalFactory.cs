using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Client
{
    public class IdentityClaimsPrincipalFactory<TAccount> : AccountClaimsPrincipalFactory<TAccount> where TAccount : RemoteUserAccount
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="accessor"></param>
        public IdentityClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        
        }


        /// <summary>
        /// Role claims are by default emitted as array. However, Blazor needs single entry per role to understand it as claims.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(TAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            var claimsIdentity = user.Identity as ClaimsIdentity;

            if (account != null)
            {
                foreach (var kvp in account.AdditionalProperties)
                {                 
                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(kvp.Key));
                        var claims = element.EnumerateArray().Select(x => new Claim(kvp.Key, x.ToString()));
                        claimsIdentity.AddClaims(claims);
                    }                    
                }
            }

            return user;
        }

    }

}
