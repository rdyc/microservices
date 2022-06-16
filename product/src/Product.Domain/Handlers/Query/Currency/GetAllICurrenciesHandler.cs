using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class GetAllCurrenciesHandler : IRequestHandler<GetAllCurrenciesQuery, IEnumerable<ICurrencyDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllCurrenciesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ICurrencyDto>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Config.GetAllCurrencies()
                .WithCriteria(request.Criteria)
                .WithOrdered(request.Ordered)
                .WithPaged(request.Paged)
                .ToListAsync(cancellationToken);

            return mapper.Map<IEnumerable<ICurrencyDto>>(result);
        }
    }
}