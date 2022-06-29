using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Domain.Dtos;
using Product.Domain.Repositories;

namespace Product.Domain.Handlers;

internal class CreateProductHandler : IRequestHandler<CreateProductCommand, IProductDto>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // get currency from config
        var currency = await unitOfWork.Config.GetCurrencyAsync(request.CurrencyId, cancellationToken);
        
        // get or create currency reference
        var productCurrency = await unitOfWork.Reference.GetOrCreateCurrencyAsync(currency.Id, currency.Name, currency.Code, currency.Symbol, cancellationToken);

        // create a new product
        var product = unitOfWork.Product.Create(request.Name, request.Description, productCurrency, request.Price);

        // save all changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}