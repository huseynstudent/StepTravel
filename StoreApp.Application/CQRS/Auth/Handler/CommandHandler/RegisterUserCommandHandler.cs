using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;

namespace StoreApp.Application.CQRS.Auth.Handler.CommandHandler;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommandRequest, ResponseModel<RegisterUserCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(StoreAppDbContext db, IConfiguration configuration, ILogger<RegisterUserCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    private string GenerateRandomCode() => new Random().Next(100000, 1000000).ToString();

    public async Task<ResponseModel<RegisterUserCommandResponse>> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _db.Users.AnyAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
        if (existingUser)
        {
            _logger.LogWarning("Registration failed - Email already exists: {Email}", request.Email);
            return new ResponseModel<RegisterUserCommandResponse>(null);
        }

        string code = GenerateRandomCode();

        var user = new User
        {
            Name = request.Name ?? string.Empty,
            Surname = request.Surname ?? string.Empty,
            Email = request.Email ?? string.Empty,
            PasswordHash = PasswordHelper.Hash(request.Password ?? string.Empty),
            Birthday = request.Birthday,
            Fin = request.Fin ?? string.Empty,
            Role = UserType.Customer,
            ConfirmCode = code,
            IsConfirmed = false,
        };

        try
        {
            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User successfully saved to database: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database save failed for: {Email}", request.Email);
            return new ResponseModel<RegisterUserCommandResponse>(null);
        }

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
                    "Verification Code",
                    $"Your verification code is: <b>{code}</b>"
                );
                _logger.LogInformation("Email sent successfully to: {Email}", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Critical Email Error: {Message}", ex.Message);
            }
        });
        var responseData = new RegisterUserCommandResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };

        return new ResponseModel<RegisterUserCommandResponse>(responseData);
    }
}