namespace ApiFinancas.Src.Domain.Entities
{
    /// <summary>
    /// Categoria de movimentação. Sem UsuarioId é uma categoria padrão, visível a todos;
    /// com UsuarioId é uma categoria personalizada daquele usuário.
    /// </summary>
    public class Categoria
    {
        public const int TamanhoMaximoNome = 50;

        // Usado pelo EF Core
        private Categoria()
        {
            Nome = string.Empty;
        }

        public Categoria(string nome, Guid? usuarioId)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("O id do usuário não pode ser vazio.", nameof(usuarioId));

            Id = Guid.NewGuid();
            Nome = ValidarNome(nome);
            UsuarioId = usuarioId;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Guid? UsuarioId { get; private set; }

        /// <summary>
        /// Categoria padrão (sem dono), criada pelo sistema
        /// </summary>
        public bool Padrao => UsuarioId == null;

        public bool PertenceAo(Guid usuarioId) => UsuarioId == usuarioId;

        /// <summary>
        /// O usuário pode ver e usar a categoria se ela for padrão ou dele
        /// </summary>
        public bool VisivelPara(Guid usuarioId) => Padrao || PertenceAo(usuarioId);

        public void Renomear(string novoNome)
        {
            Nome = ValidarNome(novoNome);
        }

        private static string ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da categoria é obrigatório.", nameof(nome));

            var nomeTratado = nome.Trim();

            if (nomeTratado.Length > TamanhoMaximoNome)
                throw new ArgumentException($"O nome da categoria não pode exceder {TamanhoMaximoNome} caracteres.", nameof(nome));

            return nomeTratado;
        }
    }
}
