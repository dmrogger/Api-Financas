namespace ApiFinancas.Src.Domain.Entities
{
    public class Usuario
    {
        public Usuario(string nome, string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do usuário é obrigatório.", nameof(nome));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail do usuário é obrigatório.", nameof(email));

            Nome = nome.Trim();
            Email = NormalizarEmail(email);
            Senha = senha;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }

        public void AlterarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("O nome do usuário é obrigatório.", nameof(novoNome));

            Nome = novoNome.Trim();
        }

        public void AlterarSenha(string novaSenhaHash)
        {
            if (string.IsNullOrWhiteSpace(novaSenhaHash))
                throw new ArgumentException("O hash da senha é obrigatório.", nameof(novaSenhaHash));

            Senha = novaSenhaHash;
        }

        public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
