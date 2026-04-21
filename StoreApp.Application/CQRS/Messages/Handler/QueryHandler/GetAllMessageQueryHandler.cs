using MediatR;
using StoreApp.Application.CQRS.Messages.Query.Request;
using StoreApp.Application.CQRS.Messages.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.QueryHandler;

class GetAllMessageQueryHandler : IRequestHandler<GetAllMessageQueryRequest, Pagination<GetAllMessageQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMessageQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<GetAllMessageQueryResponse>> Handle(GetAllMessageQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.MessageRepository.GetAll();

        if (request.ForAdmin.HasValue)
            query = query.Where(m => m.ForAdmin == request.ForAdmin.Value);

        var totalDataCount = query.Count();

        var paged = query
            .OrderByDescending(m => m.CreatedDate)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .ToList();

        var response = paged.Select(m => new GetAllMessageQueryResponse
        {
            Id             = m.Id,
            SenderId       = m.SenderId,
            SenderFullName = m.Sender != null ? $"{m.Sender.Name} {m.Sender.Surname}" : string.Empty,
            Content        = m.Content,
            ForAdmin       = m.ForAdmin,
            HasBeenRead    = m.HasBeenRead,
            Response       = m.Response,
            CreatedDate    = m.CreatedDate,
        }).ToList();

        return new Pagination<GetAllMessageQueryResponse>(response, totalDataCount, request.Page, request.Limit);
    }
}
