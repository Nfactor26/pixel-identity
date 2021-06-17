using FluentValidation;
using Pixel.Identity.Shared.ViewModels;

namespace Pixel.Identity.UI.Client.Validations
{
    public class ScopeValidator : AbstractValidator<ScopeViewModel>
    {
        public ScopeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(4);
            RuleFor(x => x.DisplayName).NotEmpty().MinimumLength(4);          
        }
    }
}
