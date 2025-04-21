using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.ModalStateExtansion;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services;

public class TokenServices
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.JWT_SECRET_KEY);
        var claims = user.GetClaims();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // esse campo pode criar assinaturas para o JWT entender.
            Subject = new ClaimsIdentity(claims),
            
            // esse campo diz quanto tempo que esse token vai viver.
            Expires = DateTime.UtcNow.AddHours(8),
            
            /*
             * é dos pilares de segurança do JWT. Sem ele, o token seria só um papel sem assinatura — qualquer um poderia forjar.
             */
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}