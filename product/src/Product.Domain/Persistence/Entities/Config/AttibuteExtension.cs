using Product.Contract.Enums;

namespace Product.Domain.Persistence.Entities
{
    internal static class AttibuteExtension
    {
        public static void SetUpdate(this AttributeEntity entity, string name, AttributeType type, string unit)
        {
            entity.Name = name;
            entity.Type = type;
            entity.Unit = unit;;
            entity.IsTransient = true;
        }
    }
}