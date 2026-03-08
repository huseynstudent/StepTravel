using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
namespace StoreApp.WebApi.Controllers
{
    public class PlaneTicketController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTrainTicketCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTicket(UpdateTrainTicketCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(DeleteTrainTicketCommandRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] GetAllTrainTicketQueryRequest request)
        {
            return Ok(await Sender.Send(request));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            return Ok(await Sender.Send(new GetTrainTicketByIdQueryRequest { Id = id }));
        }
    }
}