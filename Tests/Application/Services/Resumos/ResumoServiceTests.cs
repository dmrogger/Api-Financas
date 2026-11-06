using ApiFinancas.Src.Application.DTOs.Requests.Resumos;
using ApiFinancas.Src.Application.Services.Resumos;
using ApiFinancas.Src.Domain.Consultas;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Resumos
{
    public class ResumoServiceTests
    {
        // Fuso fixo UTC-3 (como Brasília hoje), criado no teste para não depender do sistema operacional
        private static readonly TimeZoneInfo FusoBrasilia =
            TimeZoneInfo.CreateCustomTimeZone("Teste/UTC-3", TimeSpan.FromHours(-3), "UTC-3", "UTC-3");

        private readonly Mock<IMovimentacaoRepository> _movimentacaoRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly ResumoService _service;
        private readonly Guid _idUsuario = Guid.NewGuid();

        public ResumoServiceTests()
        {
            _movimentacaoRepositoryMock = new Mock<IMovimentacaoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _service = new ResumoService(_movimentacaoRepositoryMock.Object, _categoriaRepositoryMock.Object, FusoBrasilia);

            _categoriaRepositoryMock.Setup(x => x.ListarVisiveisParaUsuarioAsync(_idUsuario))
                .ReturnsAsync(new List<Categoria>());
        }

        private static ObterResumoMensalRequest Request(int ano, int mes) => new ObterResumoMensalRequest { Ano = ano, Mes = mes };

        [Fact(DisplayName = "Deve calcular os limites do mês no fuso configurado")]
        public void DeveCalcularLimitesDoMesNoFuso()
        {
            var (inicio, fim) = _service.ObterLimitesDoMesEmUtc(2026, 11);

            // 01/11 00:00 em UTC-3 = 01/11 03:00 UTC
            Assert.Equal(new DateTime(2026, 11, 1, 3, 0, 0, DateTimeKind.Utc), inicio);
            Assert.Equal(new DateTime(2026, 12, 1, 3, 0, 0, DateTimeKind.Utc), fim);
            Assert.Equal(DateTimeKind.Utc, inicio.Kind);
        }

        [Fact(DisplayName = "Dezembro deve terminar no início de janeiro do ano seguinte")]
        public void DezembroDeveVirarOAno()
        {
            var (_, fim) = _service.ObterLimitesDoMesEmUtc(2026, 12);

            Assert.Equal(new DateTime(2027, 1, 1, 3, 0, 0, DateTimeKind.Utc), fim);
        }

        [Fact(DisplayName = "Deve consultar o repositório com os limites do mês em UTC")]
        public async Task DeveConsultarRepositorioComLimitesEmUtc()
        {
            DateTime? inicioRecebido = null;
            DateTime? fimRecebido = null;

            _movimentacaoRepositoryMock
                .Setup(x => x.ObterTotaisPorCategoriaAsync(_idUsuario, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Callback<Guid, DateTime, DateTime>((_, inicio, fim) =>
                {
                    inicioRecebido = inicio;
                    fimRecebido = fim;
                })
                .ReturnsAsync(new List<TotalPorCategoria>());

            await _service.ObterResumoMensalAsync(Request(2026, 2), _idUsuario);

            Assert.Equal(new DateTime(2026, 2, 1, 3, 0, 0, DateTimeKind.Utc), inicioRecebido);
            Assert.Equal(new DateTime(2026, 3, 1, 3, 0, 0, DateTimeKind.Utc), fimRecebido);
        }

        [Fact(DisplayName = "Deve somar totais, calcular saldo e nomear as categorias")]
        public async Task DeveMontarResumoComTotaisESaldo()
        {
            var moradia = new Categoria("Moradia", null);
            var salario = new Categoria("Salário", null);

            _categoriaRepositoryMock.Setup(x => x.ListarVisiveisParaUsuarioAsync(_idUsuario))
                .ReturnsAsync(new List<Categoria> { moradia, salario });

            _movimentacaoRepositoryMock
                .Setup(x => x.ObterTotaisPorCategoriaAsync(_idUsuario, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TotalPorCategoria>
                {
                    new TotalPorCategoria(salario.Id, 5000m, 0m),
                    new TotalPorCategoria(moradia.Id, 0m, 1800m),
                    new TotalPorCategoria(null, 0m, 95.50m)
                });

            var result = await _service.ObterResumoMensalAsync(Request(2026, 11), _idUsuario);

            Assert.True(result.Success);
            var resumo = result.Data!;
            Assert.Equal(2026, resumo.Ano);
            Assert.Equal(11, resumo.Mes);
            Assert.Equal(5000m, resumo.TotalEntradas);
            Assert.Equal(1895.50m, resumo.TotalSaidas);
            Assert.Equal(3104.50m, resumo.Saldo);

            // Ordenado pelas maiores saídas
            Assert.Equal("Moradia", resumo.Categorias[0].Nome);
            Assert.Equal(ResumoService.NomeSemCategoria, resumo.Categorias[1].Nome);
            Assert.Null(resumo.Categorias[1].CategoriaId);
            Assert.Equal("Salário", resumo.Categorias[2].Nome);
        }

        [Fact(DisplayName = "Mês sem movimentações deve retornar totais zerados")]
        public async Task MesSemMovimentacoesDeveRetornarZerado()
        {
            _movimentacaoRepositoryMock
                .Setup(x => x.ObterTotaisPorCategoriaAsync(_idUsuario, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TotalPorCategoria>());

            var result = await _service.ObterResumoMensalAsync(Request(2026, 11), _idUsuario);

            Assert.True(result.Success);
            Assert.Equal(0m, result.Data!.TotalEntradas);
            Assert.Equal(0m, result.Data.TotalSaidas);
            Assert.Equal(0m, result.Data.Saldo);
            Assert.Empty(result.Data.Categorias);
        }

        [Theory(DisplayName = "Não deve gerar resumo com mês ou ano inválido")]
        [InlineData(2026, 0)]
        [InlineData(2026, 13)]
        [InlineData(1999, 5)]
        [InlineData(2101, 5)]
        public async Task NaoDeveGerarResumoComPeriodoInvalido(int ano, int mes)
        {
            var result = await _service.ObterResumoMensalAsync(Request(ano, mes), _idUsuario);

            Assert.False(result.Success);
            _movimentacaoRepositoryMock.Verify(x => x.ObterTotaisPorCategoriaAsync(
                It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
