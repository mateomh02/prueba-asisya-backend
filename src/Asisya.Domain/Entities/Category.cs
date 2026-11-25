using Asisya.Domain.Common;

namespace Asisya.Domain.Entities
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }
}
