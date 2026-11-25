using Asisya.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asisya.Application.Products.Interfaces
{
    public interface IProductBulkInserter
    {
        Task<int> BulkInsertAsync(IEnumerable<Product> products, CancellationToken ct = default);
    }
}
