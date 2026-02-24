using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Usuario
{
    public class EditaUsuarioRequest : BaseRequest
    {
        [Required(ErrorMessage = "O Campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail precisa ser válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Senha atual é um campo obrigatório")]
        public required string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Senha nova é um campo obrigatório")]
        public required string SenhaNova { get; set; }
    }
}
