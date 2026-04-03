using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;

namespace StoreApp.Application.CQRS.Auth.Handler.CommandHandler;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, ResponseModel<AuthResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(StoreAppDbContext db, IConfiguration configuration, ILogger<LoginUserCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<AuthResponse>> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return new ResponseModel<AuthResponse>(null);

        var adminEmail = _configuration["Admin:Email"];
        var adminPassword = _configuration["Admin:Password"];

        // Admin girişi
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            string.Equals(request.Email.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            if (PasswordHelper.Verify(request.Password, PasswordHelper.Hash(adminPassword)))
            {
                var token = GenerateJwtToken(adminEmail, "Admin", 0);
                var adminAuth = new AuthResponse { Email = adminEmail, Role = "Admin", Token = token };

                return new ResponseModel<AuthResponse>(adminAuth);
            }
            return new ResponseModel<AuthResponse>(null);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            return new ResponseModel<AuthResponse>(null);
        }

        if (!user.IsConfirmed)
        {
            _logger.LogWarning("Login failed - email not confirmed: {Email}", request.Email);
            return new ResponseModel<AuthResponse>(null);
        }

        if (!PasswordHelper.Verify(request.Password ?? string.Empty, user.PasswordHash))
        {
            _logger.LogWarning("Login failed - invalid password for {Email}", request.Email);
            return new ResponseModel<AuthResponse>(null);
        }

        var jwt = GenerateJwtToken(user.Email, user.Role.ToString(), user.Id);

        var response = new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = jwt
        };

        return new ResponseModel<AuthResponse>(response);
    }

    private string GenerateJwtToken(string email, string role, int? userId)
    {
        var secret = _configuration["JWT:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("JWT:Secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("role", role)
        };

        if (userId.HasValue)
            claims.Add(new Claim("uid", userId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}