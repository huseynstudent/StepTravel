using FluentValidation;
using StoreApp.Application.CQRS.BonusProducts.Query.Request;
namespace StoreApp.Application.Validations.BonusProductValidations
{
    public class GetByIdBonusProductCommandRequestValidation : AbstractValidator<GetBonusProductByIdQueryRequest>
    {
        public GetByIdBonusProductCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");
        }
    }
}