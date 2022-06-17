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
    internal class DeleteProductHandler : IRequestHandler<DeleteProductCommand, IProductDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IProductDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // get existing product
            var product = await unitOfWork.Product.GetDetailAsync(request.Id.Value, cancellationToken);

            unitOfWork.Product.Update(product, p => p.IsDeleted = true);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ProductDto>(product);
        }
    }
}