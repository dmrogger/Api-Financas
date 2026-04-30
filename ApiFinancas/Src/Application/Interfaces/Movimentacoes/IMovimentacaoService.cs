using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Movimentacoes
{
    /// <summary>
    /// Interface do serviço de movimentações financeiras
    /// </summary>
    public interface IMovimentacaoService
    {
        Task<Result<CadastrarMovimentacoesResponse>>CadastraMovimentacao(CadastraMovimentacoesRequest movimentacaoRequest);
        Task<Result<CadastrarMovimentacoesResponse>> ObterMovimentacoes(ObterMovimentacoesRequest obterMovimentacoesRequest);
        Task<Result<CadastrarMovimentacoesResponse>>DeletaMovimentacao(DeletarMovimentacoesRequest deletarMovimentacaoRequest);
    }
}
