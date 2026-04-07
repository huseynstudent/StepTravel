using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StoreApp.Application.CQRS.Auth.Handler.CommandHandler;
public class CreateExecutiveCommandHandler : IRequestHandler<CreateExecutiveCommandRequest, ResponseModel<CreateExecutiveCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateExecutiveCommandHandler> _logger;

    public CreateExecutiveCommandHandler(StoreAppDbContext db, IConfiguration configuration, ILogger<CreateExecutiveCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<CreateExecutiveCommandResponse>> Handle(CreateExecutiveCommandRequest request, CancellationToken cancellationToken)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
        if (emailExists)
        {
            _logger.LogWarning("Executive creation failed - Email already exists: {Email}", request.Email);
            return new ResponseModel<CreateExecutiveCommandResponse>(null);
        }

        var executive = new Domain.Entities.User
        {
            Name = request.Name ?? string.Empty,
            Surname = request.Surname ?? string.Empty,
            Email = request.Email ?? string.Empty,
            PasswordHash = PasswordHelper.Hash(request.Password ?? string.Empty),
            Birthday = DateOnly.FromDateTime(DateTime.UtcNow), // placeholder — not required for company accounts
            Fin = string.Empty,
            Role = UserType.Company,
            IsConfirmed = true, // admin-created accounts skip email verification
        };

        try
        {
            await _db.Users.AddAsync(executive, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Executive account created: {Email} (Role: Company)", executive.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database save failed when creating executive: {Email}", request.Email);
            return new ResponseModel<CreateExecutiveCommandResponse>(null);
        }

        // Fire-and-forget: send credentials email to the new executive
        _ = Task.Run(() =>
        {
            try
            {
                string sender = _configuration["EmailSettings:SenderEmail"];
                string appPass = _configuration["EmailSettings:AppPassword"];

                var emailService = new StoreApp.Application.Service.Email();
                emailService.Send(
                    sender,
                    appPass,
                    request.Email,
                    "Your StepTravel Executive Account",
                    $@"
                        <p>Welcome to <strong>StepTravel</strong>.</p>
                        <p>An executive (Company) account has been created for you by an administrator.</p>
                        <br/>
                        <table style='border-collapse:collapse;'>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Email</td>
                                <td style='padding:4px 0;'><strong>{request.Email}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Password</td>
                                <td style='padding:4px 0;'><strong>{request.Password}</strong></td></tr>
                        </table>
                        <br/>
                        <p style='color:#c0392b;font-size:13px;'>Please change your password after your first login.</p>
                    "
                );
                _logger.LogInformation("Credentials email sent to executive: {Email}", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send credentials email to {Email}: {Message}", request.Email, ex.Message);
            }
        });

        var responseData = new CreateExecutiveCommandResponse
        {
            Id = executive.Id,
            Name = executive.Name,
            Surname = executive.Surname,
            Email = executive.Email,
            Role = executive.Role.ToString(),
        };

        return new ResponseModel<CreateExecutiveCommandResponse>(responseData);
    }
}