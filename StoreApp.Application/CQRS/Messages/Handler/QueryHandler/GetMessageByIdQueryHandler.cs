using MediatR;
using StoreApp.Application.CQRS.Messages.Query.Request;
using StoreApp.Application.CQRS.Messages.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.QueryHandler;

class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQueryRequest, ResponseModel<GetMessageByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMessageByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<GetMessageByIdQueryResponse>> Handle(GetMessageByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.Id);
        if (message == null)
            return new ResponseModel<GetMessageByIdQueryResponse>(null);

        var response = new GetMessageByIdQueryResponse
        {
            Id             = message.Id,
            SenderId       = message.SenderId,
            SenderFullName = message.Sender != null ? $"{message.Sender.Name} {message.Sender.Surname}" : string.Empty,
            SenderEmail    = message.Sender?.Email ?? string.Empty,
            Content        = message.Content,
            ForAdmin       = message.ForAdmin,
            HasBeenRead    = message.HasBeenRead,
            Response       = message.Response,
            CreatedDate    = message.CreatedDate,
        };

        return new ResponseModel<GetMessageByIdQueryResponse>(response);
    }
}
