using Coupon.Domain.AggregatesModel;
using Coupon.Shared;
using Coupon.Shared.Exceptions;
using MediatR;

namespace Coupon.Application.Features.Coupon.Queries;

public class IsCouponValidQuery : BaseQuery<bool>
{
    public string Code { get; set; }
    
    public class IsCouponValidQueryHandler : IRequestHandler<IsCouponValidQuery, bool>
    {
        private readonly ICouponRepository _repository;

        public IsCouponValidQueryHandler(ICouponRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsCouponValidQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _repository.FindByCodeAsync(request.Code, cancellationToken);
            if (coupon is null)
            {
                throw new EntityNotFoundException($"Coupon with code {request.Code} was not found");
            }

            if (coupon.Consumed)
            {
                throw new DomainException($"Coupon with code {request.Code} is already used");
            }

            return true;
        }
    }
}