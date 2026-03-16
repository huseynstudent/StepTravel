using FluentValidation;
using StoreApp.Application.CQRS.Auth.Command.Request;
namespace StoreApp.Application.Validations.AuthValidations
{
    public class LoginUserCommandRequestValidation : AbstractValidator<LoginUserCommandRequest>
    {
        public LoginUserCommandRequestValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("Valid email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email must be in a valid format.");

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
        }
    }
}