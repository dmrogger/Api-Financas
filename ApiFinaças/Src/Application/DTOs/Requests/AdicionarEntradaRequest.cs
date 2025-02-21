using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests
{
    public class AdicionarEntradaRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; private set; }

        [Required]
        public DateTime Data { get; private set; }
    }
}
