using MediatR;
using Microsoft.AspNetCore.Mvc;
using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;

namespace VatCalculatorAPI.Controllers;

/// <summary>
///  Controller for VAT calculation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VatCalculatorController(IMediator mediator) : ControllerBase
{
    /// <summary>
    ///  The calculate route method that handles the calculate request.
    /// </summary>
    /// <param name="request">The request object that is used as part of the request body.</param>
    /// <returns>The <see cref="ActionResult{T}"/> returns as response to the user request for the specific route.</returns>
    [HttpPost("calculate")]
    public async Task<ActionResult<VatCalculatorResponse>> Calculate([FromBody] VatCalculatorRequest request)
    { 
        var response = await mediator.Send(request);

        return Ok(response);
    }
}
