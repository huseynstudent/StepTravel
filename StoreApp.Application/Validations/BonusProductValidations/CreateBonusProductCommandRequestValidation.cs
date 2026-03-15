using FluentValidation;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
namespace StoreApp.Application.Validations.BonusProductValidations
{
    public class CreateBonusProductCommandRequestValidation : AbstractValidator<CreateBonusProductCommandRequest>
    {
        public CreateBonusProductCommandRequestValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The bonus product name cannot be empty !")
                .MaximumLength(100).WithMessage("The bonus product name must not exceed 100 characters !")
                .MinimumLength(1).WithMessage("The bonus product name must be at least 1 character long !");

            RuleFor(x => x.PricePoint)
                .GreaterThan(0).WithMessage("The price point must be greater than zero !")
                .NotEmpty().WithMessage("The price point cannot be empty !");

            RuleFor(x => x.InStock)
                .GreaterThanOrEqualTo(0).WithMessage("The stock quantity must be greater than or equal to zero !")
                .NotEmpty().WithMessage("The stock quantity cannot be empty !");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("The image cannot be null !")
                .Must(file => file.ContentType.StartsWith("image/")).WithMessage("The file must be an image !");
        }
    }
}