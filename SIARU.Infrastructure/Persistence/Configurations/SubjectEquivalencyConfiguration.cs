using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class SubjectEquivalencyConfiguration : IEntityTypeConfiguration<SubjectEquivalency>
{
    public void Configure(EntityTypeBuilder<SubjectEquivalency> builder)
    {
        builder.ToTable("SubjectEquivalencies");

        builder.HasKey(x => new { x.SubjectCode, x.EquivalentToSubjectCode });

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.Equivalencies)
            .HasForeignKey(x => x.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EquivalentToSubject)
            .WithMany(x => x.EquivalentToSubjects)
            .HasForeignKey(x => x.EquivalentToSubjectCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}