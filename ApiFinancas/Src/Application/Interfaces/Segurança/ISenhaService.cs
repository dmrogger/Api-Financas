using ApiFinancas.Src.Application.DTOs.Requests.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Segurança
{
    public interface ISenhaService
    {
        string HashSenha(string senha);
        bool ValidaSenha(string senha, string senhaHash);

    }
}
