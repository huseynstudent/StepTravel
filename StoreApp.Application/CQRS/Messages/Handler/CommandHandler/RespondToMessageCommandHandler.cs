using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.CommandHandler;

class RespondToMessageCommandHandler : IRequestHandler<RespondToMessageCommandRequest, ResponseModel<RespondToMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RespondToMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<RespondToMessageCommandResponse>> Handle(RespondToMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.Id);
        if (message == null)
            return new ResponseModel<RespondToMessageCommandResponse>(null);

        message.Response    = request.Response;
        message.HasBeenRead = true;

        _unitOfWork.MessageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync();

        var response = new RespondToMessageCommandResponse
        {
            Id       = message.Id,
            Content  = message.Content,
            Response = message.Response,
        };

        return new ResponseModel<RespondToMessageCommandResponse>(response);
    }
}
