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
            // 1. İstifadəçini tapırıq
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

            // 2. Yoxlamalar
            if (user == null) return false;

            // Əgər artıq təsdiqlənibsə, birbaşa true qaytarırıq
            if (user.IsConfirmed) return true;

            // KRİTİK: Kodları müqayisə edərkən Trim() istifadə edirik (boşluq qalmasın)
            if (string.IsNullOrEmpty(user.ConfirmCode) || user.ConfirmCode.Trim() != request.ConfirmCode?.Trim())
            {
                return false;
            }

            // 3. Statusu dəyişirik
            user.IsConfirmed = true;
            user.ConfirmCode = null;

            // EF-ə obyektin dəyişdiyini bildirmək üçün mütləq Update və SaveChanges lazımdır
            _db.Users.Update(user);
            var result = await _db.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }
}