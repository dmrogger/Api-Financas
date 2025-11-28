using ApiFinaças.Src.Domain.Common;

namespace ApiFinaças.Src.Domain.Entities
{
    public class Conta : AggregateRoot
    {
        public Conta(string nomeConta, TipoConta tipoConta, Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(nomeConta))
                throw new ArgumentNullException("Nome da conta é obrigatório!");

            NomeConta = nomeConta;
            TipoConta = tipoConta;
            Usuario = usuario;
        }

        public string NomeConta { get; set; }
        public TipoConta TipoConta { get; set; }
        public Usuario Usuario { get; set; }
    }
}
