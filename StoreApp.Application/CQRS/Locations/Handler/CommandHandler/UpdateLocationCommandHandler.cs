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
                location.CountryId = request.CountryId;
                location.DistanceToken = request.DistanceToken;

                _unitOfWork.LocationRepository.Update(location);
                await _unitOfWork.SaveChangesAsync();

                var saved = await _unitOfWork.LocationRepository.GetByIdAsync(location.Id);

                return new ResponseModel<UpdateLocationCommandResponse>(new UpdateLocationCommandResponse
                {
                    Id = saved.Id,
                    Name = saved.Name,
                    CountryId = saved.CountryId,
                    DistanceToken = saved.DistanceToken,
                    DisplayName = $"{saved.Country.Name}, {saved.Name}"
                });
            }

            return new ResponseModel<UpdateLocationCommandResponse>(null);
        }
    }
}