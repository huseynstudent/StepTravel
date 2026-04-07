namespace StoreApp.Application.Validations.AuthValidations;

using FluentValidation;
using StoreApp.Application.CQRS.Auth.Command.Request;
public class CreateExecutiveCommandRequestValidation : AbstractValidator<CreateExecutiveCommandRequest>
{
    public CreateExecutiveCommandRequestValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Name must be less than 50 characters.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .MinimumLength(2).WithMessage("Surname must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Surname must be less than 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email must be in a valid format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("Password must contain uppercase, lowercase, digit, and a special character (@$!%*?&).");
    }
}