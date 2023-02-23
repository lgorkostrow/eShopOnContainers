namespace Microsoft.eShopOnContainers.Services.Basket.API.Infrastructure.Repositories;

public class RedisBasketCheckoutRepository : IBasketCheckoutRepository
{
    private readonly ILogger<RedisBasketCheckoutRepository> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisBasketCheckoutRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<RedisBasketCheckoutRepository>();
        _redis = redis;
        _database = redis.GetDatabase();
    }
    
    public async Task<BasketCheckoutStoreModel> GetBasketCheckoutAsync(Guid id)
    {
        var data = await _database.StringGetAsync(id.ToString());

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<BasketCheckoutStoreModel>(data, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<Guid> SaveBasketCheckoutAsync(BasketCheckoutStoreModel basketCheckout)
    {
        var checkoutId = Guid.NewGuid();
        var created = await _database.StringSetAsync(checkoutId.ToString(), JsonSerializer.Serialize(basketCheckout));

        if (!created)
        {
            _logger.LogInformation("Problem occur persisting the item.");

            throw new Exception("Can't save basket checkout");
        }

        return checkoutId;
    }

    public async Task<bool> DeleteBasketCheckoutAsync(Guid id)
    {
        return await _database.KeyDeleteAsync(id.ToString());
    }
}