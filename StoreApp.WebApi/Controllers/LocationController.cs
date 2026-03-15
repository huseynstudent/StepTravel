using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Application.CQRS.Locations.Command.Request;
using StoreApp.Application.CQRS.Locations.Query.Request;
namespace StoreApp.WebApi.Controllers;
[AllowAnonymous]
public class LocationController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateLocation(CreateLocationCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpPut]
    public async Task<IActionResult> UpdateLocation(UpdateLocationCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteLocation(DeleteLocationCommandRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllLocations([FromQuery] GetAllLocationQueryRequest request)
    {
        return Ok(await Sender.Send(request));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById(int id)
    {
        return Ok(await Sender.Send(new GetLocationByIdQueryRequest { Id = id }));
    }
}