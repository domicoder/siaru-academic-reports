using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class TeachingAssignmentConfiguration : IEntityTypeConfiguration<TeachingAssignment>
{
    public void Configure(EntityTypeBuilder<TeachingAssignment> builder)
    {
        builder.ToTable("TeachingAssignments");

        builder.HasKey(x => new { x.SubjectCode, x.ProfessorId, x.AcademicYear });

        builder.Property(x => x.AcademicYear)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.TeachingAssignments)
            .HasForeignKey(x => x.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Professor)
            .WithMany(x => x.TeachingAssignments)
            .HasForeignKey(x => x.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}