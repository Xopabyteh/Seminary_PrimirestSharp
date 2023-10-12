namespace Yearly.Infrastructure.Services.Authentication;

public class PrimirestAdminCredentialsOptions
{
    public const string SectionName = "PrimirestAuthentication";
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}