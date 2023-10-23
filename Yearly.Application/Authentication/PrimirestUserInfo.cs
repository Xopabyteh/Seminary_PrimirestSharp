namespace Yearly.Application.Authentication;

/// <summary>
/// Raw user info from primirest auth provider
/// </summary>
/// <param name="Id"></param>
/// <param name="Username"></param>
public readonly record struct PrimirestUserInfo(int Id, string Username);