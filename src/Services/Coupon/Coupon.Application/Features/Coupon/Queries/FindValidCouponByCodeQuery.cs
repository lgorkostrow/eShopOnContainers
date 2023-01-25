using Coupon.Application.Features.Coupon.Models;
using Coupon.Domain.AggregatesModel;
using Coupon.Shared;
using Coupon.Shared.Exceptions;
using MediatR;

namespace Coupon.Application.Features.Coupon.Queries;

public class FindValidCouponByCode : BaseQuery<CouponDto>
{
    public string Code { get; set; }
    
    public class IsCouponValidQueryHandler : IRequestHandler<FindValidCouponByCode, CouponDto>
    {
        private readonly ICouponRepository _repository;

        public IsCouponValidQueryHandler(ICouponRepository repository)
        {
            _repository = repository;
        }

        public async Task<CouponDto> Handle(FindValidCouponByCode request, CancellationToken cancellationToken)
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

            return new CouponDto(
                coupon.Id,
                coupon.Code,
                coupon.Discount
            );
        }
    }
}