using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISenhaService _passwordService;
        public UsuarioService(IUsuarioRepository usuarioRepository, ISenhaService passwordService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordService = passwordService;
        }

        public async Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request)
        {
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuarioExistente != null)
                return Result<UsuarioResponse>.Fail("E-mail já cadastrado! Tente criar a conta usando outro E-mail");

            var senhaHash = _passwordService.HashSenha(request.Senha);

            var usuario = new Usuario(request.Nome, request.Email, senhaHash);

            var usuarioCriado = await _usuarioRepository.AdicionarAsync(usuario);

            if (usuarioCriado == Guid.Empty)
                return Result<UsuarioResponse>.Fail("Erro desconhecido ao criar usuário!");

            var response = new UsuarioResponse(usuarioCriado, usuario.Nome, usuario.Email);
            return Result<UsuarioResponse>.Ok(response);
        }

        public async Task<Result<string>> EditaUsuario(EditaUsuarioRequest request, Guid idUsuario)
        {
            if (string.IsNullOrWhiteSpace(request.Nome) && string.IsNullOrWhiteSpace(request.SenhaNova))
                return Result<string>.Fail("Informe ao menos um campo para alterar (Nome ou SenhaNova).");

            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);

            if (usuario == null)
                return Result<string>.Fail("Usuário não localizado!");

            if (!_passwordService.ValidaSenha(request.SenhaAtual, usuario.Senha))
                return Result<string>.Fail("Erro: Email ou senha inválidos!");

            if (!string.IsNullOrWhiteSpace(request.Nome))
                usuario.AlterarNome(request.Nome);

            if (!string.IsNullOrWhiteSpace(request.SenhaNova))
                usuario.AlterarSenha(_passwordService.HashSenha(request.SenhaNova));

            await _usuarioRepository.EditaUsuarioAsync(usuario);

            return Result<string>.Ok("Usuário atualizado com sucesso!");
        }

        public async Task<Result<string>> DeletaUsuario(ExcluiUsuarioRequest request, Guid idUsuario)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);
            if (usuario == null)
                return Result<string>.Fail("Usuário não localizado, não foi possível deletar");

            if (!_passwordService.ValidaSenha(request.Senha, usuario.Senha))
                return Result<string>.Fail("Erro: Email ou senha inválidos!");

            try
            {
                await _usuarioRepository.DeletarAsync(usuario);
            }
            catch (Exception)
            {
                return Result<string>.Fail("Erro interno ao excluir o usuário");
            }

            return Result<string>.Ok("Usuário excluído com sucesso!");
        }

        public async Task<Result<UsuarioResponse>> ConsultaUsuario(Guid idUsuario)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);

            if (usuario == null)
                return Result<UsuarioResponse>.Fail("Usuário não localizado!");

            return Result<UsuarioResponse>.Ok(new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email));
        }
    }
}
