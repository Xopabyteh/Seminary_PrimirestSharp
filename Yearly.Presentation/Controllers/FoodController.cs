using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Foods.Commands.FoodSimilarity;
using Yearly.Application.FoodSimilarityTable.Queries;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("api/food")]
public class FoodController : ApiController
{
    public FoodController(ISender mediator)
        : base(mediator)
    {

    }

    [HttpGet("similarity-table")]
    public async Task<IActionResult> GetSimilarityTable()
    {
        var records = await _mediator.Send(new GetFoodSimilarityTableDTOsQuery());

        return Ok(new FoodSimilarityTableResponse(records));
    }

    [HttpPost("similarity-table/approve")]
    public Task<IActionResult> ApproveSimilarityRecord(
        [FromForm] Guid ofFood,
        [FromForm] Guid forFood)
    {
        return PerformAuthorizedActionAsync(
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
    public Task<IActionResult> DisapproveSimilarityRecord(
        [FromForm] Guid ofFood,
        [FromForm] Guid forFood)
    {
        return PerformAuthorizedActionAsync(
           async _ =>
           {
               var command = new DisapproveSimilarityRecordCommand(new FoodId(ofFood), new FoodId(forFood));
               await _mediator.Send(command);

               return Ok();
           },
           UserRole.Admin);
    }

    [HttpPost("set-alias")]
    public Task<IActionResult> SetFoodAlias(
        [FromForm] Guid ofFood,
        [FromForm] Guid forFood)
    {
        return PerformAuthorizedActionAsync(
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