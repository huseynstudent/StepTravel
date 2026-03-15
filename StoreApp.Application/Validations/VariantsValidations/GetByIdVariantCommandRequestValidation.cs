using FluentValidation;
using StoreApp.Application.CQRS.Variants.Query.Request;
namespace StoreApp.Application.Validations.VariantsValidations
{
    public class GetByIdVariantCommandRequestValidation : AbstractValidator<GetVariantByIdQueryRequest>
    {
        public GetByIdVariantCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The variant ID must be greater than zero !")
                .NotEmpty().WithMessage("The variant ID cannot be empty !");
        }
    }
}