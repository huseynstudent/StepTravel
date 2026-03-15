using FluentValidation;
using StoreApp.Application.CQRS.Countries.Query.Request;
namespace StoreApp.Application.Validations.CountriesValidations
{
    public class GetByIdCountryCommandRequestValidation : AbstractValidator<GetCountryByIdQueryRequest>
    {
        public GetByIdCountryCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The country ID must be greater than zero !")
                .NotEmpty().WithMessage("The country ID cannot be empty !");
        }
    }
}