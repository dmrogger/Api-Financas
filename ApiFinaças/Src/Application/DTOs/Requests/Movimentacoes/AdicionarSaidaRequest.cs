using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests.Movimentacoes
{

    public class AdicionarSaidaRequest : BaseRequest
    {
        public AdicionarSaidaRequest(decimal valor, DateTime dataOperacao)
        {
            Valor = valor;
            DataOperacao = dataOperacao;
        }
        /// <summary>
        /// Valor Da Saída
        /// </summary>
        [Required(ErrorMessage = "É necessário informar um valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da Saída
        /// </summary>
        public DateTime DataOperacao { get; set; } = DateTime.UtcNow;
    }
}
