using Common.Enum;

namespace Identity.API.Contracts.Users;

public sealed record CreateUserRequest(string Name, string Email, string Password, UserProfile Profile);
