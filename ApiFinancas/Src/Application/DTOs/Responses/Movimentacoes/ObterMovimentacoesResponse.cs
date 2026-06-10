using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Enums;
using System.Drawing;

namespace ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes
{
    public class ObterMovimentacoesResponse
    {
        public ObterMovimentacoesResponse(decimal valor, string? descricao, int categoriaId, ETipoOperacao tipo, int idMovimentacao)
        {
            Valor = valor;
            Descricao = descricao;
            CategoriaId = categoriaId;
            Tipo = tipo;
            IdTransacao = idMovimentacao;
        }

        public decimal Valor { get; set; }
        public DateTime DataOperacao { get; set; } = DateTime.Now;
        public string? Descricao { get; set; }
        public int CategoriaId { get; set; }
        public ETipoOperacao Tipo { get; set; }
        public int IdTransacao { get; set; } 
    }
}
