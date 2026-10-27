using ApiFinancas.Src.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    /// <summary>
    /// Request de cadastro de movimentação (entrada ou saída)
    /// </summary>
    public class CadastraMovimentacoesRequest
    {
        public CadastraMovimentacoesRequest(decimal valor, string? descricao, int? categoriaId, ETipoMovimentacao tipo)
        {
            Valor = valor;
            Descricao = descricao;
            CategoriaId = categoriaId;
            Tipo = tipo;
        }

        /// <summary>
        /// Valor da movimentação (sempre positivo; o sinal é definido pelo Tipo)
        /// </summary>
        [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data da movimentação. Se não informada, usa a data atual (UTC)
        /// </summary>
        public DateTime? DataOperacao { get; set; }

        [MaxLength(255, ErrorMessage = "A descrição não pode exceder 255 caracteres")]
        public string? Descricao { get; set; }

        public int? CategoriaId { get; set; }

        /// <summary>
        /// 1 = Entrada, 2 = Saída
        /// </summary>
        [EnumDataType(typeof(ETipoMovimentacao), ErrorMessage = "Tipo de movimentação inválido")]
        public ETipoMovimentacao Tipo { get; set; }
    }
}
