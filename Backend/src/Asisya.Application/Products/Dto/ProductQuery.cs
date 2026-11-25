
namespace Asisya.Application.Products.Dto
{
    public class ProductQuery
    {
        public int pageSize { get; set; } = 10;
        public int page { get; set; } = 1;
        public string? search { get; set; }
        public int? categoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
