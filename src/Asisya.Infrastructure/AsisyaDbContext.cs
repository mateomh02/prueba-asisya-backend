using Microsoft.EntityFrameworkCore;
using Asisya.Domain.Common;

namespace Asisya.Infrastructure
{
    public class AsisyaDbContext : DbContext
    {
        public AsisyaDbContext(DbContextOptions<AsisyaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = typeof(IEntity).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t));

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AsisyaDbContext).Assembly);
        }
    }
}