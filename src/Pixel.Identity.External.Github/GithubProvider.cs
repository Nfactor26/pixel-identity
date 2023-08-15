using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pixel.Identity.Core;

namespace Pixel.Identity.External.Github
{
    public class GithubProvider : IExternalAuthProvider
    {
        public AuthenticationBuilder AddProvider(IConfiguration configuration, AuthenticationBuilder authenticationBuilder)
        {
            return authenticationBuilder.AddGitHub(options =>
            {
                options.ClientId = "****";
                options.ClientSecret = "***";
                options.Scope.Add("user:email");
            });
        }
    }
}