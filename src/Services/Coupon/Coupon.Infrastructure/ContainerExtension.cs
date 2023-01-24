using Coupon.Infrastructure.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coupon.Infrastructure;

public static class ContainerExtension
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<MongoConfiguration>(mongoConfiguration => 
            configuration.GetSection("MongoConfiguration").Bind(mongoConfiguration));

        serviceCollection.AddSingleton<CouponContext>();
    }
}