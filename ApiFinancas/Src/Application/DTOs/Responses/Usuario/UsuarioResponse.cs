namespace ApiFinancas.Src.Application.DTOs.Responses.Usuario
{
    public class UsuarioResponse
    {
        public UsuarioResponse()
        {
        }

        public UsuarioResponse(Guid id, string nome, string email)
        {
            idUsuario = id;
            Nome = nome;
            Email = email;
        }
        public Guid idUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}