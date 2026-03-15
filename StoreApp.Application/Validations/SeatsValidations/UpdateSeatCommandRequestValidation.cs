using FluentValidation;
using StoreApp.Application.CQRS.Seats.Command.Request;
namespace StoreApp.Application.Validations.SeatsValidations
{
    public class UpdateSeatCommandRequestValidation : AbstractValidator<UpdateSeatCommandRequest>
    {
        public UpdateSeatCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The seat ID must be greater than zero !")
                .NotEmpty().WithMessage("The seat ID cannot be empty !");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The seat name cannot be empty !")
                .MaximumLength(100).WithMessage("The seat name must not exceed 100 characters !")
                .MinimumLength(1).WithMessage("The seat name must be at least 1 character long !");

            RuleFor(x => x.IsOccupied)
                .NotNull().WithMessage("The seat occupied status cannot be null !")
                .Must(x => x == true || x == false).WithMessage("The seat occupied status must be either true or false !");

            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("The variant ID must be greater than zero !")
                .NotEmpty().WithMessage("The variant ID cannot be empty !");
        }
    }
}