using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests
{

    public class AdicionarSaidaRequest : BaseRequest
    {
        public AdicionarSaidaRequest(decimal valor, DateTime dataOperacao)
        {
            Valor = valor;
            DataOperacao = DataOperacao;
        }
        /// <summary>
        /// Valor Da Saída
        /// </summary>
        [Required(ErrorMessage = "É necessário informar um valor")]
        public decimal Valor { get; private set; }

        /// <summary>
        /// Data da Saída
        /// </summary>
        public DateTime DataOperacao { get; private set; } = DateTime.UtcNow;
    }
}
