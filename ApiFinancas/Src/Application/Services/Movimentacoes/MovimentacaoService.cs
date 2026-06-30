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

        public async Task<Result<List<CadastrarMovimentacoesResponse>>> ObterMovimentacoes(ObterMovimentacoesRequest obterMovimentacoesRequest)
        {
            if (obterMovimentacoesRequest.idUsuario == Guid.Empty)
                return Result<List<CadastrarMovimentacoesResponse>>.Fail("id de usuário não informado na requisição");

            var movimentacoesEncontradas = await _movimentacaoRepository.ObterPorUsuarioComFiltrosAsync
                (obterMovimentacoesRequest.idUsuario, 
                obterMovimentacoesRequest.DataInicial, 
                obterMovimentacoesRequest.DataFinal);

            var movimentacoes = new List<CadastrarMovimentacoesResponse>();

            foreach(var movimentacao in movimentacoesEncontradas)
            {
                movimentacoes.Add(new CadastrarMovimentacoesResponse
                {
                    MovimentacaoId = movimentacao.Id,
                    CategoriaId = movimentacao.CategoriaId,
                    Valor = movimentacao.Valor,
                    DataTransacao = movimentacao.Data
                });
            }

            return Result<List<CadastrarMovimentacoesResponse>>.Ok(movimentacoes);
        }
    }
}
