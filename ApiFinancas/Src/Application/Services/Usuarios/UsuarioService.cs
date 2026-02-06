using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task<UsuarioResponse> CriarUsuarioAsync(CriaUsuarioRequest request)
        {
            var usuario = new Usuario(request.Nome, request.Email);
            var usuarioCriado = await _usuarioRepository.AdicionarAsync(usuario);

            if (usuarioCriado != Guid.Empty) 
                return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email);

            return null;
    
        }
        public Task<bool> ValidaLogin(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
