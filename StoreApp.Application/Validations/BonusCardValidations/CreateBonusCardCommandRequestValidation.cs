using FluentValidation;
using StoreApp.Application.CQRS.BonusCards.Command.Request;

namespace StoreApp.Application.Validations.BonusCardValidations;

public class CreateBonusCardCommandRequestValidation:AbstractValidator<CreateBonusCardCommandRequest>
{
    public CreateBonusCardCommandRequestValidation()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Bonus kart nömrəsi boş ola bilməz.")
            .Length(16).WithMessage("Bonus kart nömrəsi 16 rəqəm olmalıdır.")
            .Matches(@"^\d+$").WithMessage("Bonus kart nömrəsi yalnız rəqəmlər içermelidir.");
        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0).WithMessage("Bonus kart xallar sıfırdan az olamaz.");
    }
}
