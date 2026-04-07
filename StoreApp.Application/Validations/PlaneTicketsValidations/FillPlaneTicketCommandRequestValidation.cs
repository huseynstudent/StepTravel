using FluentValidation;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
namespace StoreApp.Application.Validations.PlaneTicketsValidations
{
    public class FillPlaneTicketCommandRequestValidation : AbstractValidator<FillPlaneTicketCommandRequest>
    {
        public FillPlaneTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).WithMessage("The due date must be in the future !")
                .NotEmpty().WithMessage("The due date cannot be empty !");

            RuleFor(x => x.ChosenSeatId)
                .GreaterThan(0).WithMessage("The seat ID must be greater than zero !")
                .NotEmpty().WithMessage("The seat ID cannot be empty !");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("The note must not exceed 500 characters !");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("The state must be a valid enum value !")
                .NotEmpty().WithMessage("The state cannot be empty !");

            RuleFor(x => x.IsRoundTrip)
                .Must(x => x == true || x == false).WithMessage("The round trip status must be either true or false !");
            RuleFor(x => x.LuggageCount)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage count must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The luggage count cannot be empty !");

            RuleFor(x => x.TotalLuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The total luggage weight must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The total luggage weight cannot be empty !");
        }
    }
}