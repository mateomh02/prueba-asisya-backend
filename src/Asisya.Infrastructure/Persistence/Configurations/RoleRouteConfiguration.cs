using Asisya.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asisya.Infrastructure.Persistence.Configurations;

public class RoleRouteConfiguration : IEntityTypeConfiguration<RoleRoute>
{
    public void Configure(EntityTypeBuilder<RoleRoute> builder)
    {
        builder.HasKey(rr => new { rr.RoleId, rr.AppRouteId });

        builder.HasOne(rr => rr.Role)
            .WithMany(r => r.RoleRoutes)
            .HasForeignKey(rr => rr.RoleId);

        builder.HasOne(rr => rr.AppRoute)
            .WithMany(r => r.RoleRoutes)
            .HasForeignKey(rr => rr.AppRouteId);
    }
}