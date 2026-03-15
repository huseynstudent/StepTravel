using FluentValidation;
using StoreApp.Application.CQRS.Countries.Command.Request;
namespace StoreApp.Application.Validations.CountriesValidations
{
    public class UpdateCountryCommandRequestValidation : AbstractValidator<UpdateCountryCommandRequest>
    {
        public UpdateCountryCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The country ID must be greater than zero !")
                .NotEmpty().WithMessage("The country ID cannot be empty !");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The country name cannot be empty !")
                .MaximumLength(100).WithMessage("The country name must not exceed 100 characters !")
                .MinimumLength(2).WithMessage("The country name must be at least 2 characters long !");
        }
    }
}