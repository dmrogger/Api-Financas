using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using ApiFinancas.Src.Infrastructure.Security;

namespace ApiFinancas.Src.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly SenhaService _passwordService;
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _passwordService = new SenhaService();
        }
        public async Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request)
        {
            var usuarioExiste = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuarioExiste != null)
                return Result<UsuarioResponse>.Fail("E-mail já cadastrado! Tente criar a conta usando outro E-mail");

            var senhaHash = _passwordService.HashSenha(request.Senha);

            var usuario = new Usuario(request.Nome, request.Email, senhaHash);

            var usuarioCriado = await _usuarioRepository.AdicionarAsync(usuario);

            if (usuarioCriado == Guid.Empty)
                return Result<UsuarioResponse>.Fail("Erro desconhecido ao criar usuário!");
            
            var response = new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email);
            return Result<UsuarioResponse>.Ok(response);
    
        }
        public Task<bool> ValidaLogin(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
