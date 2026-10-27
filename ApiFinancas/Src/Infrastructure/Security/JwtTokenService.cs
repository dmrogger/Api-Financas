using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiFinancas.Src.Infrastructure.Security
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            var keyValue = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "A chave JWT não foi configurada.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "O Issuer JWT não foi configurado.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "O Audience JWT não foi configurado.");

            if (string.IsNullOrWhiteSpace(usuario.Email) ||
                string.IsNullOrWhiteSpace(usuario.Nome))
            {
                throw new ArgumentException(
                    "O usuário deve possuir e-mail e nome.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(keyValue);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "A chave JWT deve possuir pelo menos 32 bytes.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            var key = new SymmetricSecurityKey(keyBytes); // NOSONAR - chave vem de User Secrets / variável de ambiente

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var agora = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: agora,
                expires: agora.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
