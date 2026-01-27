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
            _movimentacaoRepository = movimentacaoRepository ?? throw new ArgumentNullException(nameof(movimentacaoRepository));
        }

        public async Task<AdicionarEntradaResponse> CriarEntradaAsync(AdicionarEntradaRequest request)
        {
            try
            {

                var movimentacao = new Movimentacao(
                    request.Valor,
                    request.DataOperacao,
                    request.idUsuario);

                var movimentacaoCriada = await _movimentacaoRepository.CriarAsync(movimentacao);

                return new AdicionarEntradaResponse
                {
                    MovimentacaoId = movimentacaoCriada.Id,
                    Sucesso = true,
                    Mensagem = "Entrada criada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new AdicionarEntradaResponse
                {
                    MovimentacaoId = Guid.Empty,
                    Sucesso = false,
                    Mensagem = $"Erro ao criar entrada: {ex.Message}"
                };
            }
        }

        public async Task<AdicionarSaidaResponse> CriarSaidaAsync(AdicionarSaidaRequest request)
        {
            try
            {

                var movimentacao = new Movimentacao(
                    request.Valor,
                    request.DataOperacao,
                    request.idUsuario);

                var movimentacaoCriada = await _movimentacaoRepository.CriarAsync(movimentacao);

                return new AdicionarSaidaResponse
                {
                    MovimentacaoId = movimentacaoCriada.Id,
                    Sucesso = true,
                    Mensagem = "Saída criada com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new AdicionarSaidaResponse
                {
                    MovimentacaoId = Guid.Empty,
                    Sucesso = false,
                    Mensagem = $"Erro ao criar saída: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<ObterMovimentacoesResponse>> ObterMovimentacoesAsync(
            Guid usuarioId,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null)
        {
            var movimentacoes = await _movimentacaoRepository.ObterPorUsuarioComFiltrosAsync(
                usuarioId,
                dataInicial,
                dataFinal);

            return movimentacoes.Select(m => new ObterMovimentacoesResponse
            {
                Id = m.Id,
                Valor = m.Valor,
                Data = m.Data,
                UsuarioId = m.UsuarioId
            });
        }

        public async Task<bool> DeletarMovimentacaoAsync(Guid movimentacaoId, Guid usuarioId)
        {
            var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(movimentacaoId);

            if (movimentacao == null)
                return false;

            // Validar se a movimentação pertence ao usuário
            if (movimentacao.UsuarioId != usuarioId)
                return false;

            return await _movimentacaoRepository.DeletarAsync(movimentacaoId);
        }

    }
}
