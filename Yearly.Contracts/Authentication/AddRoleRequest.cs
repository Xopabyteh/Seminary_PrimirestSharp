namespace Yearly.Contracts.Authentication;

public record AddRoleRequest(
    int UserId,
    string RoleCode);
