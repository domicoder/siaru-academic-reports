using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class SubjectIncompatibilityConfiguration : IEntityTypeConfiguration<SubjectIncompatibility>
{
    public void Configure(EntityTypeBuilder<SubjectIncompatibility> builder)
    {
        builder.ToTable("SubjectIncompatibilities");

        builder.HasKey(x => new { x.SubjectCode, x.IncompatibleWithSubjectCode });

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.Incompatibilities)
            .HasForeignKey(x => x.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.IncompatibleWithSubject)
            .WithMany(x => x.IncompatibleWithSubjects)
            .HasForeignKey(x => x.IncompatibleWithSubjectCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}