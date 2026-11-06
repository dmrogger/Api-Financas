using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Resumos
{
    public class ObterResumoMensalRequest
    {
        /// <summary>
        /// Ano do resumo (ex.: 2026)
        /// </summary>
        [Range(2000, 2100, ErrorMessage = "Ano deve estar entre 2000 e 2100")]
        public int Ano { get; set; }

        /// <summary>
        /// Mês do resumo (1 a 12)
        /// </summary>
        [Range(1, 12, ErrorMessage = "Mês deve estar entre 1 e 12")]
        public int Mes { get; set; }
    }
}
