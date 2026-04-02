using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.User.Command.Request;
using StoreApp.Application.CQRS.User.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;

namespace StoreApp.Application.CQRS.User.Handler.CommandHandler;

public class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommandRequest, ResponseModel<ResetPasswordCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(StoreAppDbContext db, ILogger<ResetPasswordCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ResponseModel<ResetPasswordCommandResponse>> Handle(
        ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return Fail("Passwords do not match.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("ResetPassword: email {Email} not found.", request.Email);
            return Fail("Invalid or expired code.");
        }
        if (string.IsNullOrEmpty(user.ConfirmCode) ||
            user.ConfirmCode.Trim() != request.Code?.Trim())
        {
            _logger.LogWarning("ResetPassword: wrong code for {Email}.", request.Email);
            return Fail("Invalid or expired code.");
        }

        user.PasswordHash = PasswordHelper.Hash(request.NewPassword);
        user.ConfirmCode = null; 

        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ResetPassword: password updated for {Email}.", request.Email);
        return new ResponseModel<ResetPasswordCommandResponse>(
            new ResetPasswordCommandResponse { Success = true, Message = "Password reset successfully." });
    }

    private static ResponseModel<ResetPasswordCommandResponse> Fail(string message) =>
        new(new ResetPasswordCommandResponse { Success = false, Message = message });
}