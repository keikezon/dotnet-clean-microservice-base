namespace Identity.API.Contracts.Users;

public sealed record LoginRequest(string Email, string Password);