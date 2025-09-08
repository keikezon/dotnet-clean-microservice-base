using System.Text.RegularExpressions;

namespace Identity.Domain.Users;

public class Login
{
    private Login() { }

    public Login(string email, string password)
    {
        Email = email;
        Password = password;
    }
    
    public string Email { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    
    private static readonly Regex Pattern = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled
    );
    
    public static Login LoginUser(string email, string password, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(email) || !Pattern.IsMatch(email))
            throw new ArgumentException("Invalid email", nameof(email));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required", nameof(password));

        return new Login
        {
            Email = email,
            Password = password,
        };
    }
}