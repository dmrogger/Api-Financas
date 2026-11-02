using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Tests.Domain.Entities
{
    public class ContaTests
    {
        [Fact(DisplayName = "Deve instanciar conta com dados válidos")]
        public void DeveInstanciarContaComDadosValidos()
        {
            var tipoConta = new TipoConta();
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            var conta = new Conta("Conta Corrente", tipoConta, usuario);

            Assert.NotEqual(Guid.Empty, conta.Id);
            Assert.Equal("Conta Corrente", conta.NomeConta);
            Assert.Same(tipoConta, conta.TipoConta);
            Assert.Same(usuario, conta.Usuario);
        }

        [Theory(DisplayName = "Não deve instanciar conta com nome inválido")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDeveInstanciarContaComNomeInvalido(string nomeConta)
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new Conta(nomeConta, new TipoConta(), usuario));

            Assert.Equal("nomeConta", excecao.ParamName);
            Assert.StartsWith("Nome da conta é obrigatório!", excecao.Message);
        }
    }
}
