using Common.Enum;

namespace Identity.API.Contracts.Users;

public sealed record UserResponse(Guid Id, string Name, string Email, UserProfile Profile);
