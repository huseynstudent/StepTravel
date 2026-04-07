namespace StoreApp.Application.CQRS.Users.Handler;

using StoreApp.Application.CQRS.Users.Command.Request;
using StoreApp.Application.CQRS.Users.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApp.Repository.Comman;

public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommandRequest, ResponseModel<ChangePasswordCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    // Konstruktorda IUnitOfWork mütləq qəbul edilməlidir
    public ChangePasswordCommandHandler(
        StoreAppDbContext db,
        ILogger<ChangePasswordCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _logger = logger;
        _unitOfWork = unitOfWork; // Xətanı aradan qaldıran əsas sətir budur
    }

    public async Task<ResponseModel<ChangePasswordCommandResponse>> Handle(
        ChangePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Fail("User not found.");

        // Cari şifrənin doğruluğunu yoxlayırıq
        if (!PasswordHelper.Verify(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("ChangePassword: wrong current password for user {UserId}.", request.UserId);
            return Fail("Current password is incorrect.");
        }

        // Yeni şifrə və təkrarının uyğunluğunu yoxlayırıq
        if (request.NewPassword != request.ConfirmPassword)
            return Fail("New password and confirmation do not match.");

        // Yeni şifrəni heşləyib mənimsədirik
        user.PasswordHash = PasswordHelper.Hash(request.NewPassword);

        // Repozitoriyanı yeniləyirik
        _unitOfWork.UserRepository.Update(user);

        // Dəyişiklikləri bazada saxlayırıq
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ChangePassword: user {UserId} password updated.", user.Id);

        return new ResponseModel<ChangePasswordCommandResponse>(
            new ChangePasswordCommandResponse { IsSuccess = true, Message = "Password changed successfully." });
    }

    private static ResponseModel<ChangePasswordCommandResponse> Fail(string message) =>
        new(new ChangePasswordCommandResponse { IsSuccess = false, Message = message });
}