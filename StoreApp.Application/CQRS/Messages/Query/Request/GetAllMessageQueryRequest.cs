using MediatR;
using StoreApp.Application.CQRS.Messages.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Query.Request;

public class GetAllMessageQueryRequest : IRequest<Pagination<GetAllMessageQueryResponse>>
{
    public int Limit { get; set; } = 15;
    public int Page { get; set; } = 1;
    /// <summary>
    /// Optional: filter by target (true = Admin inbox, false = Executive inbox).
    /// Null returns everything.
    /// </summary>
    public bool? ForAdmin { get; set; }
}
