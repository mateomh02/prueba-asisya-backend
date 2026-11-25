
using Asisya.Domain.Common;
using Asisya.Domain.Entities;

namespace Asisya.Domain
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public int CategoryId { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public short ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public ProductSource Source { get; set; } = ProductSource.Manual;

        public Supplier? Supplier { get; set; }
        public Category? Category { get; set; }
    }
}
