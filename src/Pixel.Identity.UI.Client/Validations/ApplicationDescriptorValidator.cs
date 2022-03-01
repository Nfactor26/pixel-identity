using FluentValidation;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.UI.Client.Validations
{
    public class ApplicationDescriptionValidator : AbstractValidator<ApplicationViewModel>
    {
        public ApplicationDescriptionValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.ConsentType).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Permissions).NotEmpty();
            
            //while creating a new application, client secret is mandatory for confidential clients
            RuleFor(x => x.ClientSecret).Must((c, m) =>
            {
                if(string.IsNullOrEmpty(c.Id) && (c.Type?.Equals("confidential") ?? false))
                {
                    return !string.IsNullOrEmpty(m);
                }
                return true;
            }).WithMessage("Client secret is required for confidential clients");
           
            //PostLogoutRedirectUris are required for Authorization code flow
            RuleFor(x => x.PostLogoutRedirectUris).Must((c, m) =>
            {
                if (c.Permissions.Contains(OpenIddict.Abstractions.OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode))
                {
                    return m.Count > 0;
                }
                return true;
            }).WithMessage("PostLogoutRedirectUris is required for Authorization Code Flow");

            //RedirectUris are required for Authorization code flow
            RuleFor(x => x.RedirectUris).Must((c, m) =>
            {
                if (c.Permissions.Contains(OpenIddict.Abstractions.OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode))
                {
                    return m.Count > 0;
                }
                return true;
            }).WithMessage("RedirectUris is required for Authorization Code Flow");
            
        }
    }
}
