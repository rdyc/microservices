using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Domain.Repository;
using Product.Contract.Command;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dto;
using Product.Domain.Dto;

namespace Product.Domain.Handler.Query
{
    internal class DeleteItemHandler : IRequestHandler<DeleteItemCommand, IItemDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IItemDto> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var item = await unitOfWork.ProductItem.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            unitOfWork.ProductItem.Delete(item);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ItemDto>(item);
        }
    }
}