using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIARU.Domain.Common;
using SIARU.Domain.Entities;
using SIARU.Infrastructure.Identity;

namespace SIARU.Infrastructure.Persistence;

public class SiaruDbContext : IdentityDbContext<ApplicationUser>
{
    public SiaruDbContext(DbContextOptions<SiaruDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<KnowledgeArea> KnowledgeAreas => Set<KnowledgeArea>();
    public DbSet<DegreeProgram> DegreePrograms => Set<DegreeProgram>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<OfficeHour> OfficeHours => Set<OfficeHour>();
    public DbSet<SubjectDegreeProgram> SubjectDegreePrograms => Set<SubjectDegreeProgram>();
    public DbSet<TeachingAssignment> TeachingAssignments => Set<TeachingAssignment>();
    public DbSet<SubjectIncompatibility> SubjectIncompatibilities => Set<SubjectIncompatibility>();
    public DbSet<SubjectEquivalency> SubjectEquivalencies => Set<SubjectEquivalency>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(SiaruDbContext).Assembly);

        ApplySoftDeleteQueryFilters(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // TeachingAssignment must be physically deleted
                if (entry.Entity is TeachingAssignment)
                {
                    continue;
                }

                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = utcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<KnowledgeArea>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<DegreeProgram>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Subject>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Professor>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<OfficeHour>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<TeachingAssignment>().HasQueryFilter(x => !x.IsDeleted);
    }
}