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
        // Admin-in id-si 0 ola bilər (hardcoded), DB-də record olmaya bilər
        var sender = request.SenderId > 0
            ? await _unitOfWork.UserRepository.GetByIdAsync(request.SenderId)
            : null;

        var message = new Message
        {
            SenderId = request.SenderId > 0 ? (int?)request.SenderId : null,
            Content = request.Content,
            ForAdmin = request.ForAdmin,
            HasBeenRead = false,
        };

        await _unitOfWork.MessageRepository.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateMessageCommandResponse
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderFullName = sender != null ? $"{sender.Name} {sender.Surname}" : "Admin",
            Content = message.Content,
            ForAdmin = message.ForAdmin,
            HasBeenRead = message.HasBeenRead,
            Response = message.Response,
        };

        return new ResponseModel<CreateMessageCommandResponse>(response);
    }
}