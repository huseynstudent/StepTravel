using FluentValidation;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
namespace StoreApp.Application.Validations.TrainTicketValidations
{
    public class FillTrainTicketCommandRequestValidation : AbstractValidator<FillTrainTicketCommandRequest>
    {
        public FillTrainTicketCommandRequestValidation()
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

            RuleFor(x => x.FromId)
                .GreaterThan(0).WithMessage("The from location ID must be greater than zero !")
                .NotEmpty().WithMessage("The from location ID cannot be empty !");

            RuleFor(x => x.ToId)
                .GreaterThan(0).WithMessage("The to location ID must be greater than zero !")
                .NotEmpty().WithMessage("The to location ID cannot be empty !");

            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("The variant ID must be greater than zero !")
                .NotEmpty().WithMessage("The variant ID cannot be empty !");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("The note must not exceed 500 characters !")
                .MinimumLength(0).WithMessage("The note must be at least 0 characters long !");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("The state must be a valid enum value !")
                .NotEmpty().WithMessage("The state cannot be empty !");

            RuleFor(x => x.HasPet)
                .IsInEnum().WithMessage("The payment method must be a valid enum value !")
                .NotEmpty().WithMessage("The payment method cannot be empty !");

            RuleFor(x => x.HasChild)
                .IsInEnum().WithMessage("The payment method must be a valid enum value !")
                .NotEmpty().WithMessage("The payment method cannot be empty !");

            RuleFor(x => x.IsRoundTrip)
                .NotNull().WithMessage("The round trip status cannot be null !")
                .Must(x => x == true || x == false).WithMessage("The round trip status must be either true or false !");

            RuleFor(x => x.LuggageCount)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage count must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The luggage count cannot be empty !");

             RuleFor(x => x.TotalLuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The total luggage kg must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The total luggage kg cannot be empty !");

             RuleFor(x => x.IsCashPayment)
                .NotNull().WithMessage("The cash payment status cannot be null !")
                .Must(x => x == true || x == false).WithMessage("The cash payment status must be either true or false !");

            RuleFor(x => x.TotalLuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The total luggage kg must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The total luggage kg cannot be empty !");

            RuleFor(x => x.IsCashPayment)
                .NotNull().WithMessage("The cash payment status cannot be null !")
                .Must(x => x == true || x == false).WithMessage("The cash payment status must be either true or false !");
        }
    }
}