
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Usuario
{
    public class LoginRequest : BaseRequest
    {
        public LoginRequest(string email, string senha)
        {
            Email = email;
            Senha = senha;
        }

        [Required(ErrorMessage = "O Campo E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail precisa ser válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Senha é um campo obrigatório")]
        public required string Senha { get; set; }
    }
}
