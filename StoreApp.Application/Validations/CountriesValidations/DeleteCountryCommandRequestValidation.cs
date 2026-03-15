using FluentValidation;
using StoreApp.Application.CQRS.Countries.Command.Request;
namespace StoreApp.Application.Validations.CountriesValidations
{
    public class DeleteCountryCommandRequestValidation : AbstractValidator<DeleteCountryCommandRequest>
     {
         public DeleteCountryCommandRequestValidation()
         {
             RuleFor(x => x.Id)
                 .GreaterThan(0).WithMessage("The country ID must be greater than zero !")
                 .NotEmpty().WithMessage("The country ID cannot be empty !");
         }
    }
}