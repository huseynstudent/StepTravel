using FluentValidation;
using StoreApp.Application.CQRS.User.Command.Request;

namespace StoreApp.Application.Validations.UserValidations;

public class ResetPasswordCommandRequestValidation : AbstractValidator<ResetPasswordCommandRequest>
{
    public ResetPasswordCommandRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Reset code is required.")
            .Matches(@"^\d{6}$").WithMessage("Code must be 6 digits.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
            .WithMessage("Password needs upper, lower, digit and special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}