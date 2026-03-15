using FluentValidation;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
namespace StoreApp.Application.Validations.BonusCardValidations
{
    public class UpdateBonusCardCommandRequestValidation : AbstractValidator<UpdateBonusCardCommandRequest>
    {
        public UpdateBonusCardCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The bonus card ID must be greater than zero !")
                .NotEmpty().WithMessage("The bonus card ID cannot be empty !");
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("The card number cannot be empty !")
                .Length(16).WithMessage("The card number must be 16 characters long !");
            RuleFor(x => x.Points)
                .GreaterThanOrEqualTo(0).WithMessage("The points must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The points cannot be empty !");
        }
    }
}