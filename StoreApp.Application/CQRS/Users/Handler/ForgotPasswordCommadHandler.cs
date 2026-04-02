using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.User.Command.Request;
using StoreApp.Application.CQRS.User.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;

namespace StoreApp.Application.CQRS.User.Handler.CommandHandler;

public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommandRequest, ResponseModel<ForgotPasswordCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    private static string GenerateRandomCode() => new Random().Next(100000, 1000000).ToString();

    public async Task<ResponseModel<ForgotPasswordCommandResponse>> Handle(
        ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("ForgotPassword: email {Email} not found.", request.Email);
            return Ok();
        }

        if (!user.IsConfirmed)
        {
            _logger.LogWarning("ForgotPassword: email {Email} is not confirmed.", request.Email);
            return Ok();
        }

        string code = GenerateRandomCode();
        user.ConfirmCode = code;
        await _db.SaveChangesAsync(cancellationToken);

        _ = Task.Run(() =>
        {
            try
            {
                string sender = _configuration["EmailSettings:SenderEmail"];
                string password = _configuration["EmailSettings:AppPassword"];

                var emailService = new StoreApp.Application.Service.Email();
                emailService.Send(
                    sender,
                    password,
                    request.Email,
                    "Password Reset Code",
                    $"Your password reset code is: <b>{code}</b><br/>It will be used to set a new password."
                );

                _logger.LogInformation("ForgotPassword: reset code emailed to {Email}.", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError("ForgotPassword email error: {Message}", ex.Message);
            }
        });

        return Ok();
    }

    private static ResponseModel<ForgotPasswordCommandResponse> Ok() =>
        new(new ForgotPasswordCommandResponse
        {
            Success = true,
            Message = "If that email exists, a reset code has been sent."
        });
}