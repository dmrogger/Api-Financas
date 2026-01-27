using ApiFinancas.Src.Application.DTOs.Requests;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Usuario
{
    /// <summary>
    /// Request para criação de usuário
    /// </summary>
    public class CriaUsuarioRequest : BaseRequest
    {
        public CriaUsuarioRequest(string email, string nome, string senha)
        {
            Email = email;
            Senha = senha;
            Nome = nome;
        }


        [Required(ErrorMessage = "E-mail é obrigatório para criação de usuário")]
        [EmailAddress(ErrorMessage = "E-mail precisa ser válido")]
        [MaxLength(100, ErrorMessage = "E-mail ultrapassou o número de caracteres permitidos")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo Senha é obrigatório para criação do usuário")]
        [MinLength(8, ErrorMessage = "Senha precisa ter no minímo de 8 caracteres e ao menos, " +
            "1 caracter especial e 1 letra maiúscula")]
        [MaxLength(20, ErrorMessage = "Senha não pode exceder 20 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,20}$",
        ErrorMessage = "Senha deve conter ao menos 1 letra maiúscula e 1 caractere especial.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório para criação de usuário")]
        [MaxLength(150, ErrorMessage = "Nome ultrapassou o número de caracteres permitidos")]
        public string Nome { get; set; }

    }
}
