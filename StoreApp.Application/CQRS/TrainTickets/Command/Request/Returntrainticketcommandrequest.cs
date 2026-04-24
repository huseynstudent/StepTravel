using MediatR;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.TrainTickets.Command.Request;

public class ReturnTrainTicketCommandRequest : IRequest<ResponseModel<ReturnTrainTicketCommandResponse>>
{
    public int Id { get; set; }
}

public class ReturnTrainTicketCommandResponse
{
    public int Id { get; set; }
    public string State { get; set; }
    public double Refund { get; set; }
    public string Message { get; set; }
}