using MediatR;
using StoreApp.Application.CQRS.Countries.Command.Request;
using StoreApp.Application.CQRS.Countries.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Countries.Handler.CommandHandler
{
    class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommandRequest, ResponseModel<UpdateCountryCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCountryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<UpdateCountryCommandResponse>> Handle(UpdateCountryCommandRequest request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(request.Id);

            if (country != null)
            {
                country.Name = request.Name;

                _unitOfWork.CountryRepository.UpdateAsync(country);
                await _unitOfWork.SaveChangesAsync();

                var response = new UpdateCountryCommandResponse
                {
                    Id = country.Id,
                    Name = country.Name
                };

                return new ResponseModel<UpdateCountryCommandResponse>(response);
            }

            return new ResponseModel<UpdateCountryCommandResponse>(null);
        }
    }
}