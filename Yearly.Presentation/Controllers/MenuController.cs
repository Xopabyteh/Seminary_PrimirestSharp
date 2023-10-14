using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Menus.Commands;
using Yearly.Application.Menus.Queries;
using Yearly.Contracts.Menu;

namespace Yearly.Presentation.Controllers;

public class MenuController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MenuController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("menu")]
    public async Task<IActionResult> GetMenusThisWeek([FromQuery] MenusThisWeekRequest request)
    {
        var result = await _mediator.Send(new MenusThisWeekQuery());

        return result.Match(
            menus => Ok(_mapper.Map<MenusThisWeekResponse>(menus)),
            Problem);
    }

    [HttpPost("menu/force")]
    public async Task<IActionResult> ForcePersistMenusFromExternalService(string sessionCookie)
    {
        //Todo: authentication

        var result = await _mediator.Send(new PersistMenuForThisWeekCommand());
        return result.Match(
            value => Ok(),
            Problem);
    }
}