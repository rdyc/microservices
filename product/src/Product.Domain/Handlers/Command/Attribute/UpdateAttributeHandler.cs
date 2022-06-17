using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Dtos;
using Product.Domain.Repositories;
using Product.Domain.Dtos;
using Product.Contract.Commands;

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
            // get existing attribute
            var attribute = await unitOfWork.Config.GetAttributeAsync(request.Id.Value, cancellationToken);

            // set attribute modification
            unitOfWork.Config.UpdateAttribute(attribute, p => {
                p.Name = request.Name;
                p.Type = request.Type;
                p.Unit = request.Unit;
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<AttributeDto>(attribute);
        }
    }
}