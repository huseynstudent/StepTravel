using FluentValidation;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
namespace StoreApp.Application.Validations.BonusCardValidations
{
    public class DeleteBonusCardCommandRequestValidation : AbstractValidator<DeleteBonusCardCommandRequest>
    {
        public DeleteBonusCardCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The bonus card ID must be greater than zero !")
                .NotEmpty().WithMessage("The bonus card ID cannot be empty !");
        }
    }
}