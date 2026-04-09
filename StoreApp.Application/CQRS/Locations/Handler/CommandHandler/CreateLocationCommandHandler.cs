using MediatR;
using StoreApp.Application.CQRS.Locations.Command.Request;
using StoreApp.Application.CQRS.Locations.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Locations.Handler.CommandHandler
{
    class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommandRequest, ResponseModel<CreateLocationCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateLocationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<CreateLocationCommandResponse>> Handle(CreateLocationCommandRequest request,CancellationToken cancellationToken)
        {
            var newLocation = new Location
            {
                Name = request.Name,
                CountryId = request.CountryId,
                DistanceToken = request.DistanceToken
            };

            await _unitOfWork.LocationRepository.AddAsync(newLocation);
            await _unitOfWork.SaveChangesAsync();
            var saved = await _unitOfWork.LocationRepository.GetByIdAsync(newLocation.Id);

            var response = new CreateLocationCommandResponse
            {
                Id = saved.Id,
                Name = saved.Name,
                DisplayName = $"{saved.Name},{saved.Country.Name}",
                CountryId = saved.CountryId,
                DistanceToken = saved.DistanceToken
            };

            return new ResponseModel<CreateLocationCommandResponse>(response);
        }
    }
}