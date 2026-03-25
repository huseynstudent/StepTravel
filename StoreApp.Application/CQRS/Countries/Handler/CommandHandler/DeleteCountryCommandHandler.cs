using MediatR;
using StoreApp.Application.CQRS.Countries.Command.Request;
using StoreApp.Application.CQRS.Countries.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Countries.Handler.CommandHandler
{
    class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommandRequest, ResponseModel<DeleteCountryCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCountryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<DeleteCountryCommandResponse>> Handle(DeleteCountryCommandRequest request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(request.Id);

            if(country != null)
            {
                await _unitOfWork.CountryRepository.DeleteAsync(request.Id);
                await _unitOfWork.SaveChangesAsync();

                var response = new DeleteCountryCommandResponse
                {
                    Id = country.Id,
                    Name = country.Name
                };

                return new ResponseModel<DeleteCountryCommandResponse>(response);
            }

            return new ResponseModel<DeleteCountryCommandResponse>(null);
        }
    }
}