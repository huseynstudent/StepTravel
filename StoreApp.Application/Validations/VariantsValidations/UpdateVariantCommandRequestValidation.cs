using FluentValidation;
using StoreApp.Application.CQRS.Variants.Command.Request;
namespace StoreApp.Application.Validations.VariantsValidations
{
    public class UpdateVariantCommandRequestValidation : AbstractValidator<UpdateVariantCommandRequest>
    {
        public UpdateVariantCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The variant ID must be greater than zero !")
                .NotEmpty().WithMessage("The variant ID cannot be empty !");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The variant name cannot be empty !")
                .MaximumLength(100).WithMessage("The variant name must not exceed 100 characters !");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("The variant price must be greater than zero !")
                .NotEmpty().WithMessage("The variant price cannot be empty !");

            RuleFor(x => x.IsPriority)
                .NotNull().WithMessage("The variant priority status cannot be null !")
                .Must(x => x == true || x == false).WithMessage("The variant priority status must be either true or false !");
        }
    }
}