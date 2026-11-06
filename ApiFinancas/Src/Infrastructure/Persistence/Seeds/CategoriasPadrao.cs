namespace ApiFinancas.Src.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Categorias padrão inseridas pela migration. Os IDs são fixos: se fossem gerados a cada execução,
    /// o EF entenderia como dados novos e criaria uma migration diferente toda vez.
    /// </summary>
    public static class CategoriasPadrao
    {
        public static readonly Guid Alimentacao = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e01");
        public static readonly Guid Moradia = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e02");
        public static readonly Guid Transporte = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e03");
        public static readonly Guid Saude = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e04");
        public static readonly Guid Educacao = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e05");
        public static readonly Guid Lazer = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e06");
        public static readonly Guid Salario = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e07");
        public static readonly Guid Outros = Guid.Parse("8b1f6c1e-3f0a-4c2d-9a51-0c6f2a1d7e08");

        /// <summary>
        /// Dados no formato esperado pelo HasData (objetos anônimos, pois a entidade não tem setters públicos)
        /// </summary>
        public static object[] Todas() => new object[]
        {
            new { Id = Alimentacao, Nome = "Alimentação", UsuarioId = (Guid?)null },
            new { Id = Moradia, Nome = "Moradia", UsuarioId = (Guid?)null },
            new { Id = Transporte, Nome = "Transporte", UsuarioId = (Guid?)null },
            new { Id = Saude, Nome = "Saúde", UsuarioId = (Guid?)null },
            new { Id = Educacao, Nome = "Educação", UsuarioId = (Guid?)null },
            new { Id = Lazer, Nome = "Lazer", UsuarioId = (Guid?)null },
            new { Id = Salario, Nome = "Salário", UsuarioId = (Guid?)null },
            new { Id = Outros, Nome = "Outros", UsuarioId = (Guid?)null },
        };
    }
}
