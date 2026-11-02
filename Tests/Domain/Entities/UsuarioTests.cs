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

        [Theory(DisplayName = "Não deve instanciar usuário com e-mail inválido")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDeveInstanciarUsuarioComEmailInvalido(string email)
        {
            var excecao = Assert.Throws<ArgumentException>(() =>
                new Usuario("Teste", email, "hash"));

            Assert.Equal("email", excecao.ParamName);
            Assert.StartsWith("O e-mail do usuário é obrigatório.", excecao.Message);
        }

        [Fact(DisplayName = "Deve instanciar usuário com dados válidos")]
        public void DeveInstanciarUsuarioComDadosValidos()
        {
            var usuario = new Usuario("  Teste da Silva  ", "  Teste@Gmail.COM ", "hash");

            Assert.Equal("Teste da Silva", usuario.Nome);
            Assert.Equal("teste@gmail.com", usuario.Email);
            Assert.Equal("hash", usuario.Senha);
        }

        [Theory(DisplayName = "Deve normalizar o e-mail")]
        [InlineData("teste@gmail.com", "teste@gmail.com")]
        [InlineData("TESTE@GMAIL.COM", "teste@gmail.com")]
        [InlineData("  Teste@Gmail.com  ", "teste@gmail.com")]
        public void DeveNormalizarEmail(string email, string emailEsperado)
        {
            Assert.Equal(emailEsperado, Usuario.NormalizarEmail(email));
        }

        [Fact(DisplayName = "Deve alterar o nome do usuário removendo os espaços")]
        public void DeveAlterarNome()
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            usuario.AlterarNome("  Nome Novo  ");

            Assert.Equal("Nome Novo", usuario.Nome);
        }

        [Theory(DisplayName = "Não deve alterar o nome com valor inválido")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDeveAlterarNomeComValorInvalido(string novoNome)
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            var excecao = Assert.Throws<ArgumentException>(() => usuario.AlterarNome(novoNome));

            Assert.Equal("novoNome", excecao.ParamName);
            Assert.StartsWith("O nome do usuário é obrigatório.", excecao.Message);
            Assert.Equal("Teste", usuario.Nome);
        }

        [Fact(DisplayName = "Deve alterar a senha do usuário")]
        public void DeveAlterarSenha()
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            usuario.AlterarSenha("hashNovo");

            Assert.Equal("hashNovo", usuario.Senha);
        }

        [Theory(DisplayName = "Não deve alterar a senha com hash inválido")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDeveAlterarSenhaComHashInvalido(string novaSenhaHash)
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            var excecao = Assert.Throws<ArgumentException>(() => usuario.AlterarSenha(novaSenhaHash));

            Assert.Equal("novaSenhaHash", excecao.ParamName);
            Assert.StartsWith("O hash da senha é obrigatório.", excecao.Message);
            Assert.Equal("hash", usuario.Senha);
        }
    }
}
