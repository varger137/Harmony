using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth;

public static class AuthOptions
{
    const string secret = "my_secret_is_CocaCola1234567890!@#";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(secret));

    public static string CreateToken(Dictionary<string, string> data)
    {
        List<Claim> claims = new();
        foreach (var pair in data)
            claims.Add(new Claim(pair.Key, pair.Value));

        var expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(100));
        var credentials = new SigningCredentials(GetSymmetricSecurityKey(), "HS256");

        JwtSecurityToken tokenObj = new(
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenObj);
        return token;
    }

    public static ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}