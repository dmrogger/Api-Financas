using ApiFinaças.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests
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
        public decimal Valor { get; private set; }

        /// <summary>
        /// Data da Entrada
        /// </summary>
        public DateTime DataOperacao { get; private set; } = DateTime.UtcNow;
    }
}
