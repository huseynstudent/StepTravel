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

    public ChangePasswordCommandHandler(StoreAppDbContext db, ILogger<ChangePasswordCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ResponseModel<ChangePasswordCommandResponse>> Handle(
        ChangePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Fail("User not found.");

        if (!PasswordHelper.Verify(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("ChangePassword: wrong current password for user {UserId}.", request.UserId);
            return Fail("Current password is incorrect.");
        }

        if (request.NewPassword != request.ConfirmPassword)
            return Fail("New password and confirmation do not match.");

        user.PasswordHash = PasswordHelper.Hash(request.NewPassword);

        _unitOfWork.UserRepository.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ChangePassword: user {UserId} password updated.", user.Id);
        return new ResponseModel<ChangePasswordCommandResponse>(
            new ChangePasswordCommandResponse { IsSuccess = true, Message = "Password changed successfully." });
    }

    private static ResponseModel<ChangePasswordCommandResponse> Fail(string message) =>
        new(new ChangePasswordCommandResponse { IsSuccess = false, Message = message });
}
