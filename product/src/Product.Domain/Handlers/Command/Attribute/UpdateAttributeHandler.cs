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

namespace Product.Domain.Handlers
{
    internal class UpdateAttributeHandler : IRequestHandler<UpdateAttributeCommand, IAttributeDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UpdateAttributeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IAttributeDto> Handle(UpdateAttributeCommand request, CancellationToken cancellationToken)
        {
            var attribute = await unitOfWork.Config.GetAllAttributes()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            attribute.SetUpdate(request.Name, request.Type, request.Unit);

            unitOfWork.Config.Update(attribute);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<AttributeDto>(attribute);
        }
    }
}