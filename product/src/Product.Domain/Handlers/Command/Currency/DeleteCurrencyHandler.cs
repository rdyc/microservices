using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers
{
    internal class DeleteCurrencyHandler : IRequestHandler<DeleteCurrencyCommand, ICurrencyDto>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteCurrencyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ICurrencyDto> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var currency = await unitOfWork.Config.GetAllCurrencies()
                .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), cancellationToken);

            unitOfWork.Config.Delete(currency);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<CurrencyDto>(currency);
        }
    }
}