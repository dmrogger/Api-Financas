using ApiFinancas.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    /// <summary>
    /// Request de cadastro de movimentação (entrada ou saída)
    /// </summary>
    public class CadastraMovimentacoesRequest
    {
        public CadastraMovimentacoesRequest(decimal valor, string? descricao, Guid? categoriaId, ETipoMovimentacao tipo)
        {
            Valor = valor;
            Descricao = descricao;
            CategoriaId = categoriaId;
            Tipo = tipo;
        }

        /// <summary>
        /// Valor da movimentação (sempre positivo; o sinal é definido pelo Tipo)
        /// </summary>
        // Limites em cultura invariante: em pt-BR "0.01" não é um decimal válido (o separador é vírgula)
        [Range(typeof(decimal), "0.01", "999999999999",
            ParseLimitsInInvariantCulture = true,
            ConvertValueInInvariantCulture = true,
            ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da movimentação. Se não informada, usa a data atual (UTC)
        /// </summary>
        public DateTime? DataOperacao { get; set; }

        [MaxLength(255, ErrorMessage = "A descrição não pode exceder 255 caracteres")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Categoria padrão ou do próprio usuário (veja GET /api/v1/categorias). Opcional
        /// </summary>
        public Guid? CategoriaId { get; set; }

        /// <summary>
        /// 1 = Entrada, 2 = Saída
        /// </summary>
        [EnumDataType(typeof(ETipoMovimentacao), ErrorMessage = "Tipo de movimentação inválido")]
        public ETipoMovimentacao Tipo { get; set; }
    }
}
