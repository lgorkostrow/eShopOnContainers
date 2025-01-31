using Coupon.Application.IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Coupon.Application;

public static class ContainerExtension
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(typeof(ContainerExtension).Assembly);

        serviceCollection.AddTransient<IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>, OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler>();
        serviceCollection.AddTransient<IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>, OrderStatusChangedToPaidIntegrationEventHandler>();
        serviceCollection.AddTransient<IIntegrationEventHandler<BasketCheckoutWithBonusPaymentReceivedIntegrationEvent>, BasketCheckoutWithBonusPaymentReceivedIntegrationEventHandler>();
    }

    public static void RegisterApplicationIntegrationEvents(this IApplicationBuilder builder)
    {
        var eventBus = builder.ApplicationServices.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>>();
        eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>>();
        eventBus.Subscribe<BasketCheckoutWithBonusPaymentReceivedIntegrationEvent, IIntegrationEventHandler<BasketCheckoutWithBonusPaymentReceivedIntegrationEvent>>();
    }
}