namespace ApiFinancas.Src.Application.DTOs.Responses.Resumos
{
    /// <summary>
    /// Totais do mês, no fuso horário configurado da aplicação
    /// </summary>
    public class ResumoMensalResponse
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
        public decimal Saldo => TotalEntradas - TotalSaidas;

        /// <summary>
        /// Totais por categoria, das maiores saídas para as menores
        /// </summary>
        public List<ResumoCategoriaResponse> Categorias { get; set; } = new();
    }

    public class ResumoCategoriaResponse
    {
        /// <summary>
        /// Nulo para o grupo "Sem categoria"
        /// </summary>
        public Guid? CategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
    }
}
