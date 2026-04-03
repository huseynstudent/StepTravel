using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.Email.Commands;
using StoreApp.DAL.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StoreApp.Application.Email.Handlers
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly StoreAppDbContext _db;

        public ConfirmEmailCommandHandler(StoreAppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
            if (user == null) return false;
            if (user.IsConfirmed) return true;
            if (string.IsNullOrEmpty(user.ConfirmCode) || user.ConfirmCode.Trim() != request.ConfirmCode?.Trim())
            {
                return false;
            }
            user.IsConfirmed = true;
            user.ConfirmCode = null;
            _db.Users.Update(user);
            var result = await _db.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }
}