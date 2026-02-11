namespace ApiFinancas.Src.Domain.Entities
{
    public class Categoria
    {
        public Categoria(string nome)
        {
            Nome = nome;
        }
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}

