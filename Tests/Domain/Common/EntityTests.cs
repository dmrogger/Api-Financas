using ApiFinancas.Src.Domain.Common;

namespace ApiFinancas.Tests.Domain.Common
{
    public class EntityTests
    {
        // Entity é abstrata: estas classes existem só para os testes poderem instanciá-la
        private class EntidadeTeste : Entity
        {
            public EntidadeTeste() { }
            public EntidadeTeste(Guid id) : base(id) { }
            public void Atualizar() => MarcarComoAtualizada();
        }

        private class OutraEntidadeTeste : AggregateRoot
        {
            public OutraEntidadeTeste(Guid id) : base(id) { }
        }

        [Fact(DisplayName = "Deve gerar id e data de criação ao instanciar")]
        public void DeveGerarIdEDataDeCriacao()
        {
            var antes = DateTime.UtcNow;

            var entidade = new EntidadeTeste();

            var depois = DateTime.UtcNow;

            Assert.NotEqual(Guid.Empty, entidade.Id);
            Assert.InRange(entidade.DataCriacao, antes, depois);
            Assert.Null(entidade.DataUltimaAtualizacao);
        }

        [Fact(DisplayName = "Deve manter o id informado ao instanciar")]
        public void DeveManterIdInformado()
        {
            var id = Guid.NewGuid();

            var entidade = new EntidadeTeste(id);

            Assert.Equal(id, entidade.Id);
        }

        [Fact(DisplayName = "Deve registrar a data da última atualização")]
        public void DeveRegistrarDataDaUltimaAtualizacao()
        {
            var entidade = new EntidadeTeste();
            var antes = DateTime.UtcNow;

            entidade.Atualizar();

            var depois = DateTime.UtcNow;

            Assert.NotNull(entidade.DataUltimaAtualizacao);
            Assert.InRange(entidade.DataUltimaAtualizacao!.Value, antes, depois);
        }

        [Fact(DisplayName = "Deve ser igual quando for a mesma instância")]
        public void DeveSerIgualQuandoMesmaInstancia()
        {
            var entidade = new EntidadeTeste();

            Assert.True(entidade.Equals(entidade));
        }

        [Fact(DisplayName = "Deve ser igual quando tipo e id forem iguais")]
        public void DeveSerIgualQuandoTipoEIdIguais()
        {
            var id = Guid.NewGuid();

            var primeira = new EntidadeTeste(id);
            var segunda = new EntidadeTeste(id);

            Assert.True(primeira.Equals(segunda));
            Assert.Equal(primeira.GetHashCode(), segunda.GetHashCode());
        }

        [Fact(DisplayName = "Não deve ser igual quando ids forem diferentes")]
        public void NaoDeveSerIgualQuandoIdsDiferentes()
        {
            var primeira = new EntidadeTeste(Guid.NewGuid());
            var segunda = new EntidadeTeste(Guid.NewGuid());

            Assert.False(primeira.Equals(segunda));
        }

        [Fact(DisplayName = "Não deve ser igual quando tipos forem diferentes")]
        public void NaoDeveSerIgualQuandoTiposDiferentes()
        {
            var id = Guid.NewGuid();

            var primeira = new EntidadeTeste(id);
            var segunda = new OutraEntidadeTeste(id);

            Assert.False(primeira.Equals(segunda));
        }

        [Fact(DisplayName = "Não deve ser igual quando o id for vazio")]
        public void NaoDeveSerIgualQuandoIdVazio()
        {
            var primeira = new EntidadeTeste(Guid.Empty);
            var segunda = new EntidadeTeste(Guid.Empty);

            Assert.False(primeira.Equals(segunda));
        }

        [Fact(DisplayName = "Não deve ser igual a nulo ou a objeto que não é entidade")]
        public void NaoDeveSerIgualANuloOuOutroObjeto()
        {
            var entidade = new EntidadeTeste();

            Assert.False(entidade.Equals(null));
            Assert.False(entidade.Equals("texto"));
        }
    }
}
