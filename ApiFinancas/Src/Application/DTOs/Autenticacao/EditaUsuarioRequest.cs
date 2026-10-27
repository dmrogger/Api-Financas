using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Autenticacao
{
    /// <summary>
    /// Request de edição do usuário autenticado. Informe ao menos Nome ou SenhaNova
    /// </summary>
    public class EditaUsuarioRequest
    {
        [Required(ErrorMessage = "Senha atual é um campo obrigatório")]
        public required string SenhaAtual { get; set; }

        [MaxLength(150, ErrorMessage = "Nome ultrapassou o número de caracteres permitidos")]
        public string? Nome { get; set; }

        [MinLength(8, ErrorMessage = "Senha precisa ter no mínimo 8 caracteres")]
        [MaxLength(64, ErrorMessage = "Senha não pode exceder 64 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,64}$",
            ErrorMessage = "Senha deve conter ao menos 1 letra maiúscula e 1 caractere especial.")]
        public string? SenhaNova { get; set; }
    }
}
