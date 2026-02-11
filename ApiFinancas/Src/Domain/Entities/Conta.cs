using ApiFinacas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Common;

namespace ApiFinancas.Src.Domain.Entities
{
    public class Conta : AggregateRoot
    {
        public Conta(string nomeConta, TipoConta tipoConta, Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(nomeConta))
                throw new ArgumentNullException(nameof(nomeConta), "Nome da conta é obrigatório!");

            NomeConta = nomeConta;
            TipoConta = tipoConta;
            Usuario = usuario;
        }

        public string NomeConta { get; set; }
        public TipoConta TipoConta { get; set; }
        public Usuario Usuario { get; set; }
    }
}
