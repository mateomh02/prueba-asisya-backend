using Asisya.Domain.Common;

namespace Asisya.Domain.Identity
{
    public class AppRoute : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Component { get; set; } = string.Empty;

        public ICollection<RoleRoute> RoleRoutes { get; set; } = new List<RoleRoute>();
    }
}