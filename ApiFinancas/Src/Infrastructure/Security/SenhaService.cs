
using ApiFinancas.Src.Application.Interfaces.Segurança;
using Microsoft.AspNetCore.Identity;

namespace ApiFinancas.Src.Infrastructure.Security
{
    public class SenhaService : ISenhaService
    {
        public string HashSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public bool ValidaSenha(string senha, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
    }
}
