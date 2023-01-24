using Coupon.Application.Features.Coupon.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Coupon.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CouponController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{code}/check-validity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> IsCouponValidAsync([FromRoute] IsCouponValidQuery query)
    {
        await _mediator.Send(query);

        return NoContent();
    }
}