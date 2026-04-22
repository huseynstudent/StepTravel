using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;

namespace StoreApp.Application.Validations.TrainTicketValidations
{
    internal class UpdateTrainTicketCommandRequestValidation : AbstractValidator<UpdateTrainTicketCommandRequest>
    {
        public UpdateTrainTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The Id cannot be empty !")
                .GreaterThan(0).WithMessage("The Id must be greater than zero !");

            RuleFor(x => x.TrainCompany)
                .NotEmpty().WithMessage("The train company cannot be empty !")
                .MaximumLength(100).WithMessage("The train company must not exceed 100 characters !");

            RuleFor(x => x.TrainNumber)
                .NotEmpty().WithMessage("The train number cannot be empty !")
                .MaximumLength(20).WithMessage("The train number must not exceed 20 characters !");

            RuleFor(x => x.VagonNumber)
                .GreaterThan(0).WithMessage("The vagon number must be greater than zero !");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("The state is not a valid value !");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The due date must be in the future !")
                .When(x => x.DueDate.HasValue);
        }
    }

    public class UpdateTrainTicketGroupCommandRequestValidation : AbstractValidator<UpdateTrainTicketGroupCommandRequest>
    {
        public UpdateTrainTicketGroupCommandRequestValidation()
        {
            RuleFor(x => x.TrainCompany)
                .NotEmpty().WithMessage("The train company cannot be empty !")
                .MaximumLength(100).WithMessage("The train company must not exceed 100 characters !");

            RuleFor(x => x.TrainNumber)
                .NotEmpty().WithMessage("The train number cannot be empty !")
                .MaximumLength(20).WithMessage("The train number must not exceed 20 characters !");

            RuleFor(x => x.VagonNumber)
                .GreaterThan(0).WithMessage("The vagon number must be greater than zero !");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("The due date cannot be empty !");

            RuleFor(x => x.FromId)
                .GreaterThan(0).WithMessage("FromId must be greater than zero !");

            RuleFor(x => x.ToId)
                .GreaterThan(0).WithMessage("ToId must be greater than zero !")
                .NotEqual(x => x.FromId).WithMessage("From and To locations cannot be the same !");

            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("VariantId must be greater than zero !")
                .When(x => x.VariantId.HasValue);

            RuleFor(x => x.NewTrainCompany)
                .NotEmpty().WithMessage("The new train company cannot be empty !")
                .MaximumLength(100).WithMessage("The new train company must not exceed 100 characters !");

            RuleFor(x => x.NewTrainNumber)
                .NotEmpty().WithMessage("The new train number cannot be empty !")
                .MaximumLength(20).WithMessage("The new train number must not exceed 20 characters !");

            RuleFor(x => x.NewVagonNumber)
                .GreaterThan(0).WithMessage("The new vagon number must be greater than zero !");

            RuleFor(x => x.NewState)
                .IsInEnum().WithMessage("The new state is not a valid value !");

            RuleFor(x => x.NewDueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The new due date must be in the future !")
                .When(x => x.NewDueDate.HasValue);
        }
    }
}