namespace Yearly.Infrastructure.Services.Authentication;

public record PrimirestLoginRequest(
    string Username,
    string Password);