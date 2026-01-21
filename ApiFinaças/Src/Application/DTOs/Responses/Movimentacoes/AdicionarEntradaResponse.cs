namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    /// <summary>
    /// Response de adição de entrada
    /// </summary>
    public class AdicionarEntradaResponse
    {
        /// <summary>
        /// ID da movimentação criada
        /// </summary>
        public Guid MovimentacaoId { get; set; }

        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;
    }
}
