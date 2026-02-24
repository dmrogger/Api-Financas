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
        public async Task<bool> ValidaLogin(LoginRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuario == null)
                return false;

            return _passwordService.ValidaSenha(request.Senha, usuario.Senha);
        }

        public async Task<Result<string>> AtualizaSenha(EditaUsuarioRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
            if (usuario == null)
                return Result<string>.Fail("Usário não localizado!");

            var senhaValida =  _passwordService.ValidaSenha(request.SenhaAtual, usuario.Senha);
            if (!senhaValida)
                return Result<string>.Fail("Erro: Email ou senha inválidos!");

            var senhaAlterada = _usuarioRepository.AtualizarAsync(usuario);
            if (senhaAlterada.IsCompletedSuccessfully)
                return Result<string>.Ok("Senha alterada com sucesso!");

            return Result<string>.Fail("Erro interno ao alterar senha do usuário");
        }

        public async Task<Result<UsuarioResponse>> ConsultaUsuario(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario != null)
                return Result<UsuarioResponse>.Ok(new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email));

            return Result<UsuarioResponse>.Fail("Usuário não localizado");
        }

        public async Task<Result<string>> DeletaUsuario(ExcluiUsuarioRequest request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
            if (usuario == null)
                return Result<string>.Fail("Usuário não localizado, não foi possível deletar");


            var senhaValida = _passwordService.ValidaSenha(request.Senha, usuario.Senha);
            if (!senhaValida)
                return Result<string>.Fail("Erro: Email ou senha inválidos!");
            
            if (_usuarioRepository.DeletarAsync(usuario).IsCompletedSuccessfully)
                return Result<string>.Ok("Usuário excluído com sucesso!");

            return Result<string>.Fail("Erro interno ao excluír o usuário");

        }
    }
}
