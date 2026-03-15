using FluentValidation;
using StoreApp.Application.CQRS.Locations.Query.Request;
namespace StoreApp.Application.Validations.LocationsValidations
{
    public class GetByIdLocationCommandRequestValidation : AbstractValidator<GetLocationByIdQueryRequest>
    {
        public GetByIdLocationCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The location ID must be greater than zero !")
                .NotEmpty().WithMessage("The location ID cannot be empty !");
        }
    }
}