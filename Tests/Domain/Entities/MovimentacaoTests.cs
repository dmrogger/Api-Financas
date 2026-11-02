using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Enums;

namespace ApiFinancas.Tests.Domain.Entities
{
    public class MovimentacaoTests
    {
        [Theory(DisplayName = "Deve instanciar movimentação com dados válidos")]
        [InlineData(200.00, ETipoMovimentacao.Entrada)]
        [InlineData(0.01, ETipoMovimentacao.Saida)]
        [InlineData(1500.50, ETipoMovimentacao.Saida)]
        public void DeveInstanciarMovimentacaoComDadosValidos(decimal valor, ETipoMovimentacao tipoMovimentacao)
        {

            var idUsuario = Guid.NewGuid();


            var movimentacao = new Movimentacao(valor, null, idUsuario, 3, tipoMovimentacao, "Mercado");


            Assert.NotEqual(Guid.Empty, movimentacao.Id);
            Assert.Equal(valor, movimentacao.Valor);
            Assert.Equal(idUsuario, movimentacao.UsuarioId);
            Assert.Equal(3, movimentacao.CategoriaId);
            Assert.Equal(tipoMovimentacao, movimentacao.Tipo);
            Assert.Equal("Mercado", movimentacao.Descricao);
        }

        [Theory(DisplayName = "Não deve instanciar movimentacao com dados inválidos")]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(0.00)]
        public void NaoDeveinstanciarMovimentacaoComDadosInvalidos(decimal valor)
        {
            var excecao = Assert.Throws<ArgumentException>(() =>
                new Movimentacao(valor, null, Guid.NewGuid(), null, ETipoMovimentacao.Entrada, ""));

            Assert.Equal("valor", excecao.ParamName);
            Assert.StartsWith("O valor da movimentação deve ser maior que zero.", excecao.Message);
        }

        [Theory(DisplayName = "Não deve instanciar movimentacão com tipo inválido")]
        [InlineData((ETipoMovimentacao)0)]
        [InlineData((ETipoMovimentacao)32)]
        [InlineData((ETipoMovimentacao)9999)]
        public void NaoDeveInstanciarMovimentacaoComTipoInvalido(ETipoMovimentacao tipoDeMovimentacao)
        {
            var excecao = Assert.Throws<ArgumentException>(() =>
            new Movimentacao(200, null, Guid.NewGuid(), null, tipoDeMovimentacao, ""));

            Assert.Equal("tipo", excecao.ParamName);
            Assert.StartsWith("O tipo da movimentação é inválido.", excecao.Message);
        }


    }
}
