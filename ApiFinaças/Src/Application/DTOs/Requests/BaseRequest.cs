using ApiFinaças.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            DataRequisicao = DateTime.UtcNow;
        }

        public DateTime DataRequisicao { get; set; }

        public Guid? idUsuario { get; set; }

        [Required(ErrorMessage = "A origem deve ser informada")]
        public string Origem { get; set; } = string.Empty;
    }
}
