using Yearly.Contracts.Authentication;

namespace Yearly.Contracts.Users;

public readonly record struct UserWithContextDTO(int Id, string Username, List<UserRoleDTO> Roles);