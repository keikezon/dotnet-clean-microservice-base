using System.Text.RegularExpressions;

namespace Identity.Domain.Users;

public sealed class User
{
    public User() { }

    public User(Guid id, string name, string email, string passwordHash, string profile)
    {
        this.Id = id;
        this.Name = name;
        this.Email = email;
        this.PasswordHash = passwordHash;
        this.Profile = profile;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Profile { get; private set; } = default!;
    public DateTime? CreatedAtUtc { get; private set; }

    private static readonly Regex Pattern = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled
    );
    public static User Create(string name, string email, string rawPassword, string profile, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(rawPassword)) throw new ArgumentException("Password is required", nameof(name));
        if (string.IsNullOrWhiteSpace(profile)) throw new ArgumentException("Profile is required", nameof(profile));
        if (string.IsNullOrWhiteSpace(email) || !Pattern.IsMatch(email))
            throw new ArgumentException("Invalid email", nameof(email));

        return new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email,
            PasswordHash = hasher.Hash(rawPassword ?? throw new ArgumentNullException(nameof(rawPassword))),
            Profile = profile.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(string name, string profile)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        if (!string.IsNullOrWhiteSpace(profile)) Profile = profile.Trim();
    }

    public void ChangePassword(string newPassword, IPasswordHasher hasher)
    {
        PasswordHash = hasher.Hash(newPassword);
    }
}
