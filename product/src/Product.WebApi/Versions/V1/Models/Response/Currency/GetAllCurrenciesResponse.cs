using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The api response of currencies.
    /// </summary>
    public class GetAllCurrenciesResponse : ListPagedResponse<CurrencyResponse>
    {
    }
}