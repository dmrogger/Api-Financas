using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Enums;

namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    public class ObterMovimentacoesResponse
    {
        public ObterMovimentacoesResponse(Movimentacao movimentacao)
        {
            MovimentacaoId = movimentacao.Id;
            Valor = movimentacao.Valor;
            DataTransacao = movimentacao.Data;
            Tipo = movimentacao.Tipo;
            Descricao = movimentacao.Descricao;
            CategoriaId = movimentacao.CategoriaId;
        }

        public Guid MovimentacaoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransacao { get; set; }
        public ETipoMovimentacao Tipo { get; set; }
        public string? Descricao { get; set; }
        public int? CategoriaId { get; set; }
    }
}
