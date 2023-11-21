namespace Yearly.Contracts.Authentication;

public record RoleRequest(
    int UserId,
    string RoleCode);
