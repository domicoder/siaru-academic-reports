using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIARU.Domain.Entities;

namespace SIARU.Infrastructure.Persistence.Configurations;

public class OfficeHourConfiguration : IEntityTypeConfiguration<OfficeHour>
{
    public void Configure(EntityTypeBuilder<OfficeHour> builder)
    {
        builder.ToTable("OfficeHours");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Professor)
            .WithMany(x => x.OfficeHours)
            .HasForeignKey(x => x.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}