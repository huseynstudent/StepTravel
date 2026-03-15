using FluentValidation;
using StoreApp.Application.CQRS.Locations.Command.Request;

namespace StoreApp.Application.Validations.LocationsValidations
{
   public class DeleteLocationCommandRequestValidation : AbstractValidator<DeleteLocationCommandRequest>
    {
        public DeleteLocationCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The location ID must be greater than zero !")
                .NotEmpty().WithMessage("The location ID cannot be empty !");
        }
    }
}
