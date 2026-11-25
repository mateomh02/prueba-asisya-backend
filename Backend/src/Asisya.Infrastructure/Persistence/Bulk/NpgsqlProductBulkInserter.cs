using System.Data;
using Asisya.Application.Products.Interfaces;
using Asisya.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Asisya.Infrastructure.Persistence.Bulk;

public class NpgsqlProductBulkInserter : IProductBulkInserter
{
    private const string CopyCommand =
        "COPY \"Product\" (\"ProductName\", \"SupplierId\", \"CategoryId\", \"QuantityPerUnit\", " +
        "\"UnitPrice\", \"UnitsInStock\", \"UnitsOnOrder\", \"ReorderLevel\", \"Discontinued\", \"Source\") " +
        "FROM STDIN (FORMAT BINARY)";

    private readonly AsisyaDbContext _context;

    public NpgsqlProductBulkInserter(AsisyaDbContext context)
    {
        _context = context;
    }

    public async Task<int> BulkInsertAsync(IEnumerable<Product> products, CancellationToken ct = default)
    {
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;

        if (openedHere)
            await connection.OpenAsync(ct);

        try
        {
            await using var writer = await connection.BeginBinaryImportAsync(CopyCommand, ct);
            var count = 0;

            foreach (var p in products)
            {
                await writer.StartRowAsync(ct);

                await writer.WriteAsync(p.ProductName, NpgsqlDbType.Varchar, ct);

                if (p.SupplierId.HasValue)
                    await writer.WriteAsync(p.SupplierId.Value, NpgsqlDbType.Integer, ct);
                else
                    await writer.WriteNullAsync(ct);

                await writer.WriteAsync(p.CategoryId, NpgsqlDbType.Integer, ct);

                if (p.QuantityPerUnit is null)
                    await writer.WriteNullAsync(ct);
                else
                    await writer.WriteAsync(p.QuantityPerUnit, NpgsqlDbType.Text, ct);

                await writer.WriteAsync(p.UnitPrice, NpgsqlDbType.Numeric, ct);
                await writer.WriteAsync(p.UnitsInStock, NpgsqlDbType.Smallint, ct);
                await writer.WriteAsync(p.UnitsOnOrder, NpgsqlDbType.Smallint, ct);
                await writer.WriteAsync(p.ReorderLevel, NpgsqlDbType.Smallint, ct);
                await writer.WriteAsync(p.Discontinued, NpgsqlDbType.Boolean, ct);
                await writer.WriteAsync(p.Source.ToString(), NpgsqlDbType.Varchar, ct);

                count++;
            }

            await writer.CompleteAsync(ct);
            return count;
        }
        finally
        {
            if (openedHere)
                await connection.CloseAsync();
        }
    }
}