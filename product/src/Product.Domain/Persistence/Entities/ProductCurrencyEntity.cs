using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class ProductCurrencyEntity : BaseEntity
    {
        public ProductCurrencyEntity(ProductEntity product, string name, string symbol)
        {
            ProductId = product.Id;
            Product = product;
            Name = name;
            Symbol = symbol;
            IsTransient = true;
        }

        public Guid ProductId { get; internal set; }
        public ProductEntity Product { get; internal set; }
        public string Name { get; internal set; }
        public string Symbol { get; internal set; }
    }
}