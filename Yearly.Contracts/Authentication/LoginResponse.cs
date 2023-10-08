namespace Yearly.Contracts.Authentication;

public record LoginResponse(string Id, string Username, string SessionCookie);