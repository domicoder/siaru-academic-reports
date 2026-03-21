using SIARU.Domain.Common;

namespace SIARU.Domain.Entities;

public class Department : AuditableSoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<KnowledgeArea> KnowledgeAreas { get; set; } = new List<KnowledgeArea>();
}