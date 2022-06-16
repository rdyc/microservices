using System.Collections.Generic;

namespace Product.Domain.Persistence.Entities
{
    internal static class ProductExtension
    {
        public static void SetUpdate(this ProductEntity entity, string name, string description)
        {
            entity.Name = name;
            entity.Description = description;
            entity.IsTransient = true;
        }

        public static ProductAttributeEntity AddAttribute(this ProductEntity entity, int sequence, AttributeReferenceEntity attributeReference, string value)
        {
            if (entity.Attributes == null)
            {
                entity.Attributes = new List<ProductAttributeEntity>();
            }

            var attribute = new ProductAttributeEntity(entity, sequence, attributeReference, value);
            entity.Attributes.Add(attribute);

            return attribute;
        }

        public static void ClearAttribute(this ProductEntity entity)
        {
            if (entity.Attributes != null)
            {
                entity.Attributes.Clear();
            }
        }
    }
}