using Asisya.Domain.Common;

namespace Asisya.Domain.Identity
{
    public class RoleRoute : IEntity
    {
        public int RoleId { get; set; }
        public int AppRouteId { get; set; }

        public Role? Role { get; set; }
        public AppRoute? AppRoute { get; set; }
    }
}