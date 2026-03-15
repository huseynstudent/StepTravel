using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
namespace StoreApp.Application.Validations.TrainTicketValidations
{
    public class GetByIdTrainTicketCommandRequestValidation : AbstractValidator<GetTrainTicketByIdQueryRequest>
    {
        public GetByIdTrainTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");
        }
    }
}
