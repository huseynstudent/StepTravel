using FluentValidation;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;

namespace StoreApp.Application.Validations.PlaneTicketsValidations
{
    public class CreatePlaneTicketCommandRequestValidation : AbstractValidator<CreatePlaneTicketCommandRequest>
    {
        public CreatePlaneTicketCommandRequestValidation()
        {
            RuleFor(x => x.Airline)
                .NotEmpty().WithMessage("The airline cannot be empty !")
                .MaximumLength(100).WithMessage("The airline must not exceed 100 characters !");

            RuleFor(x => x.Gate)
                .NotEmpty().WithMessage("The gate cannot be empty !")
                .MaximumLength(10).WithMessage("The gate must not exceed 10 characters !");

            RuleFor(x => x.Plane)
                .NotEmpty().WithMessage("The plane cannot be empty !")
                .MaximumLength(50).WithMessage("The plane must not exceed 50 characters !");

            RuleFor(x => x.Meal)
                .NotEmpty().WithMessage("The meal cannot be empty !")
                .MaximumLength(50).WithMessage("The meal must not exceed 50 characters !");

            RuleFor(x => x.LuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage weight must be greater than or equal to zero !");
        }
    }
}
