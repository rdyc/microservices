using System.Collections.Generic;

namespace Product.Domain.Persistence.Entities
{
    internal static class ProductExtension
    {
        internal static void SetUpdate(this ProductEntity entity, string name, string description)
        {
            entity.Name = name;
            entity.Description = description;
            entity.IsTransient = true;
        }

        internal static ProductAttributeEntity AddAttribute(this ProductEntity entity, int sequence, string name, AttributeType type, string value, string unit)
        {
            if (entity.Attributes == null)
            {
                entity.Attributes = new List<ProductAttributeEntity>();
            }

            var attribute = new ProductAttributeEntity(entity, sequence, name, type, value, unit);
            entity.Attributes.Add(attribute);

            return attribute;
        }

        internal static void ClearAttribute(this ProductEntity entity)
        {
            if (entity.Attributes != null)
            {
                entity.Attributes.Clear();
            }
        }
    }
}