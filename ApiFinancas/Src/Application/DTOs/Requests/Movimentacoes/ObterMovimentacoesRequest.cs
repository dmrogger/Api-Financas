namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    public class ObterMovimentacoesRequest
    {
        /// <summary>
        /// Data inicial (inclusive). Opcional
        /// </summary>
        public DateTime? DataInicial { get; set; }

        /// <summary>
        /// Data final (inclusive). Se informada sem horário, considera o dia inteiro. Opcional
        /// </summary>
        public DateTime? DataFinal { get; set; }
    }
}
