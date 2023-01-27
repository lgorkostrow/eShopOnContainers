using Coupon.Application.Features.Coupon.Models;
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
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDto>> FindValidCouponByCodeAsync([FromRoute] FindValidCouponByCode query)
    {
        return await _mediator.Send(query);
    }
}