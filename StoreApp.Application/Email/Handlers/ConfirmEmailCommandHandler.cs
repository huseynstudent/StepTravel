using MediatR;
using StoreApp.Application.Email.Commands;
using StoreApp.DAL.Context;
using Microsoft.EntityFrameworkCore;
namespace StoreApp.Application.Email.Handlers
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly StoreAppDbContext _context;
        public ConfirmEmailCommandHandler(StoreAppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || user.ConfirmCode != request.ConfirmCode)
            {
                return false;
            }

            user.IsConfirmed = true;
            user.ConfirmCode = null;
            
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}