using FluentValidation;
using StoreApp.Application.CQRS.Seats.Command.Request;
namespace StoreApp.Application.Validations.SeatsValidations
{
    public class DeleteSeatCommandRequestValidation : AbstractValidator<DeleteSeatCommandRequest>
    {
        public DeleteSeatCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The seat ID must be greater than zero !")
                .NotEmpty().WithMessage("The seat ID cannot be empty !");
        }
    }
}