using UsuarioEntity = ApiFinancas.Src.Domain.Entities.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Segurança
{
    public interface ITokenService
    {
        string GerarToken(UsuarioEntity usuario);
    }
}
