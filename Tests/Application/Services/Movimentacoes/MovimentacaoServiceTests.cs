using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.Services.Movimentacoes;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Enums;
using ApiFinancas.Src.Domain.Interfaces;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Movimentacoes
{
    public class MovimentacaoServiceTests
    {
        private readonly Mock<IMovimentacaoRepository> _repositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly MovimentacaoService _movimentacaoService;

        public MovimentacaoServiceTests()
        {
            _repositoryMock = new Mock<IMovimentacaoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _movimentacaoService = new MovimentacaoService(_repositoryMock.Object, _categoriaRepositoryMock.Object);
        }

        [Theory(DisplayName = "Não deve criar movimentação quando valor menor ou igual a zero")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-120.67)]
        public async Task NaoDeveCadastrarMovimentacaoQuandoValorInvalido(decimal valor)
        {

            var idUsuario = Guid.NewGuid();

            var request = new CadastraMovimentacoesRequest(valor,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           ETipoMovimentacao.Entrada);

            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);


            Assert.False(result.Success);
            Assert.Equal("O valor da movimentação deve ser maior que zero.", result.Error);
            _repositoryMock.Verify(x => x.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }


        [Theory(DisplayName = "Deve criar movimentação quando valor válido")]
        [InlineData(0.2)]
        [InlineData(1)]
        [InlineData(132.76)]
        public async Task DeveCadastrarMovimentacaoQuandoValorValido(decimal valor)
        {

            var idUsuario = Guid.NewGuid();
            var idCriacao = Guid.NewGuid();

            var request = new CadastraMovimentacoesRequest(valor,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           ETipoMovimentacao.Entrada);

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>())).ReturnsAsync(idCriacao);


            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);


            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Equal(idCriacao, result.Data!.MovimentacaoId);

            _repositoryMock.Verify(x => x.CriarAsync(It.Is<Movimentacao>(m =>
                m.Valor == valor &&
                m.UsuarioId == idUsuario &&
                m.Tipo == ETipoMovimentacao.Entrada &&
                m.Descricao == "DescricaoDeTeste")), Times.Once);
        }


        [Fact(DisplayName = "Deve tratar data sem fuso como UTC")]
        public async Task DeveTratarDataSemfusoComoUTC()
        {
            var data = new DateTime(2026,10,27,14,30,0, DateTimeKind.Unspecified);
            var request = new CadastraMovimentacoesRequest(100, "Teste", null, ETipoMovimentacao.Entrada)
            {
                DataOperacao = data
            };

            Movimentacao? EntradaRepositoryMock = null;

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
                .Callback<Movimentacao>(m => EntradaRepositoryMock = m)
                .ReturnsAsync(Guid.NewGuid());

            await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            Assert.NotNull(EntradaRepositoryMock);
            Assert.Equal(DateTimeKind.Utc, EntradaRepositoryMock!.Data.Kind);
            Assert.Equal(data.Ticks, EntradaRepositoryMock.Data.Ticks);
        }

        [Theory(DisplayName = "Não deve criar movimentação quando tipo inválido")]
        [InlineData((ETipoMovimentacao)0)]
        [InlineData((ETipoMovimentacao)3)]
        [InlineData((ETipoMovimentacao)99)]
        public async Task NaoDeveCadastrarMovimentacaoQuandoTipoInvalido(ETipoMovimentacao tipo)
        {

            var idUsuario = Guid.NewGuid();

            var request = new CadastraMovimentacoesRequest(100,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           tipo);


            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Tipo de movimentação inválido. Use 1 (Entrada) ou 2 (Saída).", result.Error);
            _repositoryMock.Verify(x => x.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar erro desconhecido quando repositório não retornar id")]
        public async Task DeveRetornarErroDesconhecidoQuandoRepositorioRetornarIdVazio()
        {

            var idUsuario = Guid.NewGuid();

            var request = new CadastraMovimentacoesRequest(100,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           ETipoMovimentacao.Entrada);

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>())).ReturnsAsync(Guid.Empty);


            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);


            Assert.False(result.Success);
            Assert.Equal("Erro desconhecido ao cadastrar movimentação", result.Error);
            Assert.Null(result.Data);
        }

        [Fact(DisplayName = "Deve criar movimentação de saída repassando a categoria")]
        public async Task DeveCadastrarMovimentacaoDeSaidaComCategoria()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var idCriacao = Guid.NewGuid();
            var categoria = new Categoria("Alimentação", null);

            var request = new CadastraMovimentacoesRequest(50,
                                                           "Mercado",
                                                           categoria.Id,
                                                           ETipoMovimentacao.Saida);

            _categoriaRepositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);
            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>())).ReturnsAsync(idCriacao);


            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);


            Assert.True(result.Success);
            Assert.Equal(idCriacao, result.Data!.MovimentacaoId);

            _repositoryMock.Verify(x => x.CriarAsync(It.Is<Movimentacao>(m =>
                m.Tipo == ETipoMovimentacao.Saida &&
                m.CategoriaId == categoria.Id &&
                m.UsuarioId == idUsuario)), Times.Once);
        }

        [Fact(DisplayName = "Deve manter data já em UTC")]
        public async Task DeveManterDataUtc()
        {

            var data = new DateTime(2026, 10, 27, 14, 30, 0, DateTimeKind.Utc);
            var request = new CadastraMovimentacoesRequest(100, "Teste", null, ETipoMovimentacao.Entrada)
            {
                DataOperacao = data
            };

            Movimentacao? EntradaRepositoryMock = null;

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
                .Callback<Movimentacao>(m => EntradaRepositoryMock = m)
                .ReturnsAsync(Guid.NewGuid());


            await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            Assert.NotNull(EntradaRepositoryMock);
            Assert.Equal(DateTimeKind.Utc, EntradaRepositoryMock!.Data.Kind);
            Assert.Equal(data, EntradaRepositoryMock.Data);
        }

        [Fact(DisplayName = "Deve converter data local para UTC")]
        public async Task DeveConverterDataLocalParaUtc()
        {

            var data = new DateTime(2026, 10, 27, 14, 30, 0, DateTimeKind.Local);
            var request = new CadastraMovimentacoesRequest(100, "Teste", null, ETipoMovimentacao.Entrada)
            {
                DataOperacao = data
            };

            Movimentacao? EntradaRepositoryMock = null;

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
                .Callback<Movimentacao>(m => EntradaRepositoryMock = m)
                .ReturnsAsync(Guid.NewGuid());


            await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            Assert.NotNull(EntradaRepositoryMock);
            Assert.Equal(DateTimeKind.Utc, EntradaRepositoryMock!.Data.Kind);
            Assert.Equal(data.ToUniversalTime(), EntradaRepositoryMock.Data);
        }

        [Fact(DisplayName = "Deve usar a data atual em UTC quando data não informada")]
        public async Task DeveUsarDataAtualQuandoDataNaoInformada()
        {

            var request = new CadastraMovimentacoesRequest(100, "Teste", null, ETipoMovimentacao.Entrada);

            Movimentacao? EntradaRepositoryMock = null;

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
                .Callback<Movimentacao>(m => EntradaRepositoryMock = m)
                .ReturnsAsync(Guid.NewGuid());

            var antes = DateTime.UtcNow;


            await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            var depois = DateTime.UtcNow;

            Assert.NotNull(EntradaRepositoryMock);
            Assert.Equal(DateTimeKind.Utc, EntradaRepositoryMock!.Data.Kind);
            Assert.InRange(EntradaRepositoryMock.Data, antes, depois);
        }

        [Fact(DisplayName = "Deve lançar exceção quando id do usuário for vazio")]
        public async Task DeveLancarExcecaoQuandoIdUsuarioVazio()
        {

            var request = new CadastraMovimentacoesRequest(100,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           ETipoMovimentacao.Entrada);


            await Assert.ThrowsAsync<ArgumentException>(() =>
                _movimentacaoService.CadastraMovimentacao(request, Guid.Empty));

            _repositoryMock.Verify(x => x.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Deve propagar exceção quando repositório falhar")]
        public async Task DevePropagarExcecaoQuandoRepositorioFalhar()
        {

            var request = new CadastraMovimentacoesRequest(100,
                                                           "DescricaoDeTeste",
                                                           null,
                                                           ETipoMovimentacao.Entrada);

            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
                .ThrowsAsync(new Exception("Erro no banco"));


            var excecao = await Assert.ThrowsAsync<Exception>(() =>
                _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid()));

            Assert.Equal("Erro no banco", excecao.Message);
        }

        [Fact(DisplayName = "Deve considerar o dia inteiro quando data final sem horário")]
        public async Task DeveConsiderarDiaInteiroQuandoDataFinalSemHorario()
        {

            var request = new ObterMovimentacoesRequest
            {
                DataFinal = new DateTime(2026, 10, 31)
            };

            DateTime? dataFinalRecebida = null;

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Callback<Guid, DateTime?, DateTime?>((_, _, fim) => dataFinalRecebida = fim)
                .ReturnsAsync(new List<Movimentacao>());

            var result = await _movimentacaoService.ObterMovimentacoes(request, Guid.NewGuid());


            Assert.True(result.Success);
            Assert.Equal(new DateTime(2026, 10, 31).AddDays(1).AddTicks(-1), dataFinalRecebida);
        }

        [Fact(DisplayName = "Deve manter data final quando informada com horário")]
        public async Task DeveManterDataFinalQuandoInformadaComHorario()
        {

            var dataFinal = new DateTime(2026, 10, 31, 15, 0, 0);
            var request = new ObterMovimentacoesRequest
            {
                DataFinal = dataFinal
            };

            DateTime? dataFinalRecebida = null;

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Callback<Guid, DateTime?, DateTime?>((_, _, fim) => dataFinalRecebida = fim)
                .ReturnsAsync(new List<Movimentacao>());


            var result = await _movimentacaoService.ObterMovimentacoes(request, Guid.NewGuid());


            Assert.True(result.Success);
            Assert.Equal(dataFinal, dataFinalRecebida);
        }

        [Fact(DisplayName = "Não deve consultar quando data inicial maior que a final")]
        public async Task NaoDeveConsultarQuandoDataInicialMaiorQueFinal()
        {

            var request = new ObterMovimentacoesRequest
            {
                DataInicial = new DateTime(2026, 11, 1),
                DataFinal = new DateTime(2026, 10, 31)
            };


            var result = await _movimentacaoService.ObterMovimentacoes(request, Guid.NewGuid());


            Assert.False(result.Success);
            Assert.Equal("A data inicial não pode ser maior que a data final.", result.Error);
            _repositoryMock.Verify(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
        }

        [Fact(DisplayName = "Deve consultar quando data inicial e final forem o mesmo dia")]
        public async Task DeveConsultarQuandoDataInicialEFinalMesmoDia()
        {

            var request = new ObterMovimentacoesRequest
            {
                DataInicial = new DateTime(2026, 10, 31),
                DataFinal = new DateTime(2026, 10, 31)
            };

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Movimentacao>());


            var result = await _movimentacaoService.ObterMovimentacoes(request, Guid.NewGuid());


            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact(DisplayName = "Deve consultar sem filtros de data quando não informadas")]
        public async Task DeveConsultarSemFiltrosQuandoDatasNaoInformadas()
        {
  
            var idUsuario = Guid.NewGuid();
            var request = new ObterMovimentacoesRequest();

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Movimentacao>());


            var result = await _movimentacaoService.ObterMovimentacoes(request, idUsuario);


            Assert.True(result.Success);
            _repositoryMock.Verify(x => x.ObterPorUsuarioComFiltrosAsync(idUsuario, null, null), Times.Once);
        }

        [Fact(DisplayName = "Deve converter datas da consulta para UTC")]
        public async Task DeveConverterDatasDaConsultaParaUtc()
        {

            var request = new ObterMovimentacoesRequest
            {
                DataInicial = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Unspecified),
                DataFinal = new DateTime(2026, 10, 31, 0, 0, 0, DateTimeKind.Unspecified)
            };

            DateTime? dataInicialRecebida = null;
            DateTime? dataFinalRecebida = null;

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Callback<Guid, DateTime?, DateTime?>((_, inicio, fim) =>
                {
                    dataInicialRecebida = inicio;
                    dataFinalRecebida = fim;
                })
                .ReturnsAsync(new List<Movimentacao>());


            await _movimentacaoService.ObterMovimentacoes(request, Guid.NewGuid());


            Assert.Equal(DateTimeKind.Utc, dataInicialRecebida!.Value.Kind);
            Assert.Equal(DateTimeKind.Utc, dataFinalRecebida!.Value.Kind);
        }

        [Fact(DisplayName = "Deve retornar as movimentações do usuário convertidas para response")]
        public async Task DeveRetornarMovimentacoesConvertidasParaResponse()
        {

            var idUsuario = Guid.NewGuid();
            var data = new DateTime(2026, 10, 27, 14, 30, 0, DateTimeKind.Utc);
            var idCategoria = Guid.NewGuid();

            var movimentacoes = new List<Movimentacao>
            {
                new Movimentacao(100, data, idUsuario, idCategoria, ETipoMovimentacao.Entrada, "Salário"),
                new Movimentacao(35.5m, data, idUsuario, null, ETipoMovimentacao.Saida, "Lanche")
            };

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(idUsuario, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(movimentacoes);


            var result = await _movimentacaoService.ObterMovimentacoes(new ObterMovimentacoesRequest(), idUsuario);


            Assert.True(result.Success);
            Assert.Equal(2, result.Data!.Count);

            var primeira = result.Data[0];
            Assert.Equal(movimentacoes[0].Id, primeira.MovimentacaoId);
            Assert.Equal(100, primeira.Valor);
            Assert.Equal(data, primeira.DataTransacao);
            Assert.Equal(ETipoMovimentacao.Entrada, primeira.Tipo);
            Assert.Equal("Salário", primeira.Descricao);
            Assert.Equal(idCategoria, primeira.CategoriaId);

            var segunda = result.Data[1];
            Assert.Equal(35.5m, segunda.Valor);
            Assert.Equal(ETipoMovimentacao.Saida, segunda.Tipo);
            Assert.Null(segunda.CategoriaId);
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando usuário não tiver movimentações")]
        public async Task DeveRetornarListaVaziaQuandoNaoHouverMovimentacoes()
        {

            _repositoryMock.Setup(x => x.ObterPorUsuarioComFiltrosAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Movimentacao>());


            var result = await _movimentacaoService.ObterMovimentacoes(new ObterMovimentacoesRequest(), Guid.NewGuid());


            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data!);
        }

        [Fact(DisplayName = "Deve excluir movimentação do próprio usuário")]
        public async Task DeveExcluirMovimentacaoDoProprioUsuario()
        {
            var idUsuario = Guid.NewGuid();
            var movimentacao = new Movimentacao(100, null, idUsuario, null, ETipoMovimentacao.Entrada, "Teste");

            _repositoryMock.Setup(x => x.ObterPorIdAsync(movimentacao.Id)).ReturnsAsync(movimentacao);

            var result = await _movimentacaoService.DeletaMovimentacao(movimentacao.Id, idUsuario);

            Assert.True(result.Success);
            _repositoryMock.Verify(x => x.DeletarAsync(movimentacao), Times.Once);
        }

        [Fact(DisplayName = "Não deve excluir movimentação inexistente")]
        public async Task NaoDeveExcluirMovimentacaoInexistente()
        {
            _repositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Movimentacao?)null);

            var result = await _movimentacaoService.DeletaMovimentacao(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Movimentação não encontrada.", result.Error);
            _repositoryMock.Verify(x => x.DeletarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve excluir movimentação de outro usuário")]
        public async Task NaoDeveExcluirMovimentacaoDeOutroUsuario()
        {
            var donoDaMovimentacao = Guid.NewGuid();
            var outroUsuario = Guid.NewGuid();
            var movimentacao = new Movimentacao(100, null, donoDaMovimentacao, null, ETipoMovimentacao.Saida, "Teste");

            _repositoryMock.Setup(x => x.ObterPorIdAsync(movimentacao.Id)).ReturnsAsync(movimentacao);

            var result = await _movimentacaoService.DeletaMovimentacao(movimentacao.Id, outroUsuario);

            // Mesma resposta de "não encontrada", para não revelar que o ID existe
            Assert.False(result.Success);
            Assert.Equal("Movimentação não encontrada.", result.Error);
            _repositoryMock.Verify(x => x.DeletarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Deve aceitar categoria personalizada do próprio usuário")]
        public async Task DeveCadastrarComCategoriaDoProprioUsuario()
        {
            var idUsuario = Guid.NewGuid();
            var categoria = new Categoria("Pets", idUsuario);
            var request = new CadastraMovimentacoesRequest(80, "Ração", categoria.Id, ETipoMovimentacao.Saida);

            _categoriaRepositoryMock.Setup(x => x.ObterPorIdAsync(categoria.Id)).ReturnsAsync(categoria);
            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>())).ReturnsAsync(Guid.NewGuid());

            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);

            Assert.True(result.Success);
            _repositoryMock.Verify(x => x.CriarAsync(It.Is<Movimentacao>(m => m.CategoriaId == categoria.Id)), Times.Once);
        }

        [Fact(DisplayName = "Não deve cadastrar movimentação com categoria inexistente")]
        public async Task NaoDeveCadastrarComCategoriaInexistente()
        {
            var request = new CadastraMovimentacoesRequest(80, "Teste", Guid.NewGuid(), ETipoMovimentacao.Saida);

            _categoriaRepositoryMock.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Categoria?)null);

            var result = await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal(MovimentacaoService.MensagemCategoriaInvalida, result.Error);
            _repositoryMock.Verify(x => x.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve cadastrar movimentação com categoria de outro usuário")]
        public async Task NaoDeveCadastrarComCategoriaDeOutroUsuario()
        {
            var categoriaDeOutro = new Categoria("Pets", Guid.NewGuid());
            var request = new CadastraMovimentacoesRequest(80, "Teste", categoriaDeOutro.Id, ETipoMovimentacao.Saida);

            _categoriaRepositoryMock.Setup(x => x.ObterPorIdAsync(categoriaDeOutro.Id)).ReturnsAsync(categoriaDeOutro);

            var result = await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            // Mesma mensagem da categoria inexistente, para não revelar que o ID existe
            Assert.False(result.Success);
            Assert.Equal(MovimentacaoService.MensagemCategoriaInvalida, result.Error);
            _repositoryMock.Verify(x => x.CriarAsync(It.IsAny<Movimentacao>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve consultar categoria quando não informada")]
        public async Task NaoDeveConsultarCategoriaQuandoNaoInformada()
        {
            var request = new CadastraMovimentacoesRequest(80, "Teste", null, ETipoMovimentacao.Entrada);
            _repositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimentacao>())).ReturnsAsync(Guid.NewGuid());

            var result = await _movimentacaoService.CadastraMovimentacao(request, Guid.NewGuid());

            Assert.True(result.Success);
            _categoriaRepositoryMock.Verify(x => x.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
