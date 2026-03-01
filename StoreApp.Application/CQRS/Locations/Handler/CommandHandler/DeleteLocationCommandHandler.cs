using MediatR;
using StoreApp.Application.CQRS.Locations.Command.Request;
using StoreApp.Application.CQRS.Locations.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Locations.Handler.CommandHandler
{
    class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommandRequest, ResponseModel<DeleteLocationCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteLocationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<DeleteLocationCommandResponse>> Handle(DeleteLocationCommandRequest request, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(request.Id);

            if (location != null)
            {
                _unitOfWork.LocationRepository.DeleteAsync(request.Id);
                await _unitOfWork.SaveChangesAsync();

                var response = new DeleteLocationCommandResponse
                {
                    Id = location.Id,
                    Name = location.Name,
                    Country = location.Country,
                    CountryId = location.CountryId,
                    DistanceToken = location.DistanceToken
                };

                return new ResponseModel<DeleteLocationCommandResponse>(response);
            })
            
            return new ResponseModel<DeleteLocationCommandResponse>(null);
        }
    }
}