using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIARU.Application.Interfaces;
using SIARU.Application.Services;
using SIARU.Domain.Interfaces;
using SIARU.Infrastructure.Identity;
using SIARU.Infrastructure.Persistence;
using SIARU.Infrastructure.Repositories;
using SIARU.Infrastructure.Seed;
using SIARU.Infrastructure.Services;

namespace SIARU.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<SiaruDbContext>(options =>
            options.UseNpgsql(connectionString));

        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<SiaruDbContext>()
            .AddDefaultTokenProviders();
            //.AddDefaultUI();

        services.Configure<AdminSeedOptions>(
            configuration.GetSection(AdminSeedOptions.SectionName));

        services.AddScoped<ApplicationDbSeeder>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();

        services.AddScoped<ISubjectReportService, SubjectReportService>();
        services.AddScoped<IProfessorReportService, ProfessorReportService>();

        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IKnowledgeAreaService, KnowledgeAreaService>();
        services.AddScoped<IDegreeProgramService, DegreeProgramService>();

        services.AddScoped<ISubjectManagementRepository, SubjectManagementRepository>();
        services.AddScoped<ISubjectService, SubjectService>();

        services.AddScoped<IProfessorManagementService, ProfessorManagementService>();

        services.AddScoped<IOfficeHourManagementService, OfficeHourManagementService>();

        services.AddScoped<ITeachingAssignmentManagementRepository, TeachingAssignmentManagementRepository>();
        services.AddScoped<ITeachingAssignmentManagementService, TeachingAssignmentManagementService>();

        return services;
    }
}