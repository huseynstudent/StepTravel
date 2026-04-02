namespace StoreApp.Application.Validations.AuthValidations;

using FluentValidation;
using StoreApp.Application.CQRS.Users.Command.Request;

public class ChangePasswordCommandRequestValidation : AbstractValidator<ChangePasswordCommandRequest>
{
    public ChangePasswordCommandRequestValidation()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
            .WithMessage("Password needs upper, lower, digit and special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}