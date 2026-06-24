
namespace ApiFinancas.Src.Domain.Entities
{
    public class Movimentacao
    {
        private Movimentacao() { }

        public Guid Id { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime Data { get; private set; }
        public int? CategoriaId { get; set; }
        public Guid ?UsuarioId { get; private set; }


        public Movimentacao(decimal valor, DateTime data, Guid? usuarioId, int categoriaId)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("O id do usuário não pode ser vazio", nameof(usuarioId));

            if (valor <= 0)
                throw new ArgumentException("O valor da movimentação deve ser maior que zero.", nameof(valor));

            Id = Guid.NewGuid();
            Valor = valor;
            Data = data;
            UsuarioId = usuarioId;
            CategoriaId = categoriaId;
        }

        public void AtualizarValor(decimal novoValor)
        {
            if (novoValor <= 0)
                throw new ArgumentException("O valor da movimentação deve ser maior que zero.", nameof(novoValor));

            Valor = novoValor;
        }

        public void AtualizarData(DateTime novaData)
        {
            if (novaData == default)
                throw new ArgumentException("A data da movimentação deve ser informada.", nameof(novaData));

            Data = novaData;
        }
    }
}
