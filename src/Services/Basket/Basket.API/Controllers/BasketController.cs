namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly IBasketCheckoutService _basketCheckoutService;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        ILogger<BasketController> logger,
        IBasketRepository repository,
        IIdentityService identityService,
        IBasketCheckoutService basketCheckoutService    
    )
    {
        _logger = logger;
        _repository = repository;
        _identityService = identityService;
        _basketCheckoutService = basketCheckoutService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _repository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        return Ok(await _repository.UpdateBasketAsync(value));
    }

    [Route("checkout")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
    {
        var userId = _identityService.GetUserIdentity();
        var userName = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
        basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
            guid : basketCheckout.RequestId;

        var result = await _basketCheckoutService.AcceptBasketCheckout(userId, userName, basketCheckout);
        if (result == false)
        {
            return BadRequest();
        }

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await _repository.DeleteBasketAsync(id);
    }
}
