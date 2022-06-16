using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Domain.Repositories;
using Product.Contract.Commands;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Dtos;
using Product.Domain.Dtos;

namespace Product.Domain.Handlers
{
    internal class DeleteAttributeHandler : IRequestHandler<DeleteAttributeCommand, IAttributeDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IAttributeDto> Handle(DeleteAttributeCommand request, CancellationToken cancellationToken)
        {
            var attribute = await unitOfWork.Config.GetAllAttributes()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            unitOfWork.Config.Delete(attribute);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<AttributeDto>(attribute);
        }
    }
}