using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Resumos;
using ApiFinancas.Src.Application.DTOs.Responses.Resumos;
using ApiFinancas.Src.Application.Interfaces.Resumos;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Resumos
{
    /// <summary>
    /// Serviço de resumos financeiros
    /// </summary>
    public class ResumoService : IResumoService
    {
        public const string NomeSemCategoria = "Sem categoria";

        private readonly IMovimentacaoRepository _movimentacaoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly TimeZoneInfo _fusoHorario;

        /// <param name="fusoHorario">Fuso que define início e fim do mês (configurado no Program.cs)</param>
        public ResumoService(
            IMovimentacaoRepository movimentacaoRepository,
            ICategoriaRepository categoriaRepository,
            TimeZoneInfo fusoHorario)
        {
            _movimentacaoRepository = movimentacaoRepository;
            _categoriaRepository = categoriaRepository;
            _fusoHorario = fusoHorario;
        }

        public async Task<Result<ResumoMensalResponse>> ObterResumoMensalAsync(ObterResumoMensalRequest request, Guid idUsuario)
        {
            if (request.Ano < 2000 || request.Ano > 2100)
                return Result<ResumoMensalResponse>.Fail("Ano deve estar entre 2000 e 2100.");

            if (request.Mes < 1 || request.Mes > 12)
                return Result<ResumoMensalResponse>.Fail("Mês deve estar entre 1 e 12.");

            var (inicioUtc, fimUtc) = ObterLimitesDoMesEmUtc(request.Ano, request.Mes);

            var totais = await _movimentacaoRepository.ObterTotaisPorCategoriaAsync(idUsuario, inicioUtc, fimUtc);

            // Os nomes vêm das categorias que o usuário enxerga (padrão + próprias); são poucas linhas
            var nomes = (await _categoriaRepository.ListarVisiveisParaUsuarioAsync(idUsuario))
                .ToDictionary(c => c.Id, c => c.Nome);

            var categorias = totais
                .Select(t => new ResumoCategoriaResponse
                {
                    CategoriaId = t.CategoriaId,
                    Nome = t.CategoriaId.HasValue && nomes.TryGetValue(t.CategoriaId.Value, out var nome)
                        ? nome
                        : NomeSemCategoria,
                    TotalEntradas = t.TotalEntradas,
                    TotalSaidas = t.TotalSaidas
                })
                .OrderByDescending(c => c.TotalSaidas)
                .ThenByDescending(c => c.TotalEntradas)
                .ToList();

            return Result<ResumoMensalResponse>.Ok(new ResumoMensalResponse
            {
                Ano = request.Ano,
                Mes = request.Mes,
                TotalEntradas = categorias.Sum(c => c.TotalEntradas),
                TotalSaidas = categorias.Sum(c => c.TotalSaidas),
                Categorias = categorias
            });
        }

        /// <summary>
        /// Converte o mês no fuso configurado em um intervalo UTC [início, fim).
        /// Ex.: novembro/2026 em Brasília (UTC-3) = 01/11 03:00 UTC até 01/12 03:00 UTC
        /// </summary>
        public (DateTime InicioUtc, DateTime FimUtcExclusivo) ObterLimitesDoMesEmUtc(int ano, int mes)
        {
            var inicioLocal = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var fimLocal = inicioLocal.AddMonths(1);

            return (ParaUtc(inicioLocal), ParaUtc(fimLocal));
        }

        private DateTime ParaUtc(DateTime dataLocal)
        {
            // Em anos com horário de verão, a meia-noite pode não existir no dia da mudança
            if (_fusoHorario.IsInvalidTime(dataLocal))
                dataLocal = dataLocal.AddHours(1);

            return TimeZoneInfo.ConvertTimeToUtc(dataLocal, _fusoHorario);
        }
    }
}
