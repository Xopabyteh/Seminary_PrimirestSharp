namespace Yearly.Contracts.Authentication;

public record LoginResponse(int Id, string Username, string SessionCookie);