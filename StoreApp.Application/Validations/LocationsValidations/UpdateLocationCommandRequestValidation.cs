using FluentValidation;
using StoreApp.Application.CQRS.Locations.Command.Request;
namespace StoreApp.Application.Validations.LocationsValidations
{
    public class UpdateLocationCommandRequestValidation : AbstractValidator<UpdateLocationCommandRequest>
    {
        public UpdateLocationCommandRequestValidation() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The location ID must be greater than zero !")
                .NotEmpty().WithMessage("The location ID cannot be empty !");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The location name cannot be empty !")
                .MaximumLength(100).WithMessage("The location name must not exceed 100 characters !")
                .MinimumLength(2).WithMessage("The location name must be at least 2 characters long !");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("The country ID must be greater than zero !")
                .NotEmpty().WithMessage("The country ID cannot be empty !");

            RuleFor(x => x.DistanceToken)
                .GreaterThan(0).WithMessage("The distance token must be greater than zero !")
                .NotEmpty().WithMessage("The distance token cannot be empty !");

        }
    }
}