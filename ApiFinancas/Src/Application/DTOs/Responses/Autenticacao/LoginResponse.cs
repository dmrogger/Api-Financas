namespace ApiFinancas.Src.Application.DTOs.Responses.Usuario
{
    public class LoginResponse
    {
        public LoginResponse()
        {
        }

        public LoginResponse(Guid id, string nome, string email, string token)
        {
            idUsuario = id;
            Nome = nome;
            Email = email;
            Token = token;
        }
        public Guid idUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}