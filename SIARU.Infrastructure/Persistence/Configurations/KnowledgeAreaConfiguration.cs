using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class KnowledgeAreaConfiguration : IEntityTypeConfiguration<KnowledgeArea>
{
    public void Configure(EntityTypeBuilder<KnowledgeArea> builder)
    {
        builder.ToTable("KnowledgeAreas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(x => x.Department)
            .WithMany(x => x.KnowledgeAreas)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}