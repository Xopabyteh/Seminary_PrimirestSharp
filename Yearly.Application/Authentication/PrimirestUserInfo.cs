namespace Yearly.Application.Authentication;

/// <summary>
/// Raw user info from primirest auth provider
/// </summary>
/// <param name="Id"></param>
/// <param name="Username"></param>
/// <param name="AdditionalInfo">
/// This hopefully contains the users age in format: "2B77CF8; sk. 3 (studenti 15 a více let)".
/// It is a really really ugly thing that primirest cooked up...
/// </param>
public readonly record struct PrimirestUserInfo(int Id, string Username, string AdditionalInfo);