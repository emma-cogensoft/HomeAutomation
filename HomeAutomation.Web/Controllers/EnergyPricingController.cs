using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeAutomation.Application.EnergyPricing;

namespace HomeAutomation.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyPricingController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnergyPricingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<EnergyPricingResponse>> GetPricing(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetEnergyPricing(), cancellationToken);

            if (result == null)
                return NotFound("Energy pricing data not available");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { message = "Energy pricing service unavailable", error = ex.Message });
        }
    }
}
