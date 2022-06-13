using System.Collections.Generic;
using Product.Contract.Query;

namespace Product.WebApi.Model
{
    public class GetAllItemsRequest
    {
        public string Find { get; set; }
        public ItemField? FindBy { get; set; }
        public int? Index { get; set; }
        public int? Size { get; set; }
    }

    public class CriteriaRequest
    {
        public IList<CriteriaRequestValue> Values { get; set; }
    }

    public class CriteriaRequestValue
    {
        public string Value { get; set; }
        public ItemField Field { get; set; }
    }
}