using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Autenticacao;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiFinancas.Src.Application.Services.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AutenticacaoService(
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuario == null)
                return Result<LoginResponse>.Fail("Usuário ou senha inválidos");

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
                return Result<LoginResponse>.Fail("Usuário ou senha inválidos");

            var token = GerarToken(usuario);

            var response = new LoginResponse(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                token
            );

            return Result<LoginResponse>.Ok(response);
        }


        private string GerarToken(Usuario usuario)
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
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        usuario.Id.ToString()),

                    new Claim(
                        ClaimTypes.Email,
                        usuario.Email),

                    new Claim(
                        ClaimTypes.Name,
                        usuario.Nome)
                  };

            var key = new SymmetricSecurityKey(keyBytes);

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
