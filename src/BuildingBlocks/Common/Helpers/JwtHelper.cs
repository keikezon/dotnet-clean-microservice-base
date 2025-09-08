using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Common.Helpers;

public class JwtHelper
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;

    public string Issuer { get; }
    public string Audience { get; }

    public JwtHelper(string base64Key, string issuer, string audience)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
            throw new ArgumentException("Chave JWT não pode ser vazia.");

        // Decodifica a chave Base64
        var keyBytes = Convert.FromBase64String(base64Key.Trim());
        _securityKey = new SymmetricSecurityKey(keyBytes);

        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        _tokenHandler = new JwtSecurityTokenHandler();

        Issuer = issuer;
        Audience = audience;
    }

    // Gera token JWT com claims personalizados e expiração em minutos
    public string GenerateToken(Dictionary<string, string> claimsDict, int expireMinutes = 60)
    {
        var claims = claimsDict.Select(c => new Claim(c.Key, c.Value)).ToList();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = _signingCredentials,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    // Valida token e retorna claims (ou null se inválido)
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true
            };
            
            // Remove "Bearer " caso esteja presente
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring("Bearer ".Length).Trim();

            return _tokenHandler.ValidateToken(token, validationParams, out _);
        }
        catch(Exception ex)
        {
            throw ex;
            return null;
        }
    }
}