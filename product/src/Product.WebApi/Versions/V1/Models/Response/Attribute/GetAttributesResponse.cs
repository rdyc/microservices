using Shared.Infrastructure.Reponses;

namespace Product.WebApi.Versions.V1.Models
{
    /// <summary>
    /// The api response of attributes.
    /// </summary>
    public class GetAllAttributesResponse : ListPagedResponse<AttributeResponse>
    {
    }
}