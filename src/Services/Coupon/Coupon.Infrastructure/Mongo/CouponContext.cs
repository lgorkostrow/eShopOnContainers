using Coupon.Domain.AggregatesModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public class CouponContext
{
    public IMongoCollection<Domain.AggregatesModel.Coupon> Coupons => _database.GetCollection<Domain.AggregatesModel.Coupon>("CouponCollection");
    public IMongoCollection<Benefits> Benefits => _database.GetCollection<Benefits>("BenefitsCollection");

    private IMongoClient _client;
    private readonly IMongoDatabase _database = null;
    private IClientSessionHandle? _currentTransaction = null;

    public CouponContext(IOptions<MongoConfiguration> settings, ILogger<CouponContext> logger)
    {
        logger.LogInformation($"--- Connecting to {settings.Value.ConnectionString} ---");
        
        _client = new MongoClient(settings.Value.ConnectionString);
        if (_client is null)
        {
            throw new MongoConfigurationException("Cannot connect to the database. The connection string is not valid or the database is not accessible");
        }

        _database = _client.GetDatabase(settings.Value.CouponMongoDatabase);
        
        ConfigureModels();
    }

    public async Task<IClientSessionHandle?> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction != null)
        {
            return null;
        }
        
        _currentTransaction = await _client.StartSessionAsync(cancellationToken: cancellationToken);
        _currentTransaction.StartTransaction();
        
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
        {
            throw new ArgumentNullException(nameof(_currentTransaction));
        }

        try
        {
            await _currentTransaction.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken)
    {
        try
        {
            await _currentTransaction?.AbortTransactionAsync(cancellationToken);
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    private void ConfigureModels()
    {
        BsonClassMap.RegisterClassMap<Domain.AggregatesModel.Coupon>(cm => 
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
        });
        
        BsonClassMap.RegisterClassMap<Benefits>(cm => 
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
        });
    }
}