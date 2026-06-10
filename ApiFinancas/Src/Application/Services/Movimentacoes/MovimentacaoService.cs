using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;
using ApiFinancas.Src.Application.Interfaces.Movimentacoes;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Movimentacoes
{
    /// <summary>
    /// Serviço de movimentações financeiras
    /// </summary>
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly IMovimentacaoRepository _movimentacaoRepository;

        public MovimentacaoService(IMovimentacaoRepository movimentacaoRepository)
        {
            _movimentacaoRepository = movimentacaoRepository;
        }
        public async Task<Result<CadastrarMovimentacoesResponse>> CadastraMovimentacao(CadastraMovimentacoesRequest movimentacaoRequest)
        {
            if (movimentacaoRequest.Valor < 0)
                return Result<CadastrarMovimentacoesResponse>.Fail("Erro ao cadastrar movimentação");

           var movimentacao = new Movimentacao(movimentacaoRequest.Valor,movimentacaoRequest.DataRequisicao,movimentacaoRequest.idUsuario, movimentacaoRequest.CategoriaId);

           var movimentacaoCadastrada = await _movimentacaoRepository.CriarAsync(movimentacao);

            if (movimentacaoCadastrada == Guid.Empty)
                return Result<CadastrarMovimentacoesResponse>.Fail("Erro desconhecido ao cadastrar movimentação");

            var response = new CadastrarMovimentacoesResponse(movimentacaoCadastrada);

            return Result<CadastrarMovimentacoesResponse>.Ok(response);
        }

        public Task<Result<CadastrarMovimentacoesResponse>> DeletaMovimentacao(DeletarMovimentacoesRequest deletarMovimentacaoRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CadastrarMovimentacoesResponse>> ObterMovimentacoes(ObterMovimentacoesRequest obterMovimentacoesRequest)
        {
            throw new NotImplementedException();
        }
    }
}
