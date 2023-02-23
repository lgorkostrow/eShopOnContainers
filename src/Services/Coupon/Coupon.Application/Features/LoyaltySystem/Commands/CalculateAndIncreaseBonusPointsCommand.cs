using Coupon.Domain.AggregatesModel;
using MediatR;

namespace Coupon.Application.Features.LoyaltySystem.Commands;

public class CalculateAndIncreaseBonusPointsCommand : IRequest
{
    public string UserId { get; set; }
    public decimal OrderTotalPrice { get; set; }

    public class CalculateAndIncreaseBonusPointsCommandHandler : IRequestHandler<CalculateAndIncreaseBonusPointsCommand>
    {
        private readonly IBenefitsRepository _benefitsRepository;

        public CalculateAndIncreaseBonusPointsCommandHandler(IBenefitsRepository benefitsRepository)
        {
            _benefitsRepository = benefitsRepository;
        }

        public async Task<Unit> Handle(CalculateAndIncreaseBonusPointsCommand command, CancellationToken cancellationToken)
        {
            var benefits = await _benefitsRepository.FindByUserIdAsync(command.UserId, cancellationToken);
            if (benefits is null)
            {
                benefits = new Benefits(
                    Guid.NewGuid(),
                    command.UserId,
                    CalculateBonusPoints(command.OrderTotalPrice)
                );
                await _benefitsRepository.CreateAsync(benefits, cancellationToken);
                
                return Unit.Value;
            }
            
            benefits.IncreaseBonusPoints(CalculateBonusPoints(command.OrderTotalPrice));
            await _benefitsRepository.UpdateAsync(benefits, cancellationToken);
            
            return Unit.Value;
        }

        private decimal CalculateBonusPoints(decimal orderTotalPrice)
        {
            return orderTotalPrice / 100 * Benefits.CashbackPercent;
        }
    }
}