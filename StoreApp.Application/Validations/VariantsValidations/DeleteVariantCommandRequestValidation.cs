using FluentValidation;
using StoreApp.Application.CQRS.Variants.Command.Request;
namespace StoreApp.Application.Validations.VariantsValidations
{
    public class DeleteVariantCommandRequestValidation : AbstractValidator<DeleteVariantCommandRequest>
    {
        public DeleteVariantCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The variant ID must be greater than zero !")
                .NotEmpty().WithMessage("The variant ID cannot be empty !");
        }
    }
}