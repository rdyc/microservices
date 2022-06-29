using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers;

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
        // get currency from config
        var currency = await unitOfWork.Config.GetCurrencyAsync(request.CurrencyId, cancellationToken);

        // get or create currency reference
        var productCurrency = await unitOfWork.Reference.GetOrCreateCurrencyAsync(currency.Id, currency.Name, currency.Code, currency.Symbol, cancellationToken);

        // get existing product
        var product = await unitOfWork.Product.GetDetailAsync(request.Id.Value, cancellationToken);

        // set product modification
        unitOfWork.Product.Update(product, p =>
        {
            p.Name = request.Name;
            p.Description = request.Description;
            p.Currency = productCurrency;
            p.Price = request.Price;
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}