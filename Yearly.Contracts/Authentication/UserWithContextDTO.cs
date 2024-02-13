namespace Yearly.Contracts.Authentication;

public readonly record struct UserWithContextDTO(int Id, string Username, List<UserRoleDTO> Roles);
