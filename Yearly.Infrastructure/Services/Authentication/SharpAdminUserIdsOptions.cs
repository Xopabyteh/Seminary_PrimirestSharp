namespace Yearly.Infrastructure.Services.Authentication;
public class SharpAdminUserIdsOptions
{
    public const string SectionName = "AdminIds";
    public required string[] Ids { get; set; }
}
