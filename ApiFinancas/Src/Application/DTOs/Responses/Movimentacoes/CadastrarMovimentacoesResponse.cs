namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    /// <summary>
    /// Response de cadastro de movimentação
    /// </summary>
    public class CadastrarMovimentacoesResponse
    {
        public CadastrarMovimentacoesResponse(Guid movimentacaoId)
        {
            MovimentacaoId = movimentacaoId;
        }

        /// <summary>
        /// ID da movimentação criada
        /// </summary>
        public Guid MovimentacaoId { get; set; }
    }
}
