using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Categorias;
using ApiFinancas.Src.Application.Services.Categorias;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Categorias
{
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaRepository> _repositoryMock;
        private readonly CategoriaService _service;
        private readonly Guid _idUsuario = Guid.NewGuid();

        public CategoriaServiceTests()
        {
            _repositoryMock = new Mock<ICategoriaRepository>();
            _service = new CategoriaService(_repositoryMock.Object);
        }

        private static CategoriaRequest Request(string nome) => new CategoriaRequest { Nome = nome };

        [Fact(DisplayName = "Deve listar categorias padrão e do usuário")]
        public async Task DeveListarCategorias()
        {
            var categorias = new List<Categoria>
            {
                new Categoria("Alimentação", null),
                new Categoria("Pets", _idUsuario)
            };

            _repositoryMock.Setup(x => x.ListarVisiveisParaUsuarioAsync(_idUsuario)).ReturnsAsync(categorias);

            var result = await _service.ListarAsync(_idUsuario);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data!.Count);
            Assert.True(result.Data[0].Padrao);
            Assert.False(result.Data[1].Padrao);
            Assert.Equal("Pets", result.Data[1].Nome);
        }

        [Fact(DisplayName = "Deve criar categoria personalizada para o usuário")]
        public async Task DeveCriarCategoria()
        {
            _repositoryMock.Setup(x => x.ExisteNomeVisivelParaUsuarioAsync(_idUsuario, "Pets", null)).ReturnsAsync(false);

            var result = await _service.CriarAsync(Request("  Pets "), _idUsuario);

            Assert.True(result.Success);
            Assert.Equal("Pets", result.Data!.Nome);
            Assert.False(result.Data.Padrao);
            _repositoryMock.Verify(x => x.AdicionarAsync(It.Is<Categoria>(c =>
                c.Nome == "Pets" && c.UsuarioId == _idUsuario)), Times.Once);
        }

        [Fact(DisplayName = "Não deve criar categoria com nome já existente")]
        public async Task NaoDeveCriarCategoriaComNomeDuplicado()
        {
            _repositoryMock.Setup(x => x.ExisteNomeVisivelParaUsuarioAsync(_idUsuario, "Alimentação", null)).ReturnsAsync(true);

            var result = await _service.CriarAsync(Request("Alimentação"), _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.Conflito, result.TipoErro);
            Assert.Equal(CategoriaService.MensagemNomeDuplicado, result.Error);
            _repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact(DisplayName = "Deve renomear categoria do próprio usuário")]
        public async Task DeveEditarCategoriaPropria()
        {
            var categoria = new Categoria("Pets", _idUsuario);

            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);
            _repositoryMock.Setup(x => x.ExisteNomeVisivelParaUsuarioAsync(_idUsuario, "Animais", categoria.Id)).ReturnsAsync(false);

            var result = await _service.EditarAsync(categoria.Id, Request("Animais"), _idUsuario);

            Assert.True(result.Success);
            Assert.Equal("Animais", result.Data!.Nome);
            _repositoryMock.Verify(x => x.AtualizarAsync(categoria), Times.Once);
        }

        [Fact(DisplayName = "Não deve renomear para nome de outra categoria visível")]
        public async Task NaoDeveEditarParaNomeDuplicado()
        {
            var categoria = new Categoria("Pets", _idUsuario);

            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);
            _repositoryMock.Setup(x => x.ExisteNomeVisivelParaUsuarioAsync(_idUsuario, "Lazer", categoria.Id)).ReturnsAsync(true);

            var result = await _service.EditarAsync(categoria.Id, Request("Lazer"), _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.Conflito, result.TipoErro);
            Assert.Equal("Pets", categoria.Nome);
            _repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve editar categoria padrão")]
        public async Task NaoDeveEditarCategoriaPadrao()
        {
            var categoria = new Categoria("Alimentação", null);
            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);

            var result = await _service.EditarAsync(categoria.Id, Request("Comida"), _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.Proibido, result.TipoErro);
            Assert.Equal(CategoriaService.MensagemCategoriaPadrao, result.Error);
            _repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact(DisplayName = "Categoria de outro usuário deve ser tratada como não encontrada na edição")]
        public async Task NaoDeveEditarCategoriaDeOutroUsuario()
        {
            var categoria = new Categoria("Pets", Guid.NewGuid());
            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);

            var result = await _service.EditarAsync(categoria.Id, Request("Meus pets"), _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.NaoEncontrado, result.TipoErro);
            Assert.Equal(CategoriaService.MensagemNaoEncontrada, result.Error);
            _repositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve editar categoria inexistente")]
        public async Task NaoDeveEditarCategoriaInexistente()
        {
            _repositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Categoria?)null);

            var result = await _service.EditarAsync(Guid.NewGuid(), Request("Pets"), _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.NaoEncontrado, result.TipoErro);
        }

        [Fact(DisplayName = "Deve excluir categoria do próprio usuário")]
        public async Task DeveDeletarCategoriaPropria()
        {
            var categoria = new Categoria("Pets", _idUsuario);
            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);

            var result = await _service.DeletarAsync(categoria.Id, _idUsuario);

            Assert.True(result.Success);
            _repositoryMock.Verify(x => x.DeletarAsync(categoria), Times.Once);
        }

        [Fact(DisplayName = "Não deve excluir categoria padrão")]
        public async Task NaoDeveDeletarCategoriaPadrao()
        {
            var categoria = new Categoria("Moradia", null);
            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);

            var result = await _service.DeletarAsync(categoria.Id, _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.Proibido, result.TipoErro);
            _repositoryMock.Verify(x => x.DeletarAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve excluir categoria de outro usuário")]
        public async Task NaoDeveDeletarCategoriaDeOutroUsuario()
        {
            var categoria = new Categoria("Pets", Guid.NewGuid());
            _repositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);

            var result = await _service.DeletarAsync(categoria.Id, _idUsuario);

            Assert.False(result.Success);
            Assert.Equal(ETipoErro.NaoEncontrado, result.TipoErro);
            _repositoryMock.Verify(x => x.DeletarAsync(It.IsAny<Categoria>()), Times.Never);
        }
    }
}
