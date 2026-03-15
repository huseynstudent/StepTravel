using FluentValidation;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
namespace StoreApp.Application.Validations.PlaneTicketsValidations
{
    public class GetByIdPlaneTicketCommandRequestValidation : AbstractValidator<GetPlaneTicketByIdQueryRequest>
    {
        public GetByIdPlaneTicketCommandRequestValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("The Id must be greater than zero !")
                .NotEmpty().WithMessage("The Id cannot be empty !");
        }
    }
}