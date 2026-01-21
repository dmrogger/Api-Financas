using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Usuario
{
    public class EditaUsuário : BaseRequest
    {
        [Required(ErrorMessage = "O Campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail precisa ser válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Senha é um campo obrigatório")]
        public required string Senha { get; set; }
    }
}
