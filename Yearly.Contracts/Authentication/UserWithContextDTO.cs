namespace Yearly.Contracts.Authentication;

public readonly record struct UserWithContextDTO(string Name, List<UserRoleDTO> Roles);
