using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.CommandHandler;

class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommandRequest, ResponseModel<MarkMessageAsReadCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkMessageAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<MarkMessageAsReadCommandResponse>> Handle(MarkMessageAsReadCommandRequest request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.Id);
        if (message == null)
            return new ResponseModel<MarkMessageAsReadCommandResponse>(null);

        message.HasBeenRead = true;

        _unitOfWork.MessageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync();

        var response = new MarkMessageAsReadCommandResponse
        {
            Id          = message.Id,
            HasBeenRead = message.HasBeenRead,
        };

        return new ResponseModel<MarkMessageAsReadCommandResponse>(response);
    }
}
