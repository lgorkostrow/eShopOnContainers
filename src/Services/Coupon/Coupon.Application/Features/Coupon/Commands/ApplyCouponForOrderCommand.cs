using Coupon.Application.IntegrationEvents;
using Coupon.Domain.AggregatesModel;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;

namespace Coupon.Application.Features.Coupon.Commands;

public class ApplyCouponForOrderCommand : IRequest
{
    public int OrderId { get; set; }
    public string CouponCode { get; set; }

    public class ApplyCouponForOrderCommandHandler : IRequestHandler<ApplyCouponForOrderCommand>
    {
        private readonly ICouponRepository _repository;
        private readonly IEventBus _eventBus;

        public ApplyCouponForOrderCommandHandler(
            ICouponRepository repository,
            IEventBus eventBus
        )
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(ApplyCouponForOrderCommand command, CancellationToken cancellationToken)
        {
            var coupon = await _repository.FindByCodeAsync(command.CouponCode, cancellationToken);
            if (coupon is null || coupon.Consumed)
            {
                var couponRejectedIntegrationEvent = new OrderCouponRejectedIntegrationEvent(
                    command.OrderId,
                    command.CouponCode
                );
                _eventBus.Publish(couponRejectedIntegrationEvent);
                
                return Unit.Value;
            }

            await ProcessValidCoupon(command.OrderId, coupon, cancellationToken);
            
            return Unit.Value;
        }

        private async Task ProcessValidCoupon(int orderId, Domain.AggregatesModel.Coupon coupon, CancellationToken cancellationToken)
        {
            coupon.Consume(orderId);
            
            await _repository.Update(coupon, cancellationToken);

            var couponConfirmedIntegrationEvent = new OrderCouponConfirmedIntegrationEvent(
                orderId,
                coupon.Discount
            );
            _eventBus.Publish(couponConfirmedIntegrationEvent);
        }
    }
}