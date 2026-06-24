using ApiFinancas.Src.Application.Services.Movimentacoes;
using ApiFinancas.Src.Domain.Interfaces;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Movimentacao
{
    public class MovimentacaoServiceTests
    {
        private readonly Mock<IMovimentacaoRepository> _repositoryMock;
        private readonly MovimentacaoService _movimentacaoService;

        public MovimentacaoServiceTests()
        {
            _repositoryMock = new Mock<IMovimentacaoRepository>();
            _movimentacaoService = new MovimentacaoService( _repositoryMock.Object );
        }


    }
}
