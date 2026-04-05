using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.Auth.Query.Request;
using StoreApp.Application.CQRS.Auth.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;

namespace StoreApp.Application.CQRS.Users.Handler;

public class GetMeQueryHandler : IRequestHandler<GetMeQueryRequest, ResponseModel<GetMeQueryResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly ILogger<GetMeQueryHandler> _logger;

    public GetMeQueryHandler(StoreAppDbContext db, ILogger<GetMeQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ResponseModel<GetMeQueryResponse>> Handle(GetMeQueryRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("GetMe: user {UserId} not found.", request.UserId);
            return new ResponseModel<GetMeQueryResponse>(null);
        }

        return new ResponseModel<GetMeQueryResponse>(new GetMeQueryResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Fin = user.Fin,
            Birthday = user.Birthday,
            Role = user.Role.ToString()
        });
    }
}