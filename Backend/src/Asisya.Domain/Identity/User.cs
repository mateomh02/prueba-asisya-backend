using Asisya.Domain.Common;

namespace Asisya.Domain.Identity
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }

        public Role? Role { get; set; }
    }
}