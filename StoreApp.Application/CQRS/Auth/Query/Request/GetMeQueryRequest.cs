using MediatR;
using StoreApp.Application.CQRS.Auth.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Auth.Query.Request;

public class GetMeQueryRequest : IRequest<ResponseModel<GetMeQueryResponse>>
{
    public int UserId { get; set; }
    public string? ProfilePicture { get; set; }
}