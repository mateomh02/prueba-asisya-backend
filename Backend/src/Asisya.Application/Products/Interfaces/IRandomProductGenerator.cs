using Asisya.Domain;

namespace Asisya.Application.Products.Interfaces
{
    public interface IRandomProductGenerator
    {
        IEnumerable<Product> Generate(int count, IReadOnlyList<int> categoryIds);
    }
}
