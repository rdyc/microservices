using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Dto;
using Product.Domain.Repository;
using Product.Domain.Dto;
using Product.Contract.Command;
using Product.Domain.Entity;

namespace Product.Domain.Handler.Query
{
    internal class CreateItemHandler : IRequestHandler<CreateItemCommand, IItemDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var item = new ItemEntity(request.Name, request.Description);

            unitOfWork.ProductItem.Add(item);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ItemDto>(item);
        }
    }
}