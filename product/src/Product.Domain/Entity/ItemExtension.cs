namespace Product.Domain.Entity
{
    internal static class ItemExtension
    {
        internal static void SetUpdate(this ItemEntity entity, string name, string description)
        {
            entity.Name = name;
            entity.Description = description;
        }
    }
}