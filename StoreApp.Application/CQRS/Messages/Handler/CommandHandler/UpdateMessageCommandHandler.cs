using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.CommandHandler;

class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommandRequest, ResponseModel<UpdateMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<UpdateMessageCommandResponse>> Handle(UpdateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.Id);
        if (message == null)
            return new ResponseModel<UpdateMessageCommandResponse>(null);

        message.Content  = request.Content;
        message.ForAdmin = request.ForAdmin;

        _unitOfWork.MessageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateMessageCommandResponse
        {
            Id       = message.Id,
            Content  = message.Content,
            ForAdmin = message.ForAdmin,
        };

        return new ResponseModel<UpdateMessageCommandResponse>(response);
    }
}
