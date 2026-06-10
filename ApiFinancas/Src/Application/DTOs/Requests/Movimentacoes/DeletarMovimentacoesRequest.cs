using ApiFinancas.Src.Domain.Enums;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    public class DeletarMovimentacoesRequest : BaseRequest
    {
        public DeletarMovimentacoesRequest(Guid idMovimentacao)
        {
             Id = idMovimentacao;
        }
        public Guid Id {  get; set; }
        public decimal Valor { get; set; }
        public DateTime DataOperacao { get; set; }
        public string? Descricao { get; set; }
        public int CategoriaId { get; set; }
        public ETipoOperacao Tipo { get; set; }
        public Guid IdTransacao { get; set; }
    }
}
