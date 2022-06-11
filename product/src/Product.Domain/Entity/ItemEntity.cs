using System;

namespace Product.Domain.Entity
{
    internal class ItemEntity
    {
        public ItemEntity(string name, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
    }
}