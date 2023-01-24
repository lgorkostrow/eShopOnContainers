using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Coupon.Application;

public static class ContainerExtension
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(typeof(ContainerExtension).Assembly);
    }
}