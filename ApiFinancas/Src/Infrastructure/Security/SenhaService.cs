
using ApiFinancas.Src.Application.Interfaces.Segurança;
using Microsoft.AspNetCore.Identity;

namespace ApiFinancas.Src.Infrastructure.Security
{
    public class SenhaService : ISenhaService
    {
        private readonly PasswordHasher<string> _passwordHasher;
        public SenhaService()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        public string HashSenha(string senha)
        {
            return _passwordHasher.HashPassword(null!, senha);
        }

        public bool ValidaSenha(string senha, string senhaHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, senhaHash, senha);

            return result == PasswordVerificationResult.Success;
        }
    }
}
