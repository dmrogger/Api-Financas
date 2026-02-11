namespace ApiFinancas.Src.Domain.Entities
{
    public class Usuario
    {
        public Usuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }
        public Guid Id { get; set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
    }
    
}
