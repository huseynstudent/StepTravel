using FluentValidation;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;

namespace StoreApp.Application.Validations.PlaneTicketsValidations
{
    public class DeletePlaneTicketCommandRequestValidation: AbstractValidator<DeletePlaneTicketGroupCommandRequest>
    {
        public DeletePlaneTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The Id cannot be empty !")
                .GreaterThan(0).WithMessage("The Id must be greater than zero !");
        }
    }
}