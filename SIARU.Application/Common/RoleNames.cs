namespace SIARU.Application.Common;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string StudentServicesStaff = "StudentServicesStaff";
    public const string ReadOnlyUser = "ReadOnlyUser";

    public static readonly string[] All = new[] {
        Administrator,
        StudentServicesStaff,
        ReadOnlyUser
    };
}