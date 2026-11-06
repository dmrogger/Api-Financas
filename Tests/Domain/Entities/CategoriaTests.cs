using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Tests.Domain.Entities
{
    public class CategoriaTests
    {
        [Fact(DisplayName = "Deve criar categoria personalizada vinculada ao usuário")]
        public void DeveCriarCategoriaPersonalizada()
        {
            var idUsuario = Guid.NewGuid();

            var categoria = new Categoria("  Pets  ", idUsuario);

            Assert.NotEqual(Guid.Empty, categoria.Id);
            Assert.Equal("Pets", categoria.Nome);
            Assert.Equal(idUsuario, categoria.UsuarioId);
            Assert.False(categoria.Padrao);
        }

        [Fact(DisplayName = "Categoria sem usuário deve ser padrão")]
        public void CategoriaSemUsuarioDeveSerPadrao()
        {
            var categoria = new Categoria("Alimentação", null);

            Assert.True(categoria.Padrao);
            Assert.Null(categoria.UsuarioId);
        }

        [Theory(DisplayName = "Não deve criar categoria com nome vazio")]
        [InlineData("")]
        [InlineData("   ")]
        public void NaoDeveCriarCategoriaComNomeVazio(string nome)
        {
            var excecao = Assert.Throws<ArgumentException>(() => new Categoria(nome, Guid.NewGuid()));

            Assert.Equal("nome", excecao.ParamName);
        }

        [Fact(DisplayName = "Não deve criar categoria com nome acima do limite")]
        public void NaoDeveCriarCategoriaComNomeLongo()
        {
            var nome = new string('a', Categoria.TamanhoMaximoNome + 1);

            var excecao = Assert.Throws<ArgumentException>(() => new Categoria(nome, Guid.NewGuid()));

            Assert.Equal("nome", excecao.ParamName);
        }

        [Fact(DisplayName = "Não deve criar categoria com id de usuário vazio")]
        public void NaoDeveCriarCategoriaComUsuarioVazio()
        {
            var excecao = Assert.Throws<ArgumentException>(() => new Categoria("Pets", Guid.Empty));

            Assert.Equal("usuarioId", excecao.ParamName);
        }

        [Fact(DisplayName = "Categoria padrão deve ser visível para qualquer usuário")]
        public void CategoriaPadraoDeveSerVisivelParaTodos()
        {
            var categoria = new Categoria("Moradia", null);

            Assert.True(categoria.VisivelPara(Guid.NewGuid()));
            Assert.False(categoria.PertenceAo(Guid.NewGuid()));
        }

        [Fact(DisplayName = "Categoria personalizada deve ser visível só para o dono")]
        public void CategoriaPersonalizadaDeveSerVisivelSoParaODono()
        {
            var dono = Guid.NewGuid();
            var categoria = new Categoria("Pets", dono);

            Assert.True(categoria.VisivelPara(dono));
            Assert.True(categoria.PertenceAo(dono));
            Assert.False(categoria.VisivelPara(Guid.NewGuid()));
        }

        [Fact(DisplayName = "Deve renomear categoria removendo espaços")]
        public void DeveRenomearCategoria()
        {
            var categoria = new Categoria("Pets", Guid.NewGuid());

            categoria.Renomear("  Animais  ");

            Assert.Equal("Animais", categoria.Nome);
        }

        [Fact(DisplayName = "Não deve renomear categoria para nome vazio")]
        public void NaoDeveRenomearParaNomeVazio()
        {
            var categoria = new Categoria("Pets", Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => categoria.Renomear(" "));
            Assert.Equal("Pets", categoria.Nome);
        }
    }
}
