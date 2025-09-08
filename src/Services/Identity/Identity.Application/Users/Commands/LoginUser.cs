using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common.Abstractions;
using Common.Helpers;
using Identity.Application.Users.Abstractions;
using Identity.Contracts.Events;
using Identity.Domain.Users;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Users.Commands;

public sealed class LoginUser
{
    public sealed record Command(string Email, string Password);

    public interface IHandler
    {
        Task<string?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IUserRepository repo, IPasswordHasher hasher, IPublishEndpoint bus, IConfiguration configuration) : IHandler
    {
        public async Task<string?> Handle(Command cmd, CancellationToken ct)
        {
            var login = Login.LoginUser(cmd.Email, cmd.Password, hasher);
            var userModel = await repo.LoginAsync(login, ct);
            
            bool valid = Pbkdf2PasswordHasher.VerifyPassword(login.Password, userModel.PasswordHash);
            if (!valid) throw new ArgumentException("Invalid password"); 

            // Publica evento via contrato
            var @event = new UserLoginIntegrationEvent(login.Email);
            await bus.Publish(@event, ct);
            
            return GenerateAccessToken(userModel, configuration);
        }
        
        private string GenerateAccessToken(User user, IConfiguration configuration)
        {
            var jwtModel = new Jwt
            {
                Audience = configuration["Jwt:Audience"],
                Issuer = configuration["Jwt:Issuer"],
                Key = configuration["Jwt:Key"],
                SecurityAlgorithm = configuration["Jwt:SecurityAlgorithm"],

            };
            
            var jwtHelper = new JwtHelper(jwtModel.Key, jwtModel.Issuer, jwtModel.Audience);
            
            var claims = new Dictionary<string, string>
            {
                { JwtRegisteredClaimNames.Sub, user.Id.ToString() },
                { ClaimTypes.Role, user.Profile }
            };
            var token = jwtHelper.GenerateToken(claims, 60);

            return token;
        }
    }
}