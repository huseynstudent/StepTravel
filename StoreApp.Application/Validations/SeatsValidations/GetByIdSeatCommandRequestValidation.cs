using FluentValidation;
using StoreApp.Application.CQRS.Seats.Query.Request;
namespace StoreApp.Application.Validations.SeatsValidations
{
    public class GetByIdSeatCommandRequestValidation : AbstractValidator<GetSeatByIdQueryRequest>
    {
        public GetByIdSeatCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The seat ID must be greater than zero !")
                .NotEmpty().WithMessage("The seat ID cannot be empty !");
        }
    }
}