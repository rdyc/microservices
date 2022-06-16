using System;
using MediatR;
using Product.Contract.Dtos;
using Product.Contract.Enums;

namespace Product.Contract.Commands
{
    public class AttributeCommand : IRequest<IAttributeDto>
    {
        public Guid? Id { get; protected set; }
        public string Name { get; protected set; }
        public AttributeType Type { get; protected set; }
        public string Unit { get; protected set; }
    }
}