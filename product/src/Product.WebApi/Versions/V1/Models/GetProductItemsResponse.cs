using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The api response of products.
    /// </summary>
    public class GetAllProductsResponse : ListPagedResponse<ProductResponse>
    {
    }
}