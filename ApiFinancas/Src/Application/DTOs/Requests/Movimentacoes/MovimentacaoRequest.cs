using ApiFinancas.Src.Application.DTOs.Requests;
using ApiFinancas.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    /// <summary>
    /// Request de adição de entradas
    /// </summary>
    public class MovimentacaoRequest : BaseRequest
    {
        public MovimentacaoRequest(decimal valor, string? descricao, Guid categoriaId, ETipoOperacao tipo)
        {
            Valor = valor;
            Descricao = descricao;
            CategoriaId = categoriaId;
            Tipo = tipo;
        }

        public decimal Valor { get; set; }
        public DateTime DataOperacao { get; set; } = DateTime.Now;
        public string? Descricao { get; set; }
        public Guid CategoriaId { get; set; }
        public ETipoOperacao Tipo {  get; set; }
    }
}
