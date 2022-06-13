using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Dto;
using Product.Domain.Repository;
using Product.Domain.Dto;
using Product.Contract.Command;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entity;

namespace Product.Domain.Handler.Query
{
    internal class UpdateItemHandler : IRequestHandler<UpdateItemCommand, IItemDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UpdateItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await unitOfWork.ProductItem.GetAll()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            item.SetUpdate(request.Name, request.Description);

            unitOfWork.ProductItem.Update(item);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<ItemDto>(item);
        }
    }
}