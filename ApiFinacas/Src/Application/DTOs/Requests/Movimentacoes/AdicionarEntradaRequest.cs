using ApiFinancas.Src.Application.DTOs.Requests;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    /// <summary>
    /// Request de adição de entradas
    /// </summary>
    public class AdicionarEntradaRequest : BaseRequest
    {
        public AdicionarEntradaRequest(decimal valor, DateTime dataOperacao)
        {
            Valor = valor;
            DataOperacao = dataOperacao;
        }

        /// <summary>
        /// Valor Da Entrada 
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da Entrada
        /// </summary>
        public DateTime DataOperacao { get; set; } = DateTime.UtcNow;
    }
}
