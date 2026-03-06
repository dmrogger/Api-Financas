using ApiFinancas.Src.Application.DTOs.Responses.Base;

namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    /// <summary>
    /// Response de adição de entrada
    /// </summary>
    public class MovimentacaoResponse
    {
        public MovimentacaoResponse(Guid movimentacaoId)
        {
            MovimentacaoId = movimentacaoId;
        }
        /// <summary>
        /// ID da movimentação criada
        /// </summary>
        public Guid MovimentacaoId { get; set; }
    }
}
