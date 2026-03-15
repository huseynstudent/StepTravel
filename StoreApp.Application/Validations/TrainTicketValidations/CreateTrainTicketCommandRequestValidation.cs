using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
namespace StoreApp.Application.Validations.TrainTicketValidations
{
    public class CreateTrainTicketCommandRequestValidation : AbstractValidator<CreateTrainTicketCommandRequest>
    {
        public CreateTrainTicketCommandRequestValidation()
        {
            RuleFor(x => x.TrainCompany)
                .NotEmpty().WithMessage("The train company cannot be empty !")
                .MaximumLength(100).WithMessage("The train company must not exceed 100 characters !")
                .MinimumLength(1).WithMessage("The train company must be at least 1 character long !");

            RuleFor(x => x.TrainNumber)
                .NotEmpty().WithMessage("The train number cannot be empty !")
                .MaximumLength(50).WithMessage("The train number must not exceed 50 characters !")
                .MinimumLength(1).WithMessage("The train number must be at least 1 character long !");

            RuleFor(x => x.VagonNumber)
                .NotEmpty().WithMessage("The vagon number cannot be empty !")
                .GreaterThan(0).WithMessage("The vagon number must be greater than zero !");
        }
    }
}