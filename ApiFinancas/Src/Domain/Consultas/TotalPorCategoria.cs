namespace ApiFinancas.Src.Domain.Consultas
{
    /// <summary>
    /// Resultado da soma das movimentações de um período agrupadas por categoria.
    /// CategoriaId nulo agrupa as movimentações sem categoria
    /// </summary>
    public record TotalPorCategoria(Guid? CategoriaId, decimal TotalEntradas, decimal TotalSaidas);
}
