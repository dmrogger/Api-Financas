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

        public DateTime DataRequisicao { get; private set; }

        [Required(ErrorMessage = "O id do usuário deve ser informado")]
        public Guid idUsuario { get; private set; }

        [Required(ErrorMessage = "A origem deve ser informada")]
        public string Origem { get; private set; }

        //public ETipoOperacao TipoOperacao { get; set; }

    }
}
