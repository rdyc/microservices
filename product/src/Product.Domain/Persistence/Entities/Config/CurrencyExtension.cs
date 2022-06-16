namespace Product.Domain.Persistence.Entities
{
    internal static class CurrencyExtension
    {
        public static void SetUpdate(this CurrencyEntity entity, string name, string code, string symbol)
        {
            entity.Name = name;
            entity.Code = code;
            entity.Symbol = symbol;;
            entity.IsTransient = true;
        }
    }
}