using MediatR;
namespace StoreApp.Application.Email.Commands
{
    public class ConfirmEmailCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public string ConfirmCode { get; set; }
    }
}