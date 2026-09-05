using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            DataRequisicao = DateTime.UtcNow;
        }

        public DateTime DataRequisicao { get; set; }

        [Required(ErrorMessage = "A origem deve ser informada")]
        public string Origem { get; set; } = string.Empty;
    }
}
