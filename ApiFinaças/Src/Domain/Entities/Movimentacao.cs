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

            if (UsuarioId == Guid.Empty)
                throw new ArgumentException("O id do usuário não pode ser vazio", nameof(Id));

            if (valor <= 0)
                throw new ArgumentException("O valor da movimentação deve ser maior que zero.");


            Id = Guid.NewGuid();
            Valor = valor;
            Data = data;
            CategoriaId = categoriaId;
            UsuarioId = usuarioId;
        }

        public void AtualizarValor(decimal novoValor, Guid Id)
        {
            if (novoValor <= 0)
                throw new ArgumentException("O valor da movimentação deve ser maior que zero.");

            if (Id == Guid.Empty)
                throw new ArgumentException("O id da movimentação deve ser informado");

            Valor = novoValor;
        }
    }
}
