using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests.Usuario
{
    /// <summary>
    /// Request para criação de usuário
    /// </summary>
    public class CriaUsuarioRequest : BaseRequest
    {
        public CriaUsuarioRequest(string email, string nome, Guid idUsuario)
        {
            Email = email;
            Nome = nome;
            IdUsuario = idUsuario;
        }

        [Required(ErrorMessage = "E-mail é obrigatório para criação de usuário")]
        [MaxLength(100, ErrorMessage = "E-mail ultrapassou o número de caracteres permitidos")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório para criação de usuário")]
        [MaxLength(150, ErrorMessage = "Nome ultrapassou o número de caracteres permitidos")]
        public string Nome { get; set; }
        public Guid IdUsuario { get; set; }

    }
}
