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
            var keyValue = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyValue))
                throw new Exception("JWT Key não configurada");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
