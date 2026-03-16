using FluentValidation;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
namespace StoreApp.Application.Validations.BonusCardValidations;
public class CreateBonusCardCommandRequestValidation:AbstractValidator<CreateBonusCardCommandRequest>
{
    public CreateBonusCardCommandRequestValidation()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("The bonus card number cannot be empty !")
            .Length(16).WithMessage("The bonus card number must be 16 digits long !")
            .Matches(@"^\d+$").WithMessage("The bonus card number must contain only digits !");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0).WithMessage("The bonus card points cannot be less than zero !");
    }
}