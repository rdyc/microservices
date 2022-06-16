using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class GetCurrencyHandler : IRequestHandler<GetCurrencyQuery, ICurrencyDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCurrencyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ICurrencyDto> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Config.GetAllCurrencies()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken);

            return mapper.Map<CurrencyDto>(result);
        }
    }
}