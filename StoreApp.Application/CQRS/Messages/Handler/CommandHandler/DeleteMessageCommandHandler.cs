using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.CommandHandler;

class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommandRequest, ResponseModel<DeleteMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<DeleteMessageCommandResponse>> Handle(DeleteMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.Id);
        if (message == null)
            return new ResponseModel<DeleteMessageCommandResponse>(null);

        await _unitOfWork.MessageRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteMessageCommandResponse
        {
            Id      = message.Id,
            Content = message.Content,
        };

        return new ResponseModel<DeleteMessageCommandResponse>(response);
    }
}
