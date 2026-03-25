using MediatR;
using StoreApp.Application.CQRS.Locations.Command.Request;
using StoreApp.Application.CQRS.Locations.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Locations.Handler.CommandHandler
{
    class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommandRequest, ResponseModel<UpdateLocationCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateLocationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<UpdateLocationCommandResponse>> Handle(UpdateLocationCommandRequest request, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(request.Id);

            if (location != null)
            {
                location.Name = request.Name;
                location.Country = request.Country;
                location.CountryId = request.CountryId;
                location.DistanceToken = request.DistanceToken;

                _unitOfWork.LocationRepository.Update(location);
                await _unitOfWork.SaveChangesAsync();

                var response = new UpdateLocationCommandResponse
                {
                    Id = location.Id,
                    Name = location.Name,
                    Country = location.Country,
                    CountryId = location.CountryId,
                    DistanceToken = location.DistanceToken
                };

                return new ResponseModel<UpdateLocationCommandResponse>(response);
            }

            return new ResponseModel<UpdateLocationCommandResponse>(null);
        }
    }
}