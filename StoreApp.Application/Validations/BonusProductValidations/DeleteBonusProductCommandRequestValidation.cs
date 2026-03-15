using FluentValidation;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
namespace StoreApp.Application.Validations.BonusProductValidations
{
    public class DeleteBonusProductCommandRequestValidation : AbstractValidator<DeleteBonusProductCommandRequest>
    {
        public DeleteBonusProductCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");
        }
    }
}