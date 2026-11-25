using System.ComponentModel.DataAnnotations;

namespace Asisya.Application.Products.Dto
{
    public class BulkCreateProductsRequest
    {
        [Range(1, 1_000_000)]
        public int Count { get; set; } = 100_000;
    }
}
