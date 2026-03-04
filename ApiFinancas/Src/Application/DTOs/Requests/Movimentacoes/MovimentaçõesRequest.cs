using ApiFinancas.Src.Application.DTOs.Requests;
using ApiFinancas.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    /// <summary>
    /// Request de adição de entradas
    /// </summary>
    public class MovimentaçõesRequest : BaseRequest
    {
        public MovimentaçõesRequest(decimal valor, DateTime dataOperacao)
        {
            Valor = valor;
            DataOperacao = dataOperacao;
        }

        public decimal Valor { get; set; }
        public DateTime DataOperacao { get; set; } = DateTime.Now;
        public string Descricao { get; set; }
        public Guid CategoriaId { get; set; }
        public ETipoOperacao Tipo {  get; set; }
    }
}
