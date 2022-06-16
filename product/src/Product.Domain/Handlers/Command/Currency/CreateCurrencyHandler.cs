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
    internal class CreateCurrencyHandler : IRequestHandler<CreateCurrencyCommand, ICurrencyDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateCurrencyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ICurrencyDto> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = new CurrencyEntity(request.Name, request.Code, request.Symbol);

            unitOfWork.Config.Add(currency);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<CurrencyDto>(currency);
        }
    }
}