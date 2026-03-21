using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using SIARU.Application.Common;
using SIARU.Domain.Entities;
using SIARU.Domain.Enums;
using SIARU.Infrastructure.Identity;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Seed;

public class ApplicationDbSeeder
{
    private readonly SiaruDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<AdminSeedOptions> _adminOptions;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        SiaruDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IOptions<AdminSeedOptions> adminOptions,
        ILogger<ApplicationDbSeeder> logger)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _adminOptions = adminOptions;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await _context.Database.MigrateAsync();

                await SeedRolesAsync();
                await SeedAdminAsync();
                await SeedDemoAcademicDataAsync();

                _logger.LogInformation("Database migration and seeding completed successfully.");
                return;
            }
            catch (Exception ex) when (
                ex is Npgsql.NpgsqlException ||
                ex is DbUpdateException ||
                ex.InnerException is Npgsql.NpgsqlException)
            {
                _logger.LogWarning(ex, "Database not ready yet. Attempt {Attempt}/{MaxRetries}", attempt, maxRetries);

                if (attempt == maxRetries)
                    throw;

                await Task.Delay(delay);
            }
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in RoleNames.All)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    private async Task SeedAdminAsync()
    {
        var email = _adminOptions.Value.Email?.Trim();
        var password = _adminOptions.Value.Password;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Admin seed skipped because AdminSeed credentials were not configured.");
            return;
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, RoleNames.Administrator))
            {
                await _userManager.AddToRoleAsync(existingUser, RoleNames.Administrator);
            }

            return;
        }

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(admin, password);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        await _userManager.AddToRoleAsync(admin, RoleNames.Administrator);
    }

    private async Task SeedDemoAcademicDataAsync()
    {
        if (await _context.Departments.AnyAsync())
        {
            return;
        }

        var department = new Department
        {
            Name = "Ingeniería y Ciencia de los Computadores",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var area = new KnowledgeArea
        {
            Name = "Ciencia de la Computación e Inteligencia Artificial",
            Department = department,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var degreeProgram = new DegreeProgram
        {
            Name = "Ing. Técnica Informática Gestión",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var subject = new Subject
        {
            Code = "F38",
            Name = "Robótica",
            Course = 3,
            TheoreticalCredits = 2.5m,
            LabCredits = 2.5m,
            Type = SubjectType.Optional,
            AdmissionLimit = null,
            KnowledgeArea = area,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var professor = new Professor
        {
            Name = "José Antonio López García",
            Office = "TI1292",
            KnowledgeArea = area,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var subjectDegreeProgram = new SubjectDegreeProgram
        {
            Subject = subject,
            DegreeProgram = degreeProgram,
            IsFreeConfiguration = true,
            Quadrimester = 6
        };

        var officeHour1 = new OfficeHour
        {
            Professor = professor,
            DayOfWeek = WeekDay.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var officeHour2 = new OfficeHour
        {
            Professor = professor,
            DayOfWeek = WeekDay.Thursday,
            StartTime = new TimeOnly(12, 0),
            EndTime = new TimeOnly(14, 0),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        var teachingAssignment = new TeachingAssignment
        {
            Subject = subject,
            Professor = professor,
            AcademicYear = "2025/26",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

        _context.Departments.Add(department);
        _context.KnowledgeAreas.Add(area);
        _context.DegreePrograms.Add(degreeProgram);
        _context.Subjects.Add(subject);
        _context.Professors.Add(professor);
        _context.SubjectDegreePrograms.Add(subjectDegreeProgram);
        _context.OfficeHours.AddRange(officeHour1, officeHour2);
        _context.TeachingAssignments.Add(teachingAssignment);

        await _context.SaveChangesAsync();
    }
}