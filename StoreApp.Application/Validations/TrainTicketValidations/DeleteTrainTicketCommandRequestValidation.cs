using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;

namespace StoreApp.Application.Validations.TrainTicketValidations
{
    public class DeleteTrainTicketCommandRequestValidation : AbstractValidator<DeleteTrainTicketCommandRequest>
    {
        public DeleteTrainTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The Id cannot be empty !")
                .GreaterThan(0).WithMessage("The Id must be greater than zero !");
        }
    }
}
