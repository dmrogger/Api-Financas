using ApiFinancas.Src.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Categorias
{
    /// <summary>
    /// Request de criação ou edição de categoria
    /// </summary>
    public class CategoriaRequest
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [MaxLength(Categoria.TamanhoMaximoNome, ErrorMessage = "O nome da categoria não pode exceder 50 caracteres")]
        public required string Nome { get; set; }
    }
}
