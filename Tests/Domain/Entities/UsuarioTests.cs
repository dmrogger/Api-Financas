using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Tests.Domain.Entities
{
    public class UsuarioTests
    {
        [Theory(DisplayName = "Não deve instanciar usuário com nomes inválido")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDeveInstanciarUsuarioComNomesInvalidos(string nome)
        {
            var excecao = Assert.Throws<ArgumentException>(() =>
                new Usuario(nome,"teste@gmail.com","teste123@"));

            Assert.Equal("nome", excecao.ParamName);
            Assert.StartsWith("O nome do usuário é obrigatório.", excecao.Message);
        }


    }
}
