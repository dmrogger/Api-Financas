namespace ApiFinaças.Src.Domain.Entities
{
    public class Movimentacao
    {
        public Guid Id { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime Data { get; private set; }
        public Guid CategoriaId { get; private set; }
        public Guid UsuarioId { get; private set; }


        public Movimentacao(decimal valor, DateTime data, Guid categoriaId, Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("O id do usuário não pode ser vazio", nameof(usuarioId));

            if (categoriaId == Guid.Empty)
                throw new ArgumentException("O id da categoria não pode ser vazio", nameof(categoriaId));

            if (valor <= 0)
                throw new ArgumentException("O valor da movimentação deve ser maior que zero.", nameof(valor));

            Id = Guid.NewGuid();
            Valor = valor;
            Data = data;
            CategoriaId = categoriaId;
            UsuarioId = usuarioId;
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
