using Asisya.Domain.Common;

namespace Asisya.Domain.Identity
{
    public class Role : IEntity
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<RoleRoute> RoleRoutes { get; set; } = new List<RoleRoute>();
    }
}
