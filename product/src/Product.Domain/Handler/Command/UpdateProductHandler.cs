using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Dtos;
using Product.Domain.Repositories;
using Product.Domain.Dtos;
using Product.Contract.Commands;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Persistence.Entities;

namespace Product.Domain.Handler.Query
{
    internal class UpdateProductHandler : IRequestHandler<UpdateProductCommand, IProductDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Product.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            product.SetUpdate(request.Name, request.Description);

            unitOfWork.Product.Update(product);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ProductDto>(product);
        }
    }
}