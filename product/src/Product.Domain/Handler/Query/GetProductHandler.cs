using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Repositories;
using Product.Domain.Dtos;

namespace Product.Domain.Handler.Query
{
    internal class GetProductHandler : IRequestHandler<GetProductQuery, IProductDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Product.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id), cancellationToken);

            return mapper.Map<ProductDto>(result);
        }
    }
}