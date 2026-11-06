using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Application.DTOs.Responses.Categorias
{
    public class CategoriaResponse
    {
        public CategoriaResponse(Categoria categoria)
        {
            CategoriaId = categoria.Id;
            Nome = categoria.Nome;
            Padrao = categoria.Padrao;
        }

        public Guid CategoriaId { get; set; }
        public string Nome { get; set; }

        /// <summary>
        /// true para categorias do sistema (não podem ser alteradas nem excluídas)
        /// </summary>
        public bool Padrao { get; set; }
    }
}
