using ApiFinancas.Src.Domain.Enums;
using ApiFinancas.Src.Application.DTOs.Requests;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    public class ExcluirOperacoesRequest : BaseRequest
    {
        public ExcluirOperacoesRequest()
        {

        }

        [Required(ErrorMessage = "Id da operação necessário")]
        public Guid IdOperacao { get; set; }

        [Required(ErrorMessage = "Tipo da operação necessário")]
        public ETipoOperacao TipoOperacao { get; set; }
    }
}
