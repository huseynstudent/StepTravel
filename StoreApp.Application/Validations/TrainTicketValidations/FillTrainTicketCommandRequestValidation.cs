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

            RuleFor(x => x.ChosenSeatId)
                .GreaterThan(0).WithMessage("The seat ID must be greater than zero !")
                .NotEmpty().WithMessage("The seat ID cannot be empty !");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("The note must not exceed 500 characters !");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("The state must be a valid enum value !")
                .NotEmpty().WithMessage("The state cannot be empty !");

            RuleFor(x => x.HasPet)
                .Must(v => v == true || v == false).WithMessage("HasPet must be true or false !");

            RuleFor(x => x.HasChild)
                .Must(v => v == true || v == false).WithMessage("HasChild must be true or false !");
            RuleFor(x => x.LuggageCount)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage count must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The luggage count cannot be empty !");

            RuleFor(x => x.TotalLuggageKg)
               .GreaterThanOrEqualTo(0).WithMessage("The total luggage kg must be greater than or equal to zero !")
               .NotEmpty().WithMessage("The total luggage kg cannot be empty !");

            RuleFor(x => x.TotalLuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The total luggage kg must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The total luggage kg cannot be empty !");

        }
    }
}