using System;

namespace Product.WebApi.Model
{
    public class ItemRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}