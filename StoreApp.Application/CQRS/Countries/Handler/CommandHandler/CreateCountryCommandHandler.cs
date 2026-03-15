using MediatR;
using StoreApp.Application.CQRS.Countries.Command.Request;
using StoreApp.Application.CQRS.Countries.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Countries.Handler.CommandHandler
{
    class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommandRequest,ResponseModel<CreateCountryCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCountryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<CreateCountryCommandResponse>> Handle(CreateCountryCommandRequest request, CancellationToken cancellationToken)
        {
            var country = new Country
            {

                Name = request.Name
            };

            await _unitOfWork.CountryRepository.AddAsync(country);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateCountryCommandResponse
            {
                Id = country.Id,
                Name = country.Name
            };

            return new ResponseModel<CreateCountryCommandResponse>(response);
        }
    }
}