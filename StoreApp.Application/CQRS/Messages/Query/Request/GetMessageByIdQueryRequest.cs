using MediatR;
using StoreApp.Application.CQRS.Messages.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Query.Request;

public class GetMessageByIdQueryRequest : IRequest<ResponseModel<GetMessageByIdQueryResponse>>
{
    public int Id { get; set; }
}
