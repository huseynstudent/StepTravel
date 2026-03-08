using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Countries.Command.Request;
using StoreApp.Application.CQRS.Countries.Query.Request;
namespace StoreApp.WebApi.Controllers
{
    public class CountryController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateCountry(CreateCountryCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCountry(UpdateCountryCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(DeleteCountryCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCountries([FromQuery] GetAllCountryQueryRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            return Ok(await Sender.Send(new GetCountryByIdQueryRequest { Id = id }));
        }
    }
}