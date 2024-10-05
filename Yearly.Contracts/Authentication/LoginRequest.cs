namespace Yearly.Contracts.Authentication;

public record LoginRequest(string Username, string Password, int? PreferredUserInTenantId);