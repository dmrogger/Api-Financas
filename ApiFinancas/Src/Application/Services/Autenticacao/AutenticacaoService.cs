using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISenhaService _senhaService;
        private readonly ITokenService _tokenService;

        public AutenticacaoService(
            IUsuarioRepository usuarioRepository,
            ISenhaService senhaService,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _senhaService = senhaService;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuario == null)
                return Result<LoginResponse>.Fail("Usuário ou senha inválidos");

            if (!_senhaService.ValidaSenha(request.Senha, usuario.Senha))
                return Result<LoginResponse>.Fail("Usuário ou senha inválidos");

            var token = _tokenService.GerarToken(usuario);

            var response = new LoginResponse(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                token
            );

            return Result<LoginResponse>.Ok(response);
        }
    }
}
