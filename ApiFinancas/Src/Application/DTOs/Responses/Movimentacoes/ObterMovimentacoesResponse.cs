namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    /// <summary>
    /// Response de obtenção de movimentações
    /// </summary>
    public class ObterMovimentacoesResponse
    {
        /// <summary>
        /// ID da movimentação
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Valor da movimentação
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da movimentação
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// ID da categoria
        /// </summary>
        public Guid CategoriaId { get; set; }

        /// <summary>
        /// ID do usuário
        /// </summary>
        public Guid? UsuarioId { get; set; }
    }
}
