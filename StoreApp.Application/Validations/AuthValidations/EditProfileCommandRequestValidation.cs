namespace StoreApp.Application.Validations.AuthValidations;

using FluentValidation;
using StoreApp.Application.CQRS.Users.Command.Request;
public class EditProfileCommandRequestValidation : AbstractValidator<EditProfileCommandRequest>
{
    public EditProfileCommandRequestValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(2).MaximumLength(50);

        RuleFor(x => x.Surname)
            .NotEmpty().MinimumLength(2).MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        RuleFor(x => x.Birthday)
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-120)));

        RuleFor(x => x.Fin)
            .NotEmpty()
            .Matches(@"^[A-Z0-9]{7}$").WithMessage("FIN must be exactly 7 uppercase letters/digits.");
    }
}