using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Autenticacao
{
    /// <summary>
    /// Request de exclusão do usuário autenticado (confirmação por senha)
    /// </summary>
    public class ExcluiUsuarioRequest
    {
        [Required(ErrorMessage = "Senha é um campo obrigatório")]
        public required string Senha { get; set; }
    }
}
