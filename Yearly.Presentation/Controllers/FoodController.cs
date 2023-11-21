using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Foods.Commands.FoodSimilarity;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Queries.DTORepositories;

namespace Yearly.Presentation.Controllers;

[Route("food")]
public class FoodController : ApiController
{
    private readonly IMapper _mapper;
    private readonly FoodSimilarityTableDTORepository _foodSimilarityTableDtoRepository;

    public FoodController(IMapper mapper, ISender mediator, FoodSimilarityTableDTORepository foodSimilarityTableDtoRepository)
        : base(mediator)
    {
        _mapper = mapper;
        _foodSimilarityTableDtoRepository = foodSimilarityTableDtoRepository;
    }

    [HttpGet("similarity-table")]
    public async Task<IActionResult> GetSimilarityTable()
    {
        var records = await _foodSimilarityTableDtoRepository.GetFoodSimilarityTableAsync();

        return Ok(new FoodSimilarityTableResponse(records));
    }

    [HttpPost("similarity-table/approve")]
    public Task<IActionResult> ApproveSimilarityRecord([FromForm] Guid ofFood, [FromForm] Guid forFood, [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async _ =>
            {
                var command = new ApproveSimilarityRecordCommand(new FoodId(ofFood), new FoodId(forFood));
                var result = await _mediator.Send(command);

                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.Admin);
    }

    [HttpPost("similarity-table/disapprove")]
    public Task<IActionResult> DisapproveSimilarityRecord([FromForm] Guid ofFood, [FromForm] Guid forFood, [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
           sessionCookie,
           async _ =>
           {
               var command = new DisapproveSimilarityRecordCommand(new FoodId(ofFood), new FoodId(forFood));
               await _mediator.Send(command);

               return Ok();
           },
           UserRole.Admin);
    }

    [HttpPost("set-alias")]
    public Task<IActionResult> SetFoodAlias([FromForm] Guid ofFood, [FromForm] Guid forFood, [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async _ =>
            {
                var command = new SetFoodAliasCommand(new FoodId(ofFood), new FoodId(forFood));
                var result = await _mediator.Send(command);

                return result.Match(
                    _ => Ok(), 
                    Problem);
            },
            UserRole.Admin);
    }
}