using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class SubjectDegreeProgramConfiguration : IEntityTypeConfiguration<SubjectDegreeProgram>
{
    public void Configure(EntityTypeBuilder<SubjectDegreeProgram> builder)
    {
        builder.ToTable("SubjectDegreePrograms", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_SubjectDegreePrograms_Quadrimester_Range",
                "\"Quadrimester\" >= 1 AND \"Quadrimester\" <= 20");
        });

        builder.HasKey(x => new { x.SubjectCode, x.DegreeProgramId });

        builder.Property(x => x.Quadrimester)
            .IsRequired();

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.SubjectDegreePrograms)
            .HasForeignKey(x => x.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DegreeProgram)
            .WithMany(x => x.SubjectDegreePrograms)
            .HasForeignKey(x => x.DegreeProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}