using FluentValidation;
using StoreApp.Application.CQRS.Auth.Command.Request;
namespace StoreApp.Application.Validations.AuthValidations
{
    public class RegisterUserCommandRequestValidation : AbstractValidator<RegisterUserCommandRequest>
    {
        public RegisterUserCommandRequestValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name must be less than 50 characters long.");


            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MinimumLength(2).WithMessage("Surname must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Surname must be less than 50 characters long.");


            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("Valid email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email must be in a valid format.");


            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")

            RuleFor(x => x.Birthday)
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Birthday must be in the past.")
                .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-120))).WithMessage("Birthday must be within a reasonable range.")
                 .NotEmpty().WithMessage("Birthday is required.");


            RuleFor(x => x.Fin)
                .NotEmpty().WithMessage("Fin is required.")
                .Matches(@"^[A-Z0-9]{7}$").WithMessage("Fin must be exactly 7 characters long and contain only uppercase letters and digits.");
        }
    }
}