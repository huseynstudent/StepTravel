using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
namespace StoreApp.Application.Validations.TrainTicketValidations
{
    public class UpdateTrainTicketCommandRequestValidation : AbstractValidator<UpdateTrainTicketCommandRequest>
    {
        public UpdateTrainTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");

            RuleFor(x => x.TrainCompany)
                 .NotEmpty().WithMessage("The train company cannot be empty !")
                .MaximumLength(100).WithMessage("The train company must not exceed 100 characters !");

            RuleFor(x => x.TrainNumber)
                .NotEmpty().WithMessage("The train number cannot be empty !")
                .MaximumLength(20).WithMessage("The train number must not exceed 20 characters !");

            RuleFor(x => x.VagonNumber)
                .GreaterThan(0).WithMessage("The vagon number must be greater than zero !")
                .NotEmpty().WithMessage("The vagon number cannot be empty !");
        }
    }
}
