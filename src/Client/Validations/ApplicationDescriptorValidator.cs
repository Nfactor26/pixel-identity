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
            //while creating a new application, client secret is mandatory for confidential clients
            RuleFor(x => x.ClientSecret).Must((c, m) =>
            {
                if(string.IsNullOrEmpty(c.Id) && (c.Type?.Equals("confidential") ?? false))
                {
                    return !string.IsNullOrEmpty(m);
                }
                return true;
            }).WithMessage("Client secret is required for confidential clients");
            RuleFor(x => x.PostLogoutRedirectUris).NotEmpty();
            RuleFor(x => x.RedirectUris).NotEmpty();
            RuleFor(x => x.Permissions).NotEmpty();
        }
    }
}
