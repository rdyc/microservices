using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Persistence.Entities;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class CreateProductHandler : IRequestHandler<CreateProductCommand, IProductDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductEntity(request.Name, request.Description);

            unitOfWork.Product.Add(product);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ProductDto>(product);
        }
    }
}