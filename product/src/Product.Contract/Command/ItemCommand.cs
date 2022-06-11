using System;
using MediatR;
using Product.Contract.Dto;

namespace Product.Contract.Command
{
    public class ItemCommand : IRequest<IItemDto>
    {
        public Guid? Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
    }
}