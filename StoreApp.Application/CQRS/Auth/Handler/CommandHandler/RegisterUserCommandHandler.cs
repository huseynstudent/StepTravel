namespace StoreApp.Application.CQRS.Auth.Handler.CommandHandler;
using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    public string RandomCode()
    {
        var Random = new Random();
        return $"{Random.Next(100000, 1000000)}";
    }

    public async Task<ResponseModel<RegisterUserCommandResponse>> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
    {
        var exists_e = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (exists_e != null)
        {
            _logger.LogInformation("Registration failed - email exists: {Email}", request.Email);
            return new ResponseModel<RegisterUserCommandResponse>(null);
        }

        string code = RandomCode();

        var adminEmail = _configuration["Admin:Email"];
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            string.Equals(request.Email?.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Attempt to register with reserved admin email: {Email}", request.Email);
            return new ResponseModel<RegisterUserCommandResponse>(null);
        }

        try
        {
            string senderEmail = _configuration["Email:SenderAddress"]!;
            string appPassword = _configuration["Email:AppPassword"]!;

            var emailService = new StoreApp.Application.Service.Email();

            bool sentEmail = emailService.Send(
                senderEmail,
                appPassword,
                request.Email,
                "Your Confirmation Code",
                $"Your confirmation code is : {code}"
            );

            if (!sentEmail)
            {
                _logger.LogError("\n\t\t Email could not be sent !");
                return new ResponseModel<RegisterUserCommandResponse>(null);
            }

            _logger.LogInformation("\n\t Confirmation code has been sent to your email !");
        }
        catch (Exception ex)
        {
            _logger.LogError("\n\t\t Email could not be sent ! Error: " + ex.Message);
            return new ResponseModel<RegisterUserCommandResponse>(null);
        }

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
            IsConfirmed = false
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New user registered: {Email} (Id: {Id})", user.Email, user.Id);

        var response = new RegisterUserCommandResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };

        return new ResponseModel<RegisterUserCommandResponse>(response);
    }
}