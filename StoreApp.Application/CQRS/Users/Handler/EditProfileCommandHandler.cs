using StoreApp.Application.CQRS.Users.Command.Request;
using StoreApp.Application.CQRS.Users.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Users.Handler;
public class EditProfileCommandHandler
    : IRequestHandler<EditProfileCommandRequest, ResponseModel<EditProfileCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EditProfileCommandHandler> _logger;

    public EditProfileCommandHandler(StoreAppDbContext db, ILogger<EditProfileCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ResponseModel<EditProfileCommandResponse>> Handle(
        EditProfileCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("EditProfile: user {UserId} not found.", request.UserId);
            return new ResponseModel<EditProfileCommandResponse>(null);
        }
        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            bool emailTaken = await _db.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId && !u.IsDeleted,
                          cancellationToken);
            if (emailTaken)
            {
                _logger.LogWarning("EditProfile: email {Email} already in use.", request.Email);
                return new ResponseModel<EditProfileCommandResponse>(null);
            }
        }

        user.Name = request.Name;
        user.Surname = request.Surname;
        user.Email = request.Email;
        user.Birthday = request.Birthday;
        user.Fin = request.Fin;

        _unitOfWork.UserRepository.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("EditProfile: user {UserId} updated.", user.Id);

        return new ResponseModel<EditProfileCommandResponse>(new EditProfileCommandResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Birthday = user.Birthday,
            Fin = user.Fin,
        });
    }
}