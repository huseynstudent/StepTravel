using FluentValidation;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Domain.Enums;

namespace StoreApp.Application.Validations.PlaneTicketsValidations
{
    public class UpdatePlaneTicketCommandRequestValidation : AbstractValidator<UpdatePlaneTicketCommandRequest>
    {
        public UpdatePlaneTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero!");

            RuleFor(x => x.Airline)
                .NotEmpty().WithMessage("The airline cannot be empty!")
                .MaximumLength(100).WithMessage("The airline must not exceed 100 characters!");

            RuleFor(x => x.Gate)
                .NotEmpty().WithMessage("The gate cannot be empty!")
                .MaximumLength(10).WithMessage("The gate must not exceed 10 characters!");

            RuleFor(x => x.Meal)
                .NotEmpty().WithMessage("The meal cannot be empty!")
                .MaximumLength(50).WithMessage("The meal must not exceed 50 characters!");

            RuleFor(x => x.LuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage weight must be greater than or equal to zero!");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("The state is not a valid value!");
        }
    }

    public class UpdatePlaneTicketGroupCommandRequestValidation : AbstractValidator<UpdatePlaneTicketGroupCommandRequest>
    {
        public UpdatePlaneTicketGroupCommandRequestValidation()
        {
            // Group match fields
            RuleFor(x => x.Airline)
                .NotEmpty().WithMessage("The airline cannot be empty!")
                .MaximumLength(100).WithMessage("The airline must not exceed 100 characters!");

            RuleFor(x => x.Plane)
                .NotEmpty().WithMessage("The plane cannot be empty!")
                .MaximumLength(50).WithMessage("The plane must not exceed 50 characters!");

            RuleFor(x => x.Gate)
                .NotEmpty().WithMessage("The gate cannot be empty!")
                .MaximumLength(10).WithMessage("The gate must not exceed 10 characters!");

            RuleFor(x => x.Meal)
                .NotEmpty().WithMessage("The meal cannot be empty!")
                .MaximumLength(50).WithMessage("The meal must not exceed 50 characters!");

            RuleFor(x => x.LuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The luggage weight must be greater than or equal to zero!");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("The due date cannot be empty!")
                .GreaterThan(DateTime.UtcNow).WithMessage("The due date must be in the future!");

            RuleFor(x => x.FromId)
                .GreaterThan(0).WithMessage("FromId must be greater than zero!");

            RuleFor(x => x.ToId)
                .GreaterThan(0).WithMessage("ToId must be greater than zero!")
                .NotEqual(x => x.FromId).WithMessage("From and To locations cannot be the same!");

            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("VariantId must be greater than zero!")
                .When(x => x.VariantId.HasValue);

            // New values to apply
            RuleFor(x => x.NewAirline)
                .NotEmpty().WithMessage("The new airline cannot be empty!")
                .MaximumLength(100).WithMessage("The new airline must not exceed 100 characters!");

            RuleFor(x => x.NewGate)
                .NotEmpty().WithMessage("The new gate cannot be empty!")
                .MaximumLength(10).WithMessage("The new gate must not exceed 10 characters!");

            RuleFor(x => x.NewMeal)
                .NotEmpty().WithMessage("The new meal cannot be empty!")
                .MaximumLength(50).WithMessage("The new meal must not exceed 50 characters!");

            RuleFor(x => x.NewLuggageKg)
                .GreaterThanOrEqualTo(0).WithMessage("The new luggage weight must be greater than or equal to zero!");

            RuleFor(x => x.NewState)
                .IsInEnum().WithMessage("The new state is not a valid value!");

            RuleFor(x => x.NewDueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The new due date must be in the future!")
                .When(x => x.NewDueDate.HasValue);
        }
    }
}