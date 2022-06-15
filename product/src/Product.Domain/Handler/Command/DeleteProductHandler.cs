using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Domain.Repositories;
using Product.Contract.Commands;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Domain.Dtos;

namespace Product.Domain.Handler.Query
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
            var product = await unitOfWork.Product.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            unitOfWork.Product.Delete(product);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ProductDto>(product);
        }
    }
}