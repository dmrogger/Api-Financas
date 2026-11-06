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
        public const string MensagemCategoriaInvalida = "Categoria inválida.";

        private readonly IMovimentacaoRepository _movimentacaoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public MovimentacaoService(IMovimentacaoRepository movimentacaoRepository, ICategoriaRepository categoriaRepository)
        {
            _movimentacaoRepository = movimentacaoRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Result<CadastrarMovimentacoesResponse>> CadastraMovimentacao(CadastraMovimentacoesRequest movimentacaoRequest, Guid idUsuario)
        {
            if (movimentacaoRequest.Valor <= 0)
                return Result<CadastrarMovimentacoesResponse>.Fail("O valor da movimentação deve ser maior que zero.");

            if (!Enum.IsDefined(movimentacaoRequest.Tipo))
                return Result<CadastrarMovimentacoesResponse>.Fail("Tipo de movimentação inválido. Use 1 (Entrada) ou 2 (Saída).");

            if (movimentacaoRequest.CategoriaId.HasValue)
            {
                var categoria = await _categoriaRepository.ObterPorIdAsync(movimentacaoRequest.CategoriaId.Value);

                // Mesma mensagem para categoria inexistente ou de outro usuário, para não revelar que o ID existe
                if (categoria == null || !categoria.VisivelPara(idUsuario))
                    return Result<CadastrarMovimentacoesResponse>.Fail(MensagemCategoriaInvalida);
            }

            var data = movimentacaoRequest.DataOperacao.HasValue
                ? ParaUtc(movimentacaoRequest.DataOperacao.Value)
                : (DateTime?)null;

            var movimentacao = new Movimentacao(
                movimentacaoRequest.Valor,
                data,
                idUsuario,
                movimentacaoRequest.CategoriaId,
                movimentacaoRequest.Tipo,
                movimentacaoRequest.Descricao);

            var movimentacaoCadastrada = await _movimentacaoRepository.CriarAsync(movimentacao);

            if (movimentacaoCadastrada == Guid.Empty)
                return Result<CadastrarMovimentacoesResponse>.Fail("Erro desconhecido ao cadastrar movimentação");

            return Result<CadastrarMovimentacoesResponse>.Ok(new CadastrarMovimentacoesResponse(movimentacaoCadastrada));
        }

        public async Task<Result<string>> DeletaMovimentacao(Guid idMovimentacao, Guid idUsuario)
        {
            var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(idMovimentacao);

            // Movimentação de outro usuário recebe a mesma resposta de "não encontrada",
            // para não revelar que o ID existe
            if (movimentacao == null || movimentacao.UsuarioId != idUsuario)
                return Result<string>.Fail("Movimentação não encontrada.", ETipoErro.NaoEncontrado);

            await _movimentacaoRepository.DeletarAsync(movimentacao);

            return Result<string>.Ok("Movimentação excluída com sucesso!");
        }

        public async Task<Result<List<ObterMovimentacoesResponse>>> ObterMovimentacoes(ObterMovimentacoesRequest obterMovimentacoesRequest, Guid idUsuario)
        {
            var dataInicial = obterMovimentacoesRequest.DataInicial.HasValue
                ? ParaUtc(obterMovimentacoesRequest.DataInicial.Value)
                : (DateTime?)null;

            var dataFinal = obterMovimentacoesRequest.DataFinal.HasValue
                ? ParaUtc(obterMovimentacoesRequest.DataFinal.Value)
                : (DateTime?)null;

            // Data final informada sem horário (ex.: 2026-10-31) deve incluir o dia inteiro
            if (dataFinal.HasValue && dataFinal.Value.TimeOfDay == TimeSpan.Zero)
                dataFinal = dataFinal.Value.AddDays(1).AddTicks(-1);

            if (dataInicial.HasValue && dataFinal.HasValue && dataInicial > dataFinal)
                return Result<List<ObterMovimentacoesResponse>>.Fail("A data inicial não pode ser maior que a data final.");

            var movimentacoesEncontradas = await _movimentacaoRepository.ObterPorUsuarioComFiltrosAsync(
                idUsuario,
                dataInicial,
                dataFinal);

            var movimentacoes = movimentacoesEncontradas
                .Select(m => new ObterMovimentacoesResponse(m))
                .ToList();

            return Result<List<ObterMovimentacoesResponse>>.Ok(movimentacoes);
        }

        /// <summary>
        /// O Npgsql exige DateTimeKind.Utc para colunas timestamptz.
        /// Datas sem fuso (ex.: vindas da query string) são tratadas como UTC.
        /// </summary>
        private static DateTime ParaUtc(DateTime data) => data.Kind switch
        {
            DateTimeKind.Utc => data,
            DateTimeKind.Local => data.ToUniversalTime(),
            _ => DateTime.SpecifyKind(data, DateTimeKind.Utc)
        };
    }
}
