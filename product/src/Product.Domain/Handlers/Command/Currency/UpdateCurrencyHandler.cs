using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class UpdateCurrencyHandler : IRequestHandler<UpdateCurrencyCommand, ICurrencyDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UpdateCurrencyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ICurrencyDto> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            // get existing currency
            var currency = await unitOfWork.Config.GetCurrencyAsync(request.Id.Value, cancellationToken);

            // set attribute modification
            unitOfWork.Config.UpdateCurrency(currency, p =>
            {
                p.Name = request.Name;
                p.Code = request.Code;
                p.Symbol = request.Symbol;
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<CurrencyDto>(currency);
        }
    }
}