namespace Yearly.Application.Authentication;

/// <summary>
/// Raw user info from external auth provider
/// </summary>
/// <param name="Id"></param>
/// <param name="Username"></param>
public readonly record struct ExternalUserInfo(int Id, string Username);