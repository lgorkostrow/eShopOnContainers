using Coupon.Application.IntegrationEvents;
using Coupon.Domain.AggregatesModel;
using Coupon.Infrastructure.Mongo;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace Coupon.Application.Features.Coupon.Commands;

public class CreateCouponCodeFromBonusPointsCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public Guid BasketCheckoutId { get; set; }
    public decimal BonusPointsAmount { get; set; }
    
    public class CreateCouponCodeFromBonusPointsCommandHandler : IRequestHandler<CreateCouponCodeFromBonusPointsCommand, bool>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IBenefitsRepository _benefitsRepository;
        private readonly CouponContext _couponContext;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CreateCouponCodeFromBonusPointsCommandHandler> _logger;

        public CreateCouponCodeFromBonusPointsCommandHandler(
            ICouponRepository couponRepository,
            IBenefitsRepository benefitsRepository,
            CouponContext couponContext,
            IEventBus eventBus,
            ILogger<CreateCouponCodeFromBonusPointsCommandHandler> logger
        )
        {
            _couponRepository = couponRepository;
            _benefitsRepository = benefitsRepository;
            _couponContext = couponContext;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateCouponCodeFromBonusPointsCommand command, CancellationToken cancellationToken)
        {
            var benefits = await _benefitsRepository.FindByUserIdAsync(command.UserId, cancellationToken);
            if (benefits is null)
            {
                _eventBus.Publish(
                    new CouponCreationForBasketCheckoutFailedIntegrationEvent(command.UserId, command.UserName, command.BasketCheckoutId)
                );
                
                return false;
            }

            try
            {
                await _couponContext.BeginTransactionAsync(cancellationToken);

                benefits.DecreaseBonusPoints(command.BonusPointsAmount);
                var userPersonalCoupon = new Domain.AggregatesModel.Coupon(
                    $"DISC:{Guid.NewGuid()}-{command.UserName}-{command.BonusPointsAmount}",
                    command.BonusPointsAmount,
                    Guid.Parse(command.UserId)
                );
                
                await _couponRepository.CreateAsync(userPersonalCoupon, cancellationToken);
                await _benefitsRepository.UpdateAsync(benefits, cancellationToken);
                
                await _couponContext.CommitTransactionAsync(cancellationToken);

                var eventMessage = new CouponForBasketCheckoutCreatedIntegrationEvent(
                    command.UserId, 
                    command.UserName, 
                    command.BasketCheckoutId,
                    userPersonalCoupon.Code,
                    userPersonalCoupon.Discount
                );
                
                _eventBus.Publish(eventMessage);
            }
            catch
            {
                await _couponContext.RollbackTransaction(cancellationToken);
                _eventBus.Publish(
                    new CouponCreationForBasketCheckoutFailedIntegrationEvent(command.UserId, command.UserName, command.BasketCheckoutId)
                );
                
                throw;
            }

            return true;
        }
    }
}