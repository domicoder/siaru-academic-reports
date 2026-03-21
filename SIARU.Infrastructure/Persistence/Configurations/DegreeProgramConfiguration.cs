using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class DegreeProgramConfiguration : IEntityTypeConfiguration<DegreeProgram>
{
    public void Configure(EntityTypeBuilder<DegreeProgram> builder)
    {
        builder.ToTable("DegreePrograms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}