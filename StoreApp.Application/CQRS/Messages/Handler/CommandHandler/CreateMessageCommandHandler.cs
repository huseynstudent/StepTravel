using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Request;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Messages.Handler.CommandHandler;

class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest, ResponseModel<CreateMessageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<CreateMessageCommandResponse>> Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
    {
        var sender = await _unitOfWork.UserRepository.GetByIdAsync(request.SenderId);
        if (sender == null)
            return new ResponseModel<CreateMessageCommandResponse>(null);

        var message = new Message
        {
            SenderId  = request.SenderId,
            Content   = request.Content,
            ForAdmin  = request.ForAdmin,
            HasBeenRead = false,
        };

        await _unitOfWork.MessageRepository.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateMessageCommandResponse
        {
            Id             = message.Id,
            SenderId       = message.SenderId,
            SenderFullName = $"{sender.Name} {sender.Surname}",
            Content        = message.Content,
            ForAdmin       = message.ForAdmin,
            HasBeenRead    = message.HasBeenRead,
            Response       = message.Response,
        };

        return new ResponseModel<CreateMessageCommandResponse>(response);
    }
}
